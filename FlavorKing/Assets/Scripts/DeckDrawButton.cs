using UnityEngine;

public class DeckDrawButton : MonoBehaviour
{
    public DeckManager deckManager;

    public void DrawIngredient() => deckManager.DrawFromDeck(CardCategory.Ingredient);
    public void DrawSeasoning() => deckManager.DrawFromDeck(CardCategory.Seasoning);
    public void DrawTechnique() => deckManager.DrawFromDeck(CardCategory.Technique);
    public void DrawTool() => deckManager.DrawFromDeck(CardCategory.Tool);
}
