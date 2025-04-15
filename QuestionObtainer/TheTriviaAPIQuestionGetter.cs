using System.Net;
using RestSharp;

namespace QuestionObtainer;

public class TheTriviaAPIQuestionGetter : IQuestionGetter
{
    
    public async Task<IList<IQuestionEntry>> GetQuestions(int count)
    {
        List<TriviaAPIResponse> response;
        try
        {
            response = await GetTriviaAPI(count);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Failed to get The Trivia API. Exception: {ex}");
            return new List<IQuestionEntry>();
        }
        List<IQuestionEntry> questions = StandardizeResponse(response);
        return questions;
    }

    private static async Task<List<TriviaAPIResponse>> GetTriviaAPI(int count)
    {
        if (count<1||count>50) throw new ArgumentException("Count must be between 1 and 50");
        var client = new RestClient("https://the-trivia-api.com/v2"); //https://the-trivia-api.com/v2/questions?limit=50&categories=9&types="text_choice"
        var request = new RestRequest("/questions")
            .AddParameter("limit", count)
            .AddParameter("categories", "9"); //General knowledge
        List<TriviaAPIResponse> response = await client.GetAsync<List<TriviaAPIResponse>>(request) ?? throw new InvalidOperationException();
        return response;
    }
    
    private static List<IQuestionEntry> StandardizeResponse(List<TriviaAPIResponse> response)
    {
        List<IQuestionEntry> questions = new List<IQuestionEntry>();
        foreach (TriviaAPIResponse question in response)
        {
            IQuestionEntry questionEntry = new QuestionEntry();
            questionEntry.Question = WebUtility.HtmlDecode(question.Question.Text);
            questionEntry.CorrectAnswer = WebUtility.HtmlDecode(question.CorrectAnswer);
            questionEntry.WrongAnswers = new List<string>();
            foreach (string answer in question.IncorrectAnswers)
                questionEntry.WrongAnswers.Add(WebUtility.HtmlDecode(answer));
            //Naudojate 'switch' su 'when' raktažodžiu (0.5 t.)
            switch (question.Difficulty)
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
                    throw new InvalidDataException("TheTriviaAPI - Unrecognized difficulty");
            }
            questions.Add(questionEntry);
        }
        return questions;
    }

    private sealed class TriviaAPIResponse
    {
        
        public string Category { get; set; }
        public string ID { get; set; }
        public string CorrectAnswer { get; set; }
        public List<string> IncorrectAnswers { get; set; }
        public Question Question { get; set; }
        public List<string> Tags { get; set; }
        public string Type { get; set; }
        public string Difficulty { get; set; }
        public List<string> Regions { get; set; }
        public bool IsNiche { get; set; }
    }

    private sealed class Question
    {
        public string Text { get; set; }
    }
}