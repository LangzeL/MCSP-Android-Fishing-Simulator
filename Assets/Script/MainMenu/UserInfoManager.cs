using UnityEngine;
using UnityEngine.UI;
using Firebase.Auth;
using TMPro;
using System.Collections.Generic;

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
        HashSet<string> fishCaughtSet = new HashSet<string>(userData.fishCaught);
        fishCaughtCountText.text = "Fish Caught: " + fishCaughtSet.Count + " species";
    }
}

