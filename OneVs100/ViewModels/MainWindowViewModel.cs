using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using CommunityToolkit.Mvvm.ComponentModel;
using OneVs100.ViewModels.MainGame;
using OneVs100.ViewModels.MainMenu;

namespace OneVs100.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public MainWindowViewModel()
    {
        foreach (var viewModel in viewModels)
        {
            viewModel.viewChangeDelegate += ChangeView;
        }
        selectedViewModel = viewModels[0];
    }

    private readonly List<PageViewModelBase> viewModels = [
        new MainMenuViewModel(), 
        new MainGameViewModel()
    ];
    private PageViewModelBase selectedViewModel;
    public PageViewModelBase SelectedViewModel
    {
        get => selectedViewModel;
        private set => SetProperty(ref selectedViewModel, value);
    }

    private void ChangeView(object sender, Windows window)
    {
        switch (window) // Could replace with a Dictionary and type match
        {
            case Windows.MainMenu:
                SelectedViewModel = viewModels[0];
                viewModels[0].OnActivate();
                break;
            case Windows.MainGame:
                SelectedViewModel = viewModels[1];
                viewModels[1].OnActivate();
                break;
            default:
                break;
        }
    }
}

public delegate void ViewChangeDelegate(object sender, Windows window);
public enum Windows
{
    MainMenu,
    MainGame
}