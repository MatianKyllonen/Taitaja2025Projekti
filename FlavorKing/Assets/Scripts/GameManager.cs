using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Card selectedCard;
    public RecipeData currentRecipe;
    public TextMeshProUGUI flavorPointsText;
    public CardEffectManager cardEffectManager;
    public TextMeshProUGUI drawsText;

    public int currentFlavorPoints = 0;
    public int cardsPlayed = 0;
    public int maxCardsPerRound = 8;

    public DeckManager deckManager;
    public List<RecipeData> allRecipes;

    private HashSet<string> usedIngredients = new();
    private HashSet<string> usedTools = new();
    private HashSet<string> usedTechniques = new();
    private HashSet<string> usedSeasonings = new();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void StartRound()
    {
        currentFlavorPoints = 0;
        cardsPlayed = 0;

        usedIngredients.Clear();
        usedTools.Clear();
        usedTechniques.Clear();
        usedSeasonings.Clear();

        deckManager.BeginRound();

        if (currentRecipe == null || allRecipes.Count == 0)
        {
            Debug.LogWarning("No recipes available!");
            return;
        }

        currentRecipe = allRecipes[Random.Range(0, allRecipes.Count)];
        flavorPointsText.text = currentFlavorPoints.ToString();

        Debug.Log($"Round started! Recipe: {currentRecipe.recipeName}. Required flavor: {currentRecipe.flavorPointsRequired}");
    }

    public void SelectCard(Card card)
    {
        selectedCard = card;
        Debug.Log("Card selected: " + card.cardType.cardName);
    }

    public void PlayCard(Card card)
    {
        CardType type = card.cardType;

        switch (type.category)
        {
            case CardCategory.Ingredient:
                if (!usedIngredients.Contains(type.cardName))
                {
                    usedIngredients.Add(type.cardName);
                    if (type.cardName == currentRecipe.requiredIngredient)
                        AddFlavorPoints(30);
                }
                break;

            case CardCategory.Tool:
                if (!usedTools.Contains(type.cardName))
                {
                    usedTools.Add(type.cardName);
                    if (type.cardName == currentRecipe.requiredTool)
                        AddFlavorPoints(20);
                }
                break;

            case CardCategory.Technique:
                if (!usedTechniques.Contains(type.cardName))
                {
                    usedTechniques.Add(type.cardName);
                    if (type.cardName == currentRecipe.requiredTechnique)
                        AddFlavorPoints(30);
                }
                break;

            case CardCategory.Seasoning:
                if (!usedSeasonings.Contains(type.cardName))
                {
                    usedSeasonings.Add(type.cardName);
                    if (type.cardName == currentRecipe.preferredSeasoning)
                        AddFlavorPoints(20);
                    else
                        AddFlavorPoints(10);
                }
                break;
        }

        cardsPlayed++;

        if (cardsPlayed >= maxCardsPerRound)
        {
            EndRound();
        }
        else
        {
            CheckRecipeCompletion(); // Optional mid-turn feedback
        }
    }

    void AddFlavorPoints(int amount)
    {
        currentFlavorPoints += amount;
        flavorPointsText.text = currentFlavorPoints.ToString();
        Debug.Log($"Flavor points added: {amount}. Current: {currentFlavorPoints}/{currentRecipe.flavorPointsRequired}");
    }

    void CheckRecipeCompletion()
    {
        if (currentFlavorPoints >= currentRecipe.flavorPointsRequired)
        {
            Debug.Log("Dish completed successfully!");
        }
    }

    void EndRound()
    {
        Debug.Log("Round finished.");
        if (currentFlavorPoints >= currentRecipe.flavorPointsRequired)
        {
            Debug.Log("Success! Loading next recipe...");
            StartRound(); // Or wait for player to click “Next”
        }
        else
        {
            Debug.Log("Game Over! Not enough flavor.");
            // Add a Game Over screen or return to menu
        }
    }
}
