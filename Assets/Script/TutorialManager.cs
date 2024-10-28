using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    public GameObject tutorialCanvas;            // Reference to TutorialCanvas
    public Image tutorialImage;                  // Reference to TutorialImage
    public TextMeshProUGUI tutorialText;                    // Reference to TutorialText
    public List<TutorialStep> tutorialSteps;      // List of all tutorial steps
    public float fadeDuration = 0.5f;            // Duration for fade effects
    public float blinkInterval = 0.5f;           // Interval for blinking effect

    private int currentStepIndex = 0;            // Current tutorial step index
    private bool isBlinking = false;             // Flag to control blinking

    void Start()
    {
        // Check if tutorial has been completed
        if (PlayerPrefs.GetInt("TutorialCompleted", 0) == 0)
        {
            ShowTutorial();
        }
        else
        {
            tutorialCanvas.SetActive(false);
        }

    }

    // Display the tutorial canvas and start with the first step
    public void ShowTutorial()
    {
        tutorialCanvas.SetActive(true);
        currentStepIndex = 0;
        DisplayCurrentStep();
        StartBlinking();
        StartCoroutine(FadeInTutorial());
    }

    // Display the current tutorial step
    void DisplayCurrentStep()
    {
        if (currentStepIndex < tutorialSteps.Count)
        {
            tutorialImage.sprite = tutorialSteps[currentStepIndex].stepImage;
            tutorialText.text = tutorialSteps[currentStepIndex].stepDescription;
        }
        else
        {
            EndTutorial();
        }
    }

    // Proceed to the next tutorial step
    public void NextStep()
    {
        currentStepIndex++;
        if (currentStepIndex < tutorialSteps.Count)
        {
            DisplayCurrentStep();
            StartCoroutine(FadeInTutorial());
        }
        else
        {
            EndTutorial();
        }
    }

    // Skip the tutorial
    public void SkipTutorial()
    {
        EndTutorial();
    }

    // End the tutorial and mark it as completed
    void EndTutorial()
    {
        StopAllCoroutines();
        StartCoroutine(FadeOutTutorial());
        PlayerPrefs.SetInt("TutorialCompleted", 1);
        PlayerPrefs.Save();
    }

    // Start the blinking effect
    void StartBlinking()
    {
        if (!isBlinking)
        {
            isBlinking = true;
            StartCoroutine(BlinkTutorial());
        }
    }

    // Coroutine to handle blinking (fade in and out) for both image and text
    IEnumerator BlinkTutorial()
    {
        while (isBlinking)
        {
            // Fade out
            yield return StartCoroutine(FadeOutTutorialElements());
            // Fade in
            yield return StartCoroutine(FadeInTutorialElements());
        }
    }

    // Fade out the tutorial image and text
    IEnumerator FadeOutTutorialElements()
    {
        float elapsed = 0f;
        Color imgColor = tutorialImage.color;
        Color txtColor = tutorialText.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            tutorialImage.color = new Color(imgColor.r, imgColor.g, imgColor.b, alpha);
            tutorialText.color = new Color(txtColor.r, txtColor.g, txtColor.b, alpha);
            yield return null;
        }

        tutorialImage.color = new Color(imgColor.r, imgColor.g, imgColor.b, 0f);
        tutorialText.color = new Color(txtColor.r, txtColor.g, txtColor.b, 0f);
    }

    // Fade in the tutorial image and text
    IEnumerator FadeInTutorialElements()
    {
        float elapsed = 0f;
        Color imgColor = tutorialImage.color;
        Color txtColor = tutorialText.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            tutorialImage.color = new Color(imgColor.r, imgColor.g, imgColor.b, alpha);
            tutorialText.color = new Color(txtColor.r, txtColor.g, txtColor.b, alpha);
            yield return null;
        }

        tutorialImage.color = new Color(imgColor.r, imgColor.g, imgColor.b, 1f);
        tutorialText.color = new Color(txtColor.r, txtColor.g, txtColor.b, 1f);
    }

    // Coroutine for fading in the entire tutorial panel
    IEnumerator FadeInTutorial()
    {
        CanvasGroup canvasGroup = tutorialCanvas.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            while (canvasGroup.alpha < 1f)
            {
                canvasGroup.alpha += Time.deltaTime / fadeDuration;
                yield return null;
            }
            canvasGroup.alpha = 1f;
        }
    }

    // Coroutine for fading out the entire tutorial panel
    IEnumerator FadeOutTutorial()
    {
        CanvasGroup canvasGroup = tutorialCanvas.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            while (canvasGroup.alpha > 0f)
            {
                canvasGroup.alpha -= Time.deltaTime / fadeDuration;
                yield return null;
            }
            canvasGroup.alpha = 0f;
            tutorialCanvas.SetActive(false);
        }
    }

    // Stop the blinking effect
    public void StopBlinking()
    {
        isBlinking = false;
    }

    // Optional: Reset the tutorial (for testing or user request)
    public void ResetTutorial()
    {
        PlayerPrefs.SetInt("TutorialCompleted", 0);
        PlayerPrefs.Save();
        ShowTutorial();
    }
}


[System.Serializable]
public class TutorialStep
{
    public Sprite stepImage;           // Image for the tutorial step
    public string stepDescription;     // Description text for the step
}