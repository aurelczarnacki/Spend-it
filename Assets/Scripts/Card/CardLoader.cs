using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CardLoader : MonoBehaviour
{
    public List<DevelopmentCard> developmentCards = new List<DevelopmentCard>();
    public static CardLoader Instance { get; private set; }

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
    }
    public List<DevelopmentCard> LoadCardsByCategory(int tier)
    {
        List<DevelopmentCard> developmentCardsTier = new List<DevelopmentCard>();
        foreach (DevelopmentCard developmentCard in developmentCards)
        {
            if (developmentCard.tier == tier)
            {
                developmentCardsTier.Add(developmentCard);
            }
        }
        return developmentCardsTier;
    }

    public DevelopmentCard FindCardByName(string cardName)
    {
        foreach (DevelopmentCard developmentCard in developmentCards)
        {
            if (developmentCard.name == cardName)
            {
                return developmentCard;
            }
        }
        return null;
    }
}
