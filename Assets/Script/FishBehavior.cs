using UnityEngine;

public class FishBehavior : MonoBehaviour
{
    public float swimSpeed = 2.0f;               // �ζ��ٶ�
    public float directionChangeInterval = 1.0f; // �ı䷽���ʱ����
    private Vector3 swimDirection;               // �ζ�����
    private Camera mainCamera;                   // �������

    void Start()
    {
        mainCamera = Camera.main; // ��ȡ�������
        InvokeRepeating("ChangeDirection", 0f, directionChangeInterval); // ���ڸı��ζ�����
    }

    void ChangeDirection()
    {
        // ����һ���������
        float xDirection = Random.Range(-1f, 1f);
        float yDirection = Random.Range(-1f, 1f);
        float zDirection = 0f; // �����2D������ֻ���� x �� y ����

        swimDirection = new Vector3(xDirection, yDirection, zDirection).normalized;

        // ������ĳ���ʹ�������ζ��ķ���
        UpdateFishRotation();
    }

    void Update()
    {
        // �ƶ���
        transform.position += swimDirection * swimSpeed * Time.deltaTime;

        // ����Ƿ������������Ұ��Χ��
        KeepWithinCameraBounds();
    }

    void KeepWithinCameraBounds()
    {
        Vector3 viewportPosition = mainCamera.WorldToViewportPoint(transform.position);

        // ���������Ļ�߽磬��ת�ζ����򲢵���λ��
        if (viewportPosition.x < 0f || viewportPosition.x > 1f)
        {
            swimDirection.x = -swimDirection.x; // ��תˮƽ����
            viewportPosition.x = Mathf.Clamp(viewportPosition.x, 0.01f, 0.99f); // ȷ���㱣���ڱ߽���

            // ������ĳ����Է�ӳ������ķ���
            UpdateFishRotation();
        }

        if (viewportPosition.y < 0f || viewportPosition.y > 1f)
        {
            swimDirection.y = -swimDirection.y; // ��ת��ֱ����
            viewportPosition.y = Mathf.Clamp(viewportPosition.y, 0.01f, 0.99f); // ȷ���㱣���ڱ߽���

            // ������ĳ����Է�ӳ������ķ���
            UpdateFishRotation();
        }

        // ���ӿ�����ת����������
        transform.position = mainCamera.ViewportToWorldPoint(viewportPosition);
    }

    void UpdateFishRotation()
    {
        // ����ĳ��������ζ�����
        if (swimDirection.x > 0)
        {
            // �����Ҳ�
            transform.rotation = Quaternion.Euler(0f, -90f, 0f);
        }
        else if (swimDirection.x < 0)
        {
            // �������
            transform.rotation = Quaternion.Euler(0f, 90f, 0f);
        }
    }
}
