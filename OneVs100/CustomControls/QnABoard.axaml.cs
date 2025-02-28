using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using OneVs100.ViewModels;
using OneVs100.Views;

namespace OneVs100.CustomControls;

public partial class QnABoard : UserControl
{
    public QnABoard()
    {
        InitializeComponent();
    }

    private void Answer_OnClick(object? sender, RoutedEventArgs e)
    {
        //TODO: Block selection if answer has already been selected.
        if (sender is Button button && button.Parent is Border border)
        {
            border.BorderBrush = Brushes.Red;
        }
    }
}