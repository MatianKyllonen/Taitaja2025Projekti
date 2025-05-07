using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public CardType cardType;
    private Button button;

    public TextMeshProUGUI cardName;
    public TextMeshProUGUI cardDescription;
    public Image cardImage;
    public Image backSideImage;
    public Image frontSideImage;
    public bool isUsable;
    public GameObject cardOutline;
    private Vector3 originalPosition;
    private float originalY;



    public Sprite[] spritesheet;

    private bool isCurrentlySelected = false; // Track selection state
    private Vector3 originalScale; // Store the default scale

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnCardClicked);

        originalScale = transform.localScale; // Capture original scale

        cardName.text = cardType.cardName;
        cardDescription.text = cardType.cardDescription;
        cardImage.sprite = cardType.cardImage;
        SetCardImages();
    }

    private void Update()
    {
        if (GameManager.instance.selectedCard == this)
        {
            if (!isCurrentlySelected)
            {
                SetSelected(true);
            }
        }
        else
        {
            if (isCurrentlySelected)
            {
                SetSelected(false);
            }
        }

        button.enabled = isUsable;
    }

    private void SetSelected(bool selected)
    {
        isCurrentlySelected = selected;
        cardOutline.SetActive(selected);

        float moveOffset = 200f;

        if (selected)
        {
            originalY = transform.localPosition.y; // Store starting Y
            transform.DOScale(originalScale * 1.4f, 0.2f).SetEase(Ease.OutBack);
            transform.DOLocalMoveY(originalY + moveOffset, 0.2f).SetEase(Ease.OutBack);
        }
        else
        {
            transform.DOScale(originalScale, 0.2f).SetEase(Ease.OutBack);
            transform.DOLocalMoveY(originalY, 0.2f).SetEase(Ease.OutBack); // Return to stored original Y
        }
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

        if (GameManager.instance.deckManager.isDiscarding)
        {
            if(GameManager.instance.selectedCard == this)
            {
                GameManager.instance.selectedCard = null;
            }
            else if(GameManager.instance.selectedCard != null)
            {
                DiscardCard();
                GameManager.instance.selectedCard.DiscardCard();
                GameManager.instance.deckManager.AddDraw();
                GameManager.instance.deckManager.StopDiscarding();
            }
        }
        else if (GameManager.instance.selectedCard == this)
        {
            PlayCard();
            AnimateClick();
        }

        GameManager.instance.SelectCard(this);
      
    }

    public void PlayCard()
    {
        isUsable = false;
        GameManager.instance.PlayCard(this);

        // Discard the card after playing
        DeckManager deckManager = FindFirstObjectByType<DeckManager>();
        if (deckManager != null)
        {
            deckManager.DiscardCard(cardType, gameObject);
        }
        else
        {
            Debug.LogError("DeckManager not found in the scene.");
        }
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
            });
        });
    }

    public void DiscardCard()
    {
        FlipCard(false); 
        Destroy(gameObject); 
    }
}
