using UnityEngine;

public class FishHookEvent : MonoBehaviour
{
    public Transform bait;
    public float hookDistance = 1.0f; // 距离内可以被钓到
    public float hookTime = 11.0f; // 在距离内停留超过 1 秒则上钩

    private FishBehavior hookedFish = null; // 记录被钓到的鱼
    private float timeInHookRange = 0.0f; // 在钓钩范围内的时间

    void Update()
    {
        if (hookedFish != null)
        {
            // 已经有鱼被钓到
            return;
        }

        FishBehavior[] allFish = FindObjectsOfType<FishBehavior>();

        foreach (FishBehavior fish in allFish)
        {
            if (fish.IsHooked()) continue; // 如果鱼已经被钓住，跳过

            float distanceToBait = Vector3.Distance(fish.transform.position, bait.position);

            if (distanceToBait <= hookDistance)
            {
                // 在钓钩范围内
                timeInHookRange += Time.deltaTime;

                // 如果在范围内停留时间超过设定值，则上钩
                if (timeInHookRange >= hookTime)
                {
                    hookedFish = fish; // 标记被钓的鱼
                    hookedFish.OnFishHooked(bait.position);
                    RemoveOtherFish(hookedFish);
                    break;
                }
            }
            else
            {
                // 如果鱼不在范围内，重置计时器
                timeInHookRange = 0.0f;
            }
        }
    }

    void RemoveOtherFish(FishBehavior hookedFish)
    {
        // 查找所有鱼对象
        FishBehavior[] allFish = FindObjectsOfType<FishBehavior>();
        foreach (FishBehavior fish in allFish)
        {
            // 如果不是当前钓住的鱼，则销毁
            if (fish != hookedFish)
            {
                Destroy(fish.gameObject);
            }
        }
    }
}
