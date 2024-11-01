using UnityEngine;
using System.Collections;

public class TiltSceneManager : MonoBehaviour
{
    private readonly Vector3 defaultFishPosition = new Vector3(4f, 1f, 0f);

    [Header("Fish Collider Settings")]
    [Tooltip("Size of the fish's box collider in the tilt scene")]
    public Vector2 fishColliderSize = new Vector2(1f, 1f);

    void Start()
    {
        // Remove any placeholder fish in the scene
        FishBehavior[] placeholderFish = FindObjectsOfType<FishBehavior>();
        foreach (FishBehavior fish in placeholderFish)
        {
            if (fish.gameObject.name.Contains("ClownFish"))
            {
                Debug.Log($"Removing placeholder fish: {fish.gameObject.name}");
                Destroy(fish.gameObject);
            }
        }

        // Try to find FishFightController
        FishFightController controller = FindObjectOfType<FishFightController>();

        if (controller != null)
        {
            FishBehavior caughtFish = controller.GetHookedFish();

            if (caughtFish != null)
            {
                Debug.Log($"Setting up actual hooked fish: {caughtFish.gameObject.name}");
                SetupFishForTiltScene(caughtFish);
            }
            else
            {
                Debug.LogError("No caught fish found in FishFightController!");
            }
        }
        else
        {
            Debug.LogError("FishFightController not found! Make sure it persists between scenes.");
        }
    }
    IEnumerator SetupFishWithRetry()
    {
        FishBehavior caughtFish = null;
        float timeoutDuration = 5f;
        float startTime = Time.time;

        while (caughtFish == null && Time.time - startTime < timeoutDuration)
        {
            FishFightController controller = FishFightController.Instance;
            if (controller != null)
            {
                caughtFish = controller.GetHookedFish();
                if (caughtFish != null)
                {
                    SetupFishForTiltScene(caughtFish);
                    break;
                }
            }
            yield return new WaitForSeconds(0.1f);
        }

        if (caughtFish == null)
        {
            Debug.LogError("Failed to find caught fish after timeout!");
        }
    }

    private void SetupFishForTiltScene(FishBehavior fish)
    {
        // Position the fish
        fish.transform.position = defaultFishPosition;
        Debug.Log($"Fish positioned at: {fish.transform.position}");

        // Setup collider first
        BoxCollider2D boxCollider = SetupFishCollider(fish.gameObject);

        // Ensure the collider is properly sized and enabled
        boxCollider.size = fishColliderSize;
        boxCollider.offset = Vector2.zero; // Reset offset if any
        boxCollider.enabled = true;

        // Prepare the fish for tilt scene
        fish.PrepareForTiltScene();

        // Force the correct initial rotation
        fish.transform.rotation = Quaternion.Euler(0f, 90f, 0f);

        Debug.Log($"Fish prepared. Scale: {fish.transform.localScale}, Rotation: {fish.transform.rotation.eulerAngles}");

        // Setup NetController with retry
        StartCoroutine(SetupNetControllerWithRetry(fish.gameObject));
    }

    private BoxCollider2D SetupFishCollider(GameObject fishObject)
    {
        BoxCollider2D boxCollider = fishObject.GetComponent<BoxCollider2D>();
        if (boxCollider == null)
        {
            boxCollider = fishObject.AddComponent<BoxCollider2D>();
            Debug.Log("Added BoxCollider2D to fish");
        }

        // Make sure the collider is enabled
        boxCollider.enabled = true;
        Debug.Log($"Fish collider setup complete. Size: {boxCollider.size}, Enabled: {boxCollider.enabled}");

        return boxCollider;
    }

    private IEnumerator SetupNetControllerWithRetry(GameObject fishObject)
    {
        float timeout = 5f;
        float startTime = Time.time;
        NetController netController = null;

        while (Time.time - startTime < timeout)
        {
            netController = FindObjectOfType<NetController>();
            if (netController != null)
            {
                netController.fish = fishObject;
                Debug.Log("Fish assigned to NetController successfully");

                // Force collider setup
                CircleCollider2D netCollider = netController.GetComponent<CircleCollider2D>();
                if (netCollider == null)
                {
                    netCollider = netController.gameObject.AddComponent<CircleCollider2D>();
                    netCollider.radius = 1f; // Adjust as needed
                }
                netCollider.enabled = true;

                break;
            }
            yield return new WaitForSeconds(0.1f);
        }

        if (netController == null)
        {
            Debug.LogError("Failed to find NetController after timeout!");
        }
    }
}