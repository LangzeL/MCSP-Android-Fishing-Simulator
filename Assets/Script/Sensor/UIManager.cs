using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject fishingPanel;

    void Start()
    {
        if (fishingPanel != null)
        {
            fishingPanel.SetActive(false);
        }

        // Subscribe to the fishing start event
        FishingGestureDetector gestureDetector = FindObjectOfType<FishingGestureDetector>();
        if (gestureDetector != null)
        {
            gestureDetector.OnFishingStart += ShowFishingPanel;
        }
    }

    void ShowFishingPanel()
    {
        if (fishingPanel != null)
        {
            fishingPanel.SetActive(true);
            Debug.Log("Fishing panel displayed.");
        }
    }

    void OnDestroy()
    {
        // Unsubscribe from the event
        FishingGestureDetector gestureDetector = FindObjectOfType<FishingGestureDetector>();
        if (gestureDetector != null)
        {
            gestureDetector.OnFishingStart -= ShowFishingPanel;
        }
    }
}
