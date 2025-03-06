using System.Collections;

namespace QuestionObtainer;

public class QuestionGetter
{
    public async Task FetchQuestions()
    {
        OpenTDBAPIQuestionGetter openTDBAPIQuestionGetter = new OpenTDBAPIQuestionGetter();
        Task<List<IQuestionEntry>> task1 = Task.Run(()=>openTDBAPIQuestionGetter.GetQuestions(50));
        TheTriviaAPIQuestionGetter theTriviaAPIQuestionGetter = new TheTriviaAPIQuestionGetter();
        Task<List<IQuestionEntry>> task2 = Task.Run(()=>theTriviaAPIQuestionGetter.GetQuestions(50));
        await Task.WhenAll(task1, task2);
        var questions = task1.Result+task2.Result;
    }
}

interface IQuestionGetter
{
    public List<IQuestionEntry> GetQuestions(int count);
}

public interface IQuestionEntry
{
    public string Question { get; set; }
    public string CorrectAnswer { get; set; }
    public List<string> WrongAnswers { get; set;  }
    public float Difficulty { get; set; }
}

public struct QuestionEntry : IQuestionEntry
{
    public string Question { get; set; }
    public string CorrectAnswer { get; set; }
    public List<string> WrongAnswers { get; set; }
    public float Difficulty { get; set; }
}

public struct QuestionSet
{
    private List<IQuestionEntry> innerList;

    public QuestionSet()
    {
        innerList = new List<IQuestionEntry>();
    }

    public QuestionSet(IEnumerable<IQuestionEntry> collection)
    {
        innerList = new List<IQuestionEntry>(collection);
    }
    
    public static QuestionSet operator +(QuestionSet a, QuestionSet b)
    {
        var result = new QuestionSet(a.innerList);
        result.innerList.AddRange(b.innerList);
        return result;
    }
}