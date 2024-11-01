using UnityEngine;

public class FishHookEvent : MonoBehaviour
{
    public Transform bait;
    public float hookDistance = 1.0f; // �����ڿ��Ա�����
    public float hookTime = 11.0f; // �ھ�����ͣ������ 1 �����Ϲ�

    private FishBehavior hookedFish = null; // ��¼����������
    private float timeInHookRange = 0.0f; // �ڵ�����Χ�ڵ�ʱ��

    void Update()
    {
        if (hookedFish != null)
        {
            // �Ѿ����㱻����
            return;
        }

        FishBehavior[] allFish = FindObjectsOfType<FishBehavior>();

        foreach (FishBehavior fish in allFish)
        {
            if (fish.IsHooked()) continue; // ������Ѿ�����ס������

            float distanceToBait = Vector3.Distance(fish.transform.position, bait.position);

            if (distanceToBait <= hookDistance)
            {
                // �ڵ�����Χ��
                timeInHookRange += Time.deltaTime;

                // ����ڷ�Χ��ͣ��ʱ�䳬���趨ֵ�����Ϲ�
                if (timeInHookRange >= hookTime)
                {
                    hookedFish = fish; // ��Ǳ�������
                    hookedFish.OnFishHooked(bait.position);
                    RemoveOtherFish(hookedFish);
                    break;
                }
            }
            else
            {
                // ����㲻�ڷ�Χ�ڣ����ü�ʱ��
                timeInHookRange = 0.0f;
            }
        }
    }

    void RemoveOtherFish(FishBehavior hookedFish)
    {
        // �������������
        FishBehavior[] allFish = FindObjectsOfType<FishBehavior>();
        foreach (FishBehavior fish in allFish)
        {
            // ������ǵ�ǰ��ס���㣬������
            if (fish != hookedFish)
            {
                Destroy(fish.gameObject);
            }
        }
    }
}
