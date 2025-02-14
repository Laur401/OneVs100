using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace OneVs100.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    
    [ObservableProperty] public string question = "";
    [ObservableProperty] public string questionNumber = "";
    [ObservableProperty] public string answerA = "";
    [ObservableProperty] public object answerB = "";
    [ObservableProperty] public object answerC = "";
    
    //[ObservableProperty]
    //public string correctMessage = "";
    
    private int currentQuestionNumber = 0;

    public MainWindowViewModel()
    {
        LoadQuestions();
        Task.Run(async () => await LoadNextQuestion());
    }
    
    [RelayCommand]
    public void AnswerCommand(char answer)
    {
        QuestionInfo question = questionDict[currentQuestionNumber];
        if (answer == question.CorrectAnswer)
        {
            Task.Run(async () => await LoadNextQuestion());
        }
    }

    private async Task LoadNextQuestion()
    {
        await Task.Delay(1000);
        currentQuestionNumber++;
        QuestionInfo currentQuestion = questionDict[currentQuestionNumber];
        AnswerA = currentQuestion.AnswerA;
        AnswerB = currentQuestion.AnswerB;
        AnswerC = currentQuestion.AnswerC;
        Question = currentQuestion.Question;
        QuestionNumber = "Q"+currentQuestionNumber;
    }

    Dictionary<int, QuestionInfo> questionDict = new Dictionary<int, QuestionInfo>();
    
    private void LoadQuestions()
    {
        questionDict.Add(1, new QuestionInfo("SampleQuestion1","SampleAnswerA","SampleAnswerB","SampleAnswerC",'A'));
        questionDict.Add(2, new QuestionInfo("SampleQuestion2","SampleAnswer2A","SampleAnswer2B","SampleAnswer2C",'B'));
        questionDict.Add(3, new QuestionInfo("SampleQuestion3","SampleAnswer3A","SampleAnswer3B","SampleAnswer3C",'C'));
        questionDict.Add(4, new QuestionInfo("LastQuestion", "LastAnswerA", "LastAnswerB", "LastAnswerC", 'D'));
    }
    List<MobMember> mobMembers = new List<MobMember>();

    private void CreateMobMembers()
    {
        for (int i = 0; i < 10; i++)
        {
            mobMembers.Add(new MobMember(i));
        }
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