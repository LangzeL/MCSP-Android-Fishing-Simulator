using UnityEngine;

public class FishBehavior : MonoBehaviour
{
    public float swimSpeed = 2.0f;               // �ζ��ٶ�
    public float directionChangeInterval = 1.0f; // �ı䷽���ʱ����
    private Vector3 swimDirection;               // �ζ�����
    private Camera mainCamera;                   // �������
    private bool isHooked = false;               // �Ƿ񱻹�ס

    void Start()
    {
        mainCamera = Camera.main; // ��ȡ�������
        InvokeRepeating("ChangeDirection", 0f, directionChangeInterval); // ���ڸı��ζ�����
    }

    void ChangeDirection()
    {
        if (isHooked) return; // �������ס�����ٸı䷽��

        // ��������ζ�����
        float xDirection = Random.Range(-1f, 1f);
        float yDirection = Random.Range(-1f, 1f);
        float zDirection = 0f; // �����2D������ֻ���� x �� y ����

        swimDirection = new Vector3(xDirection, yDirection, zDirection).normalized;

        // ������ĳ���
        UpdateFishRotation();
    }

    void Update()
    {
        if (isHooked) return; // ����ס�����ƶ�

        // �ƶ���
        transform.position += swimDirection * swimSpeed * Time.deltaTime;

        // ����Ƿ������������Ұ��Χ��
        KeepWithinCameraBounds();
    }

    void KeepWithinCameraBounds()
    {
        Vector3 viewportPosition = mainCamera.WorldToViewportPoint(transform.position);

        if (viewportPosition.x < 0f || viewportPosition.x > 1f)
        {
            swimDirection.x = -swimDirection.x; // ��תˮƽ����
            viewportPosition.x = Mathf.Clamp(viewportPosition.x, 0.01f, 0.99f);
            UpdateFishRotation();
        }

        if (viewportPosition.y < 0f || viewportPosition.y > 1f)
        {
            swimDirection.y = -swimDirection.y; // ��ת��ֱ����
            viewportPosition.y = Mathf.Clamp(viewportPosition.y, 0.01f, 0.99f);
            UpdateFishRotation();
        }

        transform.position = mainCamera.ViewportToWorldPoint(viewportPosition);
    }

    void UpdateFishRotation()
    {
        if (swimDirection.x > 0)
        {
            transform.rotation = Quaternion.Euler(0f, -90f, 0f); // �����Ҳ�
        }
        else if (swimDirection.x < 0)
        {
            transform.rotation = Quaternion.Euler(0f, 90f, 0f); // �������
        }
    }

    public bool IsHooked()
    {
        return isHooked;
    }

    public void OnFishHooked(Vector3 hookPosition)
    {
        isHooked = true;

        // �����λ������Ϊ������λ��
        transform.position = hookPosition;

        // ��ʼ���㶯��
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("OnHooked"); // ��������ס�Ķ���
        }

        // ��������ת��ʹ���б�����ĸо�
        transform.rotation = Quaternion.Euler(90f, -90f, 0f);

        Debug.Log("Fish Hooked! Animation triggered.");
    }
}
