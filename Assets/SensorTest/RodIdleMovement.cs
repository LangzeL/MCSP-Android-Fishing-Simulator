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
    private float smoothingFactor = 0.3f;

    private Vector3 smoothPosition = Vector3.zero;  
    private Quaternion smoothRotation = Quaternion.identity;

    [Header("Fishing Settings")]
    [Tooltip("Reference to the Bait GameObject.")]
    public GameObject bait;

    [Tooltip("Prefab for the 'Fishing...' UI window.")]
    public GameObject fishingUIWindowPrefab;

    [Tooltip("Reference to the rotation center GameObject.")]
    public GameObject rodRotationCentre;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private bool isFishingStarted = false;

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
        }
        else
        {
            float tiltY = accel.y;

            float rotX = initialRotation.eulerAngles.x + tiltY * rotationSensitivityZ * 10;
            Quaternion targetRotation = Quaternion.Euler(rotX, initialRotation.eulerAngles.y, initialRotation.eulerAngles.z);

            smoothRotation = Quaternion.Slerp(smoothRotation, targetRotation, smoothingFactor);

            rodRotationCentre.transform.position = initialPosition;
            rodRotationCentre.transform.rotation = smoothRotation;
        }
    }

    void StartFishing()
    {
        if (isFishingStarted)
            return;

        isFishingStarted = true;
        Debug.Log("Fishing has started.");

        if (bait != null)
        {
            bait.transform.parent = null;

            Rigidbody baitRb = bait.GetComponent<Rigidbody>();
            if (baitRb == null)
            {
                baitRb = bait.AddComponent<Rigidbody>();
            }

            baitRb.isKinematic = false;
            baitRb.useGravity = true;

            Vector3 launchDirection = rodRotationCentre.transform.forward;
            float launchForce = 10f;
            baitRb.AddForce(launchDirection * launchForce, ForceMode.Impulse);

            Debug.Log("Bait has been detached and launched.");
        }
        else
        {
            Debug.LogError("Bait GameObject is not assigned.");
        }

        if (fishingUIWindowPrefab != null)
        {
            Canvas mainCanvas = FindObjectOfType<Canvas>();
            if (mainCanvas == null)
            {
                GameObject canvasGO = new GameObject("MainCanvas");
                mainCanvas = canvasGO.AddComponent<Canvas>();
                mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasGO.AddComponent<CanvasScaler>();
                canvasGO.AddComponent<GraphicRaycaster>();
            }

            GameObject fishingUI = Instantiate(fishingUIWindowPrefab, mainCanvas.transform);
            fishingUI.transform.SetAsLastSibling();

            FishFightController fishFightController = FindObjectOfType<FishFightController>();
            if (fishFightController != null)
            {
                fishFightController.fishingPanel = fishingUI;
                fishFightController.StartFishBite();
            }
            else
            {
                Debug.LogError("FishFightController not found.");
            }

            Debug.Log("'Fishing...' UI window displayed.");
        }
        else
        {
            Debug.LogError("FishingUIWindowPrefab is not assigned.");
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
}
