using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardItem : MonoBehaviour
{
    public Image avatarImage; // Default avatar image
    public Text rankText;
    public Text usernameText;
    public Text scoreText;

    public void Setup(int rank, string username, int totalScore)
    {
        // Set default avatar (can specify in the Inspector)
        // avatarImage.sprite = defaultAvatarSprite;

        rankText.text = rank.ToString();
        usernameText.text = username;
        scoreText.text = totalScore.ToString();
    }
}

