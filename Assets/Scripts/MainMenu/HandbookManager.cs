using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HandbookManager : MonoBehaviour
{
    public GameObject fishItemPrefab;
    public Transform contentParent; // Reference to the ScrollView's Content

    void OnEnable()
    {
        DisplayHandbook();
    }

    void DisplayHandbook()
    {
        // Clear existing content
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        List<FishData> allFish = GameManager.Instance.allFishData;
        List<string> caughtFishIDs = GameManager.Instance.currentUserData.fishCaught;

        foreach (FishData fish in allFish)
        {
            GameObject item = Instantiate(fishItemPrefab, contentParent);
            FishItem fishItem = item.GetComponent<FishItem>();
            bool isCaught = caughtFishIDs.Contains(fish.fishID);
            fishItem.Setup(fish, isCaught);
        }
    }
}
