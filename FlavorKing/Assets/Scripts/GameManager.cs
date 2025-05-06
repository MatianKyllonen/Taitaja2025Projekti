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
    public TextMeshProUGUI recipeNameText;
    public TextMeshProUGUI recipeRequirementsText;

    public int currentFlavorPoints = 0;
    public int cardsPlayed = 0;
    public int maxCardsPerRound = 8;

    public DeckManager deckManager;
    public List<CardType> allCardTypes;  // Updated to use all card types for generating recipes

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

    private void Start()
    {
        StartRound();
    }

    // Generate a random recipe
    private void GenerateRandomRecipe()
    {
        // Randomly select a recipe name and required flavor points (just an example)
        string randomRecipeName = "Dish " + Random.Range(1, 1000);
        int randomFlavorPoints = Random.Range(150, 500); // Random flavor points

        // Randomly select an ingredient, technique, seasoning, and tool
        string randomIngredient = GetRandomCardOfCategory(CardCategory.Ingredient)?.cardName ?? "Unknown Ingredient";
        string randomTechnique = GetRandomCardOfCategory(CardCategory.Technique)?.cardName ?? "";
        string randomTool = GetRandomCardOfCategory(CardCategory.Tool)?.cardName ?? "";
        string randomSeasoning = GetRandomCardOfCategory(CardCategory.Seasoning)?.cardName ?? "";

        // Create the new random recipe
        currentRecipe = ScriptableObject.CreateInstance<RecipeData>();
        currentRecipe.recipeName = randomRecipeName;
        currentRecipe.flavorPointsRequired = randomFlavorPoints;
        currentRecipe.requiredIngredient = randomIngredient;
        currentRecipe.requiredTechnique = randomTechnique;
        currentRecipe.requiredTool = randomTool;
        currentRecipe.preferredSeasoning = randomSeasoning;
    }

    // Get a random card from a specific category
    private CardType GetRandomCardOfCategory(CardCategory category)
    {
        // Filter cards by category
        List<CardType> filteredCards = allCardTypes.FindAll(card => card.category == category);
        if (filteredCards.Count > 0)
        {
            return filteredCards[Random.Range(0, filteredCards.Count)];
        }
        return null;
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

        if (allCardTypes == null || allCardTypes.Count == 0)
        {
            Debug.LogWarning("No cards available to generate recipes!");
            return;
        }

        // Generate a random recipe
        GenerateRandomRecipe();

        // Display the recipe information
        recipeNameText.text = $"Recipe: {currentRecipe.recipeName}";
        recipeRequirementsText.text =
            $"Ingredient: {currentRecipe.requiredIngredient}\n" +
            (string.IsNullOrEmpty(currentRecipe.requiredTechnique) ? "" : $"Technique: {currentRecipe.requiredTechnique}\n") +
            (string.IsNullOrEmpty(currentRecipe.requiredTool) ? "" : $"Tool: {currentRecipe.requiredTool}\n") +
            (string.IsNullOrEmpty(currentRecipe.preferredSeasoning) ? "" : $"Preferred Seasoning: {currentRecipe.preferredSeasoning}\n") +
            $"Flavor Needed: {currentRecipe.flavorPointsRequired}";

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

        type.ExecuteEffect(cardEffectManager);

        if (cardsPlayed >= maxCardsPerRound)
        {
            EndRound();
        }
        else
        {
            CheckRecipeCompletion(); // Optional mid-turn feedback
        }
    }

    public void AddFlavorPoints(int amount)
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
