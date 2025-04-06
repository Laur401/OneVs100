using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Hosting;
using OneVs100.Helpers;

namespace OneVs100.ViewModels.MainGame;

public partial class LifelineManager
{
    private static Lazy<LifelineManager> lazyInstance = new Lazy<LifelineManager>(()=> new LifelineManager());
    public static LifelineManager Instance => lazyInstance.Value;
    private LifelineManager() { }
    
    private RandomList random = new RandomList();

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

    public void PollTheMob(char answer, Func<char, List<MobMember>> mobMembersWithAnswer, Action<int> highlightMobMember)
    {
        foreach (var mobMember in mobMembersWithAnswer(answer))
        {
            highlightMobMember(mobMember.Number);
        }
    }

    private int askTheMobOneNumber;
    private int askTheMobTwoNumber;
    public void AskTheMob(char correctAnswer, Func<char, List<MobMember>> mobMembersWithAnswer,
        Action<int> highlightMobMember, Action<(int, char), (int, char)> insertAnswers)
    {
        List<char> incorrectAnswers = ['A', 'B', 'C'];
        incorrectAnswers.Remove(correctAnswer);
        char incorrectAnswer = incorrectAnswers[random.Next(0, incorrectAnswers.Count)];
        
        var correctMobMemberList = mobMembersWithAnswer(correctAnswer);
        if (correctMobMemberList.Count == 0)
        {
            foreach (char c in incorrectAnswers)
            {
                var mobMembers = mobMembersWithAnswer(c);
                if (mobMembers.Count == 0) continue;
                correctMobMemberList = mobMembers;
                break;
            }
        }
        int correctMobMemberSelection = random.Next(0, correctMobMemberList.Count);
        
        var incorrectMobMemberList = mobMembersWithAnswer(incorrectAnswer);
        if (incorrectMobMemberList.Count == 0)
        {
            List<char> answers = [..incorrectAnswers, correctAnswer];
            foreach (char c in answers)
            {
                var mobMembers = mobMembersWithAnswer(c);
                if (mobMembers.Count == 0) continue;
                incorrectMobMemberList = mobMembers;
                break;
            }
        }
        int incorrectMobMemberSelection = random.Next(0, incorrectMobMemberList.Count);
        
        int correctPlacementSelection = random.GetItems([0, 1], 1)[0];
        switch (correctPlacementSelection)
        {
            case 0:
                insertAnswers((correctMobMemberList[correctMobMemberSelection].Number, correctAnswer),
                    (incorrectMobMemberList[incorrectMobMemberSelection].Number, incorrectAnswer));
                askTheMobOneNumber = correctMobMemberList[correctMobMemberSelection].Number;
                askTheMobTwoNumber = incorrectMobMemberList[incorrectMobMemberSelection].Number;
                break;
            case 1:
                insertAnswers((incorrectMobMemberList[incorrectMobMemberSelection].Number, incorrectAnswer),
                    (correctMobMemberList[correctMobMemberSelection].Number, correctAnswer));
                askTheMobOneNumber = incorrectMobMemberList[incorrectMobMemberSelection].Number;
                askTheMobTwoNumber = correctMobMemberList[correctMobMemberSelection].Number;
                break;
        }
        highlightMobMember(askTheMobOneNumber);
        highlightMobMember(askTheMobTwoNumber);
    }

    public void ClearLifeline(Action<int> removeHighlightMobMember)
    {
        removeHighlightMobMember(askTheMobOneNumber);
        removeHighlightMobMember(askTheMobTwoNumber);
    }
    
}