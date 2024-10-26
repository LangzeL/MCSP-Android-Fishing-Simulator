using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class VolumetricLightSetup : MonoBehaviour
{
    [Header("Light Settings")]
    public Color lightColor = new Color(0.5f, 0.7f, 1f, 0.5f);
    public float lightIntensity = 1.5f;
    public float lightRange = 20f;
    public float spotAngle = 30f;

    [Header("Material Settings")]
    public float density = 0.5f;
    public float noiseScale = 10f;
    public float noiseSpeed = 0.5f;

    [ContextMenu("Setup Volumetric Light")]
    public void SetupVolumetricLight()
    {

        #if UNITY_EDITOR

        if (!AssetDatabase.IsValidFolder("Assets/Materials"))
        {
            AssetDatabase.CreateFolder("Assets", "Materials");
        }

        Material volumetricMaterial = new Material(Shader.Find("Custom/VolumetricLight"));
        AssetDatabase.CreateAsset(volumetricMaterial, "Assets/Materials/VolumetricLightMaterial.mat");


        volumetricMaterial.SetColor("_Color", lightColor);
        volumetricMaterial.SetFloat("_Intensity", lightIntensity);
        volumetricMaterial.SetFloat("_Density", density);
        volumetricMaterial.SetFloat("_NoiseScale", noiseScale);
        volumetricMaterial.SetFloat("_NoiseSpeed", noiseSpeed);
        #endif


        GameObject lightObj = new GameObject("VolumetricLight");
        lightObj.transform.parent = transform;
        Light light = lightObj.AddComponent<Light>();
        light.type = LightType.Spot;
        light.color = lightColor;
        light.intensity = lightIntensity;
        light.range = lightRange;
        light.spotAngle = spotAngle;


        GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        quad.name = "VolumetricLightQuad";
        quad.transform.parent = lightObj.transform;
        quad.transform.localPosition = Vector3.zero;
        quad.transform.localRotation = Quaternion.Euler(45, 0, 0);
        quad.transform.localScale = new Vector3(2, 2, 2);


        #if UNITY_EDITOR
        Material mat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/VolumetricLightMaterial.mat");
        if (mat != null)
        {
            quad.GetComponent<MeshRenderer>().material = mat;
        }
        #endif

        Debug.Log("Volumetric light setup complete!");
    }
}