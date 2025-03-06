using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using OneVs100.ViewModels.MainGame;

namespace OneVs100.CustomControls;

public partial class OneVsXHorizontalElement : UserControl
{
    public OneVsXHorizontalElement()
    {
        InitializeComponent();
        DataContext = MobMemberManager.Instance;
    }
}