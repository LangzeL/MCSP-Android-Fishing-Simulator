using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ResetButtonController : MonoBehaviour
{
    [Tooltip("Button to reset the game and reload the scene.")]
    public Button resetButton; // Reference to the reset button

    [Tooltip("The name of the scene to reload. Leave empty to reload the current scene.")]
    public string sceneName = "InGameScenes"; // Optional scene name

    void Start()
    {
        // Set up the reset button to call the ResetScene method when clicked
        if (resetButton != null)
        {
            resetButton.onClick.AddListener(ResetScene);

            // Set button text if necessary
            Text buttonText = resetButton.GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                buttonText.text = "Reset Game"; // Set desired button text
            }
        }
    }

    /// <summary>
    /// Reloads the specified scene or the current scene if no scene name is provided.
    /// </summary>
    void ResetScene()
    {
        // If a scene name is provided, load that scene; otherwise, reload the current scene
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.Log("Reloading current scene...");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
