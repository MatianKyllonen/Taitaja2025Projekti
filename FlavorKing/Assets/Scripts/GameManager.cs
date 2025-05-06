using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Card selectedCard;
    public RecipeData currentRecipe;

    public int currentFlavorPoints = 0;
    public int maxFlavorPoints = 10;

    public DeckManager deckManager;

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
        usedIngredients.Clear();
        usedTools.Clear();
        usedTechniques.Clear();
        usedSeasonings.Clear();

        deckManager.BeginRound();
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
                        AddFlavorPoints(3);
                }
                break;

            case CardCategory.Tool:
                if (!usedTools.Contains(type.cardName))
                {
                    usedTools.Add(type.cardName);
                    if (type.cardName == currentRecipe.requiredTool)
                        AddFlavorPoints(2);
                }
                break;

            case CardCategory.Technique:
                if (!usedTechniques.Contains(type.cardName))
                {
                    usedTechniques.Add(type.cardName);
                    if (type.cardName == currentRecipe.requiredTechnique)
                        AddFlavorPoints(3);
                }
                break;

            case CardCategory.Seasoning:
                if (!usedSeasonings.Contains(type.cardName))
                {
                    usedSeasonings.Add(type.cardName);
                    if (type.cardName == currentRecipe.preferredSeasoning)
                        AddFlavorPoints(2);
                    else
                        AddFlavorPoints(1);
                }
                break;
        }

        CheckRecipeCompletion();
    }

    void AddFlavorPoints(int amount)
    {
        currentFlavorPoints += amount;
        Debug.Log($"Flavor points added: {amount}. Current: {currentFlavorPoints}/{currentRecipe.flavorPointsRequired}");
    }

    void CheckRecipeCompletion()
    {
        if (currentFlavorPoints >= currentRecipe.flavorPointsRequired)
        {
            Debug.Log("Dish completed successfully!");
        }
    }
}
