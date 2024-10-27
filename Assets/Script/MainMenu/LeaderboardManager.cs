using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;
using Firebase.Extensions;
using System.Collections.Generic;
using TMPro;
using Firebase.Auth;
using static UnityEngine.EventSystems.EventTrigger;

public class LeaderboardManager : MonoBehaviour
{
    public GameObject leaderboardItemPrefab;
    public Transform contentParent;

    // Reference to Current Player Panel UI elements
    public Text currentPlayerRankText;
    public Text currentPlayerNameText;
    public Text currentPlayerScoreText;

    void OnEnable()
    {
        LoadLeaderboardData();
    }

    void LoadLeaderboardData()
    {
        Debug.Log("Loading leaderboard data...");
        DatabaseReference databaseRef = FirebaseDatabase.DefaultInstance.RootReference;
        databaseRef.Child("users").OrderByChild("totalScore").LimitToLast(100).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                foreach (var innerException in task.Exception.InnerExceptions)
                {
                    Debug.LogError("Inner exception: " + innerException.Message);
                }
            }
            else if (task.IsCanceled)
            {
                Debug.LogError("Loading leaderboard data was canceled.");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                Debug.Log("Successfully retrieved leaderboard data.");

                // Clear existing content
                foreach (Transform child in contentParent)
                {
                    Destroy(child.gameObject);
                }

                List<LeaderboardEntry> leaderboardEntries = new List<LeaderboardEntry>();

                foreach (DataSnapshot childSnapshot in snapshot.Children)
                {
                    string username = childSnapshot.Child("username").Value.ToString();
                    int totalScore = int.Parse(childSnapshot.Child("totalScore").Value.ToString());

                    string userId = childSnapshot.Key;

                    LeaderboardEntry entry = new LeaderboardEntry
                    {
                        userId = userId,
                        username = username,
                        totalScore = totalScore
                    };
                    leaderboardEntries.Add(entry);
                }

                // Sort by total score in descending order
                leaderboardEntries.Sort((a, b) => b.totalScore.CompareTo(a.totalScore));

                DisplayLeaderboard(leaderboardEntries);
            }
            else
            {
                Debug.LogError("Failed to load leaderboard data: " + task.Exception);
            }
        });
    }

    void DisplayLeaderboard(List<LeaderboardEntry> entries)
    {
        string currentUserId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        LeaderboardEntry currentPlayerEntry = null;
        int currentPlayerRank = 0;

        int rank = 1;
        foreach (LeaderboardEntry entry in entries)
        {
            if (entry.userId == currentUserId)
            {
                currentPlayerEntry = entry;
                currentPlayerRank = rank;
            }

            GameObject item = Instantiate(leaderboardItemPrefab, contentParent);
            LeaderboardItem leaderboardItem = item.GetComponent<LeaderboardItem>();
            leaderboardItem.Setup(rank, entry.username, entry.totalScore);
            rank++;
        }

        if (currentPlayerEntry != null)
        {
            DisplayCurrentPlayer(currentPlayerEntry, currentPlayerRank);
        }
        else
        {
            GetCurrentPlayerData(currentUserId);
        }
    }


    void GetCurrentPlayerData(string userId)
    {
        DatabaseReference userRef = FirebaseDatabase.DefaultInstance.GetReference("users").Child(userId);
        userRef.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                currentPlayerRankText.text = "100+";
                currentPlayerNameText.text = snapshot.Child("username").Value.ToString();
                currentPlayerScoreText.text = snapshot.Child("totalScore").Value.ToString();
            }
            else
            {
                Debug.LogError("Failed to load current player data: " + task.Exception);
            }
        });
    }

    void DisplayCurrentPlayer(LeaderboardEntry entry, int rank)
    {
        Debug.Log($"Displaying current player: {entry.username}, Rank: {rank}, Score: {entry.totalScore}");
        currentPlayerRankText.text = rank.ToString();
        currentPlayerNameText.text = entry.username;
        currentPlayerScoreText.text = entry.totalScore.ToString();
    }

}

public class LeaderboardEntry
{
    public string userId;
    public string username;
    public int totalScore;
}
