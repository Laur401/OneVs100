using Avalonia.Controls;
using OneVs100.ViewModels;

namespace OneVs100.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        //DataContext = new MainWindowViewModel(this);
    }
    //TODO: Add code for instantiating MobMemberControls and binding to MobMember.cs
    /*MobMemberControl uiControl = new MobMemberControl
       {
           DataContext = this
       };
       Panel? storagePanel = window.FindControl<Panel>("MobStorage");
       if (storagePanel != null)
           storagePanel.Children.Add(uiControl);*/
}