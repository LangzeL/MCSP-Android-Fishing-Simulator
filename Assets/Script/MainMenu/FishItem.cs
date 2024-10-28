using UnityEngine;
using UnityEngine.UI;

public class FishItem : MonoBehaviour
{
    public Image fishImage;
    public Text fishNameText;
    public GameObject lockOverlay; // Lock overlay UI element

    private FishData fishData;

    public void Setup(FishData data, bool isCaught)
    {
        fishData = data;
        fishImage.sprite = data.image;

        if (isCaught)
        {
            fishNameText.text = data.fishName;
        }
        else
        {
            fishNameText.text = "Unknown";
        }

        lockOverlay.SetActive(!isCaught);

        // Optionally, add button event to view details
    }
}

