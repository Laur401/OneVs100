using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace OneVs100.CustomControls;

public partial class MobMemberControl : UserControl
{
    public MobMemberControl()
    {
        InitializeComponent();
        
    }
    /*public static readonly StyledProperty<bool> ActiveProperty =
        AvaloniaProperty.Register<Control, bool>(nameof(Active), defaultValue: true);

    public bool Active
    {
        get => GetValue(ActiveProperty);
        set => SetValue(ActiveProperty, value);
    }*/
    public static readonly StyledProperty<int> MemberNumberProperty =
        AvaloniaProperty.Register<Control, int>(nameof(MemberNumber));
    
    public int MemberNumber
    {
        get => GetValue(MemberNumberProperty);
        set
        {
            SetValue(MemberNumberProperty, value);
            MobMemberNumber.Text = value.ToString();
        }
    }

    public void DisableMobMember()
    {
        Border.Background = Brushes.Black;
    }

    public void MobMemberWrong()
    {
        Border.Background = Brushes.Red;
    }
}