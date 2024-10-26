using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class UnderwaterEffects : MonoBehaviour
{
    [Header("Particle Settings")]
    public int bubbleCount = 100;
    public float spawnRadius = 10f;
    public float bubbleSpeed = 1f;
    public GameObject bubblePrefab;
    
    [Header("Light Settings")]
    public Color volumeLightColor = new Color(0.5f, 0.7f, 1f, 0.5f);
    public float lightIntensity = 1f;
    public int rayCount = 10;

    private ParticleSystem bubbleSystem;
    private Volume postProcessVolume;

    void OnEnable()
    {
        SetupBubbleSystem();
        SetupVolumeEffects();
    }

    void SetupBubbleSystem()
    {
        if (bubbleSystem == null)
        {
            GameObject bubbleObj = new GameObject("UnderwaterBubbles");
            bubbleObj.transform.parent = transform;
            bubbleSystem = bubbleObj.AddComponent<ParticleSystem>();
        }

        var main = bubbleSystem.main;
        main.loop = true;
        main.startLifetime = 5f;
        main.startSpeed = bubbleSpeed;
        main.startSize = 0.1f;
        main.maxParticles = bubbleCount;

        var emission = bubbleSystem.emission;
        emission.rateOverTime = bubbleCount / 5f;

        var shape = bubbleSystem.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = spawnRadius;
        shape.randomDirectionAmount = 0.2f;

        var forceOverLifetime = bubbleSystem.forceOverLifetime;
        forceOverLifetime.enabled = true;

        var force = new ParticleSystem.MinMaxCurve(1f);
        forceOverLifetime.y = force;

        forceOverLifetime.x = new ParticleSystem.MinMaxCurve(-0.1f, 0.1f);
        forceOverLifetime.z = new ParticleSystem.MinMaxCurve(-0.1f, 0.1f);

        var colorOverLifetime = bubbleSystem.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { 
                new GradientColorKey(Color.white, 0.0f), 
                new GradientColorKey(Color.white, 1.0f) 
            },
            new GradientAlphaKey[] { 
                new GradientAlphaKey(0f, 0.0f), 
                new GradientAlphaKey(0.5f, 0.2f),
                new GradientAlphaKey(0.5f, 0.8f),
                new GradientAlphaKey(0f, 1.0f) 
            }
        );
        colorOverLifetime.color = gradient;
    }

    void SetupVolumeEffects()
    {

        GameObject volumeObject = new GameObject("Underwater Post Process");
        volumeObject.transform.parent = transform;
        postProcessVolume = volumeObject.AddComponent<Volume>();
        postProcessVolume.isGlobal = true;

        var profile = ScriptableObject.CreateInstance<VolumeProfile>();
        postProcessVolume.profile = profile;


        var chromaticAberration = profile.Add<ChromaticAberration>(true);
        chromaticAberration.intensity.value = 0.5f;

        var colorAdjustments = profile.Add<ColorAdjustments>(true);
        colorAdjustments.colorFilter.value = new Color(0.8f, 0.9f, 1f);

        var vignette = profile.Add<Vignette>(true);
        vignette.intensity.value = 0.4f;
        vignette.color.value = new Color(0f, 0.2f, 0.4f);

        var depthOfField = profile.Add<DepthOfField>(true);
        depthOfField.focusDistance.value = 10f;
        depthOfField.aperture.value = 5.6f;
    }

    void Update()
    {
        if (bubbleSystem != null)
        {
            var main = bubbleSystem.main;
            main.startSpeed = bubbleSpeed;
            main.maxParticles = bubbleCount;

            var emission = bubbleSystem.emission;
            emission.rateOverTime = bubbleCount / 5f;

            var shape = bubbleSystem.shape;
            shape.radius = spawnRadius;
        }
    }

    private void OnDisable()
    {
        if (bubbleSystem != null)
        {
            Destroy(bubbleSystem.gameObject);
        }

        if (postProcessVolume != null)
        {
            Destroy(postProcessVolume.gameObject);
        }
    }
}
