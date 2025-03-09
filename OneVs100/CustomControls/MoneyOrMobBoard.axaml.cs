using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using SoundFlow.Interfaces;

namespace OneVs100.CustomControls;

public partial class MoneyOrMobBoard : UserControl
{
    public MoneyOrMobBoard()
    {
        InitializeComponent();
    }
    
    private bool SelectLock = false;
    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        if (!SelectLock)
        {
            SelectLock = true;
            if (sender is Button button && button.Parent is Border border)
            {
                border.Background = Brushes.Snow;
            }
        }
    }
    
    public void ResetBoard()
    {
        foreach (var element in Grid.Children)
        {
            if (element is Border border && border.Background == Brushes.Snow)
                border.Background = Brushes.Transparent;
        }
        SelectLock = false;
    }
}