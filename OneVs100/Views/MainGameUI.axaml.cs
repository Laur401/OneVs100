using System;
using System.Collections.Generic;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Messaging;
using OneVs100.CustomControls;

namespace OneVs100.Views;

public partial class MainGameUI : UserControl
{
    public MainGameUI()
    {
        InitializeComponent();

        qnABoard = new QnABoard();
        moneyLadderBoard = new MoneyLadderBoard();
        moneyOrMobBoard = new MoneyOrMobBoard();
        
        WeakReferenceMessenger.Default.Register<MainGameUI, MobMemberStatusMessage>(
            this, (recipient, message) =>
            {
                recipient.MobMessageReceiver(message.MemberNumber, message.Status);
            });
        WeakReferenceMessenger.Default.Register<MainGameUI, BoardStatusMessage>(
            this, (recipient, message) =>
            {
                recipient.BoardMessageReceiver(message.Status);
            });
    }
    
    private QnABoard qnABoard;
    private MoneyLadderBoard moneyLadderBoard;
    private MoneyOrMobBoard moneyOrMobBoard;
    

    public void BoardMessageReceiver(int status)
    {
        switch (status)
        {
            case 0:
                Board.Content = qnABoard;
                break;
            case 1:
                Board.Content = moneyLadderBoard;
                break;
            case 2:
                Board.Content = moneyOrMobBoard;
                break;
            default:
                Console.Write("Error in BoardMessageReceiver");
                break;
        }
    }

    public void MobMessageReceiver(int number, int status)
    {
        switch (status)
        {
            case 0:
                AddMobMember(number);
                break;
            case 1:
                MarkWrongMobMember(number);
                break;
            case 2:
                DisableMobMember(number);
                break;
            default:
                Console.Write("Error in MobMessageReceiver");
                break;
        }
    }
    
    Dictionary<int, MobMemberControl> mobMemberControls = new Dictionary<int, MobMemberControl>();
    public void AddMobMember(int number)
    {
        MobMemberControl mobMemberControl = new MobMemberControl();
        mobMemberControl.MemberNumber = number;
        //MobStorage.Children.Add(mobMemberControl);
        List<StackPanel> MobStorage = [MobStorageTop, MobStorageLeft, MobStorageRight, MobStorageBottom];
        AddChild();
        
        void AddChild()
        {
            foreach (StackPanel mobStorage in MobStorage)
            {
                foreach (StackPanel stackPanel in mobStorage.Children)
                {
                    if (stackPanel.Children.Count < Convert.ToInt32(stackPanel.Tag))
                    {
                        mobMemberControls.Add(number, mobMemberControl);
                        stackPanel.Children.Add(mobMemberControl);
                        return;
                    }
                }
            }
        }
    }

    public void MarkWrongMobMember(int number)
    {
        mobMemberControls[number].MobMemberWrong();
        moneyLadderBoard.AddWrongMobMember();
        
        
    }
    
    public void DisableMobMember(int number)
    {
        mobMemberControls[number].DisableMobMember();
    }
}

public class MobMemberStatusMessage(int memberNumber, int status)
{
    public int MemberNumber { get; } = memberNumber;
    public int Status { get; } = status; //TODO: Convert status to an Enum
}
public class BoardStatusMessage(int status)
{
    public int Status { get; } = status; //TODO: Convert status to an Enum
}