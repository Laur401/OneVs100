using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CommunityToolkit.Mvvm.Input;
using OneVs100.ViewModels;
using OneVs100.Views;

namespace OneVs100.CustomControls;

public partial class QnABoard : UserControl
{

    private IBrush DefaultAnswerBrush = new SolidColorBrush(0xFFB676D3);
    private IBrush DefaultAnswerTextBrush = Brushes.Snow;
    public QnABoard()
    {
        InitializeComponent();
        // Naudojami operatoriai ?. ?[] ?? arba ??= (0.5 t.)
        DefaultAnswerBrush = BorderA.BorderBrush ?? DefaultAnswerBrush;
        //DefaultAnswerTextBrush = AnswerTextA.Foreground ?? DefaultAnswerTextBrush;
    }
    
    private TaskCompletionSource<bool> AudioTrackFinished;
    public void OnAudioTrackFinished()
    {
        AudioTrackFinished?.TrySetResult(true);
    }
    private async Task WaitForAnswerAudioTrack()
    {
        AudioTrackFinished = new TaskCompletionSource<bool>();
        await AudioTrackFinished.Task;
    }

    public void EnableSelectingAnswer() => AnswerLock = false;

    private bool AnswerLock = true;
    private void Answer_OnClick(object? sender, RoutedEventArgs e)
    {
        if (!AnswerLock)
        {
            Dispatcher.UIThread.InvokeAsync(()=>SelectAnswer(sender));
            AnswerLock = true;
        }
    }

    private async Task SelectAnswer(object? sender)
    {
        if (sender is Button button && button.Parent is Border border)
        {
            await WaitForAnswerAudioTrack();
            border.BorderBrush = Brushes.Red;
            if (border.Parent != null)
            {
                foreach (var element in border.Parent.GetLogicalChildren())
                {
                    if (element is Border borderElement && borderElement.Child is TextBlock textBlock)
                    {
                        borderElement.BorderBrush = Brushes.Crimson;
                        borderElement.Background = Brushes.Firebrick;
                        //textBlock.Foreground = Brushes.DarkRed;
                    }
                }
            }
        }
        
    }

    public void ShowCorrectAnswer(char correctAnswer)
    {
        switch (correctAnswer)
        {
            case 'A':
                BorderA.Background = Brushes.Snow;
                foreach (var descendant in BorderA.GetVisualDescendants())
                    if (descendant is TextBlock textBlock)
                        textBlock.Foreground = Brushes.Black;
                break;
            case 'B':
                BorderB.Background = Brushes.Snow;
                foreach (var descendant in BorderB.GetVisualDescendants())
                    if (descendant is TextBlock textBlock)
                        textBlock.Foreground = Brushes.Black;
                break;
            case 'C':
                BorderC.Background = Brushes.Snow;
                foreach (var descendant in BorderC.GetVisualDescendants())
                    if (descendant is TextBlock textBlock)
                        textBlock.Foreground = Brushes.Black;
                break;
            default:
                Console.Error.WriteLine($"Invalid correct answer in {nameof(ShowCorrectAnswer)}");
                break;
        }
    }
    
    public void ResetBoard()
    {
        foreach (var element in Grid.Children)
        {
            if (element is Panel panel)
            {
                foreach (var element2 in panel.Children)
                {
                    if (element2 is Border border)
                    {
                        if (border.Child is Button)
                        {
                            border.BorderBrush = DefaultAnswerBrush;
                            border.Background = Brushes.Transparent;
                            foreach (var descendant in border.GetVisualDescendants())
                                if (descendant is TextBlock textBlock)
                                    textBlock.Foreground = DefaultAnswerTextBrush;
                        }
                        else if (border.Child is TextBlock textBlock)
                        {
                            border.BorderBrush = new SolidColorBrush(0xFF683D6A);
                            border.Background = new SolidColorBrush(0xFF4B2451);
                            //textBlock.Foreground = Brushes.Snow;
                        }
                    }
                }
            }
        }
        AnswerLock = false;
    }
}