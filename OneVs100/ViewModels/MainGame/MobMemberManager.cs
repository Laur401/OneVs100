using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using OneVs100.Helpers;
using OneVs100.Views;
using OneVs100.Views.MainGame;

namespace OneVs100.ViewModels.MainGame;

public class MobMemberManager
{
    private static readonly Lazy<MobMemberManager> lazyInstance = new(() => new MobMemberManager());
    public static MobMemberManager Instance => lazyInstance.Value;
    private readonly List<MobMember> mobMembers = new List<MobMember>();
    private readonly RandomList randomiser = new RandomList();
    public int wrongMobMemberCount = 0;
    private MobMemberManager() { }
    
    public void CreateMobMembers(int count)
    {
        for (int i = 0; i < count; i++)
        {
            mobMembers.Add(new MobMember(i+1));
            WeakReferenceMessenger.Default.Send(new MobMemberStatusMessage(i+1, 0));
        }
    }
    
    public void DisableMobMembers()
    {
        for (int i=0; i<mobMembers.Count; i++)
        {
            if (mobMembers[i].IsKnockedOut)
            {
                WeakReferenceMessenger.Default.Send(new MobMemberStatusMessage(i+1, 2));
                //MobMembersLeft -= 1;
            }
        }
    }

    public void SelectAnswers(char answer, float difficulty, int questionNr)
    {
        foreach (MobMember mobMember in mobMembers)
        {
            mobMember.SelectAnswer(answer, difficulty, questionNr);
        }
    }
    
    public async Task MarkWrongAnswers(char correctAnswer)
    {
        //Get wrong answers
        List<MobMember> wrongMobMembers = new List<MobMember>();
        foreach (MobMember mobMember in mobMembers)
        {
            if (!mobMember.IsKnockedOut&&!mobMember.IsAnswerCorrect(correctAnswer))
            {
                wrongMobMembers.Add(mobMember);
            }
        }
        
        //Mark wrong answers
        randomiser.Shuffle(wrongMobMembers);
        foreach (MobMember wrongMobMember in wrongMobMembers)
        {
            wrongMobMember.IsKnockedOut = true;
            wrongMobMemberCount++;
            WeakReferenceMessenger.Default.Send(new MobMemberStatusMessage(wrongMobMember.Number, 1));
            await Task.Delay(500);
        }
    }
}