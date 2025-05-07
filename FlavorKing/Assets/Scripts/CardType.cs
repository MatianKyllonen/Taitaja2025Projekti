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
        switch (cardName)
        {
            case "Salt": effectManager.ApplySaltEffect(); break;
            case "Dressing": effectManager.ApplyDressingEffect(); break;
            case "Chili": effectManager.ApplySpicyEffect(); break;
            case "Boil": effectManager.ApplyBoilingEffect(); break;
            case "Fry": effectManager.ApplyFryingEffect(); break;
            case "Chop": effectManager.ApplyChopEffect(); break;
            case "Knife": effectManager.ApplyKnifeEffect(); break;
            case "Pot": effectManager.ApplyPotEffect(); break;
            case "Pan": effectManager.ApplyPanEffect(); break;
            default: Debug.Log("No effect assigned for: " + cardName); break;
        }
    }

}

