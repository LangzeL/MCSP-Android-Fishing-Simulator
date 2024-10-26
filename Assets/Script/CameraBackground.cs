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

            // ��ת��������Ӧ������ʾ
            rawImage.rectTransform.localEulerAngles = new Vector3(0, 0, -90);

            // ��ʼ��������ͷ����
            webcamTexture.Play();
        }
        else
        {
            Debug.LogError("û�м�⵽����ͷ");
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
