using System.Collections;
// ReSharper disable InconsistentNaming

namespace QuestionObtainer;

public class QuestionGetter : IQuestionGetter
{
    public async Task<IList<IQuestionEntry>> GetQuestions(int questionCountPerSource)
    {
        OpenTDBAPIQuestionGetter openTDBAPIQuestionGetter = new OpenTDBAPIQuestionGetter();
        Task<IList<IQuestionEntry>> task1 = Task.Run(()=>openTDBAPIQuestionGetter.GetQuestions(questionCountPerSource));
        TheTriviaAPIQuestionGetter theTriviaAPIQuestionGetter = new TheTriviaAPIQuestionGetter();
        Task<IList<IQuestionEntry>> task2 = Task.Run(()=>theTriviaAPIQuestionGetter.GetQuestions(questionCountPerSource));
        LocalQuestionGetter localQuestionGetter = new LocalQuestionGetter();
        Task<IList<IQuestionEntry>> task3 = Task.Run(()=>localQuestionGetter.GetQuestions());

        List<Task<IList<IQuestionEntry>>> taskList = [task1, task2, task3];
        QuestionSet questions = new QuestionSet();
        while (taskList.Any())
        {
            try
            {
                var completedTask = await Task.WhenAny(taskList);
                taskList.Remove(completedTask);
                await completedTask;
                
                if (!completedTask.Result.Any()) continue;
                questions += new QuestionSet(completedTask.Result);
            }
            catch (NoQuestionsException ex)
            {
                Console.Error.WriteLine($"Failed to get questions from API (possibly down/moved). {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Failed to get questions. Exception: {ex}");
            }
        }
        return questions;
    }
}

//Sukūrėte ir pritaikėte savo sąsają (interface) (0.5 t.)
public interface IQuestionGetter
{
    public Task<IList<IQuestionEntry>> GetQuestions(int questionCount);
}

public interface IQuestionEntry
{
    public string Question { get; set; }
    public string CorrectAnswer { get; set; }
    public List<string> WrongAnswers { get; set;  }
    public float Difficulty { get; set; }
}

public struct QuestionSet : IList<IQuestionEntry>
{
    private List<IQuestionEntry> _innerList;

    public QuestionSet()
    {
        _innerList = new List<IQuestionEntry>();
    }

    public QuestionSet(IEnumerable<IQuestionEntry> collection)
    {
        _innerList = new List<IQuestionEntry>(collection);
    }
    
    //Naudojamas operatorių perkrovimas (0.5 t)
    public static QuestionSet operator +(QuestionSet a, QuestionSet b)
    {
        var result = new QuestionSet(a._innerList);
        result._innerList.AddRange(b._innerList);
        return result;
    }
    
    //IList method implementation
    public IEnumerator<IQuestionEntry> GetEnumerator()
    {
        return _innerList.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_innerList).GetEnumerator();
    }

    public void Add(IQuestionEntry item)
    {
        _innerList.Add(item);
    }

    public void Clear()
    {
        _innerList.Clear();
    }

    public bool Contains(IQuestionEntry item)
    {
        return _innerList.Contains(item);
    }

    public void CopyTo(IQuestionEntry[] array, int arrayIndex)
    {
        _innerList.CopyTo(array, arrayIndex);
    }

    public bool Remove(IQuestionEntry item)
    {
        return _innerList.Remove(item);
    }

    public int Count => _innerList.Count;

    public bool IsReadOnly => ((ICollection<IQuestionEntry>)_innerList).IsReadOnly;

    public int IndexOf(IQuestionEntry item)
    {
        return _innerList.IndexOf(item);
    }

    public void Insert(int index, IQuestionEntry item)
    {
        _innerList.Insert(index, item);
    }

    public void RemoveAt(int index)
    {
        _innerList.RemoveAt(index);
    }

    public IQuestionEntry this[int index]
    {
        get => _innerList[index];
        set => _innerList[index] = value;
    }
}
// Sukūrėte ir panaudojote savo išimties tipus (1 t.)
public class NoQuestionsException : Exception
{
    private string _APISource;

    public NoQuestionsException(string APISource)
    {
        _APISource = APISource;
    }
    
    public override string Message => $"No questions were found from {_APISource}.";
}