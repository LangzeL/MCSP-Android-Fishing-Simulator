using UnityEngine;

public class FishBehavior : MonoBehaviour
{
    public float swimSpeed = 2.0f;
    public float directionChangeInterval = 1.0f;
    private Vector3 swimDirection;
    private Camera mainCamera;
    private bool isHooked = false;
    private bool isInTiltScene = false;
    private bool isCaptured = false;

    [SerializeField]
    private string fishType = "Normal";


    void Start()
    {
        SetupCamera();
        InvokeRepeating("ChangeDirection", 0f, directionChangeInterval);
    }

    void SetupCamera()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main camera not found!");
        }
    }
    void OnEnable()
    {
        // Reset camera reference when enabled
        mainCamera = Camera.main;
        if (!isInTiltScene && !isHooked)
        {
            // Only start movement if not in special states
            StartMovement();
        }
    }
    void StartMovement()
    {
        CancelInvoke("ChangeDirection");
        ChangeDirection(); // Immediate first direction
        InvokeRepeating("ChangeDirection", 0f, directionChangeInterval);
    }
    void ChangeDirection()
    {
        if ((isHooked && !isInTiltScene) || isCaptured) return;

        float xDirection = Random.Range(-1f, 1f);
        float yDirection = Random.Range(-1f, 1f);
        float zDirection = 0f;

        swimDirection = new Vector3(xDirection, yDirection, zDirection).normalized;
        UpdateFishRotation();
    }

    void Update()
    {
        if ((isHooked && !isInTiltScene) || isCaptured) return;

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null) return;
        }

        transform.position += swimDirection * swimSpeed * Time.deltaTime;
        KeepWithinCameraBounds();
    }

    void KeepWithinCameraBounds()
    {
        if (mainCamera == null) return;

        Vector3 viewportPosition = mainCamera.WorldToViewportPoint(transform.position);

        if (viewportPosition.x < 0f || viewportPosition.x > 1f)
        {
            swimDirection.x = -swimDirection.x;
            viewportPosition.x = Mathf.Clamp(viewportPosition.x, 0.01f, 0.99f);
            UpdateFishRotation();
        }

        if (viewportPosition.y < 0f || viewportPosition.y > 1f)
        {
            swimDirection.y = -swimDirection.y;
            viewportPosition.y = Mathf.Clamp(viewportPosition.y, 0.01f, 0.99f);
            UpdateFishRotation();
        }

        transform.position = mainCamera.ViewportToWorldPoint(viewportPosition);
    }

    void UpdateFishRotation()
    {
        if (isInTiltScene)
        {
            // Only rotate around Y axis based on X direction
            if (swimDirection.x > 0)
            {
                transform.rotation = Quaternion.Euler(0f, -90f, 0f);  // Facing right
            }
            else
            {
                transform.rotation = Quaternion.Euler(0f, 90f, 0f);   // Facing left
            }
            Debug.Log($"Updated rotation in tilt scene: {transform.rotation.eulerAngles}"); // Debug log
        }
        else
        {
            if (swimDirection.x > 0)
            {
                transform.rotation = Quaternion.Euler(0f, -90f, 0f);
            }
            else if (swimDirection.x < 0)
            {
                transform.rotation = Quaternion.Euler(0f, 90f, 0f);
            }
        }
    }

    public bool IsHooked()
    {
        return isHooked;
    }

    public void OnFishHooked(Vector3 hookPosition)
    {
        isHooked = true;
        transform.position = hookPosition;

        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("OnHooked");
        }

        transform.rotation = Quaternion.Euler(90f, -90f, 0f);
        Debug.Log("Fish Hooked! Animation triggered.");
    }

    public string GetFishType()
    {
        return fishType;
    }


    public void PrepareForTiltScene()
    {
        Debug.Log("Preparing fish for tilt scene - START"); // Debug log

        // First, ensure we're not hooked and can move
        isInTiltScene = true;
        isHooked = false;
        isCaptured = false;

        // Force the scale to 0.3
        transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        Debug.Log($"Scale set to: {transform.localScale}"); // Debug log

        // Set initial rotation (facing left)
        transform.rotation = Quaternion.Euler(0f, 90f, 0f);
        Debug.Log($"Initial rotation set to: {transform.rotation.eulerAngles}"); // Debug log

        // Reset speed to default
        swimSpeed = 2.0f;

        // Stop any ongoing animations
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.enabled = false;
        }

        // Restart movement system
        StartMovement();

        Debug.Log("Preparing fish for tilt scene - COMPLETE"); // Debug log
    }

    public void SetMovementParameters(float newSpeed, float newInterval)
    {
        swimSpeed = newSpeed;
        directionChangeInterval = newInterval;

        // Restart the direction change cycle
        CancelInvoke("ChangeDirection");
        ChangeDirection(); // Immediate change
        InvokeRepeating("ChangeDirection", directionChangeInterval, directionChangeInterval);

        Debug.Log($"Movement parameters updated - Speed: {newSpeed}, Interval: {newInterval}");
    }

    public void OnCaptured()
    {
        isCaptured = true;
        CancelInvoke("ChangeDirection");
        swimDirection = Vector3.zero;
    }
}