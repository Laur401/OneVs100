using System;
using System.Collections.Generic;

namespace OneVs100.Helpers;

public class RandomList : Random
{
    public void Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Next(n + 1);
            (list[n], list[k]) = (list[k], list[n]);
        }
    }
}