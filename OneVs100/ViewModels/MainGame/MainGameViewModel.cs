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
using SoundFlow.Components;

namespace OneVs100.ViewModels.MainGame;

public partial class MainGameViewModel : PageViewModelBase
{
    [ObservableProperty] private string questionText = "";
    [ObservableProperty] private string questionNumber = "";
    [ObservableProperty] private string answerA = "";
    [ObservableProperty] private object answerB = "";
    [ObservableProperty] private object answerC = "";
    
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
    private readonly MoneyManager moneyManager = MoneyManager.Instance;
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
        moneyLadderValuesString = moneyManager.InitializeStringValues();
    }

    private void ResetInstance()
    {
        TotalMoney = "0 €";
        GeneralControlText = "";
        AnswerLock = true;
        MoneyOrMobLock = false;
    }
    
    private void ResetQnABoardValues()
    {
        WeakReferenceMessenger.Default.Send(new BoardStatusMessage(BoardStatusMessageOptions.ResetQnABoard));
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
        audioPlayer.StopAllSounds();
        audioPlayer.PlaySound(SoundEffects.BumperNextQuestion);
        await Task.Delay(2500);
        audioPlayer.PlaySound(SoundEffects.TransitionNextQuestion);
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
        await Task.Delay(3000);
        AnswerA = answerA;
        audioPlayer.PlaySound(SoundEffects.AnswerShow);
        await Task.Delay(1500);
        AnswerB = answerB;
        audioPlayer.PlaySound(SoundEffects.AnswerShow);
        await Task.Delay(1500);
        AnswerC = answerC;
        audioPlayer.PlaySound(SoundEffects.AnswerShow);
        //TODO: Insert mob selection scene here
        await Task.Delay(1500);
        mobMemberManager.SelectAnswers(questionManager.CorrectAnswer, 
            questionManager.QuestionDifficulty, questionManager.CurrentQuestion);
        audioPlayer.PlaySound(SoundEffects.BackgroundQuestion, out var qBackSoundPlayer);
        WeakReferenceMessenger.Default.Send(new BoardStatusMessage(BoardStatusMessageOptions.EnableSelectingAnswer));
        AnswerLock = false;
        questionBackgroundSoundPlayer = qBackSoundPlayer;
    }

    private SoundPlayer? questionBackgroundSoundPlayer;
    
    private bool AnswerLock = true;
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
        audioPlayer.StopSound(ref questionBackgroundSoundPlayer);
        int waitFor = audioPlayer.PlaySound(SoundEffects.AnswerSelect);
        await Task.Delay(waitFor);
        WeakReferenceMessenger.Default.Send(new BoardStatusMessage(BoardStatusMessageOptions.ShowSelectedAnswer));
        await Task.Delay(3000);
        
        waitFor = audioPlayer.PlaySound(SoundEffects.AnswerWrong);
        await Task.Delay(waitFor);
        
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
        audioPlayer.StopSound(ref questionBackgroundSoundPlayer);
        int waitFor = audioPlayer.PlaySound(SoundEffects.AnswerSelect);
        await Task.Delay(waitFor);
        WeakReferenceMessenger.Default.Send(new BoardStatusMessage(BoardStatusMessageOptions.ShowSelectedAnswer));
        await Task.Delay(3000);
        
        ShowCorrectAnswer();
        audioPlayer.PlaySound(SoundEffects.AnswerCorrect);
        await Task.Delay(2000);
        
        waitFor = audioPlayer.PlaySound(SoundEffects.TransitionMobWrongBoard);
        await Task.Delay(waitFor);
        boardManager.LoadMoneyLadderBoard();
        await Task.Delay(1500);
        
        await mobMemberManager.MarkWrongAnswers(questionManager.CorrectAnswer);
        await Task.Delay(1500); //TODO: Replace this with "Next" button
        
        mobMemberManager.DisableMobMembers();
        TotalMoney = moneyManager.GetCurrentPrizeMoney(mobMemberManager.WrongMobMemberCount, MoneyLadderValuesString);
        if (mobMemberManager.MobMembersRemainingCount == 0)
        {
            Dispatcher.UIThread.InvokeAsync(WinGame);
            return;
        }
        
        boardManager.LoadMoneyOrMobBoard();
        audioPlayer.PlaySound(SoundEffects.BackgroundMoneyOrMob);
    }
    
    private void ShowCorrectAnswer()
    {
        WeakReferenceMessenger.Default.Send(new BoardStatusMessage(BoardStatusMessageOptions.ShowCorrectAnswer,
            questionManager.CorrectAnswer));
    }

    private async Task WinGame()
    {
        audioPlayer.PlaySound(SoundEffects.Victory);
        boardManager.LoadGeneralTextBoard();
        GeneralControlText = $"Congratulations! You have beaten the entire Mob and have just won {TotalMoney}!\n" +
                             "Thank you for playing 1 vs. 100!";
        await WaitForNextButtonPress();
        LeaveGame();
    }
    
    private bool MoneyOrMobLock = false;
    //Money or Mob Options
    [RelayCommand]
    public void TakeMob()
    {
        if (!MoneyOrMobLock)
        {
            MoneyOrMobLock = true;
            audioPlayer.StopAllSounds();
            Dispatcher.UIThread.InvokeAsync(MoneyOrMobToNextQuestion);
        }
    }

    [RelayCommand]
    public async Task TakeMoney()
    {
        if (!MoneyOrMobLock)
        {
            MoneyOrMobLock = true;
            audioPlayer.StopAllSounds();
            audioPlayer.PlaySound(SoundEffects.TakeMoney);
            await Task.Delay(3000);
            boardManager.LoadGeneralTextBoard();
            GeneralControlText = $"Congratulations! You are walking away with {TotalMoney}!\n" +
                                 $"Thank you for playing!";
            await WaitForNextButtonPress();
            LeaveGame();
        }
    }
    
    private async Task MoneyOrMobToNextQuestion()
    {
        audioPlayer.PlaySound(SoundEffects.TakeMob);
        await Task.Delay(3000);
        audioPlayer.PlaySound(SoundEffects.BumperNextQuestion);
        await Task.Delay(2500);
        audioPlayer.PlaySound(SoundEffects.TransitionNextQuestion);
        boardManager.LoadQnABoard();
        await LoadNextQuestion();
        MoneyOrMobLock = false;
    }

    private void LeaveGame()
    {
        ResetInstance();
        boardManager.ResetAllBoards();
        mobMemberManager.ResetInstance();
        questionManager.ResetInstance();
        audioPlayer.StopAllSounds();
        viewChangeDelegate.Invoke(this, Windows.MainMenu);
    }
}