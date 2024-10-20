using UnityEngine;

public class FishHookEvent : MonoBehaviour
{
    public Transform bait;
    public float hookDistance = 1.0f; // Distance within which the fish can be hooked
    public float hookProbability = 0.5f; // 这里我先设成0.5 太低再往上调

    private FishBehavior fishBehavior;
    private bool isHooked = false;

    void Start()
    {
        fishBehavior = GetComponent<FishBehavior>();
    }

    void Update()
    {
        if (!isHooked && Vector3.Distance(transform.position, bait.position) <= hookDistance)
        {
            // Check if the fish gets hooked based on the probability
            if (Random.value < hookProbability)
            {
                isHooked = true;
                OnFishHooked();
            }
        }
    }

    void OnFishHooked()
    {
        // Stop the fish from swimming
        fishBehavior.enabled = false;

        // TODO: Start the fish fighting mechanics
        Debug.Log("Fish Hooked! Start fighting the fish.");
    }
}
