using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class ResetButtonController : MonoBehaviour
{
    [Tooltip("Button to reset the game and reload the scene.")]
    public Button resetButton;

    [Tooltip("The name of the scene to reload. Leave empty to reload the current scene.")]
    public string sceneName = "InGameScenes";

    void Start()
    {
        if (resetButton != null)
        {
            resetButton.onClick.AddListener(ResetScene);

            Text buttonText = resetButton.GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                buttonText.text = "Refish";
            }
        }
    }

    void ResetScene()
    {
        Debug.Log("Reset button clicked - starting reset process");

        // Clear any caught fish and reset UI state if FishFightController exists
        FishFightController controller = FishFightController.Instance;
        if (controller != null)
        {
            controller.ResetFishing(); // Call this first to reset UI
            controller.ClearHookedFish();
        }

        // Find and reset any active NetController
        NetController netController = FindObjectOfType<NetController>();
        if (netController != null)
        {
            netController.ClearWinWindow();
        }

        // Load the specified scene
        if (!string.IsNullOrEmpty(sceneName))
        {
            Debug.Log($"Loading scene: {sceneName}");
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.Log("Reloading current scene...");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        // Add a delayed reset check
        StartCoroutine(EnsureReset());
    }

    IEnumerator EnsureReset()
    {
        yield return new WaitForSeconds(0.1f);

        // Double-check UI state after scene load
        FishFightController controller = FishFightController.Instance;
        if (controller != null)
        {
            controller.ResetFishing();
        }
    }

    void OnDestroy()
    {
        if (resetButton != null)
        {
            resetButton.onClick.RemoveListener(ResetScene);
        }
    }
}