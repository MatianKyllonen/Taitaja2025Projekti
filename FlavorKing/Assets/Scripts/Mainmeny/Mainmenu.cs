using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


public class Mainmenu : MonoBehaviour
{
    public GameObject mainmeny;
    public GameObject tutorial;

    public List<GameObject> tutorialImages;
    private int currentImageIndex = 0;

    public void Start()
    {
        mainmeny.SetActive(true);
        tutorial.SetActive(false);
        HideAllTutorialImages();
    }

    public void Playgame()
    {
        mainmeny.SetActive(false);
        tutorial.SetActive(true);
        ShowImage(0); // show first image
    }

    public void NextImage()
{
    if (currentImageIndex < tutorialImages.Count - 1)
    {
        tutorialImages[currentImageIndex].SetActive(false);
        currentImageIndex++;
        tutorialImages[currentImageIndex].SetActive(true);
    }
    else
    {
        tutorialImages[currentImageIndex].SetActive(false); // hide last image
        Debug.Log("done");
        SceneManager.LoadScene("SampleScene");
    }
}


    public void Quit()
    {
        Application.Quit();
    }

    private void ShowImage(int index)
    {
        HideAllTutorialImages();
        if (index >= 0 && index < tutorialImages.Count)
        {
            tutorialImages[index].SetActive(true);
            currentImageIndex = index;
        }
    }

    private void HideAllTutorialImages()
    {
        foreach (var img in tutorialImages)
        {
            img.SetActive(false);
        }
    }
}
