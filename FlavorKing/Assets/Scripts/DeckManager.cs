using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

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


    public int cardsPerRound = 8;
    private int cardsDrawnThisRound = 0;

    public GameObject cardPrefab;
    public Transform hand; // Parent transform for drawn cards

    private List<CardType> currentHand = new();
    public int CardsDrawnThisRound => cardsDrawnThisRound;

    public int RemainingDraws()
    {
        return cardsPerRound - cardsDrawnThisRound;
    }

    private void ClearOldPileCards()
    {
        foreach (Transform child in transform)
        {
            if (child.CompareTag("Card")) // Make sure prefab has this tag!
                Destroy(child.gameObject);
        }
    }



    private void Start()
    {
        BeginRound();
    }

    private void ShuffleIntoDecks()
    {
        ClearOldPileCards();

        foreach (var card in allCards)
        {
            List<CardType> targetDeck = null;
            Transform pilePos = null;

            switch (card.category)
            {
                case CardCategory.Ingredient:
                    targetDeck = ingredientDeck;
                    pilePos = ingredientPilePos;
                    break;
                case CardCategory.Seasoning:
                    targetDeck = seasoningDeck;
                    pilePos = seasoningPilePos;
                    break;
                case CardCategory.Technique:
                    targetDeck = techniqueDeck;
                    pilePos = techniquePilePos;
                    break;
                case CardCategory.Tool:
                    targetDeck = toolDeck;
                    pilePos = toolPilePos;
                    break;
            }

            if (targetDeck != null && pilePos != null)
            {
                targetDeck.Add(card);
                GameObject cardGO = Instantiate(cardPrefab, pilePos.transform);
                Card cardComp = cardGO.GetComponent<Card>();
                cardComp.cardType = card;
                cardComp.SetCardImages();
                cardComp.FlipCard(false); // Show back
                cardGO.transform.position = pilePos.position;
                cardGO.transform.rotation = pilePos.rotation;

                // Optional: slight random rotation or offset
                cardGO.transform.DORotate(new Vector3(0, 180 + Random.Range(-5f, 5f), 0), 0.3f).SetEase(Ease.OutBack);
            }
        }

        Shuffle(ingredientDeck);
        Shuffle(seasoningDeck);
        Shuffle(techniqueDeck);
        Shuffle(toolDeck);
    }

    public void BeginRound()
    {
        cardsDrawnThisRound = 0;
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
        GameManager.instance.drawsText.text = $"{RemainingDraws()} / 8";
        if (cardsDrawnThisRound >= cardsPerRound)
        {
            Debug.Log("You have already drawn 8 cards.");
            return;
        }

        List<CardType> deck = category switch
        {
            CardCategory.Ingredient => ingredientDeck,
            CardCategory.Seasoning => seasoningDeck,
            CardCategory.Technique => techniqueDeck,
            CardCategory.Tool => toolDeck,
            _ => null
        };

        if (deck == null || deck.Count == 0)
        {
            Debug.Log("No cards left in this deck.");
            return;
        }

        var cardType = deck[Random.Range(0, deck.Count)];
        currentHand.Add(cardType);
        deck.Remove(cardType);
        cardsDrawnThisRound++;

        // Determine the pile position based on the card's category
        Transform pilePos = category switch
        {
            CardCategory.Ingredient => ingredientPilePos,
            CardCategory.Seasoning => seasoningPilePos,
            CardCategory.Technique => techniquePilePos,
            CardCategory.Tool => toolPilePos,
            _ => null
        };

        if (pilePos != null)
        {
            // Instantiate visual card
            GameObject cardGO = Instantiate(cardPrefab, pilePos); // Parent to the pile transform
            cardGO.transform.position = pilePos.position;
            cardGO.transform.rotation = pilePos.rotation;
            cardGO.transform.localScale = Vector3.one * 0.75f;

            Card cardComponent = cardGO.GetComponent<Card>();
            cardComponent.FlipCard(true); // Show the back of the card
            cardComponent.cardType = cardType;
            cardComponent.SetCardImages();

            // Animate movement to hand
            cardGO.transform.DOMove(hand.position + new Vector3(Random.Range(-2f, 2f), 0, 0), 0.5f)
                .SetEase(Ease.OutQuad).OnComplete(() =>
                {
                    cardGO.transform.SetParent(hand); // Reparent to the hand after animation
                    cardComponent.isUsable = true;
                });
        }
        else
        {
            Debug.LogWarning("No valid pile position found for the card's category.");
        }
    }
    public void DiscardCard(CardType card)
    {
        if (currentHand.Contains(card))
        {
            // Remove the card from the current hand
            currentHand.Remove(card);

            // Add the card to the discard pile
            discardedCards.Add(card);

            Debug.Log($"Card {card.cardName} has been discarded.");
        }
        else
        {
            Debug.LogWarning("The card is not in the current hand and cannot be discarded.");
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
