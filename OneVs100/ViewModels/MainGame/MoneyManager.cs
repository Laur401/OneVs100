using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace OneVs100.ViewModels.MainGame;

public class MoneyManager
{
    private static Lazy<MoneyManager> lazyInstance = new Lazy<MoneyManager>(()=> new MoneyManager());
    public static MoneyManager Instance => lazyInstance.Value;
    private MoneyManager() { }
    
    private readonly List<int> moneyLadderValues = [1000, 5000, 10000, 25000, 50000, 75000, 100000, 250000, 500000, 1000000];

    public List<string> InitializeStringValues()
    {
        List<string> values = new List<string>();
        foreach (int val in moneyLadderValues)
        {
            values.Add(val.ToString("N0")+" €");
        }
        return values;
    }
    
    public string GetCurrentPrizeMoney(int wrongMobMemberCount, List<string> moneyLadderValuesString)
    {
        int pos = wrongMobMemberCount/10;
        return pos - 1 >= 0 && pos - 1 < moneyLadderValuesString.Count
            ? moneyLadderValuesString[pos - 1]
            : "0 €";
    }
}