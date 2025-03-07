using System.Text.Json;

namespace QuestionObtainer;

public class LocalQuestionGetter : IQuestionGetter
{
    public async Task<IList<IQuestionEntry>> GetQuestions(int _=0)
    {
        List<IQuestionEntry> questions = GetLocalJSON();
        return questions;
    }

    private List<IQuestionEntry> GetLocalJSON()
    {
        List<IQuestionEntry> questionList = new List<IQuestionEntry>();
        try
        {
            string json = File.ReadAllText("LocalQuestions.json");
            List<ManualList>? questions = JsonSerializer.Deserialize<List<ManualList>>(json);
            if (questions != null)
            {
                foreach (var question in questions)
                {
                    QuestionEntry questionEntry = new QuestionEntry
                    {
                        Question = question.question,
                        CorrectAnswer = question.correctAnswer,
                        WrongAnswers = question.wrongAnswers,
                        Difficulty = question.difficulty
                    };
                    questionList.Add(questionEntry);
                }
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error in LocalQuestionGetter: {ex.Message}");
            return questionList;
        }
        return questionList;
    }

    private IQuestionEntry AddQuestion(float difficulty, string question, string answer, params List<string> wrongAnswers)
    {
        IQuestionEntry questionEntry = new QuestionEntry();
        questionEntry.Difficulty = difficulty;
        questionEntry.Question = question;
        questionEntry.CorrectAnswer = answer;
        questionEntry.WrongAnswers = wrongAnswers;
        return questionEntry;
    }

    private sealed class ManualList
    {
        public required string question { get; set; }
        public required string correctAnswer { get; set; }
        public required List<string> wrongAnswers { get; set; }
        public float difficulty { get; set; }
    }
}