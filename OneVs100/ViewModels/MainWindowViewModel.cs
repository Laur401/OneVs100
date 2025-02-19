using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using OneVs100.Views;

namespace OneVs100.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    
    [ObservableProperty] public string questionText = "";
    [ObservableProperty] public string questionNumber = "";
    [ObservableProperty] public string answerA = "";
    [ObservableProperty] public object answerB = "";
    [ObservableProperty] public object answerC = "";
    
    private int currentQuestionNumber = 0;

    public MainWindowViewModel()
    {
        LoadQuestions();
        CreateMobMembers();
        Task.Run(async () => await LoadNextQuestion());
    }
    
    private async Task LoadNextQuestion()
    {
        await Task.Delay(1000);
        //DisableMobMembers();
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
            MarkWrongAnswers();
            Task.Run(async () => 
            {
                await LoadNextQuestion();
            });
        }
    }

    private void MarkWrongAnswers()
    {
        for (int i=0; i<mobMembers.Count; i++)
        {
            if (!mobMembers[i].isAnswerCorrect(questionDict[currentQuestionNumber].CorrectAnswer))
            {
                WeakReferenceMessenger.Default.Send(new MobMemberStatusMessage(i+1, 1));
            }
        }
    }

    private void DisableMobMembers() //TODO: Redo this later with something more sensible, maybe move disabling individual members to MainWindow?
    {
        for (int i=0; i<mobMembers.Count; i++)
        {
            if (!mobMembers[i].isAnswerCorrect(questionDict[currentQuestionNumber].CorrectAnswer))
            {
                WeakReferenceMessenger.Default.Send(new MobMemberStatusMessage(i+1, 2));
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
}

internal class QuestionInfo(string question, string answerA, string answerB, string answerC, char correctAnswer)
{
    internal string Question { get; set; } = question;
    internal string AnswerA { get; set; } = answerA;
    internal string AnswerB { get; set; } = answerB;
    internal string AnswerC { get; set; } = answerC;
    internal char CorrectAnswer { get; set; } = correctAnswer;
}