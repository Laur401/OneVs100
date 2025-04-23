using Microsoft.EntityFrameworkCore;

namespace QuestionObtainer;
//Projektas sudarytas iš daugiau nei vieno modulio (assembly) (1 t.)
public static class Program
{
    public static async Task Main(string[] args)
    {
        QuestionGetter questionGetter = new QuestionGetter();
        await DatabaseManager.CheckDatabase(questionGetter);
    }

    public static async Task<List<QuestionEntry>> GetQuestions(bool forceLoad = false)
    {
        QuestionGetter questionGetter = new QuestionGetter();
        await DatabaseManager.CheckDatabase(questionGetter, forceLoad);
        return await DatabaseManager.GetQuestionsFromDatabase();
    }
}




