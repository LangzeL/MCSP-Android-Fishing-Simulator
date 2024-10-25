using UnityEngine;
using UnityEngine.UI;

public class FishingGestureDetector : MonoBehaviour
{
    [Header("Gesture Detection Settings")]
    [Tooltip("Minimum overall acceleration magnitude to consider for gesture detection.")]
    public float magnitudeThreshold = 1.5f;

    [Tooltip("Acceleration threshold for detecting directional movement.")]
    public float directionalThreshold = 0.5f;

    [Tooltip("Time window to detect gesture steps (in seconds).")]
    public float gestureTimeWindow = 0.5f;

    [Tooltip("Cooldown time after a gesture is detected (in seconds).")]
    public float cooldownTime = 1.0f;


    public enum Handedness { Left, Right }
    [Tooltip("Select player's handedness.")]
    public Handedness playerHandedness = Handedness.Right;

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
    private enum GestureState { None, FirstPhase, SecondPhase }
    private GestureState currentState = GestureState.None;

    void Update()
    {
        // Read accelerometer data
        Vector3 accel = Input.acceleration;
        float accelX = accel.x; // Left/Right
        float accelY = accel.y; // Front/Back
        float accelZ = accel.z; // Up/Down
        float magnitude = accel.magnitude;

        // Update UI
        UpdateAccelerationUI(accelX, accelY, accelZ, magnitude);

        // Gesture detection
        DetectFishingGesture(accelX, magnitude);
    }

    /// <summary>
    /// Updates the UI Text elements with current acceleration data.
    /// </summary>
    void UpdateAccelerationUI(float x, float y, float z, float magnitude)
    {
        if (accelFrontBackText != null)
        {
            accelFrontBackText.text = $"Front/Back: {y:F2}";
        }

        if (accelUpDownText != null)
        {
            accelUpDownText.text = $"Up/Down: {z:F2}";
        }

        if (accelLeftRightText != null)
        {
            accelLeftRightText.text = $"Left/Right: {x:F2}";
        }

        if (accelMagnitudeText != null)
        {
            accelMagnitudeText.text = $"Magnitude: {magnitude:F2}";
        }
    }

    /// <summary>
    /// Detects the fishing start gesture based on acceleration magnitude and directional movement.
    /// </summary>
    void DetectFishingGesture(float accelX, float magnitude)
    {
        // Check if in cooldown period
        if (Time.time - lastGestureTime < cooldownTime)
            return;

        // Check if overall magnitude exceeds threshold
        if (magnitude < magnitudeThreshold)
        {
            // Reset if magnitude drops below threshold
            ResetGestureDetection();
            return;
        }

        // Determine the direction based on handedness
        float firstPhaseThreshold = 0f;
        float secondPhaseThreshold = 0f;

        if (playerHandedness == Handedness.Right)
        {
            // For right-handed: acceleration to the left goes above zero then below zero
            firstPhaseThreshold = directionalThreshold;  // Left acceleration > 0.5
            secondPhaseThreshold = -directionalThreshold; // Left acceleration < -0.5
        }
        else
        {
            // For left-handed: acceleration to the right goes above zero then below zero
            firstPhaseThreshold = -directionalThreshold; // Right acceleration < -0.5
            secondPhaseThreshold = directionalThreshold;  // Right acceleration > 0.5
        }

        switch (currentState)
        {
            case GestureState.None:
                // Check for first phase of the gesture
                if ((playerHandedness == Handedness.Right && accelX > firstPhaseThreshold) ||
                    (playerHandedness == Handedness.Left && accelX < firstPhaseThreshold))
                {
                    currentState = GestureState.FirstPhase;
                    gestureStartTime = Time.time;
                    Debug.Log("First phase detected");
                }
                break;

            case GestureState.FirstPhase:
                // Check if time window has elapsed
                if (Time.time - gestureStartTime > gestureTimeWindow)
                {
                    // Time window elapsed without completing gesture
                    ResetGestureDetection();
                    Debug.Log("Gesture timed out");
                    break;
                }

                // Check for second phase of the gesture
                if ((playerHandedness == Handedness.Right && accelX < secondPhaseThreshold) ||
                    (playerHandedness == Handedness.Left && accelX > secondPhaseThreshold))
                {
                    // Gesture completed
                    OnFishingStart?.Invoke();
                    lastGestureTime = Time.time;
                    Debug.Log("Fishing gesture detected");

                    // Reset state
                    ResetGestureDetection();
                }
                break;
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
