using AvaloniaDialogs.Views;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using OneVs100.Views;

namespace OneVs100.ViewModels.MainMenu;

public partial class MainMenuViewModel : PageViewModelBase
{
    [RelayCommand]
    public void StartGame()
    {
        AudioPlayer.Instance.StopAllSounds();
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
        WeakReferenceMessenger.Default.Send(new CloseWindowMessage(true));
    }

    public override void OnActivate()
    {
        AudioPlayer.Instance.PlaySound(SoundEffects.MainIntro);
    }
}

