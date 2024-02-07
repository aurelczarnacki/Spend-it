using System;
using System.Collections.Generic;
using System.Linq;

public static class Utility
{
    public static void InitializeColorDictionary(Dictionary<CardColor, int> dictionary)
    {
        foreach (CardColor color in Enum.GetValues(typeof(CardColor)))
        {
            dictionary[color] = 0;
        }
    }
    public static string PrintDictionary(Dictionary<CardColor, int> dictionary)
    {
        return string.Join(", ", dictionary.Select(entry => $"{entry.Key}: {entry.Value}"));
    }
}
