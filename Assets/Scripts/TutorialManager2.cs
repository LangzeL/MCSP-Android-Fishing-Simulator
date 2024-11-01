using UnityEngine;
using System.Collections;

public class TutorialManager2 : MonoBehaviour
{
    [Header("UI References")]
    public CanvasGroup tutorialCanvasGroup; // Reference to the CanvasGroup component

    [Header("Blink Settings")]
    public float blinkDuration = 3f;        // Total duration for the blinking effect in seconds
    public float blinkInterval = 0.5f;      // Duration for each fade in/out cycle in seconds

    private float elapsedTime = 0f;         // Timer to track the elapsed time
    private bool isFadingIn = true;         // Flag to determine whether to fade in or out

    void Start()
    {
        // Check if the CanvasGroup reference is assigned
        if (tutorialCanvasGroup == null)
        {
            Debug.LogError("TutorialManager: TutorialCanvasGroup is not assigned!");
            return;
        }

        // Ensure the Canvas starts fully opaque
        tutorialCanvasGroup.alpha = 1f;

        // Start the blinking coroutine
        StartCoroutine(BlinkCanvas());
    }

    IEnumerator BlinkCanvas()
    {
        while (elapsedTime < blinkDuration)
        {
            float targetAlpha = isFadingIn ? 1f : 0f;       // Determine the target alpha based on the fading state
            float startAlpha = tutorialCanvasGroup.alpha;  // Current alpha before fading
            float timer = 0f;                              // Timer for the current fade cycle

            while (timer < blinkInterval)
            {
                timer += Time.deltaTime;
                float t = Mathf.Clamp01(timer / blinkInterval); // Normalized time between 0 and 1
                tutorialCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t); // Smoothly interpolate alpha
                yield return null; // Wait for the next frame
            }

            // Toggle the fading state for the next cycle
            isFadingIn = !isFadingIn;
            elapsedTime += blinkInterval; // Update the elapsed time
        }

        // After blinking, hide the Canvas by setting alpha to 0 and deactivating it
        tutorialCanvasGroup.alpha = 0f;
        tutorialCanvasGroup.gameObject.SetActive(false);
    }
}
