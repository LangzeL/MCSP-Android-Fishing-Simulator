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
        // ��������Ƿ�Ϊ��
        if (fishPrefabs == null || fishPrefabs.Length == 0)
        {
            Debug.LogError("���Ԥ��������Ϊ�գ����� Inspector �з�������һ��Ԥ����");
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
        // �������һ������Ļ�ڵ� x �� y λ��
        float randomX = Random.Range(0f, 1f); // ������Ļ�ķ�Χ (0 �� 1)
        float randomY = Random.Range(0f, 1f); // ������Ļ�ķ�Χ (0 �� 1)

        // ���ӿ�����ת��Ϊ��������
        Vector3 viewportPosition = new Vector3(randomX, randomY, 3f); // ���� z ֵΪ 3��ʹ�������������������
        Vector3 worldPosition = Camera.main.ViewportToWorldPoint(viewportPosition);

        return worldPosition;
    }
}
