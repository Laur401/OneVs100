using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using OneVs100.Helpers;
using OneVs100.Views;

namespace OneVs100.ViewModels;

public partial class MainGameViewModel : PageViewModelBase
{
    [ObservableProperty] private string questionText = "";
    [ObservableProperty] private string questionNumber = "";
    [ObservableProperty] private string answerA = "";
    [ObservableProperty] private object answerB = "";
    [ObservableProperty] private object answerC = "";
    private float totalMoney = 0;
    public float? TotalMoney
    {
        get => totalMoney;
        set
        {
            float valueConv = Convert.ToSingle(value);
            SetProperty(ref totalMoney, valueConv);
        }
    }
    private int currentQuestionNumber = 0;

    public override void OnActivate()
    {
        LoadQuestions();
        CreateMobMembers();
        LoadQnABoard();
        Dispatcher.UIThread.InvokeAsync(LoadNextQuestion);
    }

    private async Task LoadNextQuestion()
    {
        //await Task.Delay(1000);
        
        currentQuestionNumber++;
        QuestionInfo currentQuestion = questionDict[currentQuestionNumber];
        AnswerA = currentQuestion.AnswerA;
        AnswerB = currentQuestion.AnswerB;
        AnswerC = currentQuestion.AnswerC;
        QuestionText = currentQuestion.Question;
        QuestionNumber = "Q"+currentQuestionNumber;
        SelectAnswers(currentQuestion.CorrectAnswer, 2, currentQuestionNumber);
    }
    
    [RelayCommand]
    public void AnswerCommand(char answer)
    {
        QuestionInfo question = questionDict[currentQuestionNumber];
        if (answer == question.CorrectAnswer)
        {
            Dispatcher.UIThread.InvokeAsync(MarkWrongAnswers);
            LoadMoneyLadderBoard();
        }
    }

    private async Task MarkWrongAnswers()
    {
        List<MobMember> wrongAnswers = new List<MobMember>();
        for (int i=0; i<mobMembers.Count; i++)
        {
            if (!mobMembers[i].isAnswerCorrect(questionDict[currentQuestionNumber].CorrectAnswer)&&!mobMembers[i].isKnockedOut)
            {
                wrongAnswers.Add(mobMembers[i]);
            }
        }
        RandomList randomiser = new RandomList();
        randomiser.Shuffle(wrongAnswers);
        for (int i = 0; i < wrongAnswers.Count; i++)
        {
            wrongAnswers[i].isKnockedOut = true;
            WeakReferenceMessenger.Default.Send(new MobMemberStatusMessage(wrongAnswers[i].Number, 1));
            await Task.Delay(500);
        }
        await Task.Delay(1500); //TODO: Replace this with "Next" button
        DisableMobMembers();
        LoadMoneyOrMobBoard();
    }

    private void DisableMobMembers()
    {
        for (int i=0; i<mobMembers.Count; i++)
        {
            if (mobMembers[i].isKnockedOut)
            {
                WeakReferenceMessenger.Default.Send(new MobMemberStatusMessage(i+1, 2));
                TotalMoney += 1;
            }
        }
    }
    
    //Mob member functionality
    List<MobMember> mobMembers = new List<MobMember>();
    
    private void CreateMobMembers()
    {
        for (int i = 0; i < 100; i++)
        {
            MobMember mobMember = new MobMember(i+1);
            mobMembers.Add(mobMember);
            WeakReferenceMessenger.Default.Send(new MobMemberStatusMessage(i+1, 0));
        }
    }

    private void SelectAnswers(char answer, float difficulty, int questionNr)
    {
        for (int i=0; i<mobMembers.Count; i++)
        {
            mobMembers[i].selectAnswer(answer, difficulty, questionNr);
        }
    }
    
    //Question loading (Temporary)
    private Dictionary<int, QuestionInfo> questionDict = new Dictionary<int, QuestionInfo>();
    
    private void LoadQuestions()
    {
        questionDict.Add(1, new QuestionInfo("SampleQuestion1","SampleAnswerA","SampleAnswerB","SampleAnswerC",'A'));
        questionDict.Add(2, new QuestionInfo("SampleQuestion2","SampleAnswer2A","SampleAnswer2B","SampleAnswer2C",'B'));
        questionDict.Add(3, new QuestionInfo("SampleQuestion3","SampleAnswer3A","SampleAnswer3B","SampleAnswer3C",'C'));
        questionDict.Add(4, new QuestionInfo("SampleQuestion1","SampleAnswerA","SampleAnswerB","SampleAnswerC",'A'));
        questionDict.Add(5, new QuestionInfo("SampleQuestion2","SampleAnswer2A","SampleAnswer2B","SampleAnswer2C",'B'));
        questionDict.Add(6, new QuestionInfo("SampleQuestion3","SampleAnswer3A","SampleAnswer3B","SampleAnswer3C",'C'));
        questionDict.Add(7, new QuestionInfo("SampleQuestion1","SampleAnswerA","SampleAnswerB","SampleAnswerC",'A'));
        questionDict.Add(8, new QuestionInfo("SampleQuestion2","SampleAnswer2A","SampleAnswer2B","SampleAnswer2C",'B'));
        questionDict.Add(9, new QuestionInfo("SampleQuestion3","SampleAnswer3A","SampleAnswer3B","SampleAnswer3C",'C'));
        questionDict.Add(10, new QuestionInfo("SampleQuestion1","SampleAnswerA","SampleAnswerB","SampleAnswerC",'A'));
        questionDict.Add(11, new QuestionInfo("SampleQuestion2","SampleAnswer2A","SampleAnswer2B","SampleAnswer2C",'B'));
        questionDict.Add(12, new QuestionInfo("SampleQuestion3","SampleAnswer3A","SampleAnswer3B","SampleAnswer3C",'C'));
        questionDict.Add(13, new QuestionInfo("LastQuestion", "LastAnswerA", "LastAnswerB", "LastAnswerC", 'D'));
    }
    
    //Money or Mob stuff
    private void LoadQnABoard()
    {
        WeakReferenceMessenger.Default.Send(new BoardStatusMessage(0));
    }

    private void LoadMoneyLadderBoard()
    {
        WeakReferenceMessenger.Default.Send(new BoardStatusMessage(1));
    }

    private void LoadMoneyOrMobBoard()
    {
        WeakReferenceMessenger.Default.Send(new BoardStatusMessage(2));
    }
    
    [RelayCommand]
    public void TakeMob()
    {
        LoadQnABoard();
        Dispatcher.UIThread.InvokeAsync(LoadNextQuestion);
    }
}

internal class QuestionInfo(string question, string answerA, string answerB, string answerC, char correctAnswer)
{
    internal string Question { get; } = question;
    internal string AnswerA { get; } = answerA;
    internal string AnswerB { get; } = answerB;
    internal string AnswerC { get; } = answerC;
    internal char CorrectAnswer { get; } = correctAnswer;
}