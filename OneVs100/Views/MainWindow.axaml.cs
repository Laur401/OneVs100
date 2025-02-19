using System;
using System.Collections.Generic;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Messaging;
using OneVs100.CustomControls;

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
                MarkWrongMobMember(number);
                break;
            case 2:
                DisableMobMember(number);
                break;
            default:
                break;
        }
    }
    
    Dictionary<int, MobMemberControl> mobMemberControls = new Dictionary<int, MobMemberControl>();
    public void AddMobMember(int number)
    {
        MobMemberControl mobMemberControl = new MobMemberControl();
        mobMemberControl.MemberNumber = number;
        //MobStorage.Children.Add(mobMemberControl);
        List<StackPanel> MobStorage = new List<StackPanel>();
        MobStorage.Add(MobStorageTop);
        MobStorage.Add(MobStorageLeft);
        MobStorage.Add(MobStorageRight);
        MobStorage.Add(MobStorageBottom);
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
