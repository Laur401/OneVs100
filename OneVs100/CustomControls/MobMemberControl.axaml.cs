using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using Color = Avalonia.Media.Color;

namespace OneVs100.CustomControls;

public partial class MobMemberControl : UserControl
{
    public MobMemberControl()
    {
        InitializeComponent();
        InitializeBackground(Color.Parse("#1EC3FF"), 105, 1d/9);
    }

    private void SetBackground(Color color)
    {
        InitializeBackground(color, 105, 1d / 9); //temp
    }
    
    private void InitializeBackground(Color defaultColor, double size, double tileScale)
    {
      RadialGradientBrush backgroundSubpanelBrush = new RadialGradientBrush();
      backgroundSubpanelBrush.GradientStops.Add(new GradientStop { Color = defaultColor, Offset = 0.5 });
      backgroundSubpanelBrush.GradientStops.Add(new GradientStop { Color = Brushes.Black.Color, Offset = 1 });
      
      VisualBrush background = new VisualBrush
      {
          TileMode = TileMode.Tile,
          Transform = new ScaleTransform { ScaleX = tileScale, ScaleY = tileScale },
          Visual = new Panel
          {
              Height = size,
              Width = size,
              Background = backgroundSubpanelBrush
          }
      };
      PanelBackground.Background = background;
    }
    
    /*public static readonly StyledProperty<bool> ActiveProperty =
        AvaloniaProperty.Register<Control, bool>(nameof(Active), defaultValue: true);

    public bool Active
    {
        get => GetValue(ActiveProperty);
        set => SetValue(ActiveProperty, value);
    }*/

    private static readonly StyledProperty<int> MemberNumberProperty =
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
        SetBackground(Brushes.Black.Color);
    }

    public void MobMemberWrong()
    {
        SetBackground(Brushes.Red.Color);
    }
}