using UnityEngine;

public class ExitGlow : MonoBehaviour
{
    [Header("Glow Settings")]
    public float glowSpeed = 2f;
    public float glowIntensity = 0.3f;
    
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }
    
    void Update()
    {
        if (spriteRenderer != null)
        {
            float glow = Mathf.Sin(Time.time * glowSpeed) * glowIntensity;
            Color glowColor = originalColor;
            glowColor.a = originalColor.a + glow;
            spriteRenderer.color = glowColor;
        }
    }
}
