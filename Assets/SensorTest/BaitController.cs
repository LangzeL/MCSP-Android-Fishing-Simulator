using UnityEngine;

public class BaitController : MonoBehaviour
{
    public Vector3 baitPositionOffset = new Vector3(0, 0, -7f);
    public float throwSpeed = 10f;
    public float throwArcHeight = 2f;
    public float throwDuration = 0.5f;

    private Rigidbody baitRigidbody;
    private bool hasRelocated = false;
    private bool isThrowingInProgress = false;
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float throwStartTime;
    private FishFightController fishFightController;

    void Start()
    {
        // Setup Rigidbody
        baitRigidbody = GetComponent<Rigidbody>();
        if (baitRigidbody != null)
        {
            baitRigidbody.isKinematic = true;
            baitRigidbody.useGravity = false;
        }

        // Setup Collider
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        if (boxCollider != null)
        {
            boxCollider.isTrigger = true;
        }

        // Find FishFightController
        fishFightController = FindObjectOfType<FishFightController>();
        if (fishFightController == null)
        {
            Debug.LogError("FishFightController not found in scene!");
        }

        // Subscribe to fishing gesture
        FishingGestureDetector gestureDetector = FindObjectOfType<FishingGestureDetector>();
        if (gestureDetector != null)
        {
            gestureDetector.OnFishingStart += RelocateBait;
        }
    }

    void Update()
    {
        if (isThrowingInProgress)
        {
            float elapsedTime = Time.time - throwStartTime;
            float throwProgress = elapsedTime / throwDuration;

            if (throwProgress >= 1f)
            {
                FinishThrow();
            }
            else
            {
                Vector3 currentPosition = Vector3.Lerp(startPosition, targetPosition, throwProgress);
                float arcHeight = throwArcHeight * Mathf.Sin(throwProgress * Mathf.PI);
                currentPosition.y += arcHeight;
                transform.position = currentPosition;
            }
        }
    }

    void RelocateBait()
    {
        if (hasRelocated) return;
        hasRelocated = true;

        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        float cellWidth = screenWidth / 3;
        float cellHeight = screenHeight / 3;

        float minX = cellWidth;
        float maxX = cellWidth * 2;
        float minY = cellHeight;
        float maxY = cellHeight * 2;

        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);
        Vector3 targetScreenPosition = new Vector3(randomX, randomY, baitPositionOffset.z);

        Vector3 targetWorldPosition = Camera.main.ScreenToWorldPoint(targetScreenPosition);
        targetPosition = new Vector3(targetWorldPosition.x, targetWorldPosition.y, baitPositionOffset.z);

        startPosition = transform.position;
        throwStartTime = Time.time;
        isThrowingInProgress = true;

        if (baitRigidbody != null)
        {
            baitRigidbody.isKinematic = true;
            baitRigidbody.velocity = Vector3.zero;
            baitRigidbody.angularVelocity = Vector3.zero;
        }

        Debug.Log("Starting bait throw animation.");
    }

    private void FinishThrow()
    {
        isThrowingInProgress = false;
        transform.position = targetPosition;

        if (baitRigidbody != null)
        {
            baitRigidbody.isKinematic = true;
            baitRigidbody.velocity = Vector3.zero;
            baitRigidbody.angularVelocity = Vector3.zero;
        }

        // Notify FishFightController that bait is ready
        if (fishFightController != null)
        {
            fishFightController.OnBaitReady(transform);
        }

        Debug.Log("Bait throw complete - notifying FishFightController.");
    }

    void OnDestroy()
    {
        FishingGestureDetector gestureDetector = FindObjectOfType<FishingGestureDetector>();
        if (gestureDetector != null)
        {
            gestureDetector.OnFishingStart -= RelocateBait;
        }
    }
}