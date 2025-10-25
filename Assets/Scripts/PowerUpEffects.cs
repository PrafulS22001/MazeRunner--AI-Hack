using UnityEngine;
using System.Collections;

/// <summary>
/// Beautiful animations and effects for power-ups
/// Includes: Collection animation, floating, pulsing, particles
/// </summary>
public class PowerUpEffects : MonoBehaviour
{
    [Header("Idle Animations")]
    public bool floatAnimation = true;
    public float floatSpeed = 2f; // Faster floating
    public float floatHeight = 0.5f; // Higher bounce
    
    public bool rotateAnimation = true;
    public float rotateSpeed = 180f; // Faster rotation
    
    public bool pulseAnimation = true;
    public float pulseSpeed = 3f; // Faster pulse
    public float pulseAmount = 0.25f; // Bigger pulse
    
    [Header("Collection Effects")]
    public bool spawnParticles = true;
    public bool playSound = true;
    public bool showText = true;
    
    private Vector3 startPosition;
    private Vector3 originalScale;
    private SpriteRenderer spriteRenderer;
    private PowerUp powerUp;
    private bool isCollected = false;
    
    void Start()
    {
        startPosition = transform.position;
        originalScale = transform.localScale * 1.2f; // Make them bigger!
        transform.localScale = originalScale;
        
        spriteRenderer = GetComponent<SpriteRenderer>();
        powerUp = GetComponent<PowerUp>();
        
        // Ensure sprite renderer exists
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = CreatePowerUpSprite();
        }
        
        // Set color based on power-up type
        SetColorByType();
        
        // Add glow effect
        CreateGlowEffect();
        
