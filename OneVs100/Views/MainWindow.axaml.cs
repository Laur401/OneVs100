using Avalonia.Controls;
using CommunityToolkit.Mvvm.Messaging;
using OneVs100.CustomControls;
using OneVs100.ViewModels;

namespace OneVs100.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        //DataContext = new MainWindowViewModel(this);
        
        WeakReferenceMessenger.Default.Register<MainWindow, MobMemberStatusMessage>(
            this, (recipient, message) =>
            {
                recipient.MessageReceiver(message.MemberNumber, message.Status);
            });
    }

    public void MessageReceiver(int number, int status)
    {
        switch (status)
        {
            case 0:
                AddMobMember(number);
                break;
            case 1:
                DisableMobMember(number);
                break;
            default:
                break;
        }
    }
    
    //add Event stuff to trigger the methods
    public void AddMobMember(int number)
    {
        MobMemberControl mobMemberControl = new MobMemberControl();
        mobMemberControl.MemberNumber = number;
        MobStorage.Children.Add(mobMemberControl);
    }

    public void DisableMobMember(int number)
    {
        if (MobStorage.Children[number-1] is MobMemberControl mobMemberControl)
        {
            mobMemberControl.DisableMobMember();
        }
    }
}

public class MobMemberStatusMessage(int memberNumber, int status)
{
    public int MemberNumber { get; } = memberNumber;
    public int Status { get; } = status; //TODO: Convert status to an Enum
}
