using UnityEngine;

public class BaitSpawner : MonoBehaviour
{
    public GameObject baitPrefab; // Assign the bait prefab in the Inspector
    public Vector3 baitSpawnPosition = new Vector3(0, 0, -7f); // Default bait spawn position
    private bool baitSpawned = false; // Flag to ensure bait is only spawned once

    void Start()
    {
        if (!baitSpawned)
        {
            SpawnBait();
            baitSpawned = true; // Set the flag to true after spawning
        }
    }

    void SpawnBait()
    {
        if (baitPrefab == null)
        {
            Debug.LogError("Bait prefab is not assigned in the Inspector!");
            return;
        }

        // Instantiate bait at the specified position
        GameObject bait = Instantiate(baitPrefab, baitSpawnPosition, Quaternion.identity);

        // Optionally, you can add some debug logs to verify
        Debug.Log($"Bait spawned at position: {baitSpawnPosition}");
    }
}
