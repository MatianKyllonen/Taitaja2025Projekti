using UnityEngine;

public class CardEffectManager : MonoBehaviour
{
    public bool isSalted;    // Tracks if the food has been salted

    // Effect for the "Salt" card
    public void ApplySaltEffect()
    {
        if (isSalted)
        {
            
        }
        if (Random.value <= 0.1f) // 10% chance
        {
            GameManager.instance.currentFlavorPoints -= 50;
            Debug.Log("Salt made the food go bad! -50 flavor points.");
        }
        else
        {
            GameManager.instance.currentFlavorPoints += 100;
            Debug.Log("Salt improved the flavor! +100 flavor points.");
        }

        isSalted = true;
    }

    // Effect for the "Boiling" card
    public void ApplyBoilingEffect()
    {
        if (isSalted)
        {
            GameManager.instance.currentFlavorPoints += 50;
            Debug.Log("Boiling improved the flavor! +50 flavor points.");
        }
        else
        {
            Debug.Log("Boiling had no effect because the food wasn't salted.");
        }
    }

    // Add more card effects here as needed
}