using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;

public class NetController : MonoBehaviour
{
    private float baseMoveSpeed = 0.5f; // Base movement speed
    public Text pitchText;
    public Text rollText;
    public GameObject fish;
    public GameObject winWindowPrefab; // Prefab for the "You Win" panel
    public GameObject bubbleObject;    // Reference to the existing bubble GameObject in the scene
    private GameObject winWindowInstance;

    private Vector3 netPosition;
    private float pitch;
    private float roll;

    private float captureDelay = 0.5f; // Time in seconds required to capture
    private float captureTimer = 0f;
    private bool isCapturing = false;

    void Start()
    {
        netPosition = transform.position;

        // Ensure the win window and bubble are hidden at the start
        if (winWindowPrefab != null)
        {
            winWindowPrefab.SetActive(false);
        }

        if (bubbleObject != null)
        {
            bubbleObject.SetActive(false);
        }
    }

    void Update()
    {
        // Get tilt data using the new Input System
        pitch = Input.acceleration.y * 90f;
        roll = Input.acceleration.x * 90f;

        // Update UI
        UpdateTiltUI();

        // Calculate movement based on tilt
        float currentMoveSpeed = baseMoveSpeed + Mathf.Sqrt(Mathf.Abs(pitch) + Mathf.Abs(roll)) * 0.00005f;
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

        Collider2D netCollider = GetComponent<Collider2D>();
        Collider2D fishCollider = fish.GetComponent<Collider2D>();

        if (netCollider != null && fishCollider != null)
        {
            if (netCollider.bounds.Contains(fish.transform.position))
            {
                Vector2 fishPos = fish.transform.position;
                Vector2 netPos = transform.position;

                CircleCollider2D circleCollider = netCollider as CircleCollider2D;
                if (circleCollider == null)
                {
                    Debug.LogError("NetCollider is not a CircleCollider2D.");
                    return;
                }
                float netRadius = circleCollider.radius * Mathf.Max(transform.localScale.x, transform.localScale.y);

                BoxCollider2D boxCollider = fishCollider as BoxCollider2D;
                if (boxCollider == null)
                {
                    Debug.LogError("FishCollider is not a BoxCollider2D.");
                    return;
                }
                Vector2 boxSize = boxCollider.size;
                boxSize.x *= fish.transform.localScale.x;
                boxSize.y *= fish.transform.localScale.y;
                float fishRadius = (Mathf.Sqrt(boxSize.x * boxSize.x + boxSize.y * boxSize.y)) / 2f;

                float distance = Vector2.Distance(fishPos, netPos);

                if (distance + fishRadius < netRadius)
                {
                    if (!isCapturing)
                    {
                        isCapturing = true;
                        captureTimer = 0f;
                        Debug.Log("Capture started.");
                    }

                    captureTimer += Time.deltaTime;

                    if (captureTimer >= captureDelay)
                    {
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
        Canvas mainCanvas = FindObjectOfType<Canvas>();

        if (winWindowPrefab != null && mainCanvas != null)
        {
            // Instantiate the "You Win" panel and make it visible
            winWindowInstance = Instantiate(winWindowPrefab, mainCanvas.transform);
            winWindowInstance.SetActive(true); // Ensure the panel is shown
            winWindowInstance.transform.SetAsLastSibling();

            // Start the text color-changing coroutine
            Text winText = winWindowInstance.transform.Find("WinText").GetComponent<Text>();
            if (winText != null)
            {
                StartCoroutine(ChangeTextColor(winText));
            }

            // Enable the bubble object
            if (bubbleObject != null)
            {
                bubbleObject.SetActive(true);
                Debug.Log("Bubble effect revealed.");
            }

            Debug.Log("You Win panel instantiated successfully.");
        }
        else
        {
            Debug.LogError("WinWindowPrefab or Canvas not found in the scene.");
        }

        this.enabled = false;

        if (fish != null)
        {
            FishController fishController = fish.GetComponent<FishController>();
            if (fishController != null)
            {
                fishController.enabled = false;
            }
            else
            {
                Debug.LogWarning("FishController component not found on Fish GameObject.");
            }
        }
    }

    IEnumerator ChangeTextColor(Text winText)
    {
        while (true)
        {
            winText.color = Color.green;
            yield return new WaitForSeconds(0.5f);
            winText.color = Color.blue;
            yield return new WaitForSeconds(0.5f);
        }
    }
}
