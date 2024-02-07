using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Bank : MonoBehaviour
{
    public Dictionary<CardColor, int> bankGems = new Dictionary<CardColor, int>();
    public static Bank Instance { get; private set; }
    public CardStore cardStore;
    public CoinsInventoryDisplay playerInventoryDisplay;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            
        }
        else
        {
            Destroy(gameObject);
        }

        InitializeBankGems();
    }


    void InitializeBankGems()
    {
        foreach (CardColor color in System.Enum.GetValues(typeof(CardColor)))
        {
            if (color == CardColor.Gold)
                bankGems[color] = 5;
            else
                bankGems[color] = 7;
        }
    }

    public void RemoveGemFromBank(CardColor color, int amount)
    {
        bankGems[color] -= amount;
        playerInventoryDisplay.RemoveCoin(color, amount);
        Debug.Log("Take " + color + amount);
        Debug.Log(Utility.PrintDictionary(bankGems));
    }


    public void AddGemToBank(CardColor color, int amount)
    {
        bankGems[color] += amount;
        playerInventoryDisplay.AddCoin(color, amount);
        Debug.Log("Add " + color + amount);
        Debug.Log(Utility.PrintDictionary(bankGems));
    }
}