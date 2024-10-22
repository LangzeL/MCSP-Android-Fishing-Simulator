using UnityEngine;
using Firebase.Database;
using Firebase.Extensions;
using System.Collections.Generic;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager Instance;

    void Awake()
    {
        // Ensure there is only one instance of DatabaseManager
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

    public void SaveUserData(string userId, UserData userData)
    {
        string jsonData = JsonUtility.ToJson(userData);

        FirebaseManager.Instance.databaseRef.Child("users").Child(userId).SetRawJsonValueAsync(jsonData).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("User data saved successfully");
            }
            else
            {
                Debug.LogError($"Failed to save user data: {task.Exception}");
            }
        });
    }

    public void LoadUserData(string userId, System.Action<UserData> onDataLoaded)
    {
        FirebaseManager.Instance.databaseRef.Child("users").Child(userId).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    string jsonData = snapshot.GetRawJsonValue();
                    UserData userData = JsonUtility.FromJson<UserData>(jsonData);
                    Debug.Log("User data loaded successfully");
                    onDataLoaded?.Invoke(userData);
                }
                else
                {
                    Debug.Log("User data does not exist");
                    onDataLoaded?.Invoke(null);
                }
            }
            else
            {
                Debug.LogError($"Failed to load user data: {task.Exception}");
            }
        });
    }
}

[System.Serializable]
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
