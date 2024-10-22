using UnityEngine;
using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections.Generic;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager Instance;

    private DatabaseReference databaseReference;

    void Awake()
    {
        // Ensure only one instance of DatabaseManager exists
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

    void Start()
    {
        // Get the root reference location of the database
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    // Save user data to the database
    public void SaveUserData(string userId, UserData userData)
    {
        string jsonData = JsonUtility.ToJson(userData);

        databaseReference.Child("users").Child(userId).SetRawJsonValueAsync(jsonData).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("User data saved successfully.");
            }
            else
            {
                Debug.LogError("Failed to save user data: " + task.Exception);
            }
        });
    }

    // Load user data from the database
    public void LoadUserData(string userId, Action<UserData> onDataLoaded)
    {
        databaseReference.Child("users").Child(userId).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    string jsonData = snapshot.GetRawJsonValue();
                    UserData userData = JsonUtility.FromJson<UserData>(jsonData);
                    Debug.Log("User data loaded successfully.");
                    onDataLoaded?.Invoke(userData);
                }
                else
                {
                    Debug.Log("User data does not exist.");
                    onDataLoaded?.Invoke(null);
                }
            }
            else
            {
                Debug.LogError("Failed to load user data: " + task.Exception);
            }
        });
    }
}


[Serializable]
public class UserData
{
    public string username;
    public int totalScore;
    public int totalAssets;
    public List<string> fishCaught;

    public UserData()
    {
        fishCaught = new List<string>();
    }
}
