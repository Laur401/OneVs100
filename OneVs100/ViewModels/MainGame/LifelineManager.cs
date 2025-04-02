using System;
using System.Collections.Generic;

namespace OneVs100.ViewModels.MainGame;

public class LifelineManager
{
    private static Lazy<LifelineManager> lazyInstance = new Lazy<LifelineManager>(()=> new LifelineManager());
    public static LifelineManager Instance => lazyInstance.Value;
    private LifelineManager() { }

    public char TrustTheMob(Func<char, List<MobMember>> mobMembersWithAnswer, char correctAnswer)
    {
        Dictionary<char, int> answerCounts = new Dictionary<char, int>();
        answerCounts['A'] = mobMembersWithAnswer('A').Count;
        answerCounts['B'] = mobMembersWithAnswer('B').Count;
        answerCounts['C'] = mobMembersWithAnswer('C').Count;

        char max;

        if (answerCounts['A'] >= answerCounts['B'] && answerCounts['A'] >= answerCounts['C']) max = 'A';
        else if (answerCounts['B'] >= answerCounts['C']) max = 'B';
        else max = 'C';

        if (max != correctAnswer && answerCounts[correctAnswer] == answerCounts[max]) return correctAnswer;
        else return max;
    }
    
    
}