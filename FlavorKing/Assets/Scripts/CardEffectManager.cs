using UnityEngine;

public class CardEffectManager : MonoBehaviour
{
    //when a technique is played, these are used.
    public bool isSalted;
    public bool isSweetened;
    public bool isSpiced;
    public bool hasDressing;
    public bool isChopped;
    public bool isFried;
    public bool isBoiled;
    public bool isSeasoned;

    //when a tool effect is applied these are used.
    public bool potUsed;
    public bool panUsed;
    public bool knifeUsed;

    //for example, for max meat score, you play pan card, then fry card. Just frying gives score too, but having used the pan to fry gives more

    public void ApplySaltEffect()
    {
        isSeasoned = true;

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
        isSeasoned = true;
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
        isSeasoned = true;
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
        isSeasoned = true;
        if(isSalted)
        {
            GameManager.instance.AddFlavorPoints(-25);
            Debug.Log("Dont add dressing to salted food");
            return;
        }
        GameManager.instance.AddFlavorPoints(50);
        Debug.Log("Dressing added zest! +50 flavor points.");
        hasDressing = true;
    }

    public void ApplyBoilingEffect()
    {
        isBoiled = true;
        if(isSeasoned)
        {
            GameManager.instance.AddFlavorPoints(50);
            Debug.Log("Has dressing +50");
        }
        if (isSalted)
        {
            if(potUsed)
            {
                GameManager.instance.AddFlavorPoints(100);
                Debug.Log("Salted food and pot used to boil, +100");
                return;
            }

            GameManager.instance.AddFlavorPoints(50);
            Debug.Log("Boiling + salted! +50 flavor points.");
        }
        else
        {
            GameManager.instance.AddFlavorPoints(20);
            Debug.Log("Boiling without pot or salt +20");
        }       
    }

    public void ApplyFryingEffect()
    {
        isFried = true;

        if(isSeasoned)
        {          
            GameManager.instance.AddFlavorPoints(50);
            Debug.Log("Food was seasoned");
        }  

        if(isBoiled)
        {
            GameManager.instance.AddFlavorPoints(50);
            Debug.Log("Boilind before frying makes it taste better. +50");

        }
        if(panUsed)
        {
            GameManager.instance.AddFlavorPoints(100);
            Debug.Log("Pan was used to fry, + 100 flavor");
        }
        else
        {           
            GameManager.instance.AddFlavorPoints(30);
            Debug.Log("Fried without pan, +30");
        }
    }

    public void ApplyChopEffect()
    {
        if(knifeUsed)
        {
            isChopped =true;
            GameManager.instance.AddFlavorPoints(100);            
        }
        else
        {
            isChopped = true;
            GameManager.instance.AddFlavorPoints(20);
            Debug.Log("Chopping without knife, only +20");
        }      
    }

    public void ApplyKnifeEffect()
    {
        knifeUsed = true;
    }

    public void ApplyPotEffect()
    {
        potUsed = true;
    }

    public void ApplyPanEffect()
    {
        panUsed = true;
    }
}
