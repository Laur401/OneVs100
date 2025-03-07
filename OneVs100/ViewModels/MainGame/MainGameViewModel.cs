using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using OneVs100.Helpers;
using OneVs100.Views;
using OneVs100.Views.MainGame;
using QuestionObtainer;

namespace OneVs100.ViewModels.MainGame;

public partial class MainGameViewModel : PageViewModelBase
{
    [ObservableProperty] private string questionText = "";
    [ObservableProperty] private string questionNumber = "";
    [ObservableProperty] private string answerA = "";
    [ObservableProperty] private object answerB = "";
    [ObservableProperty] private object answerC = "";

    [ObservableProperty] private List<int> moneyLadderValues = [1000, 5000, 10000, 25000, 50000, 75000, 100000, 250000, 500000, 1000000];
    [ObservableProperty] private List<string> moneyLadderValuesString = new List<string>();
    
    private string? totalMoney = "0 €";
    public string? TotalMoney
    {
        get => totalMoney;
        set => SetProperty(ref totalMoney, value);
    }
    
    private readonly MobMemberManager mobMemberManager = MobMemberManager.Instance;
    private readonly QuestionManager questionManager = QuestionManager.Instance;
    private readonly BoardManager boardManager = BoardManager.Instance;
    private readonly AudioPlayer audioPlayer = AudioPlayer.Instance;
    
    //GeneralTextBoard's Next button
    private TaskCompletionSource<bool> GeneralControlButtonPressed;
    [RelayCommand] public void OnNextButtonPressed()
    {
        GeneralControlButtonPressed?.TrySetResult(true);
    }
    private async Task WaitForNextButtonPress()
    {
        GeneralControlButtonPressed = new TaskCompletionSource<bool>();
        await GeneralControlButtonPressed.Task;
    }

    public MainGameViewModel()
    {
        foreach (int val in moneyLadderValues)
        {
            moneyLadderValuesString.Add(val.ToString("N0")+" €");
        }
    }

    private void ResetInstance()
    {
        TotalMoney = "0 €";
        GeneralControlText = "";
        AnswerLock = false;
    }
    
    private void ResetQnABoardValues()
    {
        WeakReferenceMessenger.Default.Send(new BoardStatusMessage(BoardStatusMessageOptions.ResetQnABoard));
        AnswerLock = false;
        QuestionNumber = "";
        QuestionText = "";
        AnswerA = "";
        AnswerB = "";
        AnswerC = "";
    }
    
    public override void OnActivate()
    {
        ResetInstance();
        questionManager.InitializeQuestionList();
        mobMemberManager.CreateMobMembers(100);
        audioPlayer.PlaySound(SoundEffects.PlayerIntro);
        Dispatcher.UIThread.InvokeAsync(LoadGameIntro);
    }
    
    [ObservableProperty] private string generalControlText = "";
    
    private async Task LoadGameIntro()
    {
        boardManager.LoadGeneralTextBoard();
        GeneralControlText = "Welcome to 1 vs. 100! In this game, you face off against 100 people for a great cash prize!\n" +
                             "The premise is very simple - either you will win, or the MOB will win!\n" +
                             "For every 10 Mob members you eliminate, you will win a bigger prize.\n" +
                             "However, if you get even one question wrong, you will leave with nothing!\n" +
                             "Let's play 1 vs. 100!";
        await WaitForNextButtonPress();
        boardManager.LoadQnABoard();
        ResetQnABoardValues();
        await LoadNextQuestion();
    }
    
    private async Task LoadNextQuestion()
    {
        ResetQnABoardValues();
        (string question, string answerA, string answerB, string answerC) = questionManager.GetNextQuestion();
        QuestionNumber = "Q"+questionManager.CurrentQuestion;
        QuestionText = question;
        await Task.Delay(500);
        AnswerA = answerA;
        await Task.Delay(500);
        AnswerB = answerB;
        await Task.Delay(500);
        AnswerC = answerC;
        await Task.Delay(500);
        mobMemberManager.SelectAnswers(questionManager.CorrectAnswer, 
            questionManager.QuestionDifficulty, questionManager.CurrentQuestion);
    }
    
    private bool AnswerLock = false;
    [RelayCommand]
    public void AnswerCommand(char answer)
    {
        if (!AnswerLock)
        {
            AnswerLock = true;
            if (answer == questionManager.CorrectAnswer)
                Dispatcher.UIThread.InvokeAsync(AnswerToMoneyOrMob);
            else
                Dispatcher.UIThread.InvokeAsync(AnswerToWrongExit);
        }
    }

    private async Task AnswerToWrongExit()
    {
        await Task.Delay(3000);
        
        ShowCorrectAnswer();
        await Task.Delay(1500);
        
        boardManager.LoadGeneralTextBoard();
        GeneralControlText = "That's the wrong answer! The Mob wins and takes all of your money.\n" +
                             "Better luck next time!";
        await WaitForNextButtonPress();
        
        LeaveGame();
    }
    
    private async Task AnswerToMoneyOrMob()
    {
        await Task.Delay(3000);
        
        ShowCorrectAnswer();
        await Task.Delay(1500);
        
        boardManager.LoadMoneyLadderBoard();
        await mobMemberManager.MarkWrongAnswers(questionManager.CorrectAnswer);
        await Task.Delay(1500); //TODO: Replace this with "Next" button
        mobMemberManager.DisableMobMembers();
        
        UpdateCurrentPrizeMoney();
        boardManager.LoadMoneyOrMobBoard();
    }
    
    private void ShowCorrectAnswer()
    {
        WeakReferenceMessenger.Default.Send(new BoardStatusMessage(BoardStatusMessageOptions.ShowCorrectAnswer,
            questionManager.CorrectAnswer));
    }

    private void UpdateCurrentPrizeMoney()
    {
        int pos = mobMemberManager.wrongMobMemberCount/10;
        TotalMoney = pos - 1 >= 0 && pos - 1 < MoneyLadderValuesString.Count
            ? MoneyLadderValuesString[pos - 1]
            : "0 €";
    }
    
    //Money or Mob Options
    [RelayCommand]
    public void TakeMob()
    {
        Dispatcher.UIThread.InvokeAsync(MoneyOrMobToNextQuestion);
    }

    [RelayCommand]
    public async Task TakeMoney()
    {
        boardManager.LoadGeneralTextBoard();
        GeneralControlText = $"Congratulations! You are walking away with {TotalMoney}!\n" +
                             $"Thank you for playing!";
        await WaitForNextButtonPress();
        LeaveGame();
    }
    
    private async Task MoneyOrMobToNextQuestion()
    {
        boardManager.LoadQnABoard();
        await LoadNextQuestion();
    }

    private void LeaveGame()
    {
        ResetInstance();
        boardManager.ResetAllBoards();
        mobMemberManager.ResetInstance();
        questionManager.ResetInstance();
        viewChangeDelegate.Invoke(this, Windows.MainMenu);
    }
}