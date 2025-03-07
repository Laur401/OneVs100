using System.Net;
using RestSharp;

namespace QuestionObtainer;

public class OpenTDBAPIQuestionGetter : IQuestionGetter
{
    public async Task<IList<IQuestionEntry>> GetQuestions(int count)
    {
        TriviaAPIResponse response;
        try
        {
            response = await GetTriviaAPI(count);
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
        Console.WriteLine(client.BuildUri(request));
        TriviaAPIResponse response = await client.GetAsync<TriviaAPIResponse>(request) 
                                     ?? throw new InvalidOperationException();
        return response;
    }

    private static List<IQuestionEntry> StandardizeResponse(TriviaAPIResponse response)
    {
        List<IQuestionEntry> questions = new List<IQuestionEntry>();
        foreach (Result result in response.Results)
        {
            IQuestionEntry questionEntry = new QuestionEntry();
            questionEntry.Question = WebUtility.HtmlDecode(result.Question);
            questionEntry.CorrectAnswer = WebUtility.HtmlDecode(result.Correct_Answer);
            questionEntry.WrongAnswers = new List<string>();
            foreach (string answer in result.Incorrect_Answers)
                questionEntry.WrongAnswers.Add(WebUtility.HtmlDecode(answer));
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
        public List<Result> Results { get; set; }
    }
    private sealed class Result
    {
        public string Type { get; set; }
        public string Difficulty { get; set; }
        public string Category { get; set; }
        public string Question { get; set; }
        public string Correct_Answer { get; set; }
        public List<string> Incorrect_Answers { get; set; }
    }
}

