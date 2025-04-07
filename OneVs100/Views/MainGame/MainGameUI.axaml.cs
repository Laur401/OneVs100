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
    private AskTheMobLifelineBoard askTheMobLifelineBoard;
    private PollTheMobLifelineBoard pollTheMobLifelineBoard;

    private void ResetUI()
    {
        qnABoard = new QnABoard();
        moneyLadderBoard = new MoneyLadderBoard();
        moneyOrMobBoard = new MoneyOrMobBoard();
        generalTextBoard = new GeneralTextBoard();
        askTheMobLifelineBoard = new AskTheMobLifelineBoard();
        pollTheMobLifelineBoard = new PollTheMobLifelineBoard();
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
            case BoardStatusMessageOptions.DisableSelectingAnswer:
                qnABoard.DisableSelectingAnswer();
                break;
            case BoardStatusMessageOptions.ShowSelectedAnswer:
                qnABoard.OnAudioTrackFinished();
                break;
            case BoardStatusMessageOptions.ForceSelectAnswer:
                qnABoard.Answer_OnClick(Convert.ToChar(extraData));
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
            case BoardStatusMessageOptions.AskTheMobLifelineBoard:
                Board.Content = askTheMobLifelineBoard;
                break;
            case BoardStatusMessageOptions.PollTheMobLifelineBoard:
                Board.Content = pollTheMobLifelineBoard;
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
    public void MobMessageReceiver(int number, MobMemberStatusMessageOptions status)
    {
        switch (status)
        {
            case MobMemberStatusMessageOptions.CreateMobMember:
                AddMobMember(number);
                break;
            case MobMemberStatusMessageOptions.MarkWrongMobMember:
                MarkWrongMobMember(number);
                break;
            case MobMemberStatusMessageOptions.DisableMobMember:
                DisableMobMember(number);
                break;
            case MobMemberStatusMessageOptions.HighlightMobMember:
                HighlightMobMember(number);
                break;
            case MobMemberStatusMessageOptions.ClearMobMemberHighlight:
                ClearMobMemberHighlight(number);
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

    private void HighlightMobMember(int number)
    {
        mobMemberControls[number].HighlightMobMember();
    }

    private void ClearMobMemberHighlight(int number)
    {
        mobMemberControls[number].ClearMobMemberHighlight();
    }
}

public class MobMemberStatusMessage(int memberNumber, MobMemberStatusMessageOptions status)
{
    public int MemberNumber { get; } = memberNumber;
    public MobMemberStatusMessageOptions Status { get; } = status;
}

public enum MobMemberStatusMessageOptions
{
    CreateMobMember,
    MarkWrongMobMember,
    DisableMobMember,
    HighlightMobMember,
    ClearMobMemberHighlight
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
    DisableSelectingAnswer,
    ShowSelectedAnswer,
    ForceSelectAnswer,
    ShowCorrectAnswer,
    ResetQnABoard,
    MoneyLadderBoard,
    MoneyOrMobBoard,
    GeneralTextBoard,
    AskTheMobLifelineBoard,
    PollTheMobLifelineBoard,
    ResetAllBoards,
}