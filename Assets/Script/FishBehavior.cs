using UnityEngine;

public class FishBehavior : MonoBehaviour
{
    public float swimSpeed = 2.0f;               // 游动速度
    public float directionChangeInterval = 1.0f; // 改变方向的时间间隔
    private Vector3 swimDirection;               // 游动方向
    private Camera mainCamera;                   // 主摄像机

    void Start()
    {
        mainCamera = Camera.main; // 获取主摄像机
        InvokeRepeating("ChangeDirection", 0f, directionChangeInterval); // 定期改变游动方向
    }

    void ChangeDirection()
    {
        // 生成一个随机方向
        float xDirection = Random.Range(-1f, 1f);
        float yDirection = Random.Range(-1f, 1f);
        float zDirection = 0f; // 如果是2D场景，只考虑 x 和 y 方向

        swimDirection = new Vector3(xDirection, yDirection, zDirection).normalized;

        // 更新鱼的朝向，使其面向游动的方向
        UpdateFishRotation();
    }

    void Update()
    {
        // 移动鱼
        transform.position += swimDirection * swimSpeed * Time.deltaTime;

        // 检查是否在摄像机的视野范围内
        KeepWithinCameraBounds();
    }

    void KeepWithinCameraBounds()
    {
        Vector3 viewportPosition = mainCamera.WorldToViewportPoint(transform.position);

        // 如果超出屏幕边界，则反转游动方向并调整位置
        if (viewportPosition.x < 0f || viewportPosition.x > 1f)
        {
            swimDirection.x = -swimDirection.x; // 反转水平方向
            viewportPosition.x = Mathf.Clamp(viewportPosition.x, 0.01f, 0.99f); // 确保鱼保持在边界内

            // 更新鱼的朝向以反映反弹后的方向
            UpdateFishRotation();
        }

        if (viewportPosition.y < 0f || viewportPosition.y > 1f)
        {
            swimDirection.y = -swimDirection.y; // 反转垂直方向
            viewportPosition.y = Mathf.Clamp(viewportPosition.y, 0.01f, 0.99f); // 确保鱼保持在边界内

            // 更新鱼的朝向以反映反弹后的方向
            UpdateFishRotation();
        }

        // 将视口坐标转回世界坐标
        transform.position = mainCamera.ViewportToWorldPoint(viewportPosition);
    }

    void UpdateFishRotation()
    {
        // 让鱼的朝向面向游动方向
        if (swimDirection.x > 0)
        {
            // 朝向右侧
            transform.rotation = Quaternion.Euler(0f, -90f, 0f);
        }
        else if (swimDirection.x < 0)
        {
            // 朝向左侧
            transform.rotation = Quaternion.Euler(0f, 90f, 0f);
        }
    }
}
