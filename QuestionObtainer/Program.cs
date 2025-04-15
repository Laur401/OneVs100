using Microsoft.EntityFrameworkCore;

namespace QuestionObtainer;
public static class Program
{
    public static async Task Main(string[] args)
    {
        QuestionGetter questionGetter = new QuestionGetter();
        await DatabaseManager.CheckDatabase(questionGetter);
    }

    public static async Task<List<QuestionEntry>> GetQuestions()
    {
        QuestionGetter questionGetter = new QuestionGetter();
        await DatabaseManager.CheckDatabase(questionGetter);
        return await DatabaseManager.GetQuestionsFromDatabase();
    }
}




