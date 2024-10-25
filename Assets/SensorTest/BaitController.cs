using UnityEngine;

public class BaitController : MonoBehaviour
{
    public float launchForce = 10f;
    private Rigidbody baitRigidbody;
    private bool hasLaunched = false;

    void Start()
    {
        baitRigidbody = GetComponent<Rigidbody>();

        // Ensure bait is kinematic before launch
        if (baitRigidbody != null)
        {
            baitRigidbody.isKinematic = true;
        }

        // Subscribe to the fishing start event
        FishingGestureDetector gestureDetector = FindObjectOfType<FishingGestureDetector>();
        if (gestureDetector != null)
        {
            gestureDetector.OnFishingStart += LaunchBait;
        }
    }

    void LaunchBait()
    {
        if (hasLaunched)
            return;

        hasLaunched = true;

        // Detach bait from the rod
        transform.parent = null;

        // Enable physics
        if (baitRigidbody != null)
        {
            baitRigidbody.isKinematic = false;

            // Apply force to launch the bait forward
            baitRigidbody.AddForce(Vector3.forward * launchForce, ForceMode.Impulse);
        }

        Debug.Log("Bait has been launched.");
    }

    void OnDestroy()
    {
        // Unsubscribe from the event
        FishingGestureDetector gestureDetector = FindObjectOfType<FishingGestureDetector>();
        if (gestureDetector != null)
        {
            gestureDetector.OnFishingStart -= LaunchBait;
        }
    }
}
