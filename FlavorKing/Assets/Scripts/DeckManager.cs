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
    public int drawsLeft = 0;
    public bool isDiscarding;

    public GameObject cardPrefab;
    public Transform hand; // Parent transform for drawn cards

    private List<CardType> currentHand = new();


    public void AddDraw()
    {
        drawsLeft++;
        GameManager.instance.drawsText.text = $"{drawsLeft} / 8";
    }
    private void ClearOldPileCards()
    {
        foreach (Transform child in transform)
        {
            if (child.CompareTag("Card")) // Make sure prefab has this tag!
                Destroy(child.gameObject);
        }
    }

    private void ShuffleIntoDecks()
    {
        ClearOldPileCards();

        // Offset counters for stacking
        int ingredientOffset = 0;
        int seasoningOffset = 0;
        int techniqueOffset = 0;
        int toolOffset = 0;
        float offsetStep = 10f; // Adjust this for more or less spacing

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
                cardComp.FlipCard(false); // Show back
                cardGO.transform.position = pilePos.position + new Vector3(0, offsetIndex * offsetStep, 0); // Stacking offset
                cardGO.transform.rotation = pilePos.rotation;
                cardComp.isUsable = false;

                // Optional: slight random rotation
                cardGO.transform.DORotate(new Vector3(0, 180 + Random.Range(-5f, 5f), 0), 0.3f).SetEase(Ease.OutBack);
            }
        }

        Shuffle(ingredientDeck);
        Shuffle(seasoningDeck);
        Shuffle(techniqueDeck);
        Shuffle(toolDeck);
    }


    public void startDiscarding()
    {
        isDiscarding = true;
        Debug.Log("Discarding mode activated! Select a cards to discard.");
        GameManager.instance.selectedCard = null; // Reset selected card
    }

    public void BeginRound()
    {
        drawsLeft = cardsPerRound;
        currentHand.Clear();
        ingredientDeck.Clear();
        seasoningDeck.Clear();
        techniqueDeck.Clear();
        toolDeck.Clear();

        ShuffleIntoDecks();

        Debug.Log("Round started! Choose decks to draw from.");
    }

    public void EndRound()
    {
        // Clear the hand and reset the decks
        foreach (Transform child in hand)
        {
            Destroy(child.gameObject);
        }
        currentHand.Clear();
        ingredientDeck.Clear();
        seasoningDeck.Clear();
        techniqueDeck.Clear();
        toolDeck.Clear();
        Debug.Log("Round ended! All cards discarded.");
    }

    public void DrawFromDeck(CardCategory category = CardCategory.Ingredient)
    {
       


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
        GameObject cardGO = pilePos.GetChild(pilePos.childCount -1).gameObject; // Assuming the first child is the card
        Card cardComponent = cardGO.GetComponent<Card>();

        // Get the corresponding CardType (the one linked to the topmost card)
        CardType cardType = cardComponent.cardType;

        cardComponent.FlipCard(true); // Show the front side of the card
        // Add the card to the player's hand and remove it from the deck (since it's now drawn)
        currentHand.Add(cardType);
        deck.Remove(cardType); // Remove from the deck
        drawsLeft--; // Increment the drawn cards count

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

        GameManager.instance.drawsText.text = $"{drawsLeft} / 8";
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
