using UnityEngine;
using UnityEngine.UI;

public class FishFightController : MonoBehaviour
{
    [Header("Fishing Settings")]
    [Tooltip("Time in seconds before the fish bites after casting.")]
    public float fishBiteDelay = 5f;

    [Tooltip("Player's handedness.")]
    public Handedness playerHandedness = Handedness.Right;

    [Header("UI Elements")]
    [Tooltip("Reference to the 'Fishing...' UI panel.")]
    public GameObject fishingPanel;

    [Tooltip("UI Text to display status messages.")]
    public Text statusText;

    [Tooltip("UI Slider to represent progress.")]
    public Slider progressBar;

    [Tooltip("UI Text to display progress percentage.")]
    public Text progressText;

    // Enums
    public enum Handedness { Left, Right }

    // Internal variables
    private bool isFishing = false;
    private bool isFishBiting = false;
    private float fishBiteTime;
    private float lastPullTime;
    private int consecutiveTooHardPulls = 0;
    private int consecutiveNoPulls = 0;
    private float progress = 0f;

    // Constants
    private const float minValidPullAcceleration = 1.5f;
    private const float maxValidPullAcceleration = 2.5f;
    private const float tooHardPullAcceleration = 2.5f;
    private const float progressPerSecond = 10f; // Progress increases by 10% per second during valid pulls
    private const float progressDropPerWarning = 5f; // Progress decreases by 5% per warning
    private const int maxWarnings = 3; // Maximum allowed consecutive warnings
    private const float pullDetectionTimeWindow = 0.5f; // Time window to detect pull gesture
    private const float noPullInterval = 1f; // Time interval to check for no pulls
    private const float tooHardPullCooldownDuration = 0.2f; // 0.2-second cooldown after a too-hard pull

    // Pull gesture detection variables
    private float gestureStartTime = 0f;
    private PullGestureState pullState = PullGestureState.None;

    private enum PullGestureState { None, FirstPhase }

    // Cooldown flag
    private bool canDetectTooHardPull = true;

    void Start()
    {
        // Initially, isFishing is false
    }

