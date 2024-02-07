using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
[Serializable]
public class DevelopmentCard : ScriptableObject
{
    public int tier;
    public Sprite image;
    public CardColor color;
    public int blackCost;
    public int whiteCost;
    public int redCost;
    public int greenCost;
    public int blueCost;
    public int points;
    public Dictionary<CardColor, int> coinsCost = new Dictionary<CardColor, int>();
    public void Initialize()
    {
        coinsCost[CardColor.Blue] = blueCost;
        coinsCost[CardColor.Green] = greenCost;
        coinsCost[CardColor.Red] = redCost;
        coinsCost[CardColor.White] = whiteCost;
        coinsCost[CardColor.Black] = blackCost;
    }
}
