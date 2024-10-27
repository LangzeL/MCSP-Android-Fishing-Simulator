using UnityEngine;

public class FishingManager : MonoBehaviour
{
    private static FishingManager instance;
    public static FishingManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<FishingManager>();
                if (instance == null)
                {
                    GameObject go = new GameObject("FishingManager");
                    instance = go.AddComponent<FishingManager>();
                }
            }
            return instance;
        }
    }

    // Data to persist between scenes
    private FishBehavior caughtFish;
    private Vector3 fishPosition;
    private Quaternion fishRotation;
    private string fishType;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void SetCaughtFish(FishBehavior fish)
    {
        caughtFish = fish;
        if (fish != null)
        {
            fishPosition = fish.transform.position;
            fishRotation = fish.transform.rotation;
            fishType = fish.GetFishType();
        }
    }

    public FishBehavior GetCaughtFish()
    {
        return caughtFish;
    }

    public void ClearCaughtFish()
    {
        caughtFish = null;
    }

    public (Vector3 position, Quaternion rotation, string type) GetFishData()
    {
        return (fishPosition, fishRotation, fishType);
    }
}