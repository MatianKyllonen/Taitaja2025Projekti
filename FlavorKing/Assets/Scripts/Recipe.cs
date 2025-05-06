using UnityEngine;

public enum RecipeType { Ingredient, Seasoning, Technique, Tool }

[CreateAssetMenu(fileName = "Recipe", menuName = "Recipe", order = 1)]
public class RecipeData : ScriptableObject
{
    public string recipeName;

    // Add flavor meter requirements (example)
    public int flavorPointsRequired;
    public string requiredIngredient;
    public string requiredTechnique;
    public string requiredTool;
    public string preferredSeasoning; // Optional bonus
}
