using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TitleScaleEffect : MonoBehaviour
{
    [Header("Animation Settings")]
    public float minScale = 0.95f;
    public float maxScale = 1.05f;
    public float animationSpeed = 2f;
    [Range(0, 1)]
    public float glowIntensity = 0.5f;
    
    [Header("Optional Glow Effect")]
    public bool useGlowEffect = true;
    public Color glowColor = new Color(0.5f, 0.8f, 1f, 1f);
    
    private RectTransform rectTransform;
    private float initialScale;
    private float currentTime;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        initialScale = rectTransform.localScale.x;
        

        if (useGlowEffect)
        {

            if (GetComponent<TextMeshProUGUI>() != null)
            {
                var tmp = GetComponent<TextMeshProUGUI>();
                tmp.fontMaterial.EnableKeyword("GLOW_ON");
            }

            else if (GetComponent<Text>() != null)
            {
                var outline = GetComponent<Outline>();
                if (outline == null)
                    outline = gameObject.AddComponent<Outline>();
                outline.effectColor = glowColor;
                outline.effectDistance = new Vector2(3, 3);
            }
        }
    }

    void Update()
    {
        currentTime += Time.deltaTime * animationSpeed;
        

        float scaleOffset = Mathf.Sin(currentTime) * 0.5f + 0.5f;
        float newScale = Mathf.Lerp(minScale, maxScale, scaleOffset);
        

        Vector3 newScaleVector = new Vector3(newScale, newScale, 1f) * initialScale;
        rectTransform.localScale = newScaleVector;
        

        if (useGlowEffect)
        {
            float glowOffset = Mathf.Sin(currentTime * 1.5f) * 0.5f + 0.5f;
            if (GetComponent<TextMeshProUGUI>() != null)
            {
                var tmp = GetComponent<TextMeshProUGUI>();
                tmp.fontMaterial.SetFloat("_GlowPower", glowOffset * glowIntensity);
            }
            else if (GetComponent<Outline>() != null)
            {
                var outline = GetComponent<Outline>();
                Color newGlowColor = glowColor;
                newGlowColor.a = glowOffset * glowIntensity;
                outline.effectColor = newGlowColor;
            }
        }
    }

    void OnValidate()
    {
        if (minScale > maxScale)
            minScale = maxScale;
    }
}
