using UnityEngine;

public class FishSpawner : MonoBehaviour
{
    public GameObject[] fishPrefabs; // Assign this array in the inspector with your fish prefabs
    public int numberOfFish = 10;    // Number of fish to spawn

    void Start()
    {
        for (int i = 0; i < numberOfFish; i++)
        {
            SpawnFish();
        }
    }

    void SpawnFish()
    {
        Vector3 spawnPosition = GetRandomPosition();
        // Randomly select a prefab from the array
        int randomIndex = Random.Range(0, fishPrefabs.Length);
        GameObject selectedFishPrefab = fishPrefabs[randomIndex];

        Instantiate(selectedFishPrefab, spawnPosition, Quaternion.identity);
    }

    private Vector3 GetRandomPosition()
    {
        float screenHalfWidth = Camera.main.orthographicSize * Camera.main.aspect;
        float screenHalfHeight = Camera.main.orthographicSize;

        float randomX = Random.Range(-screenHalfWidth, screenHalfWidth);
        float randomY = Random.Range(-screenHalfHeight, screenHalfHeight);

        return new Vector3(randomX, randomY, 0f);
    }
}
