using UnityEngine;

public class CameraSimulator : MonoBehaviour
{
    public float rotationSpeed = 50f; // ������ת�ٶ�

    void Update()
    {
        // ��������Ҽ�����ת���
        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            // ��������ƶ���ת���
            transform.Rotate(Vector3.up, mouseX * rotationSpeed * Time.deltaTime, Space.World);
            transform.Rotate(Vector3.right, -mouseY * rotationSpeed * Time.deltaTime, Space.Self);
        }
    }
}
