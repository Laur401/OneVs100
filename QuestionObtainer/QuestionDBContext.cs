using Microsoft.EntityFrameworkCore;

namespace QuestionObtainer;

// Naudojate savo projekte duomenų bazę ir Entity Framework. (3 t.)
public class QuestionDBContext : DbContext
{
    public DbSet<QuestionEntry> Questions { get; set; }
    public string DBPath { get; }
    public QuestionDBContext()
    {
        DBPath = Path.Combine(Environment.CurrentDirectory, "questions.db");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"Data Source={DBPath}");
    }
}

public class QuestionEntry : IQuestionEntry
{
    public int ID { get; set; }
    public string Question { get; set; } = string.Empty;
    public string CorrectAnswer { get; set; } = string.Empty;
    public List<string> WrongAnswers { get; set; } = new();
    public float Difficulty { get; set; }
}