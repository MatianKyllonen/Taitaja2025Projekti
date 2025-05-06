using UnityEngine;
using UnityEngine.UI;

public enum CardCategory { Ingredient, Seasoning, Technique, Tool }

[CreateAssetMenu(fileName = "CardType", menuName = "Card System/CardType", order = 1)]
public class CardType : ScriptableObject
{
    public string cardName;
    public string cardDescription;
    public Sprite cardImage;
    public CardCategory category;
    public GameObject cardPrefab;

    public System.Action<CardEffectManager> cardEffect;

    // Execute the effect using the CardEffectManager
    public void ExecuteEffect(CardEffectManager effectManager)
    {
        cardEffect?.Invoke(effectManager);
    }
}
