using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public GameObject handbookCanvas;
    public GameObject userInfoCanvas; 
    public GameObject leaderboardCanvas;

    public void OnLeaderboardButtonClicked()
    {
        leaderboardCanvas.SetActive(true);
        handbookCanvas.SetActive(false);
        userInfoCanvas.SetActive(false);
    }

    public void OnCloseLeaderboardButtonClicked()
    {
        leaderboardCanvas.SetActive(false);
    }

    public void OnUserInfoButtonClicked()
    {
        userInfoCanvas.SetActive(true);
        handbookCanvas.SetActive(false);
        leaderboardCanvas.SetActive(false);
    }

    public void OnCloseUserInfoButtonClicked()
    {
        userInfoCanvas.SetActive(false);
    }

    public void OnHandbookButtonClicked()
    {
        handbookCanvas.SetActive(true);
        userInfoCanvas.SetActive(false);
        leaderboardCanvas.SetActive(false);
    }

    public void OnCloseHandbookButtonClicked()
    {
        handbookCanvas.SetActive(false);
    }
}

