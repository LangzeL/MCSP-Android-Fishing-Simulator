using UnityEngine;
using UnityEngine.UI;

public class CameraBackground : MonoBehaviour
{
    private WebCamTexture webcamTexture;
    private RawImage rawImage;

    void Start()
    {
        // 获取 RawImage 组件
        rawImage = GetComponent<RawImage>();

        // 获取设备上的所有摄像头
        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length > 0)
        {
            // 使用第一个可用摄像头
            webcamTexture = new WebCamTexture(devices[0].name);

            // 将摄像头的画面设置为 RawImage 的纹理
            rawImage.texture = webcamTexture;

            // 设置 RawImage 的宽高比和摄像头画面匹配
            rawImage.rectTransform.localScale = new Vector3(1, -1, 1); // 镜像翻转以获得正确方向
            rawImage.rectTransform.sizeDelta = new Vector2(webcamTexture.width, webcamTexture.height);

            // 开始播放摄像头画面
            webcamTexture.Play();
        }
        else
        {
            Debug.LogError("没有检测到摄像头");
        }
    }

    void OnDisable()
    {
        // 停止摄像头
        if (webcamTexture != null)
        {
            webcamTexture.Stop();
        }
    }
}
