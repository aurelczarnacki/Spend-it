using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System;

public class CardDisplay : MonoBehaviour
{
    public Image backgroundImage;

    public TextMeshProUGUI points;
    public Image pointImage;
    public Image leftPanelBackground;

    public TextMeshProUGUI blackCost;
    public TextMeshProUGUI greenCost;
    public TextMeshProUGUI redCost;
    public TextMeshProUGUI blueCost;
    public TextMeshProUGUI whiteCost;

    public Sprite blackIcon;
    public Sprite greenIcon;
    public Sprite redIcon;
    public Sprite blueIcon;
    public Sprite whiteIcon;
    public String owner;

    public DevelopmentCard card { get; private set; }
    public float alpha = 0.7f;

    public float pulseDuration = 1.0f;

    private Color originalColor;
    private bool isPulsing = false;

    public void SetCard(DevelopmentCard newCard)
    {
        newCard.Initialize();
        card = newCard;
        if (card.points > 0)
        {
            points.gameObject.SetActive(true);
            points.text = card.points.ToString();
        }
        else
        {
            points.gameObject.SetActive(false);
        }
        backgroundImage.sprite = card.image;
        originalColor = backgroundImage.color;
        SetPanelColor(card.color);
        SetCostTextAndVisibility(blackCost, card.blackCost);
        SetCostTextAndVisibility(greenCost, card.greenCost);
        SetCostTextAndVisibility(redCost, card.redCost);
        SetCostTextAndVisibility(blueCost, card.blueCost);
        SetCostTextAndVisibility(whiteCost, card.whiteCost);
    }
    public void OnBuyClick()
    {
        if ( string.IsNullOrEmpty(owner) || owner == PlayerQueueManager.Instance.currentPlayer.PlayerName)
        {
            bool isPurchased = Bank.Instance.cardStore.TryGetCard(this);
            if (!isPurchased && !isPulsing)
            {
                StartCoroutine(PulseBackground());
            }
        }
    }
    public void OnLoanClick()
    {
        bool isLoaned = Bank.Instance.cardStore.TryLoaneCard(this);
        if (!isLoaned && !isPulsing)
        {
            StartCoroutine(PulseBackground());
        }
    }
    void SetCostTextAndVisibility(TextMeshProUGUI costText, int costValue)
    {
        if (costValue > 0)
        {
            costText.text = costValue.ToString();
            costText.transform.parent.gameObject.SetActive(true);
        }
        else
        {
            costText.transform.parent.gameObject.SetActive(false);
        }
    }
    void SetPanelColor(CardColor color)
    {
        switch (color)
        {
            case CardColor.Blue:

                leftPanelBackground.color = new Color(0.0f, 0.0f, 1.0f, alpha);
                pointImage.sprite = blueIcon;
                break;

            case CardColor.Green:
                leftPanelBackground.color = new Color(0.0f, 1.0f, 0.0f, alpha);
                pointImage.sprite = greenIcon;
                break;

            case CardColor.Red:
                leftPanelBackground.color = new Color(1.0f, 0.0f, 0.0f, alpha);
                pointImage.sprite = redIcon;
                break;

            case CardColor.White:
                leftPanelBackground.color = new Color(1.0f, 1.0f, 1.0f, alpha);
                pointImage.sprite = whiteIcon;
                break;

            case CardColor.Black:
                leftPanelBackground.color = new Color(0.0f, 0.0f, 0.0f, alpha);
                pointImage.sprite = blackIcon;
                break;

            default:
                Debug.Log("Przy tworzeniu karty wyst¹pi³ b³¹d koloru");
                break;
        }
    }
    IEnumerator PulseBackground()
    {
        isPulsing = true;
        float elapsedTime = 0f;

        while (elapsedTime < pulseDuration)
        {
            float t = elapsedTime / pulseDuration;
            backgroundImage.color = Color.Lerp(originalColor, Color.red, Mathf.PingPong(t * 2f, 1f));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        backgroundImage.color = originalColor;
        isPulsing = false;
    }
}
