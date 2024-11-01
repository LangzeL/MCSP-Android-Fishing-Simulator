using UnityEngine;
using UnityEngine.UI;

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

    [Header("Smoothing Settings")]
    [Tooltip("Smoothing factor for idle movement. Higher value means more smoothing.")]
    private float smoothingFactor = 0.1f;

    [Header("References")]
    [Tooltip("Reference to the Bait GameObject.")]
    public GameObject bait;

    [Tooltip("Prefab for the 'Fishing...' UI window.")]
    public GameObject fishingUIWindowPrefab;

    [Tooltip("Reference to the rotation center GameObject.")]
    public GameObject rodRotationCentre;

    private Vector3 smoothPosition = Vector3.zero;
    private Quaternion smoothRotation = Quaternion.identity;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private bool isFishingStarted = false;

    private BaitController baitController;
    private FishFightController fishFightController;
    private Canvas mainCanvas;

    void Start()
    {
        if (rodRotationCentre == null)
        {
            Debug.LogError("RodRotationCentre GameObject is not assigned.");
            return;
        }

        initialPosition = rodRotationCentre.transform.position;
        initialRotation = rodRotationCentre.transform.rotation;

        smoothPosition = initialPosition;
        smoothRotation = initialRotation;

        if (bait == null)
        {
            Debug.LogError("Bait GameObject is not assigned.");
        }
        else
        {
            baitController = bait.GetComponent<BaitController>();
            if (baitController == null)
            {
                Debug.LogError("BaitController not found on bait object.");
            }
        }

        fishFightController = FindObjectOfType<FishFightController>();
        if (fishFightController == null)
        {
            Debug.LogError("FishFightController not found in scene.");
        }

        // Find or create main canvas
        mainCanvas = FindObjectOfType<Canvas>();
        if (mainCanvas == null)
        {
            GameObject canvasGO = new GameObject("MainCanvas");
            mainCanvas = canvasGO.AddComponent<Canvas>();
            mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
        }

        // Subscribe to fishing gesture
        FishingGestureDetector gestureDetector = FindObjectOfType<FishingGestureDetector>();
        if (gestureDetector != null)
        {
            gestureDetector.OnFishingStart += StartFishing;
        }
        else
        {
            Debug.LogError("FishingGestureDetector not found in the scene.");
        }
    }

    void Update()
    {
        Vector3 accel = Input.acceleration;

        if (!isFishingStarted)
        {
            // Pre-fishing idle movement
            float tiltX = accel.x;
            float tiltY = accel.y;

            float posX = initialPosition.x + tiltX * positionSensitivity * 5;
            float posY = initialPosition.y + tiltX * positionSensitivity * 2;
            Vector3 targetPosition = new Vector3(posX, posY, initialPosition.z);

            float rotX = initialRotation.eulerAngles.x + tiltY * rotationSensitivityZ * 10;
            float rotY = initialRotation.eulerAngles.y + tiltX * rotationSensitivityXY * 2;
            float rotZ = initialRotation.eulerAngles.z + tiltX * rotationSensitivityXY * 2;
            Quaternion targetRotation = Quaternion.Euler(rotX, rotY, rotZ);

            smoothPosition = Vector3.Lerp(smoothPosition, targetPosition, smoothingFactor);
            smoothRotation = Quaternion.Slerp(smoothRotation, targetRotation, smoothingFactor);

            rodRotationCentre.transform.position = smoothPosition;
            rodRotationCentre.transform.rotation = smoothRotation;

            // Update bait position if it's attached
            if (bait != null && bait.transform.parent != null)
            {
                bait.transform.position = rodRotationCentre.transform.position;
                bait.transform.rotation = rodRotationCentre.transform.rotation;
            }
        }
        else
        {
            // Post-fishing rotation only
            float tiltY = accel.y;

            float rotX = initialRotation.eulerAngles.x - tiltY * rotationSensitivityZ * 10;
            Quaternion targetRotation = Quaternion.Euler(rotX, initialRotation.eulerAngles.y, initialRotation.eulerAngles.z);

            smoothRotation = Quaternion.Slerp(smoothRotation, targetRotation, smoothingFactor);

            rodRotationCentre.transform.position = initialPosition;
            rodRotationCentre.transform.rotation = smoothRotation;
        }
    }

    void StartFishing()
    {
        if (isFishingStarted) return;

        isFishingStarted = true;
        Debug.Log("Fishing has started.");

        // Detach bait from rod
        if (bait != null)
        {
            bait.transform.parent = null;
        }

        // Create fishing UI
        if (fishingUIWindowPrefab != null && mainCanvas != null)
        {
            GameObject fishingUI = Instantiate(fishingUIWindowPrefab, mainCanvas.transform);
            fishingUI.transform.SetAsLastSibling();

            if (fishFightController != null)
            {
                fishFightController.fishingPanel = fishingUI;
            }
        }
        else
        {
            Debug.LogError("Missing fishingUIWindowPrefab or mainCanvas reference.");
        }
    }

    void OnDestroy()
    {
        FishingGestureDetector gestureDetector = FindObjectOfType<FishingGestureDetector>();
        if (gestureDetector != null)
        {
            gestureDetector.OnFishingStart -= StartFishing;
        }
    }

    // Helper method to reset fishing state if needed
    public void ResetFishing()
    {
        isFishingStarted = false;
        if (bait != null)
        {
            bait.transform.parent = transform;
            bait.transform.position = rodRotationCentre.transform.position;
            bait.transform.rotation = rodRotationCentre.transform.rotation;
        }
        rodRotationCentre.transform.position = initialPosition;
        rodRotationCentre.transform.rotation = initialRotation;
    }
}