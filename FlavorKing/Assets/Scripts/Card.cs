using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card : MonoBehaviour
{

    
    public CardType cardType; // Reference to the CardType to get the details of the card (e.g., name, category)

    private Button button;

    public TextMeshProUGUI cardName;
    public TextMeshProUGUI cardDescription;
    public Image cardImage;
    public Image backSideImage;
    public Image frontSideImage;
    public bool isUsable;

    public Sprite[] spritesheet;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnCardClicked); // Attach the method to the button click event
       

        cardName.text = cardType.cardName;
        cardDescription.text = cardType.cardDescription;
        cardImage.sprite = cardType.cardImage;
        SetCardImages(); 
    }

    private void Update()
    {
        isUsable = button.interactable; 
    }

    public void SetCardImages()
    {
        // Determine the index based on the card category
        int categoryIndex = (int)cardType.category;

        // Set the backside image (index 0-3 for Ingredients, Seasoning, Techniques, Tools)
        backSideImage.sprite = spritesheet[categoryIndex];

        // Set the frontside image (index 4 onwards for the same order)
        frontSideImage.sprite = spritesheet[4 + categoryIndex];
    }

    public void OnCardClicked()
    {
        if (!isUsable)
            return;

        GameManager.instance.SelectCard(this);

        if (GameManager.instance.selectedCard == this)
        {
            PlayCard();
            AnimateClick();
        }       
    }

    public void PlayCard()
    {
        GameManager.instance.PlayCard(this);

        // Discard the card after playing
        DeckManager deckManager = FindFirstObjectByType<DeckManager>();
        if (deckManager != null)
        {
            deckManager.DiscardCard(cardType);
        }
        else
        {
            Debug.LogError("DeckManager not found in the scene.");
        }
    }

    void OnMouseOver()
    {

        if (!isUsable)
            return;

        AnimateHoverEnter();
    }

    void OnMouseExit()
    {
        if (!isUsable)
            return;

        AnimateHoverExit();
    }

    private void AnimateHoverEnter()
    {
        if (!isUsable)
            return;
        // Scale up the card slightly on hover
        transform.DOScale(0.75f + 0.25f, 0.2f).SetEase(Ease.OutCubic);
    }

    private void AnimateHoverExit()
    {
        if (!isUsable)
            return;
        // Scale back to the original size when the hover ends
        transform.DOScale(0.75f, 0.2f).SetEase(Ease.OutCubic);
    }

    private void AnimateClick()
    {
        // Add a quick scale-down and scale-up effect on click
        transform.DOScale(1 * 0.9f, 0.1f).SetEase(Ease.InCubic).OnComplete(() =>
        {
            transform.DOScale(1, 0.1f).SetEase(Ease.OutCubic);
        });
    }

    public void FlipCard(bool showFront)
    {
        // Disable interaction during the flip
        isUsable = false;

        // Animate the card flipping
        transform.DORotate(new Vector3(0, 90, 0), 0.2f).SetEase(Ease.InCubic).OnComplete(() =>
        {
            // Switch the visible side after the first half of the flip
            backSideImage.gameObject.SetActive(!showFront);
            frontSideImage.gameObject.SetActive(showFront);

            // Complete the flip animation
            transform.DORotate(new Vector3(0, 0, 0), 0.2f).SetEase(Ease.OutCubic).OnComplete(() =>
            {
                // Re-enable interaction after the flip
                isUsable = true;
            });
        });
    }

    public void DiscardCard()
    {
        FlipCard(false); 
        Destroy(gameObject); 
    }
}
