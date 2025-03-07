using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.Messaging;

namespace OneVs100.Views;

public partial class AppUI : Window
{
    public AppUI()
    {
        InitializeComponent();
        WeakReferenceMessenger.Default.Register<AppUI, CloseWindowMessage>(
            this, (recipient, message) =>
            {
                recipient.CloseWindow(message.Status);
            });
    }

    private void CloseWindow(bool status)
    {
        if (status)
            Close();
    }
}

public class CloseWindowMessage(bool status)
{
    public bool Status = status;
}