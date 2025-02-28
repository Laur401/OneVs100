using System;
using System.Reflection.Metadata.Ecma335;
using AvaloniaDialogs.Views;
using CommunityToolkit.Mvvm.Input;

namespace OneVs100.ViewModels;

public partial class MainMenuViewModel : PageViewModelBase
{
    [RelayCommand]
    public void StartGame()
    {
        viewChangeDelegate.Invoke(this, Windows.MainGame);
    }

    public void OpenCredits()
    {
        SingleActionDialog creditsDialog = new SingleActionDialog()
        {
            Message = "Credits:\n" +
                      "Logo: cwashington2019 on DeviantArt.",
            ButtonText = "Close",
        };
        creditsDialog.ShowAsync();
    }

    public void Quit()
    {
        
    }

    public override void OnActivate() { }
}

