using UnityEngine;

public class BaitController : MonoBehaviour
{
    public Vector3 baitPositionOffset = new Vector3(0, 0, -6f); // Fixed Z-position for bait
    private Rigidbody baitRigidbody;
    private bool hasRelocated = false;

    void Start()
    {
        // Get the Rigidbody component and ensure it is kinematic initially
        baitRigidbody = GetComponent<Rigidbody>();
        if (baitRigidbody != null)
        {
            baitRigidbody.isKinematic = true;
        }

        // Subscribe to the fishing start event
        FishingGestureDetector gestureDetector = FindObjectOfType<FishingGestureDetector>();
        if (gestureDetector != null)
        {
            gestureDetector.OnFishingStart += RelocateBait;
        }
    }

    /// <summary>
    /// Relocates the bait to a position within the central cell of a 3x3 grid on the screen.
    /// </summary>
    void RelocateBait()
    {
        if (hasRelocated)
            return;

        hasRelocated = true;

        // Calculate screen dimensions
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // Determine the central cell boundaries in screen space
        float cellWidth = screenWidth / 3;
        float cellHeight = screenHeight / 3;

        float minX = cellWidth;          // Left boundary of central cell
        float maxX = cellWidth * 2;      // Right boundary of central cell
        float minY = cellHeight;         // Bottom boundary of central cell
        float maxY = cellHeight * 2;     // Top boundary of central cell

        // Randomly choose a position within the center cell
        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);
        Vector3 targetScreenPosition = new Vector3(randomX, randomY, baitPositionOffset.z);

        // Convert the screen position to world position
        Vector3 targetWorldPosition = Camera.main.ScreenToWorldPoint(targetScreenPosition);

        // Relocate the bait to the calculated position
        transform.position = new Vector3(targetWorldPosition.x, targetWorldPosition.y, baitPositionOffset.z);

        // Ensure the bait remains stationary
        if (baitRigidbody != null)
        {
            baitRigidbody.isKinematic = true; // Make the Rigidbody kinematic to prevent physics forces
            baitRigidbody.velocity = Vector3.zero; // Reset any velocity
            baitRigidbody.angularVelocity = Vector3.zero; // Reset any angular velocity
        }

        Debug.Log("Bait has been relocated to a new position within the center cell and is now stationary.");
    }

    void OnDestroy()
    {
        // Unsubscribe from the event
        FishingGestureDetector gestureDetector = FindObjectOfType<FishingGestureDetector>();
        if (gestureDetector != null)
        {
            gestureDetector.OnFishingStart -= RelocateBait;
        }
    }
}
