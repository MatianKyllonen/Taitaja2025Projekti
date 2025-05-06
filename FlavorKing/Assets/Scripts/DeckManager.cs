using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public List<CardType> allCards; // All available cards in the game
    public List<CardType> ingredientDeck = new();
    public List<CardType> seasoningDeck = new();
    public List<CardType> techniqueDeck = new();
    public List<CardType> toolDeck = new();

    public int cardsPerRound = 8;
    private int cardsDrawnThisRound = 0;

    public GameObject cardPrefab;
    public Transform hand; // Parent transform for drawn cards

    private List<CardType> currentHand = new();

    private void Start()
    {
        BeginRound();
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

        // Instantiate visual card
        GameObject cardGO = Instantiate(cardPrefab, hand);
        Card cardComponent = cardGO.GetComponent<Card>();
        cardComponent.cardType = cardType;

        Debug.Log($"Drew card: {cardType.cardName} from {category}");
    }
    private void ShuffleIntoDecks()
    {
        foreach (var card in allCards)
        {
            switch (card.category)
            {
                case CardCategory.Ingredient:
                    ingredientDeck.Add(card);
                    break;
                case CardCategory.Seasoning:
                    seasoningDeck.Add(card);
                    break;
                case CardCategory.Technique:
                    techniqueDeck.Add(card);
                    break;
                case CardCategory.Tool:
                    toolDeck.Add(card);
                    break;
            }
        }

        Shuffle(ingredientDeck);
        Shuffle(seasoningDeck);
        Shuffle(techniqueDeck);
        Shuffle(toolDeck);
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
