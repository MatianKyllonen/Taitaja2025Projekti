using UnityEngine;

public enum RecipeType { Ingredient, Seasoning, Technique, Tool }

[CreateAssetMenu(fileName = "Recipe", menuName = "Recipe", order = 1)]
public class RecipeData : ScriptableObject
{
    public string recipeName;
    public int flavorPointsRequired;

    public string requiredIngredient;
    public string requiredTechnique;  // Optional
    public string requiredTool;       // Optional
    public string preferredSeasoning; // Optional bonus
}
