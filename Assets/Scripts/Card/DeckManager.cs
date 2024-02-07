using Mirror;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

public class DeckManager : NetworkBehaviour
{
    public Transform cardPrefab;

    public Transform tier1Container;
    public Transform tier2Container;
    public Transform tier3Container;

    private List<DevelopmentCard> tier1Cards = new List<DevelopmentCard>();
    private List<DevelopmentCard> tier2Cards = new List<DevelopmentCard>();
    private List<DevelopmentCard> tier3Cards = new List<DevelopmentCard>();
    public static DeckManager Instance { get; private set; }

    [SyncVar] List<int> Tier1Indexes = new List<int>();
    [SyncVar] List<int> Tier2Indexes = new List<int>();
    [SyncVar] List<int> Tier3Indexes = new List<int>();
    [SyncVar] int RandomCardNumber;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        tier1Cards = CardLoader.Instance.LoadCardsByCategory(1);
        tier2Cards = CardLoader.Instance.LoadCardsByCategory(2);
        tier3Cards = CardLoader.Instance.LoadCardsByCategory(3);

        if (isServer)
        {
            GenerateAllIndexes();
        }

        InitCardsDeck(tier1Container, tier1Cards, Tier1Indexes);
        InitCardsDeck(tier2Container, tier2Cards, Tier2Indexes);
        InitCardsDeck(tier3Container, tier3Cards, Tier3Indexes);
    }

    public void InitCardsDeck(Transform container, List<DevelopmentCard> cards, List<int> indexes)
    {
        List<DevelopmentCard> toRemove = new List<DevelopmentCard>();
        for (int i = 0; i < 4; i++)
        {
            
            DevelopmentCard card = cards[indexes[i]];
            toRemove.Add(card);
            Transform cardObject = Instantiate(cardPrefab, container);

            Vector3 cardPosition = new Vector3(cardObject.position.x + (25 * i), cardObject.position.y, 0);
            cardObject.position = cardPosition;

            CardDisplay cardDisplay = cardObject.GetComponentInChildren<CardDisplay>();
            cardDisplay.SetCard(card);
        }
        foreach(DevelopmentCard card in toRemove)
        {
            cards.Remove(card);
        }       
    }

    [Server]
    public void GenerateTierIndexes(List<DevelopmentCard> cards, List<int> indexes)
    {
        for (int i = 0; i < 4; i++)
        {
            int rand = Random.Range(0, cards.Count);

            while (indexes.Contains(rand))
                rand = Random.Range(0, cards.Count);
            
            indexes.Add(rand);
        }
    }

    public void GenerateAllIndexes()
    {
        GenerateTierIndexes(tier1Cards, Tier1Indexes);
        GenerateTierIndexes(tier2Cards, Tier2Indexes);
        GenerateTierIndexes(tier3Cards, Tier3Indexes);
    }

    [Command(requiresAuthority = false)]
    public void CmdRequestRandomNumber(int tier, string cardName)
    {
        RpcSendRandomNumber(GenerateRandomNumber(tier), cardName);
    }
    [ClientRpc]
    public void RpcSendRandomNumber(int random, string cardName)
    {
        RandomCardNumber = random;
        MakeCard(cardName);

    }
    public int GenerateRandom(List<DevelopmentCard> cards)
    {
        int rand = Random.Range(0, cards.Count - 1);
        while (rand > cards.Count-1)
            rand = Random.Range(0, cards.Count -1);

        return rand;
    }

    private void AddCardToContainer(CardDisplay cardDisplay, List<DevelopmentCard> cards)
    {
        if (cards.Count != 0)
        {
/*            Debug.Log(cards.Count + ", " + RandomCardNumber);*/
            DevelopmentCard card = cards[RandomCardNumber];
            cardDisplay.SetCard(card);
            cards.Remove(card);
        }
        else
        {
            Destroy(cardDisplay.gameObject);
        }
    }
    private CardDisplay FindCardDisplay(DevelopmentCard developmentCard, Transform container)
    {
        CardDisplay cardDisplay = null;
        foreach (CardDisplay card in container.GetComponentsInChildren<CardDisplay>())
        {
            if (card.card == developmentCard)
            {
                cardDisplay = card;
                break;
            }
        }
        return cardDisplay;
    }


    public void TakeCard(DevelopmentCard card)
    {
        if (PlayerQueueManager.Instance.currentPlayer.isLocalPlayer)
        {
            CmdRequestRandomNumber(card.tier, card.name);
        }
    }
    public void MakeCard(string cardName)
    {
        DevelopmentCard card = CardLoader.Instance.FindCardByName(cardName);
        switch (card.tier)
        {
            case 1:
                AddCardToContainer(FindCardDisplay(card, tier1Container), tier1Cards);
                break;

            case 2:
                AddCardToContainer(FindCardDisplay(card, tier2Container), tier2Cards);
                break;

            case 3:
                AddCardToContainer(FindCardDisplay(card, tier3Container), tier3Cards);
                break;
        }
    }
    private int GenerateRandomNumber(int cardTier)
    {
        switch (cardTier)
        {
            case 1:
                return GenerateRandom(tier1Cards);
            case 2:
                return GenerateRandom(tier2Cards);
            case 3:
                return GenerateRandom(tier3Cards);
            default:
                return -1;
        }
    }
}
