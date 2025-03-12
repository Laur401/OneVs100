namespace OneVs100.ViewModels;

//Naudojate abstrakčią klasę (0.5 t.)
public abstract class PageViewModelBase : ViewModelBase
{
    public ViewChangeDelegate viewChangeDelegate;
    public abstract void OnActivate();
}