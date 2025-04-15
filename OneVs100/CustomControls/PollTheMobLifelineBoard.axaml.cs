using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace OneVs100.CustomControls;

public partial class PollTheMobLifelineBoard : UserControl
{
    public PollTheMobLifelineBoard()
    {
        InitializeComponent();
        SampleMobControl.MobMemberNumber.Text = "";
    }
}