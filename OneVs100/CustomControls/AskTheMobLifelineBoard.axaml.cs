using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using OneVs100.ViewModels.MainGame;

namespace OneVs100.CustomControls;

public partial class AskTheMobLifelineBoard : UserControl
{
    public AskTheMobLifelineBoard()
    {
        InitializeComponent();
        //DataContext = LifelineManager.Instance;
        /*var mob1 = new MobMemberControl();
        mob1.GetObservable(MobMemberControl.MemberNumberProperty);
        mob1.Bind()
        mob1.MemberNumber = DataContext.GetObservable()
        Mob1.Child = ;
        Mob2.Child = new MobMemberControl();*/
        
    }
}