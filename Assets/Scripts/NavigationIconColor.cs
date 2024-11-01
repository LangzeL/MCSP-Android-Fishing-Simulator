using UnityEngine;
using UnityEngine.UI;

public class SimpleNavigationColor : MonoBehaviour
{
    [Header("Colors")]
    public Color iconColor = new Color(0.85f, 0.95f, 1f, 1f);    // 珍珠白
    public Color textColor = new Color(1f, 0.84f, 0f, 1f);       // 金色
    
    [Header("Effects")]
    public bool addGlow = true;
    public bool addTextShadow = true;
    public float glowIntensity = 0.3f;
    
    [ContextMenu("Apply Colors")]
    public void ApplyColors()
    {

        Image[] icons = GetComponentsInChildren<Image>();
        foreach (Image icon in icons)
        {
            if (icon.gameObject != gameObject) 
            {
                icon.color = iconColor;
                
                if (addGlow)
                {
                    Outline glow = icon.GetComponent<Outline>();
                    if (glow == null) glow = icon.gameObject.AddComponent<Outline>();
                    glow.effectColor = new Color(1f, 1f, 1f, glowIntensity);
                    glow.effectDistance = new Vector2(2, 2);
                }
            }
        }
        

        Text[] texts = GetComponentsInChildren<Text>();
        foreach (Text text in texts)
        {
            text.color = textColor;
            
            if (addTextShadow)
            {
                Shadow shadow = text.GetComponent<Shadow>();
                if (shadow == null) shadow = text.gameObject.AddComponent<Shadow>();
                shadow.effectColor = new Color(0.2f, 0.3f, 0.4f, 0.5f);
                shadow.effectDistance = new Vector2(1, -1);
            }
        }
    }
}
