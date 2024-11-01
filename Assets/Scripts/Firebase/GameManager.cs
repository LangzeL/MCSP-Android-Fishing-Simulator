using System.Collections.Generic;
using Firebase.Auth;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public UserData currentUserData;
    public List<FishData> allFishData;

    void Awake()
    {
        // Ensure only one instance of GameManager exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Load all fish data
    public void LoadAllFishData()
    {
        // Manually add fish data or load from a resource file
        // Example:
        // allFishData = new List<FishData>();
        // allFishData.Add(new FishData { fishID = "001", name = "Goldfish", score = 10, image = goldfishSprite });
        // ...
    }

    // Method to save the current user's data
    public void SaveCurrentUserData()
    {
        string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        DatabaseManager.Instance.SaveUserData(userId, currentUserData);
    }

    // Call this method when a player catches a fish
    public void OnFishCaught(string fishID, int score)
    {
        // Update player's caught fish data
        currentUserData.fishCaught.Add(fishID);
        // Update player's total score
        currentUserData.totalScore += score;
        // Save user data
        SaveCurrentUserData();
    }
}
