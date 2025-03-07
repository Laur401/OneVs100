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
        Task.Run(LoadQuestions);
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
        QuestionInfo currentQuestion = questionDict[currentQuestionNumber-1];
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
        QuestionInfo question = questionDict[currentQuestionNumber-1];
        if (!AnswerLock)
        {
            AnswerLock = true;
            if (answer == question.CorrectAnswer)
                Dispatcher.UIThread.InvokeAsync(AnswerToMoneyOrMobManager);
            else
                Dispatcher.UIThread.InvokeAsync(AnswerToWrongExit);
            
        }
    }

    private void ShowCorrectAnswer()
    {
        WeakReferenceMessenger.Default.Send(new BoardStatusMessage(BoardStatusMessageOptions.ShowCorrectAnswer, questionDict[currentQuestionNumber-1].CorrectAnswer));
    }

    private async Task AnswerToWrongExit()
    {
        await Task.Delay(3000);
        ShowCorrectAnswer();
        await Task.Delay(1500);
        LoadGeneralTextBoard();
        GeneralControlText = "That's the wrong answer! The Mob wins and takes all of your money.\n" +
                             "Better luck next time!";
    }
    
    private async Task AnswerToMoneyOrMobManager()
    {
        //Show correct answer
        await Task.Delay(3000);
        ShowCorrectAnswer();
        await Task.Delay(1500);
        LoadMoneyLadderBoard();
        await mobMemberManager.MarkWrongAnswers(questionDict[currentQuestionNumber-1].CorrectAnswer);
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
    
    private List<QuestionInfo> questionDict = new List<QuestionInfo>();
    private QuestionSet questionDataSet = new QuestionSet();
    private void LoadQuestions()
    {
        var questionGetter = new QuestionGetter();
        questionDataSet = Task.Run(questionGetter.FetchQuestions).Result;
        List<QuestionInfo> questionList = new List<QuestionInfo>();
        
        Dictionary<int, char> answerConverter = new Dictionary<int, char>();
        answerConverter.Add(0, 'A');
        answerConverter.Add(1, 'B');
        answerConverter.Add(2, 'C');
        RandomList random = new RandomList();
        
        foreach (var questionData in questionDataSet)
        {
            List<string> answers = new List<string>();
            answers.Add(questionData.CorrectAnswer);
            random.Shuffle(questionData.WrongAnswers);
            answers.AddRange(questionData.WrongAnswers[0..2]);
            random.Shuffle(answers);
            QuestionInfo questionInfo = new QuestionInfo(questionData.Question, answers[0], answers[1],
                answers[2], answerConverter[answers.FindIndex(x=>x==questionData.CorrectAnswer)],
                questionData.Difficulty);
            questionList.Add(questionInfo);
        }
        questionList.Sort((q1, q2)=>q1.Difficulty.CompareTo(q2.Difficulty));
        questionDict=questionList;
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