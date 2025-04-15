using Microsoft.EntityFrameworkCore;

namespace QuestionObtainer;

public static class DatabaseManager
{
    public async static Task CheckDatabase(IQuestionGetter questionGetter)
    {
        await using var db = new QuestionDBContext();
        await db.Database.EnsureCreatedAsync();
        if (!(await db.Questions.AnyAsync()))
        {
            Console.WriteLine("Question database empty.");
            var questionSet = await questionGetter.GetQuestions(50);
            for (int i = 0; i < questionSet.Count; i++)
            {
                var question = new QuestionEntry
                {
                    Question = questionSet[i].Question,
                    CorrectAnswer = questionSet[i].CorrectAnswer,
                    WrongAnswers = questionSet[i].WrongAnswers,
                    Difficulty = questionSet[i].Difficulty,
                };
                db.Questions.Add(question);
            }
            await db.SaveChangesAsync();
            Console.WriteLine("Question set added to database.");
        }
        Console.WriteLine("Question database filled.");
    }

    public async static Task<List<QuestionEntry>> GetQuestionsFromDatabase()
    {
        await using var db = new QuestionDBContext();
        await db.Database.EnsureCreatedAsync();
        return await db.Questions.ToListAsync();
    }
}