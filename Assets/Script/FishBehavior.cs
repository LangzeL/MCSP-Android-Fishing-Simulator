using UnityEngine;

public class FishBehavior : MonoBehaviour
{
    public float swimSpeed = 2.0f;               // 游动速度
    public float directionChangeInterval = 1.0f; // 改变方向的时间间隔
    private Vector3 swimDirection;               // 游动方向
    private Camera mainCamera;                   // 主摄像机
    private bool isHooked = false;               // 是否被钩住

    void Start()
    {
        mainCamera = Camera.main; // 获取主摄像机
        InvokeRepeating("ChangeDirection", 0f, directionChangeInterval); // 定期改变游动方向
    }

    void ChangeDirection()
    {
        if (isHooked) return; // 如果被钩住，不再改变方向

        // 随机生成游动方向
        float xDirection = Random.Range(-1f, 1f);
        float yDirection = Random.Range(-1f, 1f);
        float zDirection = 0f; // 如果是2D场景，只考虑 x 和 y 方向

        swimDirection = new Vector3(xDirection, yDirection, zDirection).normalized;

        // 更新鱼的朝向
        UpdateFishRotation();
    }

    void Update()
    {
        if (isHooked) return; // 被钩住后不再移动

        // 移动鱼
        transform.position += swimDirection * swimSpeed * Time.deltaTime;

        // 检查是否在摄像机的视野范围内
        KeepWithinCameraBounds();
    }

    void KeepWithinCameraBounds()
    {
        Vector3 viewportPosition = mainCamera.WorldToViewportPoint(transform.position);

        if (viewportPosition.x < 0f || viewportPosition.x > 1f)
        {
            swimDirection.x = -swimDirection.x; // 反转水平方向
            viewportPosition.x = Mathf.Clamp(viewportPosition.x, 0.01f, 0.99f);
            UpdateFishRotation();
        }

        if (viewportPosition.y < 0f || viewportPosition.y > 1f)
        {
            swimDirection.y = -swimDirection.y; // 反转垂直方向
            viewportPosition.y = Mathf.Clamp(viewportPosition.y, 0.01f, 0.99f);
            UpdateFishRotation();
        }

        transform.position = mainCamera.ViewportToWorldPoint(viewportPosition);
    }

    void UpdateFishRotation()
    {
        if (swimDirection.x > 0)
        {
            transform.rotation = Quaternion.Euler(0f, -90f, 0f); // 朝向右侧
        }
        else if (swimDirection.x < 0)
        {
            transform.rotation = Quaternion.Euler(0f, 90f, 0f); // 朝向左侧
        }
    }

    public bool IsHooked()
    {
        return isHooked;
    }

    public void OnFishHooked(Vector3 hookPosition)
    {
        isHooked = true;

        // 将鱼的位置设置为钓钩的位置
        transform.position = hookPosition;

        // 开始上鱼动画
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("OnHooked"); // 触发被钓住的动画
        }

        // 设置鱼旋转，使其有被钓起的感觉
        transform.rotation = Quaternion.Euler(90f, -90f, 0f);

        Debug.Log("Fish Hooked! Animation triggered.");
    }
}
