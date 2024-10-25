using UnityEngine;
using UnityEngine.UI; // Ensure this namespace is included for UI interactions

/// <summary>
/// Controls the idle movement of the fishing rod based on device tilt.
/// Handles both pre-fishing idle movements and post-fishing rotations.
/// </summary>
public class RodIdleMovement : MonoBehaviour
{
    [Header("Idle Movement Settings")]
    [Tooltip("Sensitivity for position adjustments based on left/right tilt.")]
    private float positionSensitivity = 0.05f;

    [Tooltip("Sensitivity for rotation adjustments based on left/right tilt.")]
    private float rotationSensitivityXY = 5f;

    [Tooltip("Sensitivity for rotation adjustments based on up/down tilt.")]
    private float rotationSensitivityZ = 8f;

    [Header("Fishing Settings")]
    [Tooltip("Reference to the Bait GameObject.")]
    public GameObject bait; // Assign the "Bait" GameObject in the Inspector

    [Tooltip("Prefab for the 'Fishing...' UI window.")]
    public GameObject fishingUIWindowPrefab; // Assign the "Fishing..." UI prefab in the Inspector

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private bool isFishingStarted = false;

    void Start()
    {
        // Store the initial position and rotation of the rod
        initialPosition = transform.position;
        initialRotation = transform.rotation;

        // Ensure the bait is assigned
        if (bait == null)
        {
            Debug.LogError("Bait GameObject is not assigned in the RodIdleMovement script.");
        }

        // Subscribe to the fishing start event
        FishingGestureDetector gestureDetector = FindObjectOfType<FishingGestureDetector>();
        if (gestureDetector != null)
        {
            gestureDetector.OnFishingStart += StartFishing;
        }
        else
        {
            Debug.LogError("FishingGestureDetector not found in the scene. Please ensure it's added and active.");
        }
    }

    void Update()
    {
        // Read accelerometer data
        Vector3 accel = Input.acceleration;

        if (!isFishingStarted)
        {
            // ----- Pre-Fishing Idle Movement -----

            // Extract tilt values
            float tiltX = accel.x; // Left/Right tilt
            float tiltY = accel.y; // Up/Down tilt

            // Calculate new position based on left/right tilt
            float posX = initialPosition.x + tiltX * positionSensitivity * 5;
            float posY = initialPosition.y + tiltX * positionSensitivity * 2; // You can adjust this if Y movement should differ

            // Calculate new rotation based on left/right and up/down tilt
            float rotX = initialRotation.eulerAngles.x + tiltX * rotationSensitivityXY * 2; // Rotation around X-axis
            float rotY = initialRotation.eulerAngles.y + tiltX * rotationSensitivityXY * 2; // Rotation around Y-axis
            float rotZ = initialRotation.eulerAngles.z + tiltY * rotationSensitivityZ * 10;  // Rotation around Z-axis

            // Apply the calculated position and rotation to the rod
            transform.position = new Vector3(posX, posY, initialPosition.z); // Keeping Z position constant
            transform.rotation = Quaternion.Euler(rotX, rotY, rotZ);
        }
        else
        {
            // ----- Post-Fishing Rotation -----

            // Continue to adjust rotation Z based on up/down tilt
            float tiltY = accel.y; // Up/Down tilt

            // Calculate new rotation around Z-axis
            float rotZ = initialRotation.eulerAngles.z + tiltY * rotationSensitivityZ * 10;

            // Maintain initial position and rotation X & Y
            transform.position = initialPosition;
            transform.rotation = Quaternion.Euler(initialRotation.eulerAngles.x, initialRotation.eulerAngles.y, rotZ);
        }
    }

    /// <summary>
    /// Initiates the fishing process by detaching the bait and displaying the fishing UI.
    /// </summary>
    void StartFishing()
    {
        if (isFishingStarted)
            return; // Prevent multiple fishing starts

        isFishingStarted = true;
        Debug.Log("Fishing has started in RodIdleMovement.");

        // Detach the bait from the rod
        if (bait != null)
        {
            // Remove parent to detach
            bait.transform.parent = null;

            // Add Rigidbody if not present for physics interaction
            Rigidbody baitRb = bait.GetComponent<Rigidbody>();
            if (baitRb == null)
            {
                baitRb = bait.AddComponent<Rigidbody>();
            }

            // Configure Rigidbody
            baitRb.isKinematic = false; // Enable physics
            baitRb.useGravity = true;    // Allow gravity to affect the bait

            // Apply a forward force to simulate throwing the bait
            Vector3 launchDirection = transform.forward; // Adjust based on rod's orientation
            float launchForce = 10f; // Adjust force as needed
            baitRb.AddForce(launchDirection * launchForce, ForceMode.Impulse);

            Debug.Log("Bait has been detached and launched.");
        }
        else
        {
            Debug.LogError("Bait GameObject is not assigned.");
        }

        // Display the "Fishing..." UI window
        if (fishingUIWindowPrefab != null)
        {
            // Find or create a Canvas
            Canvas mainCanvas = FindObjectOfType<Canvas>();
            if (mainCanvas == null)
            {
                // Create a new Canvas if none exists
                GameObject canvasGO = new GameObject("MainCanvas");
                mainCanvas = canvasGO.AddComponent<Canvas>();
                mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasGO.AddComponent<CanvasScaler>();
                canvasGO.AddComponent<GraphicRaycaster>();
            }

            // Instantiate the fishing UI window as a child of the Canvas
            GameObject fishingUI = Instantiate(fishingUIWindowPrefab, mainCanvas.transform);
            fishingUI.transform.SetAsLastSibling(); // Ensure it appears on top

            // Start the fish bite process
            FishFightController fishFightController = FindObjectOfType<FishFightController>();
            if (fishFightController != null)
            {
                fishFightController.fishingPanel = fishingUI; // Assign the panel so it can be removed later
                fishFightController.StartFishBite();
            }
            else
            {
                Debug.LogError("FishFightController not found in the scene.");
            }

            Debug.Log("'Fishing...' UI window displayed.");
        }
        else
        {
            Debug.LogError("FishingUIWindowPrefab is not assigned in the RodIdleMovement script.");
        }
    }
    void OnDestroy()
    {
        // Unsubscribe from the fishing start event to prevent memory leaks
        FishingGestureDetector gestureDetector = FindObjectOfType<FishingGestureDetector>();
        if (gestureDetector != null)
        {
            gestureDetector.OnFishingStart -= StartFishing;
        }
    }
}
