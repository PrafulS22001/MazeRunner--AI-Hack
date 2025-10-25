using UnityEngine;

public class PowerUpAnimator : MonoBehaviour
{
    [Header("Floating Animation")]
    public float floatSpeed = 2f;
    public float floatAmount = 0.3f;
    public float rotationSpeed = 45f;
    
    [Header("Pulse Animation")]
    public float pulseSpeed = 3f;
    public float pulseAmount = 0.2f;
    
    [Header("Glow Effect")]
    public float glowSpeed = 1.5f;
    public float glowIntensity = 0.3f;
    
    private Vector3 startPosition;
    private Vector3 startScale;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    
    void Start()
    {
        startPosition = transform.position;
        startScale = transform.localScale;
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }
    
    void Update()
    {
        // Floating movement
        float floatY = Mathf.Sin(Time.time * floatSpeed) * floatAmount;
        transform.position = startPosition + new Vector3(0, floatY, 0);
        
        // Rotation
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        
        // Pulsing scale
        float pulse = Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
        transform.localScale = startScale * (1f + pulse);
        
        // Glowing effect
        if (spriteRenderer != null)
        {
            float glow = Mathf.Sin(Time.time * glowSpeed) * glowIntensity;
            Color glowColor = originalColor;
            glowColor.a = originalColor.a + glow;
            spriteRenderer.color = glowColor;
        }
    }
}
