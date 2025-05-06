using UnityEngine;

public class CardEffectManager : MonoBehaviour
{
    public bool isSalted;
    public bool isSweetened;
    public bool isSpiced;
    public bool hasDressing;
    public bool isChopped;
    public bool isFried;
    public bool isBoiled;

    public void ApplySaltEffect()
    {
        if (isSalted)
        {
            Debug.Log("Too much salt! No effect.");
            return;
        }

        if (Random.value <= 0.1f)
        {
            GameManager.instance.AddFlavorPoints(-50);
            Debug.Log("Salt ruined the dish! -50 flavor points.");
        }
        else
        {
            GameManager.instance.AddFlavorPoints(100);
            Debug.Log("Salt enhanced the dish! +100 flavor points.");
        }

        isSalted = true;
    }

    public void ApplySweetEffect()
    {
        if (isSweetened)
        {
            Debug.Log("Too sweet already. No effect.");
            return;
        }

        GameManager.instance.AddFlavorPoints(75);
        Debug.Log("Sweetness added! +75 flavor points.");
        isSweetened = true;
    }

    public void ApplySpicyEffect()
    {
        if (isSpiced)
        {
            GameManager.instance.AddFlavorPoints(-25);
            Debug.Log("Too spicy! -25 flavor points.");
            return;
        }

        GameManager.instance.AddFlavorPoints(80);
        Debug.Log("Spicy kick added! +80 flavor points.");
        isSpiced = true;
    }

    public void ApplyDressingEffect()
    {
        GameManager.instance.AddFlavorPoints(50);
        Debug.Log("Dressing added zest! +50 flavor points.");
    }

    public void ApplyBoilingEffect()
    {
        if (isSalted)
        {
            GameManager.instance.AddFlavorPoints(50);
            Debug.Log("Boiling + salted! +50 flavor points.");
        }
        else
        {
            Debug.Log("Boiling alone had no effect.");
        }

        isBoiled = true;
    }

    public void ApplyFryingEffect()
    {
        int bonus = isChopped ? 100 : 50;
        GameManager.instance.AddFlavorPoints(bonus);
        Debug.Log($"Fried the food! +{bonus} flavor points. (Bonus if chopped: {isChopped})");

        isFried = true;
    }

    public void ApplyChopEffect()
    {
        isChopped = true;
        GameManager.instance.AddFlavorPoints(50);
        Debug.Log("Chopped ingredients! +50 flavor points.");
    }

    public void ApplyKnifeEffect()
    {
        if (isChopped)
        {
            GameManager.instance.AddFlavorPoints(30);
            Debug.Log("Used knife after chopping! +30 flavor points.");
        }
        else
        {
            Debug.Log("Knife used, but nothing to chop.");
        }
    }

    public void ApplyPotEffect()
    {
        if (isBoiled)
        {
            GameManager.instance.AddFlavorPoints(40);
            Debug.Log("Pot used with boiling! +40 flavor points.");
        }
        else
        {
            Debug.Log("Pot had no effect without boiling.");
        }
    }

    public void ApplyPanEffect()
    {
        if (isFried)
        {
            GameManager.instance.AddFlavorPoints(40);
            Debug.Log("Pan used with frying! +40 flavor points.");
        }
        else
        {
            Debug.Log("Pan had no effect without frying.");
        }
    }
}
