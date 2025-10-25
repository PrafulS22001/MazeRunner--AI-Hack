using UnityEngine;

/// <summary>
/// Enhanced Player Appearance - Makes player look cool and stand out
/// Adds: Color variations, outline, glow effect, animations
/// </summary>
public class EnhancedPlayerAppearance : MonoBehaviour
{
    [Header("Appearance Settings")]
    [Tooltip("Player character color")]
    public Color playerColor = new Color(0.3f, 0.6f, 1f); // Cool blue
    
    [Tooltip("Add glow/outline effect")]
    public bool addGlowEffect = true;
    public Color glowColor = new Color(0.5f, 0.8f, 1f);
    
    [Header("Animations")]
    [Tooltip("Idle animation (breathing effect)")]
    public bool breathingAnimation = true;
    public float breathingSpeed = 1.5f;
    public float breathingAmount = 0.05f;
    
    [Tooltip("Rotation when moving")]
    public bool rotateWhenMoving = true;
    
    [Header("Player Shapes")]
    [Tooltip("Player appearance style")]
    public PlayerShape playerShape = PlayerShape.Circle;
    
    public enum PlayerShape
    {
        Circle,
        Square,
        Triangle,
        Star,
        Heart
    }
    
    private SpriteRenderer spriteRenderer;
    private Vector3 originalScale;
    private float breathingTime;
    private Rigidbody2D rb;
    private GameObject glowObject;
    
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }
        
        originalScale = transform.localScale;
        
        // Create player sprite
        CreatePlayerSprite();
        
        // Add glow effect
        if (addGlowEffect)
        {
            CreateGlowEffect();
        }
        
        // Ensure player is on correct layer
        spriteRenderer.sortingOrder = 10; // Above walls and environment
        
        Debug.Log($"✅ Enhanced player appearance created: {playerShape}");
    }
    
    void Update()
    {
        // Breathing animation
        if (breathingAnimation)
        {
            breathingTime += Time.deltaTime * breathingSpeed;
            float breatheScale = 1f + Mathf.Sin(breathingTime) * breathingAmount;
            transform.localScale = originalScale * breatheScale;
        }
        
        // Rotation when moving
        if (rotateWhenMoving && rb != null && rb.linearVelocity.magnitude > 0.1f)
        {
            float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90);
        }
        
        // Update glow
        if (glowObject != null)
        {
            // Pulse glow
            SpriteRenderer glowSR = glowObject.GetComponent<SpriteRenderer>();
            if (glowSR != null)
            {
                float glowPulse = 0.5f + Mathf.Sin(Time.time * 3f) * 0.5f;
                Color c = glowColor;
                c.a = glowPulse * 0.5f;
                glowSR.color = c;
            }
        }
    }
    
    void CreatePlayerSprite()
    {
        Sprite playerSprite = null;
        
        switch (playerShape)
        {
            case PlayerShape.Circle:
                playerSprite = CreateCircleSprite();
                break;
            case PlayerShape.Square:
                playerSprite = CreateSquareSprite();
                break;
            case PlayerShape.Triangle:
                playerSprite = CreateTriangleSprite();
                break;
            case PlayerShape.Star:
                playerSprite = CreateStarSprite();
                break;
            case PlayerShape.Heart:
                playerSprite = CreateHeartSprite();
                break;
        }
        
        spriteRenderer.sprite = playerSprite;
        spriteRenderer.color = playerColor;
        
        // Make player bigger and more visible
        transform.localScale = Vector3.one * 0.8f;
        originalScale = transform.localScale;
    }
    
    void CreateGlowEffect()
    {
        glowObject = new GameObject("PlayerGlow");
        glowObject.transform.SetParent(transform);
        glowObject.transform.localPosition = Vector3.zero;
        glowObject.transform.localRotation = Quaternion.identity;
        glowObject.transform.localScale = Vector3.one * 1.3f;
        
        SpriteRenderer glowSR = glowObject.AddComponent<SpriteRenderer>();
        glowSR.sprite = CreateCircleSprite();
        glowSR.color = new Color(glowColor.r, glowColor.g, glowColor.b, 0.3f);
        glowSR.sortingOrder = spriteRenderer.sortingOrder - 1;
    }
    
    Sprite CreateCircleSprite()
    {
        int size = 64;
        Texture2D tex = new Texture2D(size, size);
        Color[] pixels = new Color[size * size];
        
        Vector2 center = new Vector2(size / 2f, size / 2f);
        float radius = size / 2f - 2f;
        
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Vector2 pos = new Vector2(x, y);
                float dist = Vector2.Distance(pos, center);
                
                if (dist < radius)
                {
                    // Smooth edges
                    float edgeSoftness = Mathf.Clamp01((radius - dist) / 2f);
                    pixels[y * size + x] = new Color(1, 1, 1, edgeSoftness);
                }
                else
                {
                    pixels[y * size + x] = Color.clear;
                }
            }
        }
        
        tex.SetPixels(pixels);
        tex.Apply();
        
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100);
    }
    
    Sprite CreateSquareSprite()
    {
        int size = 64;
        Texture2D tex = new Texture2D(size, size);
        Color[] pixels = new Color[size * size];
        
        int padding = 8;
        
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                if (x > padding && x < size - padding && y > padding && y < size - padding)
                {
                    pixels[y * size + x] = Color.white;
                }
                else
                {
                    pixels[y * size + x] = Color.clear;
                }
            }
        }
        
        tex.SetPixels(pixels);
        tex.Apply();
        
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100);
    }
    
    Sprite CreateTriangleSprite()
    {
        int size = 64;
        Texture2D tex = new Texture2D(size, size);
        Color[] pixels = new Color[size * size];
        
        Vector2 p1 = new Vector2(size / 2f, size - 8); // Top
        Vector2 p2 = new Vector2(8, 8); // Bottom left
        Vector2 p3 = new Vector2(size - 8, 8); // Bottom right
        
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Vector2 pos = new Vector2(x, y);
                
                if (IsPointInTriangle(pos, p1, p2, p3))
                {
                    pixels[y * size + x] = Color.white;
                }
                else
                {
                    pixels[y * size + x] = Color.clear;
                }
            }
        }
        
        tex.SetPixels(pixels);
        tex.Apply();
        
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100);
    }
    
    Sprite CreateStarSprite()
    {
        int size = 64;
        Texture2D tex = new Texture2D(size, size);
        Color[] pixels = new Color[size * size];
        
        Vector2 center = new Vector2(size / 2f, size / 2f);
        
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Vector2 pos = new Vector2(x, y);
                
                float angle = Mathf.Atan2(pos.y - center.y, pos.x - center.x) * Mathf.Rad2Deg;
                float dist = Vector2.Distance(pos, center);
                
                // Create 5-pointed star shape
                float starRadius = 28f + Mathf.Cos((angle + 90) * 5f * Mathf.Deg2Rad) * 12f;
                
                if (dist < starRadius && dist > 4)
                {
                    pixels[y * size + x] = Color.white;
                }
                else
                {
                    pixels[y * size + x] = Color.clear;
                }
            }
        }
        
        tex.SetPixels(pixels);
        tex.Apply();
        
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100);
    }
    
    Sprite CreateHeartSprite()
    {
        int size = 64;
        Texture2D tex = new Texture2D(size, size);
        Color[] pixels = new Color[size * size];
        
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float px = (x - size / 2f) / (size / 4f);
                float py = (y - size / 2f) / (size / 4f);
                
                // Heart equation
                float heartEq = px * px + py * py - 1;
                heartEq = heartEq * heartEq * heartEq - px * px * py * py * py;
                
                if (heartEq < 0.05f)
                {
                    pixels[y * size + x] = Color.white;
                }
                else
                {
                    pixels[y * size + x] = Color.clear;
                }
            }
        }
        
        tex.SetPixels(pixels);
        tex.Apply();
        
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100);
    }
    
    bool IsPointInTriangle(Vector2 p, Vector2 p1, Vector2 p2, Vector2 p3)
    {
        float d1 = Sign(p, p1, p2);
        float d2 = Sign(p, p2, p3);
        float d3 = Sign(p, p3, p1);
        
        bool hasNeg = (d1 < 0) || (d2 < 0) || (d3 < 0);
        bool hasPos = (d1 > 0) || (d2 > 0) || (d3 > 0);
        
        return !(hasNeg && hasPos);
    }
    
    float Sign(Vector2 p1, Vector2 p2, Vector2 p3)
    {
        return (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
    }
    
    [ContextMenu("🎨 Change to Circle")]
    void SetCircle() { playerShape = PlayerShape.Circle; CreatePlayerSprite(); }
    
    [ContextMenu("🎨 Change to Square")]
    void SetSquare() { playerShape = PlayerShape.Square; CreatePlayerSprite(); }
    
    [ContextMenu("🎨 Change to Triangle")]
    void SetTriangle() { playerShape = PlayerShape.Triangle; CreatePlayerSprite(); }
    
    [ContextMenu("🎨 Change to Star")]
    void SetStar() { playerShape = PlayerShape.Star; CreatePlayerSprite(); }
    
    [ContextMenu("🎨 Change to Heart")]
    void SetHeart() { playerShape = PlayerShape.Heart; CreatePlayerSprite(); }
}