        // Start animations
        StartCoroutine(AnimatePowerUp());
    }
    
    void CreateGlowEffect()
    {
        // Create glow child object
        GameObject glow = new GameObject("Glow");
        glow.transform.SetParent(transform);
        glow.transform.localPosition = Vector3.zero;
        glow.transform.localScale = Vector3.one * 1.5f;
        
        SpriteRenderer glowSR = glow.AddComponent<SpriteRenderer>();
        glowSR.sprite = CreatePowerUpSprite();
        glowSR.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.3f);
        glowSR.sortingOrder = spriteRenderer.sortingOrder - 1;
        
        // Animate glow
        StartCoroutine(AnimateGlow(glowSR));
    }
    
    System.Collections.IEnumerator AnimateGlow(SpriteRenderer glowSR)
    {
        float time = 0f;
        
        while (!isCollected && glowSR != null)
        {
            time += Time.deltaTime * 2f;
            
            float pulse = 0.3f + Mathf.Sin(time) * 0.2f;
            Color c = glowSR.color;
            c.a = pulse;
            glowSR.color = c;
            
            glowSR.transform.localScale = Vector3.one * (1.5f + Mathf.Sin(time) * 0.2f);
            
            yield return null;
        }
    }
    
    void SetColorByType()
    {
        if (powerUp == null) return;
        
        Color color = Color.white;
        
        switch (powerUp.type)
        {
            case PowerUp.PowerUpType.HealthPack:
                color = new Color(0.2f, 1f, 0.3f); // Bright Green
                break;
            case PowerUp.PowerUpType.SpeedBoost:
                color = new Color(1f, 0.9f, 0.2f); // Bright Yellow
                break;
            case PowerUp.PowerUpType.Invisibility:
                color = new Color(0.3f, 1f, 1f); // Bright Cyan
                break;
            case PowerUp.PowerUpType.SpiderRepellent:
                color = new Color(1f, 0.2f, 0.2f); // Bright Red
                break;
            case PowerUp.PowerUpType.ScoreMultiplier:
                color = new Color(1f, 0.3f, 1f); // Bright Magenta
                break;
        }
        
        spriteRenderer.color = color;
        
        // Make power-ups GLOW (increase brightness)
        spriteRenderer.material = new Material(Shader.Find("Sprites/Default"));
        spriteRenderer.sortingOrder = 15; // Above most things
    }
    
    IEnumerator AnimatePowerUp()
    {
        float time = 0f;
        
        while (!isCollected)
        {
            time += Time.deltaTime;
            
            Vector3 newPosition = startPosition;
            Vector3 newScale = originalScale;
            
            // Floating animation
            if (floatAnimation)
            {
                float floatOffset = Mathf.Sin(time * floatSpeed) * floatHeight;
                newPosition.y = startPosition.y + floatOffset;
            }
            
            // Pulse animation
            if (pulseAnimation)
            {
                float pulse = 1f + Mathf.Sin(time * pulseSpeed) * pulseAmount;
                newScale = originalScale * pulse;
            }
            
            // Rotation animation
            if (rotateAnimation)
            {
                transform.Rotate(0, 0, rotateSpeed * Time.deltaTime);
            }
            
            transform.position = newPosition;
            transform.localScale = newScale;
            
            yield return null;
        }
    }
    
    public void OnCollected(GameObject collector)
    {
        if (isCollected) return;
        
        isCollected = true;
        StopAllCoroutines();
        
        StartCoroutine(CollectionAnimation(collector));
    }
    
    IEnumerator CollectionAnimation(GameObject collector)
    {
        // Spawn particles
        if (spawnParticles)
        {
            SpawnCollectionParticles();
        }
        
        // Play sound
        if (playSound)
        {
            PlayCollectionSound();
        }
        
        // Show floating text
        if (showText)
        {
            ShowFloatingText();
        }
        
        // Animate to player
        Vector3 startPos = transform.position;
        Vector3 startScale = transform.localScale;
        float duration = 0.5f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            // Smooth curve
            float smoothT = t * t * (3f - 2f * t);
            
            // Move towards player
            if (collector != null)
            {
                transform.position = Vector3.Lerp(startPos, collector.transform.position, smoothT);
            }
            
            // Scale up then down
            float scale = Mathf.Sin(t * Mathf.PI) * 0.5f + 1f;
            transform.localScale = startScale * scale;
            
            // Fade out
            if (spriteRenderer != null)
            {
                Color color = spriteRenderer.color;
                color.a = 1f - smoothT;
                spriteRenderer.color = color;
            }
            
            yield return null;
        }
        
        // Destroy after animation
        Destroy(gameObject);
    }
    
    void SpawnCollectionParticles()
    {
        int particleCount = 12;
        
        for (int i = 0; i < particleCount; i++)
        {
            GameObject particle = new GameObject("Particle");
            particle.transform.position = transform.position;
            
            SpriteRenderer sr = particle.AddComponent<SpriteRenderer>();
            sr.sprite = CreateParticleSprite();
            sr.color = spriteRenderer.color;
            sr.sortingOrder = 20;
            
            // Random direction
            float angle = (360f / particleCount) * i + Random.Range(-15f, 15f);
            Vector2 direction = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad)
            );
            
            StartCoroutine(AnimateParticle(particle, direction));
        }
    }
    
    IEnumerator AnimateParticle(GameObject particle, Vector2 direction)
    {
        float lifetime = 0.5f;
        float speed = 3f;
        Vector3 startPos = particle.transform.position;
        SpriteRenderer sr = particle.GetComponent<SpriteRenderer>();
        
        float elapsed = 0f;
        while (elapsed < lifetime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / lifetime;
            
            // Move outward
            particle.transform.position = startPos + (Vector3)(direction * speed * t);
            
            // Fade out
            Color color = sr.color;
            color.a = 1f - t;
            sr.color = color;
            
            // Scale down
            particle.transform.localScale = Vector3.one * (1f - t * 0.5f);
            
            yield return null;
        }
        
        Destroy(particle);
    }
    
    void PlayCollectionSound()
    {
        // Audio system removed - sound effects can be added later if needed
        Debug.Log($"🎵 Power-up collected sound: {powerUp?.type}");
    }
    
    void ShowFloatingText()
    {
        GameObject textGO = new GameObject("FloatingText");
        textGO.transform.position = transform.position + Vector3.up * 0.5f;
        
        // Create canvas for text
        Canvas canvas = textGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.sortingOrder = 100;
        
        RectTransform rt = textGO.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(2, 1);
        
        // Create text
        GameObject text = new GameObject("Text");
        text.transform.SetParent(textGO.transform, false);
        
        UnityEngine.UI.Text textComp = text.AddComponent<UnityEngine.UI.Text>();
        textComp.text = GetPowerUpText();
        textComp.fontSize = 24;
        textComp.fontStyle = FontStyle.Bold;
        textComp.color = spriteRenderer.color;
        textComp.alignment = TextAnchor.MiddleCenter;
        
        try
        {
            textComp.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        }
        catch
        {
            textComp.font = Resources.Load<Font>("Arial");
        }
        
        RectTransform textRT = text.GetComponent<RectTransform>();
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.offsetMin = Vector2.zero;
        textRT.offsetMax = Vector2.zero;
        
        // Animate floating text
        StartCoroutine(AnimateFloatingText(textGO));
    }
    
    IEnumerator AnimateFloatingText(GameObject textGO)
    {
        Vector3 startPos = textGO.transform.position;
        float duration = 1.5f;
        float elapsed = 0f;
        
        UnityEngine.UI.Text textComp = textGO.GetComponentInChildren<UnityEngine.UI.Text>();
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            // Float up
            textGO.transform.position = startPos + Vector3.up * t * 2f;
            
            // Fade out
            if (textComp != null)
            {
                Color color = textComp.color;
                color.a = 1f - t;
                textComp.color = color;
            }
            
            yield return null;
        }
        
        Destroy(textGO);
    }
    
    string GetPowerUpText()
    {
        if (powerUp == null) return "+Power-Up!";
        
        switch (powerUp.type)
        {
            case PowerUp.PowerUpType.HealthPack:
                return "+25 Health!";
            case PowerUp.PowerUpType.SpeedBoost:
                return "Speed Boost!";
            case PowerUp.PowerUpType.Invisibility:
                return "Invisible!";
            case PowerUp.PowerUpType.SpiderRepellent:
                return "Repellent!";
            case PowerUp.PowerUpType.ScoreMultiplier:
                return "2x Score!";
            default:
                return "+Power-Up!";
        }
    }
    
    Sprite CreatePowerUpSprite()
    {
        int size = 32;
        Texture2D tex = new Texture2D(size, size);
        Color[] pixels = new Color[size * size];
        
        Vector2 center = new Vector2(size / 2f, size / 2f);
        
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Vector2 pos = new Vector2(x, y);
                float dist = Vector2.Distance(pos, center);
                
                if (dist < size / 2f - 2f)
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
        
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }
    
    Sprite CreateParticleSprite()
    {
        int size = 8;
        Texture2D tex = new Texture2D(size, size);
        Color[] pixels = new Color[size * size];
        
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.white;
        }
        
        tex.SetPixels(pixels);
        tex.Apply();
        
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }
}

