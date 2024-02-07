using Mirror;
using System.Drawing;
using System.Linq;
using UnityEngine;

public class CardStore : NetworkBehaviour
{
    private Player player;
    private void Update()
    {
        player = PlayerQueueManager.Instance.currentPlayer.player;
    }
    public bool TryGetCard(CardDisplay cardDisplay)
    {
        if (CanAffordCard(cardDisplay.card))
        {
            CmdPurchaseCard(cardDisplay.card.name);
            return true;
        }
        return false;
    }
    public bool TryLoaneCard(CardDisplay cardDisplay)
    {
        if (player.GetLoanCardCount() < 3 && Bank.Instance.bankGems[CardColor.Gold] > 0 && player.SumCoins() < 10)
        {
            CmdLoaneCard(cardDisplay.card.name);
            return true;
        }
        return false;
    }
    [Command(requiresAuthority = false)]
    public void CmdLoaneCard(string cardName)
    {
        RpcLoaneCard(cardName);
    }
    [ClientRpc]
    public void RpcLoaneCard(string cardName)
    {
        LoaneCard(cardName);
    }
    private void LoaneCard(string cardName)
    {
        DevelopmentCard card =  CardLoader.Instance.FindCardByName(cardName);
        player.AddLoanCard(card);

        player.AddGem(CardColor.Gold, 1);
        DeckManager.Instance.TakeCard(card);
        Bank.Instance.RemoveGemFromBank(CardColor.Gold, 1);
        if (PlayerQueueManager.Instance.currentPlayer.isLocalPlayer)
        {
            PlayerQueueManager.Instance.EndTurn();
        }
    }
    private bool CanAffordCard(DevelopmentCard card)
    {
        int totalMissingCoins = 0;
        foreach (var entry in card.coinsCost)
        {
            int missingAmount = entry.Value - player.pasiveCoinsInventory[entry.Key] - player.coinsInventory[entry.Key];
            totalMissingCoins += missingAmount > 0 ? missingAmount : 0;
        }
        totalMissingCoins -= player.coinsInventory[CardColor.Gold];
        return totalMissingCoins < 1;
    }

    [Command(requiresAuthority = false)]
    public void CmdPurchaseCard(string cardName)
    {
        RpcPurchaseCard(cardName);
    }
    [ClientRpc]
    public void RpcPurchaseCard(string cardName)
    {
        PurchaseCard(cardName);
    }

    private void PurchaseCard(string cardName)
    {
        DevelopmentCard card = CardLoader.Instance.FindCardByName(cardName);
        bool isCardInReservedCards = player.cardInventory.reservedCards.Any(cardDisplay => cardDisplay.card == card);
        if (isCardInReservedCards)
        {
            Debug.Log("Remove from loan cards" + card.name);
            player.RemoveLoanCard(card);
        }
        else
        {
            Debug.Log("Remove from deck cards" + card.name);
            DeckManager.Instance.TakeCard(card);
        }
        Debug.Log("Passive before purchase: " + Utility.PrintDictionary(player.pasiveCoinsInventory));
        int totalMissingCoins = 0;
        foreach (var entry in card.coinsCost)
        {
            int finalCost = entry.Value - player.pasiveCoinsInventory[entry.Key];
            if (finalCost > 0)
            {
                if(finalCost <= player.coinsInventory[entry.Key])
                {
                    player.RemoveGem(entry.Key, finalCost);
                }
                else
                {
                    int playerGems = player.coinsInventory[entry.Key];
                    player.RemoveGem(entry.Key, playerGems);
                    totalMissingCoins = finalCost - playerGems;
                }
            }
        }
        if (totalMissingCoins > 0) player.RemoveGem(CardColor.Gold, totalMissingCoins);

        player.AddPurchasedCard(card);
        if (PlayerQueueManager.Instance.currentPlayer.isLocalPlayer)
        {
            PlayerQueueManager.Instance.EndTurn();
        }
    }
}
