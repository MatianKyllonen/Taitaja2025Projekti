using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public CardType cardType; // Reference to the CardType to get the details of the card (e.g., name, category)

    private Button button;

    public TextMeshProUGUI cardName;
    public TextMeshProUGUI cardDescription;
    public Image cardImage;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnCardClicked); // Attach the method to the button click event

        cardName.text = cardType.cardName;
        cardDescription.text = cardType.cardDescription;
        cardImage = cardType.cardImage;
    }

    public void OnCardClicked()
    {
        GameManager.instance.SelectCard(this);

        if (GameManager.instance.selectedCard == this)
            PlayCard();
    }

    public void PlayCard()
    {
        GameManager.instance.PlayCard(this);
    }
}
