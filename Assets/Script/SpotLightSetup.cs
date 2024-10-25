using UnityEngine;

[ExecuteInEditMode]
public class SpotLightSetup : MonoBehaviour
{
    [Header("Position Settings")]
    [Range(-10, 10)]
    public float posX = 0f;
    [Range(0, 10)]
    public float posY = 3f;
    [Range(-10, 10)]
    public float posZ = 0f;

    [Header("Rotation Settings")]
    [Range(0, 90)]
    public float rotationX = 70f;
    [Range(-180, 180)]
    public float rotationY = 0f;
    [Range(-180, 180)]
    public float rotationZ = 0f;

    [Header("Light Settings")]
    [Range(0, 10)]
    public float intensity = 4.5f;
    [Range(0, 50)]
    public float range = 35f;
    [Range(0, 179)]
    public float spotAngle = 110f;
    [Range(0, 100)]
    public float innerSpotAngle = 21.8f;
    public Color lightColor = new Color(0.5f, 0.7f, 1f, 1f);

    private Light spotLight;

    void OnEnable()
    {
        spotLight = GetComponent<Light>();
        if (spotLight == null)
        {
            spotLight = gameObject.AddComponent<Light>();
            spotLight.type = LightType.Spot;
        }
    }

    void Update()
    {
        if (spotLight != null)
        {

            transform.localPosition = new Vector3(posX, posY, posZ);


            transform.localRotation = Quaternion.Euler(rotationX, rotationY, rotationZ);


            spotLight.intensity = intensity;
            spotLight.range = range;
            spotLight.spotAngle = spotAngle;
            spotLight.innerSpotAngle = innerSpotAngle;
            spotLight.color = lightColor;
        }
    }

    [ContextMenu("Reset to Default Values")]
    void ResetToDefaults()
    {
        posX = 0f;
        posY = 3f;
        posZ = 0f;
        rotationX = 70f;
        rotationY = 0f;
        rotationZ = 0f;
        intensity = 4.5f;
        range = 35f;
        spotAngle = 110f;
        innerSpotAngle = 21.8f;
        lightColor = new Color(0.5f, 0.7f, 1f, 1f);
    }
}
