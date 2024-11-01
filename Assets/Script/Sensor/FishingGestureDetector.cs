using UnityEngine;
using UnityEngine.UI;

public class FishingGestureDetector : MonoBehaviour
{
    public TutorialManager tutorialManager; // Reference to TutorialManager
    private bool hasCasted = false;         // Flag to ensure the tutorial advances only once

    public AudioSource throwRodAudio;
    [Header("Gesture Detection Settings")]
    [Tooltip("Minimum overall acceleration magnitude to consider for gesture detection.")]
    public float magnitudeThreshold = 2.0f; // Magnitude should be greater than 2 m/s²

    [Tooltip("Acceleration threshold for detecting backward movement (i.e., fishing motion) in m/s².")]
    public float backwardAccelerationThreshold = -4.9f; // -0.5g -> -4.9m/s²

    [Tooltip("Time window to detect gesture steps (in seconds).")]
    public float gestureTimeWindow = 0.5f;

    [Tooltip("Cooldown time after a gesture is detected (in seconds).")]
    public float cooldownTime = 1.0f;

    [Header("UI Elements")]
    [Tooltip("UI Text to display Front/Back acceleration.")]
    public Text accelFrontBackText;

    [Tooltip("UI Text to display Up/Down acceleration.")]
    public Text accelUpDownText;

    [Tooltip("UI Text to display Left/Right acceleration.")]
    public Text accelLeftRightText;

    [Tooltip("UI Text to display current acceleration magnitude.")]
    public Text accelMagnitudeText;

    // Event to notify when fishing starts
    public delegate void FishingAction();
    public event FishingAction OnFishingStart;

    // Internal variables for gesture detection
    private float lastGestureTime = -Mathf.Infinity;
    private bool isGestureInProgress = false;
    private float gestureStartTime = 0f;

    // Gesture detection state
    private enum GestureState { None, FirstPhase }
    private GestureState currentState = GestureState.None;

    // Gravity-related variables
    private Vector3 gravity = Vector3.zero;
    private Vector3 accelerationWithoutGravity = Vector3.zero;
    private float gravitySmoothingFactor = 0.9f; // Smoothing factor to estimate gravity

    void Update()
    {
        // Read accelerometer data (in g)
        Vector3 rawAccel = Input.acceleration;

        // Estimate gravity using a low-pass filter
        gravity = Vector3.Lerp(gravity, rawAccel, gravitySmoothingFactor);

        // Subtract gravity to get the actual movement acceleration
        accelerationWithoutGravity = rawAccel - gravity;

        // Convert from g to m/s²
        Vector3 accelerationInMetersPerSecond = accelerationWithoutGravity * 9.81f;

        // Calculate the magnitude in m/s²
        float magnitude = accelerationInMetersPerSecond.magnitude;

        // Update UI
        UpdateAccelerationUI(accelerationInMetersPerSecond.x, accelerationInMetersPerSecond.y, accelerationInMetersPerSecond.z, magnitude);

        // Gesture detection
        DetectFishingGesture(accelerationInMetersPerSecond.y, magnitude);
    }

    /// <summary>
    /// Updates the UI Text elements with current acceleration data (in m/s²).
    /// </summary>
    void UpdateAccelerationUI(float x, float y, float z, float magnitude)
    {
        if (accelFrontBackText != null)
        {
            accelFrontBackText.text = $"Front/Back: {y:F2} m/s²";
        }

        if (accelUpDownText != null)
        {
            accelUpDownText.text = $"Up/Down: {z:F2} m/s²";
        }

        if (accelLeftRightText != null)
        {
            accelLeftRightText.text = $"Left/Right: {x:F2} m/s²";
        }

        if (accelMagnitudeText != null)
        {
            accelMagnitudeText.text = $"Magnitude: {magnitude:F2} m/s²";
        }
    }

    /// <summary>
    /// Detects the fishing start gesture based on acceleration magnitude and backward movement (in m/s²).
    /// </summary>
    void DetectFishingGesture(float accelY, float magnitude)
    {
        // Check if in cooldown period
        if (Time.time - lastGestureTime < cooldownTime)
            return;

        // First phase: Check for backward acceleration (accelY < backwardAccelerationThreshold)
        if (accelY < backwardAccelerationThreshold)
        {
            // Start the first phase
            currentState = GestureState.FirstPhase;
            gestureStartTime = Time.time;
            Debug.Log("First phase (backward acceleration) detected.");
        }

        // Second condition: Check if magnitude exceeds the magnitude threshold (indicating significant motion)
        if (currentState == GestureState.FirstPhase && magnitude > magnitudeThreshold)
        {
            // Fishing gesture is detected
            throwRodAudio.Play();

            if (!hasCasted)
            {
                hasCasted = true;
                tutorialManager.StopBlinking();
                tutorialManager.CloseTutorial();
            }

            OnFishingStart?.Invoke();
            lastGestureTime = Time.time;
            Debug.Log("Fishing gesture detected");

            // Reset state
            ResetGestureDetection();
        }
    }

    /// <summary>
    /// Resets the gesture detection state.
    /// </summary>
    void ResetGestureDetection()
    {
        currentState = GestureState.None;
        gestureStartTime = 0f;
    }
}
