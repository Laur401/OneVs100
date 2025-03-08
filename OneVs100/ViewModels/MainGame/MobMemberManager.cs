using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using OneVs100.Helpers;
using OneVs100.Views;
using OneVs100.Views.MainGame;

namespace OneVs100.ViewModels.MainGame;

public partial class MobMemberManager : ObservableObject
{
    private static Lazy<MobMemberManager> lazyInstance = new Lazy<MobMemberManager>(() => new MobMemberManager());
    public static MobMemberManager Instance => lazyInstance.Value;
    private MobMemberManager() { }
    
    private List<MobMember> mobMembers = new List<MobMember>();
    private RandomList randomiser = new RandomList();
    public int WrongMobMemberCount = 0;
    [ObservableProperty] private int mobMembersRemainingCount = 0;
    private readonly RandomGaussian RNG = new();
    
    public void ResetInstance()
    {
        mobMembers = new List<MobMember>();
        randomiser = new RandomList();
        WrongMobMemberCount = 0;
        MobMembersRemainingCount = 0;
    }
    
    public void CreateMobMembers(int count)
    {
        for (int i = 0; i < count; i++)
        {
            mobMembers.Add(new MobMember(i+1, RNG));
            WeakReferenceMessenger.Default.Send(new MobMemberStatusMessage(i+1, 0));
        }
        MobMembersRemainingCount += count;
    }
    
    public void DisableMobMembers()
    {
        for (int i=0; i<mobMembers.Count; i++)
        {
            if (mobMembers[i].IsKnockedOut)
            {
                WeakReferenceMessenger.Default.Send(new MobMemberStatusMessage(i+1, 2));
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

        if (wrongMobMembers.Count == 0)
        {
            AudioPlayer.Instance.PlaySound(SoundEffects.MobNoWrong);
            return;
        }
        
        //Mark wrong answers
        randomiser.Shuffle(wrongMobMembers);
        foreach (MobMember wrongMobMember in wrongMobMembers)
        {
            wrongMobMember.IsKnockedOut = true;
            WrongMobMemberCount++;
            MobMembersRemainingCount--;
            WeakReferenceMessenger.Default.Send(new MobMemberStatusMessage(wrongMobMember.Number, 1));
            await Task.Delay(500);
        }
    }
}