using System;
using CommunityToolkit.Mvvm.Input;

namespace OneVs100.ViewModels;

public partial class MainMenuViewModel : PageViewModelBase
{
    public MainMenuViewModel()
    {
        
    }
    
    [RelayCommand]
    public void StartGame()
    {
        viewChangeDelegate.Invoke(this, Windows.MainGame);
    }

    public override void OnActivate()
    {
        
    }
}

