using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;

public class NetController : MonoBehaviour
{
    private float baseMoveSpeed = 0.5f;
    public Text pitchText;
    public Text rollText;
    public GameObject fish;
    public GameObject winWindowPrefab;
    public GameObject bubbleObject;
    private GameObject winWindowInstance;

    private Vector3 netPosition;
    private float pitch;
    private float roll;

    private float captureDelay = 0.5f;
    private float captureTimer = 0f;
    private bool isCapturing = false;
    private bool isFishCaptured = false;

    private void Start()
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

        StartCoroutine(EnsureFishReference());
    }

    private IEnumerator EnsureFishReference()
    {
        float timeout = 5f;
        float startTime = Time.time;

        while (fish == null && Time.time - startTime < timeout)
        {
            // ONLY try to get from FishFightController, remove the scene search
            FishFightController controller = FishFightController.Instance;
            if (controller != null && controller.GetHookedFish() != null)
            {
                fish = controller.GetHookedFish().gameObject;
                Debug.Log($"Found hooked fish from FishFightController: {fish.name}");

                // Ensure fish has proper collider
                BoxCollider2D fishCollider = fish.GetComponent<BoxCollider2D>();
                if (fishCollider == null)
                {
                    fishCollider = fish.AddComponent<BoxCollider2D>();
                    fishCollider.size = new Vector2(1f, 1f);
                }
                fishCollider.enabled = true;

                // Ensure this NetController's collider is set up
                CircleCollider2D netCollider = GetComponent<CircleCollider2D>();
                if (netCollider == null)
                {
                    netCollider = gameObject.AddComponent<CircleCollider2D>();
                    netCollider.radius = 1f;
                }
                netCollider.enabled = true;

                break;
            }

            yield return new WaitForSeconds(0.1f);
        }

        if (fish == null)
        {
            Debug.LogError("Failed to find hooked fish reference from FishFightController!");
        }
        else
        {
            Debug.Log($"Successfully setup hooked fish reference: {fish.name}, IsHooked: {fish.GetComponent<FishBehavior>()?.IsHooked()}");
        }
    }
    void Update()
    {
        if (isFishCaptured) return;

        pitch = Input.acceleration.y * 90f;
        roll = Input.acceleration.x * 90f;

        UpdateTiltUI();

        float currentMoveSpeed = baseMoveSpeed + Mathf.Sqrt(Mathf.Abs(pitch) + Mathf.Abs(roll)) * 0.00005f;
        Vector3 movement = new Vector3(roll, pitch, 0) * currentMoveSpeed * Time.deltaTime;

        netPosition += movement;

        float screenHalfWidth = Camera.main.orthographicSize * Camera.main.aspect;
        float screenHalfHeight = Camera.main.orthographicSize;

        netPosition.x = Mathf.Clamp(netPosition.x, -screenHalfWidth, screenHalfWidth);
        netPosition.y = Mathf.Clamp(netPosition.y, -screenHalfHeight, screenHalfHeight);

        transform.position = netPosition;

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
    }

    void CheckCapture()
    {
        if (fish == null || fish.CompareTag("HookedFish") == false)
        {
            Debug.LogWarning("No valid hooked fish reference for capture check");
            return;
        }
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
        if (isFishCaptured) return;
        isFishCaptured = true;

        Canvas mainCanvas = FindObjectOfType<Canvas>();

        if (winWindowPrefab != null && mainCanvas != null)
        {
            winWindowInstance = Instantiate(winWindowPrefab, mainCanvas.transform);
            winWindowInstance.SetActive(true);
            winWindowInstance.transform.SetAsLastSibling();

            Text winText = winWindowInstance.transform.Find("WinText").GetComponent<Text>();
            if (winText != null)
            {
                StartCoroutine(ChangeTextColor(winText));
            }

            if (bubbleObject != null)
            {
                bubbleObject.SetActive(true);
                Debug.Log("Bubble effect revealed.");
            }
        }

        this.enabled = false;

        if (fish != null)
        {
            FishBehavior fishBehavior = fish.GetComponent<FishBehavior>();
            if (fishBehavior != null)
            {
                fishBehavior.OnCaptured();
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

    void OnDestroy()
    {
        if (winWindowInstance != null)
        {
            Destroy(winWindowInstance);
        }

        // Stop all coroutines when destroyed
        StopAllCoroutines();
    }

    // Public method to clear win window when resetting
    public void ClearWinWindow()
    {
        if (winWindowInstance != null)
        {
            Destroy(winWindowInstance);
        }
        if (bubbleObject != null)
        {
            bubbleObject.SetActive(false);
        }
    }
}