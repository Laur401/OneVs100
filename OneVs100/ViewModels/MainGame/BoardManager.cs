using System;
using CommunityToolkit.Mvvm.Messaging;
using OneVs100.Views.MainGame;

namespace OneVs100.ViewModels.MainGame;

public class BoardManager
{
    private static Lazy<BoardManager> lazyInstance = new Lazy<BoardManager>(() => new BoardManager());
    public static BoardManager Instance => lazyInstance.Value;
    private BoardManager() { }
    
    public void LoadQnABoard()
    {
        WeakReferenceMessenger.Default.Send(new BoardStatusMessage(BoardStatusMessageOptions.QnABoard));
    }

    public void LoadMoneyLadderBoard()
    {
        WeakReferenceMessenger.Default.Send(new BoardStatusMessage(BoardStatusMessageOptions.MoneyLadderBoard));
    }

    public void LoadMoneyOrMobBoard()
    {
        WeakReferenceMessenger.Default.Send(new BoardStatusMessage(BoardStatusMessageOptions.MoneyOrMobBoard));
    }

    public void LoadGeneralTextBoard()
    {
        WeakReferenceMessenger.Default.Send(new BoardStatusMessage(BoardStatusMessageOptions.GeneralTextBoard));
    }

    public void ResetAllBoards()
    {
        WeakReferenceMessenger.Default.Send(new BoardStatusMessage(BoardStatusMessageOptions.ResetAllBoards));
    }
}