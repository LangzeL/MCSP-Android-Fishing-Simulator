using UnityEngine;
using UnityEngine.UI;
using Firebase.Auth;
using TMPro;

public class UserInfoManager : MonoBehaviour
{
    public Text usernameText;
    public Text totalScoreText;
    public Text fishCaughtCountText;

    void OnEnable()
    {
        DisplayUserInfo();
    }

    void DisplayUserInfo()
    {
        UserData userData = GameManager.Instance.currentUserData;
        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;

        usernameText.text = "Username: " + userData.username;
        totalScoreText.text = "Total Score: " + userData.totalScore;
        fishCaughtCountText.text = "Fish Caught: " + userData.fishCaught.Count + " species";
    }
}

