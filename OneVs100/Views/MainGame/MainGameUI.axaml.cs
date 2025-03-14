using System;
using System.Collections.Generic;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Messaging;
using OneVs100.CustomControls;

namespace OneVs100.Views.MainGame;

public partial class MainGameUI : UserControl
{
    public MainGameUI()
    {
        InitializeComponent();

        ResetUI();
        
        WeakReferenceMessenger.Default.Register<MainGameUI, MobMemberStatusMessage>(
            this, (recipient, message) =>
            {
                recipient.MobMessageReceiver(message.MemberNumber, message.Status);
            });
        WeakReferenceMessenger.Default.Register<MainGameUI, BoardStatusMessage>(
            this, (recipient, message) =>
            {
                recipient.BoardMessageReceiver(message.Status, message.ExtraData);
            });
    }
    
    private QnABoard qnABoard;
    private MoneyLadderBoard moneyLadderBoard;
    private MoneyOrMobBoard moneyOrMobBoard;
    private GeneralTextBoard generalTextBoard;

    private void ResetUI()
    {
        qnABoard = new QnABoard();
        moneyLadderBoard = new MoneyLadderBoard();
        moneyOrMobBoard = new MoneyOrMobBoard();
        generalTextBoard = new GeneralTextBoard();
        mobMemberControls = new Dictionary<int, MobMemberControl>();
        List<StackPanel> mobStorages = [MobStorageTop, MobStorageLeft, MobStorageRight, MobStorageBottom];
        foreach (StackPanel mobStorage in mobStorages)
        {
            foreach (StackPanel stackPanel in mobStorage.Children)
            {
                stackPanel.Children.Clear();
            }
        }
    }
    
    // Naudojami numatyti ir vardiniai argumentai (0.5 t.)
    // ReSharper disable once MemberCanBePrivate.Global
    public void BoardMessageReceiver(BoardStatusMessageOptions status, object? extraData=null)
    {
        switch (status)
        {
            case BoardStatusMessageOptions.QnABoard:
                Board.Content = qnABoard;
                break;
            case BoardStatusMessageOptions.EnableSelectingAnswer:
                qnABoard.EnableSelectingAnswer();
                break;
            case BoardStatusMessageOptions.ShowSelectedAnswer:
                qnABoard.OnAudioTrackFinished();
                break;
            case BoardStatusMessageOptions.ShowCorrectAnswer:
                qnABoard.ShowCorrectAnswer(Convert.ToChar(extraData));
                break;
            case BoardStatusMessageOptions.ResetQnABoard:
                qnABoard.ResetBoard();
                break;
            case BoardStatusMessageOptions.MoneyLadderBoard:
                Board.Content = moneyLadderBoard;
                break;
            case BoardStatusMessageOptions.MoneyOrMobBoard:
                moneyOrMobBoard.ResetBoard();
                Board.Content = moneyOrMobBoard;
                break;
            case BoardStatusMessageOptions.GeneralTextBoard:
                Board.Content = generalTextBoard;
                break;
            case BoardStatusMessageOptions.ResetAllBoards:
                ResetUI();
                break;
            default:
                Console.Error.WriteLine("Error in BoardMessageReceiver");
                break;
        }
    }
    
    // ReSharper disable once MemberCanBePrivate.Global
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
                Console.Error.WriteLine("Error in MobMessageReceiver");
                break;
        }
    }
    
    Dictionary<int, MobMemberControl> mobMemberControls = new Dictionary<int, MobMemberControl>();
    private void AddMobMember(int number)
    {
        MobMemberControl mobMemberControl = new MobMemberControl();
        mobMemberControl.MemberNumber = number;
        
        List<StackPanel> mobStorages = [MobStorageTop, MobStorageLeft, MobStorageRight, MobStorageBottom];
        AddChild();
        
        void AddChild()
        {
            foreach (StackPanel mobStorage in mobStorages)
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

    private void MarkWrongMobMember(int number)
    {
        mobMemberControls[number].MobMemberWrong();
        moneyLadderBoard.MarkMobMemberWrong();
    }

    private void DisableMobMember(int number)
    {
        mobMemberControls[number].DisableMobMember();
    }
}

public class MobMemberStatusMessage(int memberNumber, int status)
{
    public int MemberNumber { get; } = memberNumber;
    public int Status { get; } = status; //TODO: Convert status to an Enum
}
public class BoardStatusMessage(BoardStatusMessageOptions status, object? extraData=null)
{
    public BoardStatusMessageOptions Status { get; } = status;
    public object? ExtraData { get; } = extraData;
}

public enum BoardStatusMessageOptions
{
    QnABoard,
    EnableSelectingAnswer,
    ShowSelectedAnswer,
    ShowCorrectAnswer,
    ResetQnABoard,
    MoneyLadderBoard,
    MoneyOrMobBoard,
    GeneralTextBoard,
    ResetAllBoards,
}