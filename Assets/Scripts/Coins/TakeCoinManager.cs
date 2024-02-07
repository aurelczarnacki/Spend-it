using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;

public class TakeCoinManager : NetworkBehaviour, IPointerClickHandler
{
    public CoinsInventoryDisplay takenCoinsDisplay;
    public Dictionary<CardColor, int> takenCoinsInventory = new Dictionary<CardColor, int>();

    public Transform takeButton;

    void Start()
    {
        Utility.InitializeColorDictionary(takenCoinsInventory);
        takenCoinsDisplay.DisableChildrenRecursively();
        takeButton.gameObject.SetActive(false);
    }
    public void TakeButtonOnClick()
    {
        foreach (CardColor color in takenCoinsInventory.Keys.ToList())
        {
            int sumCoins = takenCoinsInventory[color];
            if(sumCoins > 0)
            {
                PlayerQueueManager.Instance.currentPlayer.player.AddGem(color, sumCoins);
                takenCoinsDisplay.RemoveCoin(color, sumCoins);
                takenCoinsInventory[color] -= sumCoins;
            }
        }
        if (PlayerQueueManager.Instance.currentPlayer.isLocalPlayer)
        {
            PlayerQueueManager.Instance.EndTurn();
        }
        takeButton.gameObject.SetActive(false);
    }
    [Command(requiresAuthority = false)]
    public void CmdTakeButtonOnClick()
    {
        RpcTakeButtonOnClick();
    }
    [ClientRpc]
    public void RpcTakeButtonOnClick()
    {
        TakeButtonOnClick();
    }

    private bool ShouldShowTakeButton()
    {
        if (takenCoinsInventory.Values.Count(value => value == 1) == 3)
        {
            return true;
        }
        else if (takenCoinsInventory.Values.Count(value => value == 2) == 1 && takenCoinsInventory.Values.Count(value => value > 0) == 1)
        {
            return true;
        }
        return false;
    }
    public void TakeCoin(CardColor color)
    {
        Bank.Instance.RemoveGemFromBank(color, 1);
        takenCoinsDisplay.AddCoin(color, 1);

        takenCoinsInventory[color]++;
        if (ShouldShowTakeButton())
        {
            takeButton.gameObject.SetActive(true);
        }
        else
        {
            takeButton.gameObject.SetActive(false);
        }
    }
    [Command(requiresAuthority = false)]
    public void CmdTakeCoin(CardColor color)
    {
        RcpTakeCoin(color);
    }
    [ClientRpc]
    public void RcpTakeCoin(CardColor color)
    {
        TakeCoin(color);
    }
    
    public void RefundCoin(CardColor color)
    {
        Bank.Instance.AddGemToBank(color, 1);
        takenCoinsDisplay.RemoveCoin(color, 1);

        takenCoinsInventory[color]--;
        if (ShouldShowTakeButton())
        {
            takeButton.gameObject.SetActive(true);
        }
        else
        {
            takeButton.gameObject.SetActive(false);
        }
    }
    [Command(requiresAuthority = false)]
    public void CmdRefundCoin(CardColor color)
    {
        RpcRefundCoin(color);
    }
    [ClientRpc]
    public void RpcRefundCoin(CardColor color)
    {
        RefundCoin(color);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        GameObject clickedObject = eventData.pointerCurrentRaycast.gameObject;
        CoinsInventoryDisplay playerInventoryDisplay = clickedObject.GetComponentInParent<CoinsInventoryDisplay>();
        Coin coin = clickedObject.GetComponentInChildren<Coin>();
        if (coin != null)
        {
            if (playerInventoryDisplay == Bank.Instance.playerInventoryDisplay)
            {
                if ((takenCoinsInventory.Values.Count(value => value == 2) == 1) ||
                    (takenCoinsInventory.Values.Count(value => value == 1) == 3) ||
                    (takenCoinsInventory.Values.Count(value => value == 1) == 2 && takenCoinsInventory[coin.cardColor] == 1) ||
                    (Bank.Instance.bankGems[coin.cardColor] < 3 && takenCoinsInventory[coin.cardColor] == 1) ||
                    (PlayerQueueManager.Instance.currentPlayer.player.SumCoins() + takenCoinsInventory.Values.Sum() > 9)) return;
                CmdTakeCoin(coin.cardColor);
            }
            else if (playerInventoryDisplay == takenCoinsDisplay)
            {
                CmdRefundCoin(coin.cardColor);
            }
        }
    }
}
