using UnityEngine;

/// <summary>
/// Creates realistic-looking sprites procedurally
/// No more boxes and circles - everything looks professional!
/// </summary>
public class RealisticGraphicsSystem : MonoBehaviour
{
    void Start()
    {
        Invoke("EnhanceAllGraphics", 0.3f);
    }
    
    [ContextMenu("Enhance All Graphics")]
    public void EnhanceAllGraphics()
    {
        Debug.Log("🎨 === CREATING REALISTIC GRAPHICS === 🎨");
        
        EnhanceWalls();
        EnhancePlayer();
        EnhanceSpiders();
        EnhancePowerUps();
        EnhanceTrees();
        
        Debug.Log("✅ === REALISTIC GRAPHICS COMPLETE === ✅");
    }
    
    void EnhanceWalls()
    {
        GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
        Debug.Log($"🧱 Enhancing {walls.Length} walls...");
        
        foreach (GameObject wall in walls)
        {
            SpriteRenderer sr = wall.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                // Create stone brick texture
                sr.sprite = CreateStoneBrickSprite();
                sr.color = new Color(0.4f, 0.35f, 0.3f); // Dark brown stone
                sr.sortingLayerName = "Default";
                sr.sortingOrder = 0;
            }
        }
        
        Debug.Log($"✅ Enhanced {walls.Length} walls");
    }
    
    void EnhancePlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Debug.Log("👤 Enhancing player...");
            
            SpriteRenderer sr = player.GetComponent<SpriteRenderer>();
            if (sr == null)
                sr = player.AddComponent<SpriteRenderer>();
            
            // Create adventurer sprite (circle with details)
            sr.sprite = CreatePlayerSprite();
            sr.color = new Color(0.2f, 0.6f, 0.9f); // Blue adventurer
            sr.sortingLayerName = "Player";
            sr.sortingOrder = 10;
            
            // Add subtle glow effect
            AddGlowEffect(player, new Color(0.3f, 0.7f, 1f, 0.3f));
            
            Debug.Log("✅ Player enhanced");
        }
    }
    
    void EnhanceSpiders()
    {
        GameObject[] spiders = GameObject.FindGameObjectsWithTag("Spider");
        Debug.Log($"🕷️ Enhancing {spiders.Length} spiders...");
        
        foreach (GameObject spider in spiders)
        {
            SpriteRenderer sr = spider.GetComponent<SpriteRenderer>();
            if (sr == null)
                sr = spider.AddComponent<SpriteRenderer>();
            
            // Create detailed spider sprite
            sr.sprite = CreateSpiderSprite();
            sr.color = new Color(0.15f, 0.1f, 0.12f); // Dark spider
            sr.sortingLayerName = "Enemies";
            sr.sortingOrder = 5;
            
            // Add red danger glow
            AddGlowEffect(spider, new Color(1f, 0.2f, 0.2f, 0.2f));
        }
        
        Debug.Log($"✅ Enhanced {spiders.Length} spiders");
    }
    
    void EnhancePowerUps()
    {
        GameObject[] powerUps = GameObject.FindGameObjectsWithTag("PowerUp");
        Debug.Log($"⭐ Enhancing {powerUps.Length} power-ups...");
        
        foreach (GameObject powerUp in powerUps)
        {
            SpriteRenderer sr = powerUp.GetComponent<SpriteRenderer>();
            if (sr == null)
                sr = powerUp.AddComponent<SpriteRenderer>();
            
            // Determine type and create appropriate sprite
            PowerUp powerUpScript = powerUp.GetComponent<PowerUp>();
            if (powerUpScript != null)
            {
                switch (powerUpScript.type)
                {
                    case PowerUp.PowerUpType.HealthPack:
                        sr.sprite = CreateHealthPackSprite();
                        sr.color = new Color(1f, 0.3f, 0.3f); // Red health
                        AddGlowEffect(powerUp, new Color(1f, 0.3f, 0.3f, 0.4f));
                        break;
                    case PowerUp.PowerUpType.SpeedBoost:
                        sr.sprite = CreateSpeedBoostSprite();
                        sr.color = new Color(0.3f, 1f, 0.3f); // Green speed
                        AddGlowEffect(powerUp, new Color(0.3f, 1f, 0.3f, 0.4f));
                        break;
                    case PowerUp.PowerUpType.Shield:
                        sr.sprite = CreateShieldSprite();
                        sr.color = new Color(0.3f, 0.6f, 1f); // Blue shield
                        AddGlowEffect(powerUp, new Color(0.3f, 0.6f, 1f, 0.4f));
                        break;
                    case PowerUp.PowerUpType.ScoreBonus:
                        sr.sprite = CreateCoinSprite();
                        sr.color = new Color(1f, 0.8f, 0.2f); // Golden coin
                        AddGlowEffect(powerUp, new Color(1f, 0.8f, 0.2f, 0.4f));
                        break;
                }
            }
            
            sr.sortingLayerName = "Items";
            sr.sortingOrder = 8;
            
            // Add rotation animation
            AddRotationAnimation(powerUp);
        }
        
        Debug.Log($"✅ Enhanced {powerUps.Length} power-ups");
    }
    
    void EnhanceTrees()
    {
        try
        {
            GameObject[] trees = GameObject.FindGameObjectsWithTag("Tree");
            Debug.Log($"🌳 Enhancing {trees.Length} trees...");
            
            foreach (GameObject tree in trees)
            {
                SpriteRenderer sr = tree.GetComponent<SpriteRenderer>();
                if (sr == null)
                    sr = tree.AddComponent<SpriteRenderer>();
                
                // Create tree sprite
                sr.sprite = CreateTreeSprite();
                sr.color = new Color(0.2f, 0.5f, 0.2f); // Dark green
                sr.sortingLayerName = "Environment";
                sr.sortingOrder = 2;
            }
            
            Debug.Log($"✅ Enhanced {trees.Length} trees");
        }
        catch
        {
            Debug.Log("ℹ️ No trees found (tag may not exist)");
        }
    }
    
    // === SPRITE CREATION METHODS ===
    
    Sprite CreateStoneBrickSprite()
    {
        int size = 64;
        Texture2D tex = new Texture2D(size, size);
        
        // Create stone brick pattern
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                // Base stone color with variation
                float noise = Mathf.PerlinNoise(x * 0.1f, y * 0.1f);
                Color baseColor = new Color(0.5f + noise * 0.2f, 0.45f + noise * 0.15f, 0.4f + noise * 0.1f);
                
                // Add brick lines
                if (y % 16 == 0 || x % 32 == 0)
                {
                    baseColor *= 0.6f; // Darker for mortar
                }
                
                tex.SetPixel(x, y, baseColor);
            }
        }
        
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }
    
    Sprite CreatePlayerSprite()
    {
        int size = 64;
        Texture2D tex = new Texture2D(size, size);
        Vector2 center = new Vector2(size / 2, size / 2);
        float radius = size / 2.5f;
        
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), center);
                
                if (dist < radius)
                {
                    // Main body
                    float alpha = 1f - (dist / radius) * 0.3f;
                    tex.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
                    
                    // Add shading
                    if (x < center.x)
                    {
                        tex.SetPixel(x, y, tex.GetPixel(x, y) * 0.8f);
                    }
                }
                else
                {
                    tex.SetPixel(x, y, Color.clear);
                }
            }
        }
        
        // Add simple face details
        int eyeY = (int)(center.y + radius * 0.2f);
        int eyeLeft = (int)(center.x - radius * 0.3f);
        int eyeRight = (int)(center.x + radius * 0.3f);
        
        // Eyes
        for (int i = -2; i <= 2; i++)
        {
            for (int j = -2; j <= 2; j++)
            {
                tex.SetPixel(eyeLeft + i, eyeY + j, new Color(0.2f, 0.2f, 0.2f, 1f));
                tex.SetPixel(eyeRight + i, eyeY + j, new Color(0.2f, 0.2f, 0.2f, 1f));
            }
        }
        
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }
    
    Sprite CreateSpiderSprite()
    {
        int size = 64;
        Texture2D tex = new Texture2D(size, size);
        Vector2 center = new Vector2(size / 2, size / 2);
        
        // Clear background
        for (int x = 0; x < size; x++)
            for (int y = 0; y < size; y++)
                tex.SetPixel(x, y, Color.clear);
        
        // Spider body (oval)
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float dx = (x - center.x) / (size * 0.3f);
                float dy = (y - center.y) / (size * 0.25f);
                float dist = Mathf.Sqrt(dx * dx + dy * dy);
                
                if (dist < 1f)
                {
                    tex.SetPixel(x, y, new Color(1f, 1f, 1f, 1f - dist * 0.3f));
                }
            }
        }
        
        // Spider legs (8 lines)
        for (int leg = 0; leg < 8; leg++)
        {
            float angle = leg * Mathf.PI / 4f;
            for (int i = 15; i < 28; i++)
            {
                int legX = (int)(center.x + Mathf.Cos(angle) * i);
                int legY = (int)(center.y + Mathf.Sin(angle) * i);
                
                if (legX >= 0 && legX < size && legY >= 0 && legY < size)
                {
                    tex.SetPixel(legX, legY, new Color(1f, 1f, 1f, 0.9f));
                    // Thicken legs
                    if (legX + 1 < size)
                        tex.SetPixel(legX + 1, legY, new Color(1f, 1f, 1f, 0.7f));
                }
            }
        }
        
        // Red eyes
        int eyeY = (int)(center.y + 4);
        tex.SetPixel((int)center.x - 3, eyeY, new Color(1f, 0f, 0f, 1f));
        tex.SetPixel((int)center.x + 3, eyeY, new Color(1f, 0f, 0f, 1f));
        
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }
    
    Sprite CreateHealthPackSprite()
    {
        int size = 48;
        Texture2D tex = new Texture2D(size, size);
        
        // Clear background
        for (int x = 0; x < size; x++)
            for (int y = 0; y < size; y++)
                tex.SetPixel(x, y, Color.clear);
        
        // White cross (medical symbol)
        int centerX = size / 2;
        int centerY = size / 2;
        int crossWidth = 8;
        int crossLength = 30;
        
        // Vertical bar
        for (int y = centerY - crossLength / 2; y < centerY + crossLength / 2; y++)
        {
            for (int x = centerX - crossWidth / 2; x < centerX + crossWidth / 2; x++)
            {
                if (x >= 0 && x < size && y >= 0 && y < size)
                    tex.SetPixel(x, y, Color.white);
            }
        }
        
        // Horizontal bar
        for (int x = centerX - crossLength / 2; x < centerX + crossLength / 2; x++)
        {
            for (int y = centerY - crossWidth / 2; y < centerY + crossWidth / 2; y++)
            {
                if (x >= 0 && x < size && y >= 0 && y < size)
                    tex.SetPixel(x, y, Color.white);
            }
        }
        
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }
    
    Sprite CreateSpeedBoostSprite()
    {
        int size = 48;
        Texture2D tex = new Texture2D(size, size);
        
        for (int x = 0; x < size; x++)
            for (int y = 0; y < size; y++)
                tex.SetPixel(x, y, Color.clear);
        
        // Lightning bolt shape
        int[] boltX = { 30, 28, 32, 24, 26, 18, 24, 20 };
        int[] boltY = { 40, 30, 28, 20, 18, 8, 24, 22 };
        
        for (int i = 0; i < boltX.Length - 1; i++)
        {
            DrawLine(tex, boltX[i], boltY[i], boltX[i + 1], boltY[i + 1], Color.white);
        }
        
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }
    
    Sprite CreateShieldSprite()
    {
        int size = 48;
        Texture2D tex = new Texture2D(size, size);
        Vector2 center = new Vector2(size / 2, size / 2);
        
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                // Shield shape (rounded top, pointed bottom)
                float dx = x - center.x;
                float dy = y - center.y;
                
                bool inShield = false;
                if (dy > 0) // Top half - circle
                {
                    inShield = (dx * dx + dy * dy < 300);
                }
                else // Bottom half - triangle
                {
                    inShield = (Mathf.Abs(dx) < -dy * 0.8f);
                }
                
                if (inShield)
                {
                    tex.SetPixel(x, y, Color.white);
                }
                else
                {
                    tex.SetPixel(x, y, Color.clear);
                }
            }
        }
        
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }
    
    Sprite CreateCoinSprite()
    {
        int size = 48;
        Texture2D tex = new Texture2D(size, size);
        Vector2 center = new Vector2(size / 2, size / 2);
        
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), center);
                
                if (dist < 18)
                {
                    // Add shine effect
                    float shine = 1f - dist / 18f;
                    if (x < center.x + 5 && y > center.y - 5)
                    {
                        shine *= 1.3f;
                    }
                    tex.SetPixel(x, y, new Color(1f, 1f, 1f, shine));
                }
                else
                {
                    tex.SetPixel(x, y, Color.clear);
                }
            }
        }
        
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }
    
    Sprite CreateTreeSprite()
    {
        int size = 64;
        Texture2D tex = new Texture2D(size, size);
        
        for (int x = 0; x < size; x++)
            for (int y = 0; y < size; y++)
                tex.SetPixel(x, y, Color.clear);
        
        // Trunk (brown rectangle)
        for (int x = 28; x < 36; x++)
        {
            for (int y = 10; y < 30; y++)
            {
                tex.SetPixel(x, y, new Color(0.4f, 0.25f, 0.1f, 1f));
            }
        }
        
        // Foliage (green triangle/cone)
        for (int y = 25; y < 55; y++)
        {
            int width = (55 - y) / 2;
            for (int x = 32 - width; x < 32 + width; x++)
            {
                if (x >= 0 && x < size)
                {
                    float noise = Mathf.PerlinNoise(x * 0.3f, y * 0.3f);
                    tex.SetPixel(x, y, new Color(1f, 1f, 1f, 0.8f + noise * 0.2f));
                }
            }
        }
        
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }
    
    // === HELPER METHODS ===
    
    void DrawLine(Texture2D tex, int x0, int y0, int x1, int y1, Color color)
    {
        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;
        
        while (true)
        {
            if (x0 >= 0 && x0 < tex.width && y0 >= 0 && y0 < tex.height)
            {
                tex.SetPixel(x0, y0, color);
                // Thicken line
                if (x0 + 1 < tex.width) tex.SetPixel(x0 + 1, y0, color);
                if (y0 + 1 < tex.height) tex.SetPixel(x0, y0 + 1, color);
            }
            
            if (x0 == x1 && y0 == y1) break;
            
            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                y0 += sy;
            }
        }
    }
    
    void AddGlowEffect(GameObject obj, Color glowColor)
    {
        // Create a child object for the glow
        GameObject glow = new GameObject("Glow");
        glow.transform.SetParent(obj.transform);
        glow.transform.localPosition = Vector3.zero;
        glow.transform.localScale = Vector3.one * 1.5f;
        
        SpriteRenderer glowSr = glow.AddComponent<SpriteRenderer>();
        glowSr.sprite = CreateCircleSprite(32);
        glowSr.color = glowColor;
        glowSr.sortingLayerName = obj.GetComponent<SpriteRenderer>().sortingLayerName;
        glowSr.sortingOrder = obj.GetComponent<SpriteRenderer>().sortingOrder - 1;
        
        // Add pulsing animation
        AddPulseAnimation(glow);
    }
    
    Sprite CreateCircleSprite(int size)
    {
        Texture2D tex = new Texture2D(size, size);
        Vector2 center = new Vector2(size / 2, size / 2);
        float radius = size / 2f;
        
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), center);
                float alpha = 1f - (dist / radius);
                alpha = Mathf.Clamp01(alpha);
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, alpha * alpha)); // Smoother falloff
            }
        }
        
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }
    
    void AddPulseAnimation(GameObject obj)
    {
        PulseEffect pulse = obj.AddComponent<PulseEffect>();
        pulse.pulseSpeed = 2f;
        pulse.pulseAmount = 0.2f;
    }
    
    void AddRotationAnimation(GameObject obj)
    {
        RotateEffect rotate = obj.AddComponent<RotateEffect>();
        rotate.rotationSpeed = 50f;
    }
}

// === ANIMATION COMPONENTS ===

public class PulseEffect : MonoBehaviour
{
    public float pulseSpeed = 2f;
    public float pulseAmount = 0.2f;
    private Vector3 baseScale;
    
    void Start()
    {
        baseScale = transform.localScale;
    }
    
    void Update()
    {
        float scale = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
        transform.localScale = baseScale * scale;
    }
}

public class RotateEffect : MonoBehaviour
{
    public float rotationSpeed = 50f;
    
    void Update()
    {
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }
}

