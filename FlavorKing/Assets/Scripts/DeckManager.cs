using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DeckManager : MonoBehaviour
{
    public List<CardType> allCards;
    public List<CardType> discardedCards;
    public List<CardType> ingredientDeck = new();
    public List<CardType> seasoningDeck = new();
    public List<CardType> techniqueDeck = new();
    public List<CardType> toolDeck = new();

    public Transform ingredientPilePos;
    public Transform seasoningPilePos;
    public Transform techniquePilePos;
    public Transform toolPilePos;
    public AudioClip drawCardSound;

    public Sprite discardCanOpen;
    public Sprite discardCanClosed;
    public Image discardButton;

    public int cardsPerRound = 8;
    public int drawsLeft = 0;
    public bool isDiscarding;

    public GameObject cardPrefab;
    public Transform hand; // Parent transform for drawn cards

    private List<CardType> currentHand = new();

    private bool isDrawingCard = false; // NEW: Prevent rapid card draws

    public void AddDraw()
    {
        drawsLeft++;
        GameManager.instance.drawsText.text = $"{drawsLeft} / 8";
    }

    private void ClearOldPileCards()
    {
        foreach (Transform child in transform)
        {
            if (child.CompareTag("Card"))
                Destroy(child.gameObject);
        }
    }

    private void ShuffleIntoDecks()
    {
        ClearOldPileCards();

        Shuffle(allCards);

        int ingredientOffset = 0;
        int seasoningOffset = 0;
        int techniqueOffset = 0;
        int toolOffset = 0;
        float offsetStep = 10f;

        ingredientDeck.Clear();
        seasoningDeck.Clear();
        techniqueDeck.Clear();
        toolDeck.Clear();

        foreach (var card in allCards)
        {
            List<CardType> targetDeck = null;
            Transform pilePos = null;
            int offsetIndex = 0;

            switch (card.category)
            {
                case CardCategory.Ingredient:
                    targetDeck = ingredientDeck;
                    pilePos = ingredientPilePos;
                    offsetIndex = ingredientOffset++;
                    break;
                case CardCategory.Seasoning:
                    targetDeck = seasoningDeck;
                    pilePos = seasoningPilePos;
                    offsetIndex = seasoningOffset++;
                    break;
                case CardCategory.Technique:
                    targetDeck = techniqueDeck;
                    pilePos = techniquePilePos;
                    offsetIndex = techniqueOffset++;
                    break;
                case CardCategory.Tool:
                    targetDeck = toolDeck;
                    pilePos = toolPilePos;
                    offsetIndex = toolOffset++;
                    break;
            }

            if (targetDeck != null && pilePos != null)
            {
                targetDeck.Add(card);
                GameObject cardGO = Instantiate(cardPrefab, pilePos.transform);
                Card cardComp = cardGO.GetComponent<Card>();
                cardComp.cardType = card;
                cardComp.SetCardImages();
                cardGO.transform.position = pilePos.position + new Vector3(0, offsetIndex * offsetStep, 0);
                cardGO.transform.rotation = pilePos.rotation;
                cardComp.isUsable = false;

                cardGO.transform.DORotate(new Vector3(0, 180 + Random.Range(-5f, 5f), 0), 0.3f).SetEase(Ease.OutBack);
            }
        }
    }

    public void startDiscarding()
    {
        if (isDiscarding)
        {
            StopDiscarding();
            return;
        }

        discardButton.sprite = discardCanOpen;
        isDiscarding = true;
        GameManager.instance.selectedCard = null;
    }

    public void StopDiscarding()
    {
        isDiscarding = false;
        GameManager.instance.selectedCard = null;
        discardButton.sprite = discardCanClosed;
    }

    public void BeginRound()
    {
        foreach (Transform child in hand)
        {
            Destroy(child.gameObject);
        }

        drawsLeft = cardsPerRound;
        currentHand.Clear();
        ingredientDeck.Clear();
        seasoningDeck.Clear();
        techniqueDeck.Clear();
        toolDeck.Clear();

        ShuffleIntoDecks();

        Debug.Log("Round started! Choose decks to draw from.");
    }

    public void DrawFromDeck(CardCategory category = CardCategory.Ingredient)
    {
        if (isDrawingCard)
        {
            Debug.Log("Wait for previous card to finish drawing.");
            return;
        }

        if (drawsLeft <= 0)
        {
            Debug.Log("No draws left for this round.");
            return;
        }

        GameManager.instance.audioSource.PlayOneShot(drawCardSound);

        List<CardType> deck = category switch
        {
            CardCategory.Ingredient => ingredientDeck,
            CardCategory.Seasoning => seasoningDeck,
            CardCategory.Technique => techniqueDeck,
            CardCategory.Tool => toolDeck,
            _ => null
        };

        Transform pilePos = category switch
        {
            CardCategory.Ingredient => ingredientPilePos,
            CardCategory.Seasoning => seasoningPilePos,
            CardCategory.Technique => techniquePilePos,
            CardCategory.Tool => toolPilePos,
            _ => null
        };

        if (deck == null || deck.Count == 0 || pilePos == null)
        {
            Debug.Log("No cards left in this deck or invalid pile position.");
            return;
        }

        isDrawingCard = true;

        GameObject cardGO = pilePos.GetChild(pilePos.childCount - 1).gameObject;
        Card cardComponent = cardGO.GetComponent<Card>();
        cardGO.transform.SetParent(hand);

        CardType cardType = cardComponent.cardType;

        cardGO.transform.DOKill(); // NEW: Kill existing tweens
        cardComponent.FlipCard(true);

        currentHand.Add(cardType);
        deck.Remove(cardType);
        drawsLeft--;

        cardComponent.SetCardImages();

        Vector3 randomOffset = new Vector3(Random.Range(-2f, 2f), Random.Range(-0.5f, 0.5f), 0f);

        cardGO.transform.DOMove(hand.position + randomOffset, 0.5f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                cardGO.transform.SetParent(null);
                cardGO.transform.SetParent(hand);
                cardComponent.isUsable = true;
                isDrawingCard = false; // Unlock input
            });

        GameManager.instance.drawsText.text = $"{drawsLeft} / 8";
    }

    public void DiscardCard(CardType card, GameObject cardGameObject)
    {
        foreach (Transform child in hand)
        {
            Card cardComponent = child.GetComponent<Card>();
            if (cardComponent != null && cardComponent.cardType == card && cardComponent.gameObject == cardGameObject)
            {
                child.DOKill();
                child.DOMove(Vector3.zero, 0.5f).SetEase(Ease.OutQuad).OnComplete(() =>
                {
                    Destroy(child.gameObject);
                    currentHand.Remove(cardComponent.cardType);
                    discardedCards.Add(cardComponent.cardType);
                });
                break;
            }
        }
    }

    private void Shuffle(List<CardType> deck)
    {
        for (int i = 0; i < deck.Count; i++)
        {
            var temp = deck[i];
            int randomIndex = Random.Range(i, deck.Count);
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }
}
