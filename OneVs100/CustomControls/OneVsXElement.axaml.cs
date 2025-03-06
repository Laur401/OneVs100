using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using OneVs100.ViewModels.MainGame;

namespace OneVs100.CustomControls;

public partial class OneVsXElement : UserControl
{
    public OneVsXElement()
    {
        InitializeComponent();
        DataContext = MobMemberManager.Instance;
    }
}