using UnityEngine;

public class CameraSimulator : MonoBehaviour
{
    public float rotationSpeed = 50f; // 控制旋转速度

    void Update()
    {
        // 按下鼠标右键以旋转相机
        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            // 根据鼠标移动旋转相机
            transform.Rotate(Vector3.up, mouseX * rotationSpeed * Time.deltaTime, Space.World);
            transform.Rotate(Vector3.right, -mouseY * rotationSpeed * Time.deltaTime, Space.Self);
        }
    }
}
