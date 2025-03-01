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
    
    private readonly MobMemberManager mobMemberManager = MobMemberManager.Instance;

    public MainGameViewModel()
    {
        foreach (int val in moneyLadderValues)
        {
            moneyLadderValuesString.Add(val.ToString("N0")+" €");
        }
    }
    
    private string? totalMoney = "0 €";
    public string? TotalMoney
    {
        get => totalMoney;
        set => SetProperty(ref totalMoney, value);
    }
    private int currentQuestionNumber = 0;
    
    public override void OnActivate()
    {
        LoadQuestions();
        mobMemberManager.CreateMobMembers(100);
        AudioPlayer audioPlayer = AudioPlayer.Instance;
        audioPlayer.PlaySound(SoundEffects.PlayerIntro);
        Dispatcher.UIThread.InvokeAsync(LoadGameIntro);
    }
    
    [ObservableProperty] private string generalControlText = "";
    private async Task LoadGameIntro()
    {
        LoadGeneralTextBoard();
        GeneralControlText = "Welcome to 1 vs. 100! In this game, you face off against 100 people for a great cash prize!\n" +
                             "The premise is very simple - either you will win, or the MOB will win!\n" +
                             "For every 10 Mob members you eliminate, you will win a bigger prize.\n" +
                             "However, if you get even one question wrong, you will leave with nothing!\n" +
                             "Let's play 1 vs. 100!";
        await Task.Delay(5000);
        LoadQnABoard();
        ResetQnABoard();
        await LoadNextQuestion();
    }

    private void ResetQnABoard()
    {
        WeakReferenceMessenger.Default.Send(new BoardStatusMessage(BoardStatusMessageOptions.ResetQnABoard));
        AnswerLock = false;
        QuestionNumber = "";
        QuestionText = "";
        AnswerA = "";
        AnswerB = "";
        AnswerC = "";
    }

    private async Task LoadNextQuestion()
    {
        ResetQnABoard();
        currentQuestionNumber++;
        QuestionInfo currentQuestion = questionDict[currentQuestionNumber];
        QuestionNumber = "Q"+currentQuestionNumber;
        QuestionText = currentQuestion.Question;
        await Task.Delay(500);
        AnswerA = currentQuestion.AnswerA;
        await Task.Delay(500);
        AnswerB = currentQuestion.AnswerB;
        await Task.Delay(500);
        AnswerC = currentQuestion.AnswerC;
        await Task.Delay(500);
        mobMemberManager.SelectAnswers(currentQuestion.CorrectAnswer, currentQuestion.Difficulty, currentQuestionNumber);
    }
    
    private bool AnswerLock = false;
    [RelayCommand]
    public void AnswerCommand(char answer)
    {
        QuestionInfo question = questionDict[currentQuestionNumber];
        if (!AnswerLock && answer == question.CorrectAnswer)
        {
            AnswerLock = true;
            Dispatcher.UIThread.InvokeAsync(AnswerToMoneyOrMobManager);
        }
    }

    private void ShowCorrectAnswer()
    {
        WeakReferenceMessenger.Default.Send(new BoardStatusMessage(BoardStatusMessageOptions.ShowCorrectAnswer, questionDict[currentQuestionNumber].CorrectAnswer));
    }
    
    private async Task AnswerToMoneyOrMobManager()
    {
        //Show correct answer
        await Task.Delay(3000);
        ShowCorrectAnswer();
        await Task.Delay(1500);
        LoadMoneyLadderBoard();
        await mobMemberManager.MarkWrongAnswers(questionDict[currentQuestionNumber].CorrectAnswer);
        await Task.Delay(1500); //TODO: Replace this with "Next" button
        UpdateCurrentPrizeMoney();
        mobMemberManager.DisableMobMembers();
        LoadMoneyOrMobBoard();
    }

    private void UpdateCurrentPrizeMoney()
    {
        int pos = mobMemberManager.wrongMobMemberCount/10;
        TotalMoney = pos - 1 >= 0 && pos - 1 < MoneyLadderValuesString.Count
            ? MoneyLadderValuesString[pos - 1]
            : "0 €";
    }

    private async Task MoneyOrMobToQuestionManager()
    {
        LoadQnABoard();
        await LoadNextQuestion();
    }
    
    //Question loading (Temporary)
    private Dictionary<int, QuestionInfo> questionDict = new Dictionary<int, QuestionInfo>();
    
    private void LoadQuestions()
    {
        //Notes: Ratchet up the difficulty, on higher Qs dropping difficulty by over 50% means no eliminations, so maybe only throw in one or two.
        questionDict.Add(1, new QuestionInfo("SampleQuestion1","SampleAnswerA","SampleAnswerB","SampleAnswerC",'A', 1));
        questionDict.Add(2, new QuestionInfo("SampleQuestion2","SampleAnswer2A","SampleAnswer2B","SampleAnswer2C",'B', 2));
        questionDict.Add(3, new QuestionInfo("SampleQuestion3","SampleAnswer3A","SampleAnswer3B","SampleAnswer3C",'C', 3));
        questionDict.Add(4, new QuestionInfo("SampleQuestion1","SampleAnswerA","SampleAnswerB","SampleAnswerC",'A', 1));
        questionDict.Add(5, new QuestionInfo("SampleQuestion2","SampleAnswer2A","SampleAnswer2B","SampleAnswer2C",'B', 2));
        questionDict.Add(6, new QuestionInfo("SampleQuestion3","SampleAnswer3A","SampleAnswer3B","SampleAnswer3C",'C', 3));
        questionDict.Add(7, new QuestionInfo("SampleQuestion1","SampleAnswerA","SampleAnswerB","SampleAnswerC",'A', 1));
        questionDict.Add(8, new QuestionInfo("SampleQuestion2","SampleAnswer2A","SampleAnswer2B","SampleAnswer2C",'B', 2));
        questionDict.Add(9, new QuestionInfo("SampleQuestion3","SampleAnswer3A","SampleAnswer3B","SampleAnswer3C",'C', 3));
        questionDict.Add(10, new QuestionInfo("SampleQuestion1","SampleAnswerA","SampleAnswerB","SampleAnswerC",'A', 1));
        questionDict.Add(11, new QuestionInfo("SampleQuestion2","SampleAnswer2A","SampleAnswer2B","SampleAnswer2C",'B', 2));
        questionDict.Add(12, new QuestionInfo("SampleQuestion3","SampleAnswer3A","SampleAnswer3B","SampleAnswer3C",'C', 3));
        questionDict.Add(13, new QuestionInfo("LastQuestion", "LastAnswerA", "LastAnswerB", "LastAnswerC", 'D', 99));
    }
    
    //Page Loader
    private void LoadQnABoard()
    {
        WeakReferenceMessenger.Default.Send(new BoardStatusMessage(BoardStatusMessageOptions.QnABoard));
    }

    private void LoadMoneyLadderBoard()
    {
        WeakReferenceMessenger.Default.Send(new BoardStatusMessage(BoardStatusMessageOptions.MoneyLadderBoard));
    }

    private void LoadMoneyOrMobBoard()
    {
        WeakReferenceMessenger.Default.Send(new BoardStatusMessage(BoardStatusMessageOptions.MoneyOrMobBoard));
    }

    private void LoadGeneralTextBoard()
    {
        WeakReferenceMessenger.Default.Send(new BoardStatusMessage(BoardStatusMessageOptions.GeneralTextBoard));
    }
    
    //Money or Mob stuff
    [RelayCommand]
    public void TakeMob()
    {
        Dispatcher.UIThread.InvokeAsync(MoneyOrMobToQuestionManager);
    }

    [RelayCommand]
    public void TakeMoney()
    {
        LoadGeneralTextBoard();
        GeneralControlText = $"Congratulations! You are walking away with {TotalMoney}!\n" +
                             $"Thank you for playing!";
    }
}

internal class QuestionInfo(string question, string answerA, string answerB, string answerC, char correctAnswer, float difficulty)
{
    internal string Question { get; } = question;
    internal string AnswerA { get; } = answerA;
    internal string AnswerB { get; } = answerB;
    internal string AnswerC { get; } = answerC;
    internal char CorrectAnswer { get; } = correctAnswer;

    internal float Difficulty { get; } = difficulty;
}