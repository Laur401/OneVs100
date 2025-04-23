using System.Threading.Tasks;
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
                      "Logo: cwashington2019 on DeviantArt.\n" +
                      "Music, Format: NBC\n" +
                      "Based on a game show format by Endemol Shine Nederland and NBC.",
            ButtonText = "Close",
        };
        creditsDialog.ShowAsync();
    }

    public void Quit()
    {
        WeakReferenceMessenger.Default.Send(new CloseWindowMessage(true));
    }

    public async Task ForceRefreshDatabase()
    {
        TwofoldDialog creditsDialog = new TwofoldDialog()
        {
            Message = "Refresh database?",
            PositiveText = "Yes",
            NegativeText = "No"
        };
        if ((await creditsDialog.ShowAsync()).GetValueOrDefault(false))
            await QuestionObtainer.Program.GetQuestions(true);
    }

    public override void OnActivate()
    {
        AudioPlayer.Instance.PlaySound(SoundEffects.MainIntro);
    }
}

