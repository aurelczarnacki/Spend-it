using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Patron", menuName = "Patron")]
public class Patron : ScriptableObject
{
    public Sprite image;
    public int blackReq;
    public int whiteReq;
    public int redReq;
    public int greenReq;
    public int blueReq;
    public int points;
    public Dictionary<CardColor, int> requirements = new Dictionary<CardColor, int>();
    
    
    
    public void Initialize()
    {
        requirements[CardColor.Blue] = blueReq;
        requirements[CardColor.Green] = greenReq;
        requirements[CardColor.Red] = redReq;
        requirements[CardColor.White] = whiteReq;
        requirements[CardColor.Black] = blackReq;
    }
}