    /// <summary>
    /// Starts the fish bite process after casting.
    /// </summary>
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
            // Fish bites the bait
            isFishBiting = false;
            StartFishingStage();
        }

        if (isFishing)
        {
            UpdateFishing();
        }
    }

    /// <summary>
    /// Initiates the fishing stage after the fish bites.
    /// </summary>
    void StartFishingStage()
    {
        isFishing = true;

        // Remove the "Fishing..." panel
        if (fishingPanel != null)
        {
            Destroy(fishingPanel);
        }
        else
        {
            Debug.LogError("FishingPanel is not assigned in FishFightController.");
        }

        // Vibrate the phone twice to indicate fish biting the bait
        Vibration.Vibrate(500); // First vibration for 500 ms
        Debug.Log("Vibration should have started (first time)");
        Invoke(nameof(VibrateAgain), 0.5f); // Schedule second vibration after 0.5 seconds

        // Initialize variables
        lastPullTime = Time.time;
        consecutiveTooHardPulls = 0;
        consecutiveNoPulls = 0;
        progress = 0f;
        UpdateProgressUI();
    }

    void VibrateAgain()
    {
        Vibration.Vibrate(500); // Second vibration for 500 ms
        Debug.Log("Vibration should have started (second time)");
    }

    /// <summary>
    /// Updates the fishing logic, handling pull detection and progress tracking.
    /// </summary>
    void UpdateFishing()
    {
        // Check for pulling gestures
        DetectPullGesture();

        // Check for warnings due to no pulls
        if (Time.time - lastPullTime >= noPullInterval)
        {
            consecutiveNoPulls++;
            lastPullTime = Time.time; // Reset the timer
            progress -= progressDropPerWarning;
            if (progress < 0f) progress = 0f;
            UpdateProgressUI();

            if (consecutiveNoPulls >= maxWarnings)
            {
                // Failed: fish runs away
                isFishing = false;
                ShowStatus("Failed: fish runs away");
                Debug.Log("Fishing failed: fish runs away");
                // Handle failure (e.g., reset game)
            }
            else
            {
                ShowStatus($"Pull! ({consecutiveNoPulls}/{maxWarnings})");
                Debug.Log($"No pull detected. Warning {consecutiveNoPulls}/{maxWarnings}");
                Vibration.Vibrate(300); // Vibration for warning
                Debug.Log("Vibration triggered for no pull warning.");
                progress -= progressDropPerWarning; // Drop progress by 5%
                if (progress < 0f) progress = 0f;
                UpdateProgressUI();
            }
        }
    }

    /// <summary>
    /// Detects the pulling gesture during the fishing stage.
    /// </summary>
    void DetectPullGesture()
    {
        Vector3 accel = Input.acceleration;
        float accelX = accel.x; // Left/Right acceleration

        // Determine the direction thresholds based on handedness
        float pullThreshold = 0.5f;

        float firstPhaseThreshold = 0f;
        if (playerHandedness == Handedness.Right)
        {
            // For right-handed: pull is acceleration to the right (negative X)
            firstPhaseThreshold = -pullThreshold; // Acceleration X < -0.5
        }
        else
        {
            // For left-handed: pull is acceleration to the left (positive X)
            firstPhaseThreshold = pullThreshold; // Acceleration X > 0.5
        }

        float pullAccel = 0f;

        // Determine pull acceleration based on handedness
        if (playerHandedness == Handedness.Right)
        {
            pullAccel = -accelX; // Positive value indicates pull towards self
        }
        else
        {
            pullAccel = accelX; // Positive value indicates pull towards self
        }

        switch (pullState)
        {
            case PullGestureState.None:
                // Check for first phase of the pull
                if ((playerHandedness == Handedness.Right && accelX < firstPhaseThreshold) ||
                    (playerHandedness == Handedness.Left && accelX > firstPhaseThreshold))
                {
                    pullState = PullGestureState.FirstPhase;
                    gestureStartTime = Time.time;
                    Debug.Log("Pull gesture first phase detected");
                }
                break;

            case PullGestureState.FirstPhase:
                // Check if time window has elapsed
                if (Time.time - gestureStartTime > pullDetectionTimeWindow)
                {
                    // Time window elapsed without completing gesture
                    pullState = PullGestureState.None;
                    Debug.Log("Pull gesture timed out");
                    break;
                }

                // Check for valid pull magnitude along pull direction
                if (pullAccel >= minValidPullAcceleration)
                {
                    lastPullTime = Time.time;
                    consecutiveNoPulls = 0;
                    pullState = PullGestureState.None;

                    if (pullAccel >= minValidPullAcceleration && pullAccel <= maxValidPullAcceleration)
                    {
                        // Valid pull
                        progress += progressPerSecond * Time.deltaTime;
                        if (progress > 100f) progress = 100f;
                        UpdateProgressUI();

                        Debug.Log($"Valid pull detected. Progress: {progress}%");

                        // Check for success
                        if (progress >= 100f)
                        {
                            isFishing = false;
                            ShowStatus("Success! You have the fish");
                            Debug.Log("Fishing succeeded");
                            // Handle success (e.g., reward player)
                        }
                    }
                    else if (pullAccel > tooHardPullAcceleration)
                    {
                        // Too hard pull
                        if (canDetectTooHardPull)
                        {
                            consecutiveTooHardPulls++;
                            progress -= progressDropPerWarning;
                            if (progress < 0f) progress = 0f;
                            UpdateProgressUI();
                            Vibration.Vibrate(300); // Vibration for too hard pull
                            Debug.Log("Vibration triggered due to too hard pull.");
                            ShowStatus($"Too Hard! ({consecutiveTooHardPulls}/{maxWarnings})");
                            Debug.Log($"Too hard pull detected. Warning {consecutiveTooHardPulls}/{maxWarnings}");

                            if (consecutiveTooHardPulls >= maxWarnings)
                            {
                                isFishing = false;
                                ShowStatus("Failed: your rod breaks");
                                Debug.Log("Fishing failed: rod breaks");
                                // Handle failure (e.g., reset game)
                            }

                            // Set cooldown
                            canDetectTooHardPull = false;
                            Invoke(nameof(ResetTooHardPullCooldown), tooHardPullCooldownDuration);
                        }
                    }
                    else
                    {
                        // Pull is too weak but above minimum threshold, consider as valid
                        progress += progressPerSecond * Time.deltaTime;
                        if (progress > 100f) progress = 100f;
                        UpdateProgressUI();

                        Debug.Log($"Weak pull detected but accepted. Progress: {progress}%");

                        // Check for success
                        if (progress >= 100f)
                        {
                            isFishing = false;
                            ShowStatus("Success! You have the fish");
                            Debug.Log("Fishing succeeded");
                            // Handle success (e.g., reward player)
                        }
                    }
                }
                break;
        }
    }

    /// <summary>
    /// Resets the cooldown flag allowing too-hard pulls to be detected again.
    /// </summary>
    void ResetTooHardPullCooldown()
    {
        canDetectTooHardPull = true;
    }

    /// <summary>
    /// Updates the progress UI elements.
    /// </summary>
    void UpdateProgressUI()
    {
        if (progressBar != null)
        {
            progressBar.value = progress / 100f; // Assuming progressBar.maxValue = 1
        }

        if (progressText != null)
        {
            progressText.text = $"Progress: {Mathf.RoundToInt(progress)}%";
        }
    }

    /// <summary>
    /// Displays a status message on the UI.
    /// </summary>
    void ShowStatus(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
    }
}
