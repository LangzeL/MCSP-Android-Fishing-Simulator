using UnityEngine;
using UnityEngine.UI;

public class CameraBackground : MonoBehaviour
{
    private WebCamTexture webcamTexture;
    private RawImage rawImage;

    void Start()
    {
        // ��ȡ RawImage ���
        rawImage = GetComponent<RawImage>();

        // ��ȡ�豸�ϵ���������ͷ
        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length > 0)
        {
            // ʹ�õ�һ����������ͷ
            webcamTexture = new WebCamTexture(devices[0].name);

            // ������ͷ�Ļ�������Ϊ RawImage ������
            rawImage.texture = webcamTexture;

            // ��������ͷ�������ת�Ƕ�
            webcamTexture.Play();
            rawImage.uvRect = GetRotatedUVRect(webcamTexture.videoRotationAngle);

            // ���� RawImage �Ŀ�߱Ⱥ�����ͷ����ƥ��
            rawImage.rectTransform.localScale = new Vector3(1, -1, 1); // ����ת�Ի����ȷ����

            // ���� RawImage �ĳߴ�
            rawImage.rectTransform.sizeDelta = new Vector2(webcamTexture.width, webcamTexture.height);
        }
        else
        {
            Debug.LogError("û�м�⵽����ͷ");
        }
    }

    private Rect GetRotatedUVRect(int angle)
    {
        switch (angle)
        {
            case 90:
                return new Rect(1, 0, -1, 1);
            case 180:
                return new Rect(1, 1, -1, -1);
            case 270:
                return new Rect(0, 1, 1, -1);
            default:
                return new Rect(0, 0, 1, 1);
        }
    }

    void OnDisable()
    {
        // ֹͣ����ͷ
        if (webcamTexture != null)
        {
            webcamTexture.Stop();
        }
    }
}
