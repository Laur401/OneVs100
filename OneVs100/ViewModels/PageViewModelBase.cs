namespace OneVs100.ViewModels;

public abstract class PageViewModelBase : ViewModelBase
{
    public ViewChangeDelegate viewChangeDelegate;
    public abstract void OnActivate();
}