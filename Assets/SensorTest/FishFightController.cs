using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FishFightController : MonoBehaviour
{
    [Header("Fishing Settings")]
    [Tooltip("Time in seconds before the fish bites after casting.")]
    public float fishBiteDelay = 5f;

    [Header("UI Elements")]
    [Tooltip("Reference to the 'Fishing...' UI panel.")]
    public GameObject fishingPanel;

    [Tooltip("UI Text to display status messages.")]
    public Text statusText;

    [Tooltip("UI Slider to represent progress.")]
    public Slider progressBar;

    [Tooltip("UI Text to display progress percentage.")]
    public Text progressText;

    private bool isFishing = false;
    private bool isFishBiting = false;
    private float fishBiteTime;
    private float lastPullTime;
    private int consecutiveTooHardPulls = 0;
    private int consecutiveNoPulls = 0;
    private float progress = 0f;

    private const float minValidPullAcceleration = 0.5f;
    private const float maxValidPullAcceleration = 3f;
    private const float tooHardPullAcceleration = 3f;
    private const float progressIncrement = 20f;
    private const float progressDropPerWarning = 5f;
    private const int maxWarnings = 3;
    private const float pullDetectionTimeWindow = 0.5f;
    private const float noPullInterval = 1f;
    private const float tooHardPullCooldownDuration = 1f;
    private const float progressUpdateInterval = 0.5f;
    private const string targetSceneName = "TiltTestScene";
    private const float successMessageDuration = 3f;

    private float gestureStartTime = 0f;
    private PullGestureState pullState = PullGestureState.None;
    private float lastProgressUpdateTime = 0f;

    private enum PullGestureState { None, FirstPhase }

    private bool canDetectTooHardPull = true;

    void Start()
    {
        // Hide the UI elements at the start
        if (fishingPanel != null) fishingPanel.SetActive(false);
        if (statusText != null) statusText.gameObject.SetActive(false);
        if (progressBar != null) progressBar.gameObject.SetActive(false);
        if (progressText != null) progressText.gameObject.SetActive(false);
    }

    public void StartFishBite()
    {
        fishBiteTime = Time.time + fishBiteDelay;
        isFishBiting = true;
        Debug.Log($"Fish will bite in {fishBiteDelay} seconds.");
    }

    void Update()
    {
        if (isFishBiting && Time.time >= fishBiteTime)
        {
            isFishBiting = false;
            StartFishingStage();
        }

        if (isFishing)
        {
            UpdateFishing();
        }
    }

    void StartFishingStage()
    {
        isFishing = true;

        // Hide the fishing panel instead of destroying it
        if (fishingPanel != null)
        {
            fishingPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("FishingPanel is not assigned in FishFightController.");
        }

        Vibration.Vibrate(500);
        Debug.Log("Vibration should have started (first time)");
        Invoke(nameof(VibrateAgain), 0.5f);

        lastPullTime = Time.time;
        consecutiveTooHardPulls = 0;
        consecutiveNoPulls = 0;
        progress = 0f;
        lastProgressUpdateTime = Time.time;

        // Show the UI elements when fishing starts
        if (statusText != null) statusText.gameObject.SetActive(true);
        if (progressBar != null) progressBar.gameObject.SetActive(true);
        if (progressText != null) progressText.gameObject.SetActive(true);

        UpdateProgressUI();
    }

    void VibrateAgain()
    {
        Vibration.Vibrate(500);
        Debug.Log("Vibration should have started (second time)");
    }

    void UpdateFishing()
    {
        DetectPullGesture();

        if (Time.time - lastPullTime >= noPullInterval)
        {
            consecutiveNoPulls++;
            lastPullTime = Time.time;
            progress -= progressDropPerWarning;
            if (progress < 0f) progress = 0f;
            UpdateProgressUI();

            if (consecutiveNoPulls >= maxWarnings)
            {
                isFishing = false;
                ShowStatus("Failed: fish runs away");
                Debug.Log("Fishing failed: fish runs away");
            }
            else
            {
                ShowStatus($"Pull! ({consecutiveNoPulls}/{maxWarnings})");
                Debug.Log($"No pull detected. Warning {consecutiveNoPulls}/{maxWarnings}");
                Vibration.Vibrate(300);
                progress -= progressDropPerWarning;
                if (progress < 0f) progress = 0f;
                UpdateProgressUI();
            }
        }
    }

    void DetectPullGesture()
    {
        Vector3 accel = Input.acceleration;
        float accelY = accel.y;
        float magnitude = accel.magnitude;

        if (pullState == PullGestureState.None && accelY < -0.5f)
        {
            pullState = PullGestureState.FirstPhase;
            gestureStartTime = Time.time;
            Debug.Log("Pull gesture first phase detected");
        }

        if (pullState == PullGestureState.FirstPhase)
        {
            if (Time.time - gestureStartTime > pullDetectionTimeWindow)
            {
                pullState = PullGestureState.None;
                Debug.Log("Pull gesture timed out");
                return;
            }

            if (magnitude >= minValidPullAcceleration && magnitude <= maxValidPullAcceleration)
            {
                lastPullTime = Time.time;
                consecutiveNoPulls = 0;
                pullState = PullGestureState.None;

                if (Time.time - lastProgressUpdateTime >= progressUpdateInterval)
                {
                    progress += progressIncrement;
                    if (progress > 100f) progress = 100f;
                    lastProgressUpdateTime = Time.time;
                    UpdateProgressUI();
                    Debug.Log($"Valid pull detected. Progress: {progress}%");

                    if (progress >= 100f)
                    {
                        isFishing = false;
                        ShowStatus("Success! You have the fish");
                        Debug.Log("Fishing succeeded");

                        Invoke(nameof(SwitchToTiltTestScene), successMessageDuration);
                    }
                }
            }
            else if (magnitude > tooHardPullAcceleration && canDetectTooHardPull)
            {
                consecutiveTooHardPulls++;
                progress -= progressDropPerWarning;
                if (progress < 0f) progress = 0f;
                UpdateProgressUI();
                Vibration.Vibrate(300);
                ShowStatus($"Too Hard! ({consecutiveTooHardPulls}/{maxWarnings})");

                if (consecutiveTooHardPulls >= maxWarnings)
                {
                    isFishing = false;
                    ShowStatus("Failed: your rod breaks");
                    Debug.Log("Fishing failed: rod breaks");
                }

                canDetectTooHardPull = false;
                Invoke(nameof(ResetTooHardPullCooldown), tooHardPullCooldownDuration);
            }
        }
    }

    void SwitchToTiltTestScene()
    {
        SceneManager.LoadScene(targetSceneName);
    }

    void ResetTooHardPullCooldown()
    {
        canDetectTooHardPull = true;
    }

    void UpdateProgressUI()
    {
        if (progressBar != null)
        {
            progressBar.value = progress / 100f;
        }

        if (progressText != null)
        {
            progressText.text = $"Progress: {Mathf.RoundToInt(progress)}%";
        }
    }

    void ShowStatus(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
    }
}
