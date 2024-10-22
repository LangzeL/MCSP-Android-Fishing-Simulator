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

            // ���� RawImage �Ŀ�߱Ⱥ�����ͷ����ƥ��
            rawImage.rectTransform.localScale = new Vector3(1, -1, 1); // ����ת�Ի����ȷ����
            rawImage.rectTransform.sizeDelta = new Vector2(webcamTexture.width, webcamTexture.height);

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
