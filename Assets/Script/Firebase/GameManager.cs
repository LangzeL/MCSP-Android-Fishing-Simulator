using Firebase.Auth;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public UserData currentUserData;

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

    // Method to save the current user's data
    public void SaveCurrentUserData()
    {
        string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        DatabaseManager.Instance.SaveUserData(userId, currentUserData);
    }
}
