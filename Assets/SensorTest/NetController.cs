using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class NetController : MonoBehaviour
{
    private float baseMoveSpeed = 0.5f; // Base movement speed
    public Text pitchText;
    public Text rollText;
    public GameObject fish;
    public GameObject winWindowPrefab; // Prefab for the "You Win" window

    private Vector3 netPosition;
    private float pitch;
    private float roll;

    private float captureDelay = 0.5f; // Time in seconds required to capture
    private float captureTimer = 0f;
    private bool isCapturing = false;


    void Start()
    {
        netPosition = transform.position;
    }

    void Update()
    {
        // Get tilt data using the new Input System
        pitch = Input.acceleration.y * 90f;
        roll = Input.acceleration.x * 90f;

        // Update UI
        UpdateTiltUI();

        // Calculate movement based on tilt
        float currentMoveSpeed = baseMoveSpeed + (Mathf.Abs(pitch) + Mathf.Abs(roll)) * 0.00005f;
        Vector3 movement = new Vector3(roll, pitch, 0) * currentMoveSpeed * Time.deltaTime;

        // Update net position
        netPosition += movement;

        // Clamp the net position within screen bounds
        float screenHalfWidth = Camera.main.orthographicSize * Camera.main.aspect;
        float screenHalfHeight = Camera.main.orthographicSize;

        netPosition.x = Mathf.Clamp(netPosition.x, -screenHalfWidth, screenHalfWidth);
        netPosition.y = Mathf.Clamp(netPosition.y, -screenHalfHeight, screenHalfHeight);

        transform.position = netPosition;

        // Check for collision (net covering the fish)
        CheckCapture();
    }

    void UpdateTiltUI()
    {
        string pitchDirection;
        if (pitch > 1.0f)
            pitchDirection = "Up";
        else if (pitch < -1.0f)
            pitchDirection = "Down";
        else
            pitchDirection = "Level";

        string rollDirection;
        if (roll > 1.0f)
            rollDirection = "Right";
        else if (roll < -1.0f)
            rollDirection = "Left";
        else
            rollDirection = "Level";

        // Update the UI text with one decimal place
        if (pitchText != null && rollText != null)
        {
            pitchText.text = $"Pitch: {pitch:F1}° {pitchDirection}";
            rollText.text = $"Roll: {roll:F1}° {rollDirection}";
        }
        else
        {
            Debug.LogError("PitchText or RollText is not assigned in the Inspector.");
        }
    }

    void CheckCapture()
    {
        if (fish == null) return;

        // Calculate bounds
        Collider2D netCollider = GetComponent<Collider2D>();
        Collider2D fishCollider = fish.GetComponent<Collider2D>();

        if (netCollider != null && fishCollider != null)
        {
            // Check if the net completely covers the fish's center
            if (netCollider.bounds.Contains(fish.transform.position))
            {
                Vector2 fishPos = fish.transform.position;
                Vector2 netPos = transform.position;

                // Calculate net radius from CircleCollider2D, accounting for scale
                CircleCollider2D circleCollider = netCollider as CircleCollider2D;
                if (circleCollider == null)
                {
                    Debug.LogError("NetCollider is not a CircleCollider2D.");
                    return;
                }
                float netRadius = circleCollider.radius * Mathf.Max(transform.localScale.x, transform.localScale.y);

                // Calculate fish "radius" based on BoxCollider2D's half-diagonal
                BoxCollider2D boxCollider = fishCollider as BoxCollider2D;
                if (boxCollider == null)
                {
                    Debug.LogError("FishCollider is not a BoxCollider2D.");
                    return;
                }
                Vector2 boxSize = boxCollider.size;
                // Account for scale
                boxSize.x *= fish.transform.localScale.x;
                boxSize.y *= fish.transform.localScale.y;
                float fishRadius = (Mathf.Sqrt(boxSize.x * boxSize.x + boxSize.y * boxSize.y)) / 2f;

                // Calculate the distance between net and fish centers
                float distance = Vector2.Distance(fishPos, netPos);

                // Check if the net completely covers the fish
                if (distance + fishRadius < netRadius)
                {
                    if (!isCapturing)
                    {
                        isCapturing = true;
                        captureTimer = 0f;
                        Debug.Log("Capture started.");
                    }

                    // Increment the timer
                    captureTimer += Time.deltaTime;
                    Debug.Log($"Capture Timer: {captureTimer:F2} / {captureDelay}");

                    // Check if the capture delay has been met
                    if (captureTimer >= captureDelay)
                    {
                        Debug.Log("Capture completed.");
                        CaptureFish();
                    }
                }
                else
                {
                    ResetCapture();
                }
            }
            else
            {
                ResetCapture();
            }
        }
    }


    // void CheckCapture()
    // {
    //     if (fish == null) return;

    //     // Calculate bounds
    //     Collider2D netCollider = GetComponent<Collider2D>();
    //     Collider2D fishCollider = fish.GetComponent<Collider2D>();

    //     if (netCollider != null && fishCollider != null)
    //     {
    //         // Directly call CaptureFish() for testing
    //         if (netCollider.bounds.Contains(fish.transform.position))
    //         {
    //             CaptureFish();
    //         }
    //     }
    // }


    void ResetCapture()
    {
        if (isCapturing)
        {
            isCapturing = false;
            captureTimer = 0f;
            Debug.Log("Capture reset.");
        }
    }


    void CaptureFish()
    {
        // Find the Canvas in the scene
        Canvas mainCanvas = FindObjectOfType<Canvas>();

        if (winWindowPrefab != null && mainCanvas != null)
        {
            // Instantiate the prefab as a child of the Canvas
            GameObject winWindow = Instantiate(winWindowPrefab, mainCanvas.transform);
            winWindow.transform.SetAsLastSibling(); // Ensure it's on top
            Debug.Log("You Win window instantiated successfully.");
        }
        else
        {
            Debug.LogError("WinWindowPrefab or Canvas not found in the scene.");
        }

        // Disable NetController to stop net movement
        this.enabled = false;
        Debug.Log("NetController disabled.");

        // Disable FishController to stop fish movement
        if (fish != null)
        {
            FishController fishController = fish.GetComponent<FishController>();
            if (fishController != null)
            {
                fishController.enabled = false;
                Debug.Log("FishController disabled.");
            }
            else
            {
                Debug.LogWarning("FishController component not found on Fish GameObject.");
            }
        }
    }



}
