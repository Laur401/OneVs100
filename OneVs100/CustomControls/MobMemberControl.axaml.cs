using System;
using System.ComponentModel;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Color = Avalonia.Media.Color;

namespace OneVs100.CustomControls;

public partial class MobMemberControl : UserControl, INotifyPropertyChanged
{
    private bool isDisabled = false;
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
        SetBackground(Brushes.Black.Color);
        isDisabled = true;
    }

    public void MobMemberWrong()
    {
        SetBackground(Brushes.Red.Color);
    }

    public void HighlightMobMember()
    {
        SetBackground(Brushes.Snow.Color);
    }

    public void ClearMobMemberHighlight()
    {
        SetBackground(Color.Parse("#1EC3FF"));
    }
    
    Color lastColor = Color.Parse("#1EC3FF");
    private bool lockLastColor = false;
    public void AnimateBackgroundMobMember(Color color)
    {
        if (isDisabled) return;
        if (!lockLastColor && PanelBackground.Background is VisualBrush { Visual: Panel { Background: RadialGradientBrush radialGradientBrush } })
        {
            lastColor = radialGradientBrush.GradientStops[0].Color;
            lockLastColor = true;
        }
        SetBackground(color);
    }

    public void StopAnimation()
    {
        if (isDisabled) return;
        SetBackground(lastColor);
        lockLastColor = false;
    }
}