using System.ComponentModel.DataAnnotations;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using OneVs100.ViewModels.MainGame;

namespace OneVs100.CustomControls;

public partial class AskTheMobLifelineBoard : UserControl
{
    private MobMemberControl mob1;
    private MobMemberControl mob2;
    public AskTheMobLifelineBoard()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        Dispatcher.UIThread.Invoke(() =>
        {
            mob1 = new MobMemberControl();
            mob2 = new MobMemberControl();
            if (DataContext is MainGameViewModel vm)
            {
                mob1.MemberNumber = vm.AskTheMobOneNumber;
                mob2.MemberNumber = vm.AskTheMobTwoNumber;
            }
            Mob1.Child = mob1;
            Mob2.Child = mob2;
        });
    }
}