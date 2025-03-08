using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OneVs100.Helpers;
using QuestionObtainer;

namespace OneVs100.ViewModels.MainGame;

public class QuestionManager
{
    private static Lazy<QuestionManager> lazyInstance = new Lazy<QuestionManager>(()=> new QuestionManager());
    public static QuestionManager Instance => lazyInstance.Value;
    private QuestionManager() { }
    
    
    public int CurrentQuestion = 0;
    public char CorrectAnswer;
    public float QuestionDifficulty;
    private RandomGaussian RNG = new RandomGaussian();

    public void ResetInstance()
    {
        CurrentQuestion = 0;
        questionList.Clear();
    }

    public void InitializeQuestionList()
    {
        Task.Run(LoadQuestionsFromAPIs);
    }
    
    public (string, string, string, string) GetNextQuestion()
    {
        CurrentQuestion++;
        int questionPick = Convert.ToInt32(RNG.BoxMuller(0f, (questionList.Count-1) * 1f));
        string question = questionList[questionPick].Question;
        string answerA = questionList[questionPick].AnswerA;
        string answerB = questionList[questionPick].AnswerB;
        string answerC = questionList[questionPick].AnswerC;
        CorrectAnswer = questionList[questionPick].CorrectAnswer;
        QuestionDifficulty = questionList[questionPick].Difficulty;
        return (question, answerA, answerB, answerC);
    }
    
    private List<QuestionInfo> questionList = new List<QuestionInfo>();
    private async Task LoadQuestionsFromAPIs()
    {
        var questionGetter = new QuestionGetter();
        QuestionSet questionDataSet = await questionGetter.FetchQuestions();
        Dictionary<int, char> answerToCharConverter = new Dictionary<int, char>();
        answerToCharConverter.Add(0, 'A');
        answerToCharConverter.Add(1, 'B');
        answerToCharConverter.Add(2, 'C');
        RandomList random = new RandomList();

        foreach (var questionData in questionDataSet)
        {
            List<string> answers = new List<string>();
            answers.Add(questionData.CorrectAnswer);
            random.Shuffle(questionData.WrongAnswers);
            answers.AddRange(questionData.WrongAnswers[0..2]);
            random.Shuffle(answers);
            QuestionInfo questionInfo = new QuestionInfo(questionData.Question, answers[0], answers[1],
                answers[2], answerToCharConverter[answers.FindIndex(x=>x==questionData.CorrectAnswer)],
                questionData.Difficulty);
            questionList.Add(questionInfo);
        }
        questionList.Sort((q1, q2)=>q1.Difficulty.CompareTo(q2.Difficulty));

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
}