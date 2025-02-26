using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace OneVs100.CustomControls;

public partial class MobMemberMiniIcon : UserControl
{
    public MobMemberMiniIcon()
    {
        InitializeComponent();
    }

    public void DisableIcon()
    {
        Border.Background = Brushes.DarkRed;
        Avalonia.Svg.Skia.Svg.SetCss(Sillhouette, ".Person{fill: #000000;}");
    }
}