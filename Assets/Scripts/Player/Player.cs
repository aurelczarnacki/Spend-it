using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Player : NetworkBehaviour
{
    public Dictionary<CardColor, int> coinsInventory = new Dictionary<CardColor, int>();
    public Dictionary<CardColor, int> pasiveCoinsInventory = new Dictionary<CardColor, int>();

    private List<DevelopmentCard> purchasedCards = new List<DevelopmentCard>();
    public int points = 0;
    public CoinsInventoryDisplay coinsInventoryDisplay;
    public CardInventory cardInventory;
    
    public void Start()
    {
        Utility.InitializeColorDictionary(coinsInventory);
        Utility.InitializeColorDictionary(pasiveCoinsInventory);
        coinsInventoryDisplay.DisableChildrenRecursively();
    }
    public void RemoveGem(CardColor color, int amount)
    {
/*        Debug.Log(PlayerQueueManager.Instance.currentPlayer.PlayerName + " - " + color + " " + amount);*/
        coinsInventory[color] -= amount;
        coinsInventoryDisplay.RemoveCoin(color, amount);
        Bank.Instance.AddGemToBank(color, amount);
/*        Debug.Log("Coins: " + Utility.PrintDictionary(coinsInventory));*/
    }
    public void AddGem(CardColor color, int amount)
    {
/*        Debug.Log(PlayerQueueManager.Instance.currentPlayer.PlayerName + " + " + color + " " + amount);*/
        coinsInventory[color] += amount;
        coinsInventoryDisplay.AddCoin(color, amount);
/*        Debug.Log("Coins: " + Utility.PrintDictionary(coinsInventory));*/
    }

    public void AddPoints(int number)
    {
        points += number;
    }
    public int SumCoins()
    {
        int sum = 0;
        foreach (var pair in coinsInventory)
        {
            sum += pair.Value;
        }
        return sum;
    }

    public void AddPurchasedCard(DevelopmentCard card)
    {
        purchasedCards.Add(card);
        cardInventory.AddPurchasedCardDisplay(card);
        pasiveCoinsInventory[card.color] += 1;
        points += card.points;
    }

    public void RemoveLoanCard(DevelopmentCard card)
    {
        cardInventory.RemoveLoanCardDisplay(card);
    }
    public void AddLoanCard(DevelopmentCard card)
    {
        cardInventory.LoanCardDisplay(card);
    }

    public int GetLoanCardCount()
    {
        return cardInventory.reservedCards.Count;
    }
}