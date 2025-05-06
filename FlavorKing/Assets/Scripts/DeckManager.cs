using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
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
                cardComp.isUsable = false;

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

        // Get the appropriate data deck (CardType list)
        List<CardType> deck = category switch
        {
            CardCategory.Ingredient => ingredientDeck,
            CardCategory.Seasoning => seasoningDeck,
            CardCategory.Technique => techniqueDeck,
            CardCategory.Tool => toolDeck,
            _ => null
        };

        // Get the pile position for visual reference
        Transform pilePos = category switch
        {
            CardCategory.Ingredient => ingredientPilePos,
            CardCategory.Seasoning => seasoningPilePos,
            CardCategory.Technique => techniquePilePos,
            CardCategory.Tool => toolPilePos,
            _ => null
        };

        // Ensure the deck and pile position are valid
        if (deck == null || deck.Count == 0 || pilePos == null)
        {
            Debug.Log("No cards left in this deck or invalid pile position.");
            return;
        }

        // Grab the topmost card (first card) from the pile
        GameObject cardGO = pilePos.GetChild(0).gameObject; // Assuming the first child is the card
        Card cardComponent = cardGO.GetComponent<Card>();

        // Get the corresponding CardType (the one linked to the topmost card)
        CardType cardType = cardComponent.cardType;

        cardComponent.FlipCard(true); // Show the front side of the card
        // Add the card to the player's hand and remove it from the deck (since it's now drawn)
        currentHand.Add(cardType);
        deck.Remove(cardType); // Remove from the deck
        cardsDrawnThisRound++; // Increment the drawn cards count

        // Set the card visuals (images, etc.)
        cardComponent.SetCardImages(); // Update visuals

        // Move the card to the player's hand
        cardGO.transform.DOMove(hand.position + new Vector3(Random.Range(-2f, 2f), 0, 0), 0.5f)
            .SetEase(Ease.OutQuad).OnComplete(() =>
            {
                cardGO.transform.SetParent(hand); // Reparent to the hand after animation
                cardComponent.isUsable = true; // Mark the card as usable
            });

        // Optionally, you can disable the card's interaction with the pile
    }


    public void DiscardCard(CardType card)
    {
        // Find the correct card GameObject in the hand
        foreach (Transform child in hand)
        {
            Card cardComponent = child.GetComponent<Card>();
            if (cardComponent != null && cardComponent.cardType == card)
            {
                // Animate the card to the discard pile
                child.DOMove(Vector3.zero, 0.5f).SetEase(Ease.OutQuad).OnComplete(() =>
                {
                    // Remove the card from the visual hand (this removes it from the hand game object)
                    Destroy(child.gameObject); // Destroy the GameObject from the hand

                    // Remove the card from the data structure (current hand)
                    currentHand.Remove(cardComponent.cardType);

                    // Add the card to the discard pile
                    discardedCards.Add(cardComponent.cardType);
                });

                // Optionally, you could break the loop here if you only expect to discard one card
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
