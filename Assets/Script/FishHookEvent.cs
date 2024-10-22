using UnityEngine;

public class FishHookEvent : MonoBehaviour
{
    public Transform bait;
    public float hookDistance = 1.0f; // �����ڿ��Ա�����
    public float hookTime = 1.0f; // �ھ�����ͣ������ 2 �����Ϲ�

    private FishBehavior fishBehavior;
    private bool isHooked = false;
    private float timeInHookRange = 0.0f; // �ڵ�����Χ�ڵ�ʱ��

    void Start()
    {
        fishBehavior = GetComponent<FishBehavior>();
    }

    void Update()
    {
        if (isHooked)
        {
            return; // ����Ѿ������ϣ��Ͳ��ٽ����ж�
        }

        float distanceToBait = Vector3.Distance(transform.position, bait.position);

        if (distanceToBait <= hookDistance)
        {
            // �ڵ�����Χ��
            timeInHookRange += Time.deltaTime;

            // ����ڷ�Χ��ͣ��ʱ�䳬���趨ֵ�����Ϲ�
            if (timeInHookRange >= hookTime)
            {
                isHooked = true;
                OnFishHooked();
            }
        }
        else
        {
            // ����㲻�ڷ�Χ�ڣ����ü�ʱ��
            timeInHookRange = 0.0f;
        }
    }

    void OnFishHooked()
    {
        // ֹͣ����ζ�
        fishBehavior.enabled = false;

        // TODO: ��ʼ�������Ļ���
        Debug.Log("Fish Hooked! Start fighting the fish.");
    }
}
