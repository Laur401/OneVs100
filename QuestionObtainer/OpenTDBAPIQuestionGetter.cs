using RestSharp;

namespace QuestionObtainer;

public class OpenTDBAPIQuestionGetter : IQuestionGetter
{
    public List<IQuestionEntry> GetQuestions(int count)
    {
        TriviaAPIResponse response;
        try
        {
            response = Task.Run(() => GetTriviaAPI(count)).Result;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Failed to get OpenTDB API. Exception: {ex}");
            return new List<IQuestionEntry>();
        }
        List<IQuestionEntry> questions = StandardizeResponse(response);
        return questions;
    }

    private static async Task<TriviaAPIResponse> GetTriviaAPI(int count)
    {
        if (count<1||count>50) throw new ArgumentException("Count must be between 1 and 50");
        var client = new RestClient("https://opentdb.com");
        var request = new RestRequest("/api.php")
            .AddParameter("amount", count)
            .AddParameter("category", "9") //General knowledge
            .AddParameter("type",  "multiple");
        TriviaAPIResponse response = await client.GetAsync<TriviaAPIResponse>(request) 
                                     ?? throw new InvalidOperationException();
        return response;
    }

    private static List<IQuestionEntry> StandardizeResponse(TriviaAPIResponse response)
    {
        List<IQuestionEntry> questions = new List<IQuestionEntry>();
        foreach (Result result in response.Questions)
        {
            IQuestionEntry questionEntry = new QuestionEntry();
            questionEntry.Question = result.Question;
            questionEntry.CorrectAnswer = result.CorrectAnswer;
            questionEntry.WrongAnswers = result.IncorrectAnswers;
            switch (result.Difficulty)
            {
                case string s when s.Equals("easy"):
                    questionEntry.Difficulty = 1;
                    break;
                case string s when s.Equals("medium"):
                    questionEntry.Difficulty = 2;
                    break;
                case string s when s.Equals("hard"):
                    questionEntry.Difficulty = 3;
                    break;
                default:
                    throw new InvalidDataException("OpenTDB API - Unrecognized difficulty");
            }
            questions.Add(questionEntry);
        }
        return questions;
    }
    
    private sealed class TriviaAPIResponse
    {
        public int ResponseCode { get; set; }
        public List<Result> Questions { get; set; }
    }
    private sealed class Result
    {
        public string Type { get; set; }
        public string Difficulty { get; set; }
        public string Category { get; set; }
        public string Question { get; set; }
        public string CorrectAnswer { get; set; }
        public List<string> IncorrectAnswers { get; set; }
    }
}

