using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public GameObject flavorPopupPrefab;
    public RectTransform canvasTransform;
    public Animator fadeAnimator;
    public AudioSource audioSource;
    public AudioClip clickSound;
    public AudioClip playCardSound;


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
        
        int randomFlavorPoints = Random.Range(100, 300); // Random flavor points

        // Randomly select an ingredient, technique, seasoning, and tool
        string randomIngredient = GetRandomCardOfCategory(CardCategory.Ingredient)?.cardName ?? "Unknown Ingredient";
        string randomSeasoning = GetRandomCardOfCategory(CardCategory.Seasoning)?.cardName ?? "";

        if (Random.Range(1, 100) <= 50)
        {
            Debug.Log("Tool required");
            string randomTool = GetRandomCardOfCategory(CardCategory.Tool)?.cardName ?? "";
            currentRecipe.requiredTool = randomTool;
        }
        if (Random.Range(1, 100) <= 50)
        {
            Debug.Log("Technique required");
            string randomTechnique = GetRandomCardOfCategory(CardCategory.Technique)?.cardName ?? "";
            currentRecipe.requiredTechnique = randomTechnique;
        }     

        string randomRecipeName = randomIngredient + " " + randomSeasoning + " " + Random.Range(1, 1000);

        // Create the new random recipe
        currentRecipe = ScriptableObject.CreateInstance<RecipeData>();
        currentRecipe.recipeName = randomRecipeName;
        currentRecipe.flavorPointsRequired = randomFlavorPoints;
        currentRecipe.requiredIngredient = randomIngredient;

        
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
        fadeAnimator.SetTrigger("FadeOut");
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
        audioSource.PlayOneShot(clickSound);
        selectedCard = card;
        Debug.Log("Card selected: " + card.cardType.cardName);
    }

    public void PlayCard(Card card)
    {
        CardType type = card.cardType;

        audioSource.PlayOneShot(playCardSound);
        cardsPlayed++;

        type.ExecuteEffect(cardEffectManager);

        Debug.Log(deckManager.drawsLeft + " " + deckManager.hand.childCount);
        if (deckManager.drawsLeft <= 0 && (deckManager.hand.childCount -1) <= 0)
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

        if (selectedCard != null)
        {
            ShowFlavorPopup(amount, selectedCard.transform.position);
        }
    }

    public void ShowFlavorPopup(int amount, Vector3 worldPosition)
    {
        // Instantiate the popup prefab as a child of the selected card
        GameObject popupGO = Instantiate(flavorPopupPrefab, selectedCard.transform);

        // Get the FlavorPopup component from the instantiated object
        FlavorPopup popup = popupGO.GetComponent<FlavorPopup>();

        // Update the text with a "+" sign for positive amounts
        popup.text.text = (amount > 0 ? "+" : "") + amount.ToString();

        // Set the color to red if the amount is negative, or green for positive
        popup.text.color = amount >= 0 ? Color.green : Color.red;
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
            StartCoroutine(FadeDelay());
        }
        else
        {
            Debug.Log("Game Over! Not enough flavor.");
            StartCoroutine(RestartDelay());
        }
    }

    private IEnumerator FadeDelay()
    {
        fadeAnimator.SetTrigger("FadeIn");
        yield return new WaitForSeconds(0.5f);
        StartRound();
    }

    private IEnumerator RestartDelay()
    {
        fadeAnimator.SetTrigger("FadeIn");
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
