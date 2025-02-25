using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace OneVs100.CustomControls;

public partial class MoneyLadderBoard : UserControl
{
    private List<MobMemberMiniIcon> mobMemberIcons;
    private int mobMembersInRow = 10; //TODO: Pass actual number of mob members to this board, reformat to be variable number in each row.
    public MoneyLadderBoard()
    {
        InitializeComponent();
        List<StackPanel> rowsList = [Row0, Row1, Row2, Row3, Row4, Row5, Row6, Row7, Row8, Row9];
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
    public void addWrongMobMember()
    {
        mobMemberIcons[wrongMobMembers].DisableIcon();
        wrongMobMembers++;
    }

    private void CheckIfRowFull()
    {
        
    }
}
