using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class CoinsInventoryDisplay : MonoBehaviour
{
    public List<Coin> coins;

    public void DisableChildrenRecursively()
    {
        foreach (Transform child in transform)
        {
            foreach (Transform childChild in child.transform)
            {
                childChild.gameObject.SetActive(false);
            }
        }
    }
    void Start()
    {
        foreach (Transform child in transform)
        {
            foreach (Transform childChild in child.transform)
            {
                Coin coin = childChild.GetComponent<Coin>();

                if (coin != null)
                {
                    coins.Add(coin);
                }
            }
        }
    }


    public void AddCoin(CardColor cardColor, int amount)
    {
        foreach (Coin coin in coins)
        {
            if (coin.cardColor == cardColor && !coin.gameObject.activeSelf)
            {
                if (amount == 0)
                {
                    break;
                }
                coin.gameObject.SetActive(true);
                amount--;
            }
        }
    }
    public void RemoveCoin(CardColor cardColor, int amount)
    {
        for (int i = coins.Count - 1; i >= 0; i--)
        {
            Coin coin = coins[i];
            if (coin.cardColor == cardColor && coin.gameObject.activeSelf)
            {
                if (amount == 0)
                {
                    break;
                }
                coin.gameObject.SetActive(false);
                amount--;
            }

        }
    }

}
