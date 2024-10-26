using UnityEngine;

public class FishSpawner : MonoBehaviour
{
    public GameObject[] fishPrefabs; // Assign this array in the inspector with your fish prefabs
    public int numberOfFish = 3;    // Number of fish to spawn

    void Start()
    {
        for (int i = 0; i < numberOfFish; i++)
        {
            SpawnFish();
        }
    }

    void SpawnFish()
    {
        // 检查数组是否为空
        if (fishPrefabs == null || fishPrefabs.Length == 0)
        {
            Debug.LogError("鱼的预制体数组为空，请在 Inspector 中分配至少一个预制体");
            return;
        }

        Vector3 spawnPosition = GetRandomSpawnPosition();

        // Randomly select a prefab from the array
        int randomIndex = Random.Range(0, fishPrefabs.Length);
        GameObject selectedFishPrefab = fishPrefabs[randomIndex];

        // Debug log to check which prefab is being instantiated
        Debug.Log($"Spawning fish prefab at index: {randomIndex}");

        // Instantiate the fish at the spawn position
        GameObject newFish = Instantiate(selectedFishPrefab, spawnPosition, Quaternion.identity);

        // Randomly scale the fish with +-0.2 variation
        float randomScaleFactor = 1f + Random.Range(-0.2f, 0.2f);
        newFish.transform.localScale *= randomScaleFactor;
    }

    private Vector3 GetRandomSpawnPosition()
    {
        // 随机生成一个在屏幕内的 x 和 y 位置
        float randomX = Random.Range(0f, 1f); // 完整屏幕的范围 (0 到 1)
        float randomY = Random.Range(0f, 1f); // 完整屏幕的范围 (0 到 1)

        // 将视口坐标转换为世界坐标
        Vector3 viewportPosition = new Vector3(randomX, randomY, 3f); // 设置 z 值为 3，使鱼生成在摄像机更近处
        Vector3 worldPosition = Camera.main.ViewportToWorldPoint(viewportPosition);

        return worldPosition;
    }
}
