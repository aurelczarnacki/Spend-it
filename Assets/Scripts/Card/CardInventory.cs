using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class CardInventory : MonoBehaviour
{
    public Transform reservedCardPrefab;
    public Transform purchasedCardPrefab;
    public Transform patronPrefab;

    public Transform patronsContainer;
    public Transform purchasedCardsContainer;
    public Transform reservedCardsContainer;

    public List<CardDisplay> reservedCards = new List<CardDisplay>();
    private List<PurchasedCardsRow> purchasedCardsRows = new List<PurchasedCardsRow>();

    public float loanPaddingX = 13.5f;
    public int purchasedPaddingX = 3;
    public int patronPaddingX = 20;
    public int paddingY = 15;
    public float paddingZ = 0.1f;
    private int patrons = 0;

    public void AddPurchasedCardDisplay(DevelopmentCard card)
    {
        var matchingRow = purchasedCardsRows.FirstOrDefault(row => row.color == card.color);

        if (matchingRow != null)
        {
            Transform newCard = AddCard(card, matchingRow.transform, purchasedCardPrefab, matchingRow.cards, purchasedPaddingX);
            newCard.transform.Translate(Vector3.back * matchingRow.cards.Count * paddingZ, Space.Self);
        }
        else
        {
            CreateNewPurchasedCardsRow(card);
        }
    }
    private void CreateNewPurchasedCardsRow(DevelopmentCard card)
    {
        GameObject newObject = new GameObject(card.color + " Container");
        PurchasedCardsRow purchasedCardsRow = newObject.AddComponent<PurchasedCardsRow>();
        purchasedCardsRow.color = card.color;
        purchasedCardsRows.Add(purchasedCardsRow);

        newObject.transform.SetParent(purchasedCardsContainer);
        newObject.transform.position = purchasedCardsContainer.position;
        newObject.transform.rotation = purchasedCardsContainer.rotation;
        newObject.transform.Translate(Vector3.down * purchasedCardsRows.Count * paddingY, Space.Self);

        AddCard(card, purchasedCardsRow.transform, purchasedCardPrefab, purchasedCardsRow.cards, purchasedPaddingX);
    }
    public void LoanCardDisplay(DevelopmentCard card)
    {
        AddCard(card, reservedCardsContainer, reservedCardPrefab, reservedCards, loanPaddingX);
    }
    public void RemoveLoanCardDisplay(DevelopmentCard card)
    {
        CardDisplay cardToRemove = null;
        foreach (CardDisplay cardDisplay in reservedCards)
        {
            if(cardDisplay.card == card)
            {
                cardToRemove = cardDisplay;
            }
            else if (cardToRemove != null)
            {
                cardDisplay.transform.Translate(Vector3.right * -loanPaddingX, Space.Self);
            }
        }
        reservedCards.Remove(cardToRemove);
        Destroy(cardToRemove.transform.parent.parent.gameObject);
    }

    private Transform AddCard(DevelopmentCard card, Transform container, Transform cardPrefab, List<CardDisplay> cards, float paddingX)
    {
        Transform cardObject = Instantiate(cardPrefab, container);
        cardObject.transform.Translate(Vector3.right * (cards.Count * paddingX), Space.Self);
        CardDisplay cardDisplay = cardObject.GetComponentInChildren<CardDisplay>();
        cardDisplay.SetCard(card);
        cardDisplay.owner = PlayerQueueManager.Instance.currentPlayer.PlayerName;
        cards.Add(cardDisplay);
        return cardObject;
    }
    public void AddPatron(Patron patron)
    {
        Transform patronObject = Instantiate(patronPrefab, patronsContainer);
        patronObject.transform.Translate(Vector3.right * (patrons * patronPaddingX), Space.Self);
        PatronDisplay patronDisplay = patronObject.GetComponentInChildren<PatronDisplay>();
        patronDisplay.patron = patron;
        patronDisplay.patron.Initialize();
        patrons++;
    }
}
