using UnityEngine;

public class FishHookEvent : MonoBehaviour
{
    public Transform bait;
    public float hookDistance = 1.0f; // 距离内可以被钓到
    public float hookTime = 1.0f; // 在距离内停留超过 2 秒则上钩

    private FishBehavior fishBehavior;
    private bool isHooked = false;
    private float timeInHookRange = 0.0f; // 在钓钩范围内的时间

    void Start()
    {
        fishBehavior = GetComponent<FishBehavior>();
    }

    void Update()
    {
        if (isHooked)
        {
            return; // 如果已经被钓上，就不再进行判定
        }

        float distanceToBait = Vector3.Distance(transform.position, bait.position);

        if (distanceToBait <= hookDistance)
        {
            // 在钓钩范围内
            timeInHookRange += Time.deltaTime;

            // 如果在范围内停留时间超过设定值，则上钩
            if (timeInHookRange >= hookTime)
            {
                isHooked = true;
                OnFishHooked();
            }
        }
        else
        {
            // 如果鱼不在范围内，重置计时器
            timeInHookRange = 0.0f;
        }
    }

    void OnFishHooked()
    {
        // 停止鱼的游动
        fishBehavior.enabled = false;

        // TODO: 开始鱼挣扎的机制
        Debug.Log("Fish Hooked! Start fighting the fish.");
    }
}
