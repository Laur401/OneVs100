using RestSharp;

namespace QuestionObtainer;

public class TheTriviaAPIQuestionGetter : IQuestionGetter
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
            Console.Error.WriteLine($"Failed to get The Trivia API. Exception: {ex}");
            return new List<IQuestionEntry>();
        }
        List<IQuestionEntry> questions = StandardizeResponse(response);
        return questions;
    }

    private static async Task<TriviaAPIResponse> GetTriviaAPI(int count)
    {
        if (count<1||count>50) throw new ArgumentException("Count must be between 1 and 50");
        var client = new RestClient("https://the-trivia-api.com/v2");
        var request = new RestRequest("/questions")
            .AddParameter("limit", count)
            .AddParameter("categories", "9") //General knowledge
            .AddParameter("types",  "text_choice");
        TriviaAPIResponse response = await client.GetAsync<TriviaAPIResponse>(request) ?? throw new InvalidOperationException();
        return response;
    }
    
    private static List<IQuestionEntry> StandardizeResponse(TriviaAPIResponse response)
    {
        List<IQuestionEntry> questions = new List<IQuestionEntry>();
        foreach (Questions question in response.Questions)
        {
            IQuestionEntry questionEntry = new QuestionEntry();
            questionEntry.Question = question.Question.Text;
            questionEntry.CorrectAnswer = question.CorrectAnswer;
            questionEntry.WrongAnswers = question.IncorrectAnswers;
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
        public List<Questions> Questions { get; set; }
    }

    private sealed class Questions
    {
        public string Category { get; set; }
        public string ID { get; set; }
        public List<string> Tags { get; set; }
        public string Difficulty { get; set; }
        public List<string> Regions { get; set; }
        public bool IsNiche { get; set; }
        public Question Question { get; set; }
        public string CorrectAnswer { get; set; }
        public List<string> IncorrectAnswers { get; set; }
        public string Type { get; set; }
        
    }

    private sealed class Question
    {
        public string Text { get; set; }
    }
}