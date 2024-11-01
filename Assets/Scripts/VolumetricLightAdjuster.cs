using UnityEngine;

public class VolumetricLightAdjuster : MonoBehaviour
{
    [Header("Light Settings")]
    public Light spotLight;
    [Range(1, 10)]
    public float lightIntensity = 4.5f;
    [Range(10, 50)]
    public float lightRange = 35f;
    [Range(30, 150)]
    public float spotAngle = 110f;
    public Color lightColor = new Color(0.6f, 0.8f, 1f, 1f);

    [Header("Quad Settings")]
    public GameObject volumetricQuad;
    public Vector3 quadPosition = new Vector3(0, 3.5f, 0);
    public Vector3 quadRotation = new Vector3(82, 0, 0);
    public Vector3 quadScale = new Vector3(4, 4, 4);

    [Header("Material Settings")]
    public Material volumetricMaterial;
    [Range(0, 1)]
    public float materialAlpha = 0.7f;
    [Range(0, 2)]
    public float materialIntensity = 1.3f;
    [Range(0, 1)]
    public float materialDensity = 0.7f;
    [Range(0, 20)]
    public float noiseScale = 15f;
    [Range(0, 1)]
    public float noiseSpeed = 0.4f;

    [Header("Additional Effects")]
    [Range(0, 1)]
    public float colorSaturation = 0.6f;
    [Range(0, 1)]
    public float bloomIntensity = 0.8f;

    void OnValidate()
    {
        if (spotLight != null)
        {
            spotLight.intensity = lightIntensity;
            spotLight.range = lightRange;
            spotLight.spotAngle = spotAngle;
            
            Color.RGBToHSV(lightColor, out float h, out float s, out float v);
            s *= (1 + colorSaturation);
            spotLight.color = Color.HSVToRGB(h, s, v);
        }

        if (volumetricQuad != null)
        {
            volumetricQuad.transform.localPosition = quadPosition;
            volumetricQuad.transform.localRotation = Quaternion.Euler(quadRotation);
            volumetricQuad.transform.localScale = quadScale;
        }

        if (volumetricMaterial != null)
        {
            Color matColor = lightColor;
            matColor.a = materialAlpha;
            volumetricMaterial.SetColor("_Color", matColor);
            volumetricMaterial.SetFloat("_Intensity", materialIntensity);
            volumetricMaterial.SetFloat("_Density", materialDensity);
            volumetricMaterial.SetFloat("_NoiseScale", noiseScale);
            volumetricMaterial.SetFloat("_NoiseSpeed", noiseSpeed);
            
            if(volumetricMaterial.HasProperty("_BloomIntensity"))
            {
                volumetricMaterial.SetFloat("_BloomIntensity", bloomIntensity);
            }
        }
    }

    void Start()
    {
        // 初始化引用
        if (spotLight == null)
            spotLight = GetComponentInChildren<Light>();
        
        if (volumetricQuad == null)
            volumetricQuad = GetComponentInChildren<MeshRenderer>()?.gameObject;
            
        if (volumetricMaterial == null && volumetricQuad != null)
            volumetricMaterial = volumetricQuad.GetComponent<MeshRenderer>()?.material;

        OnValidate();
    }


    [ContextMenu("Create Additional Light Beams")]
    void CreateAdditionalLightBeams()
    {
        int beamCount = 3;
        float angleStep = 360f / beamCount;
        
        for(int i = 0; i < beamCount; i++)
        {
            if(i == 0) continue; 

            GameObject newBeam = Instantiate(volumetricQuad, transform);
            newBeam.name = $"VolumetricLight_Beam_{i}";
            
            float angle = angleStep * i;
            Vector3 newPosition = quadPosition;
            newPosition.x = Mathf.Sin(angle * Mathf.Deg2Rad) * 2;
            newPosition.z = Mathf.Cos(angle * Mathf.Deg2Rad) * 2;
            
            newBeam.transform.localPosition = newPosition;
            newBeam.transform.localRotation = Quaternion.Euler(quadRotation) * Quaternion.Euler(0, angle, 0);
            newBeam.transform.localScale = quadScale;

            GameObject newLight = new GameObject($"SpotLight_Beam_{i}");
            newLight.transform.parent = transform;
            Light light = newLight.AddComponent<Light>();
            light.type = LightType.Spot;
            light.intensity = lightIntensity * 0.8f;
            light.range = lightRange;
            light.spotAngle = spotAngle;
            light.color = spotLight.color;
            
            newLight.transform.position = newBeam.transform.position;
            newLight.transform.rotation = newBeam.transform.rotation;
        }
    }
}
