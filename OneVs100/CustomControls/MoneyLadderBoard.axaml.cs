using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using OneVs100.ViewModels;

namespace OneVs100.CustomControls;

public partial class MoneyLadderBoard : UserControl
{
    private List<MobMemberMiniIcon> mobMemberIcons;
    private List<StackPanel> rowsList;
    private List<Border> moneyList;
    private int mobMembersInRow = 10; //TODO: Pass actual number of mob members to this board, reformat to be variable number in each row.
    public MoneyLadderBoard()
    {
        InitializeComponent();
        rowsList = [Row0, Row1, Row2, Row3, Row4, Row5, Row6, Row7, Row8, Row9];
        moneyList = [Money0, Money1, Money2, Money3, Money4, Money5, Money6, Money7, Money8, Money9];
        mobMemberIcons = new List<MobMemberMiniIcon>();
        foreach (StackPanel row in rowsList)
        {
            for (int i = 0; i < mobMembersInRow; i++)
            {
                MobMemberMiniIcon mobMemberIcon = new MobMemberMiniIcon();
                mobMemberIcons.Add(mobMemberIcon);
                row.Children.Add(mobMemberIcon);
            }
        }
    }
    
    private int wrongMobMembers = 0;
    public void AddWrongMobMember()
    {
        mobMemberIcons[wrongMobMembers].DisableIcon();
        wrongMobMembers++;
        AudioPlayer audioPlayer = AudioPlayer.Instance;
        audioPlayer.PlaySound(SoundEffects.MobMemberOut);
        CheckIfRowFull();
    }

    private void CheckIfRowFull()
    {
        if (wrongMobMembers % mobMembersInRow == 0)
        {
            int fullRows = wrongMobMembers/mobMembersInRow; //24/10 = 2
            if (fullRows == 0) return;
            moneyList[fullRows-1].Background = Brushes.Goldenrod;
            {if (moneyList[fullRows-1].Child is TextBlock textBlock) textBlock.Foreground = Brushes.Black;}
            foreach (Border i in moneyList[..(fullRows - 1)])
            {
                i.Background = Brushes.Black;
                if (i.Child is TextBlock textBlock) textBlock.Foreground = Brushes.DimGray;
            }
        }
    }
}
