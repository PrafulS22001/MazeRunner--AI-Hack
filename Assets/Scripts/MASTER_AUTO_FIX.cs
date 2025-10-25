using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// ⚡ MASTER AUTO FIX - ONE SCRIPT TO FIX EVERYTHING! ⚡
/// Just drag this onto ANY GameObject and press Play!
/// </summary>
public class MASTER_AUTO_FIX : MonoBehaviour
{
    [Header("AUTO-FIX EVERYTHING")]
    [Tooltip("Automatically fixes everything on start")]
    public bool autoFixOnStart = true;
    
    void Start()
    {
        // Ensure GameObject is active
        if (!gameObject.activeInHierarchy)
        {
            Debug.LogError("❌ MASTER_AUTO_FIX: GameObject is INACTIVE! Please activate it in Hierarchy!");
            return;
        }
        
        if (autoFixOnStart)
        {
            StartCoroutine(FixEverythingNow());
        }
    }
    
    void OnEnable()
    {
        // Also try when enabled
        if (autoFixOnStart && Application.isPlaying)
        {
            StartCoroutine(FixEverythingNow());
        }
    }
    
    [ContextMenu("⚡ FIX EVERYTHING NOW ⚡")]
    public void FixEverythingManually()
    {
        StartCoroutine(FixEverythingNow());
    }
    
    IEnumerator FixEverythingNow()
    {
        Debug.Log("⚡⚡⚡ MASTER AUTO FIX STARTING ⚡⚡⚡");
        
        // Fix 1: Maze Size
        FixMazeSize();
        yield return new WaitForSeconds(0.1f);
        
        // Fix 2: Remove old fog/UI systems
        RemoveOldSystems();
        yield return new WaitForSeconds(0.1f);
        
        // Fix 3: Enhance ALL graphics
        EnhanceAllGraphics();
        yield return new WaitForSeconds(0.2f);
        
        // Fix 4: Fix power-ups
        FixPowerUps();
        yield return new WaitForSeconds(0.1f);
        
        // Fix 5: Setup radar
        SetupRadar();
        yield return new WaitForSeconds(0.1f);
        
        // Fix 6: Upgrade spider AI
        UpgradeSpiderAI();
        yield return new WaitForSeconds(0.1f);
        
        // Fix 7: Setup UI properly
        SetupUI();
        
        Debug.Log("✅✅✅ MASTER AUTO FIX COMPLETE! GAME READY! ✅✅✅");
    }
    
    void FixMazeSize()
    {
        Debug.Log("📏 Fixing maze size...");
        MazeGenerator maze = FindObjectOfType<MazeGenerator>();
        if (maze != null)
        {
            maze.mazeWidth = 60;
            maze.mazeHeight = 60;
            maze.gladeSize = 10;
            maze.spiderCount = 8;
            maze.powerUpCount = 15;
            Debug.Log("✅ Maze size: 60x60");
        }
    }
    
    void RemoveOldSystems()
    {
        Debug.Log("🗑️ Removing old systems...");
        
        // Remove old fog
        GameObject cloudSystem = GameObject.Find("CloudSystem");
        if (cloudSystem != null) Destroy(cloudSystem);
        
        // Disable old UI components using reflection
        DisableOldComponent("GameplayUI");
        DisableOldComponent("MenuSystem");
        DisableOldComponent("ClashStyleCloudFog");
        DisableOldComponent("ComprehensiveGameFix");
        DisableOldComponent("CompleteGameFixer");
        DisableOldComponent("CompleteGameFixer2");
        DisableOldComponent("MasterGameSetup");
        
        Debug.Log("✅ Old systems removed");
    }
    
    void DisableOldComponent(string typeName)
    {
        try
        {
            Object[] components = FindObjectsOfType(typeof(Component), true);
            foreach (Object obj in components)
            {
                if (obj is Component comp && comp.GetType().Name == typeName)
                {
                    if (comp is Behaviour behaviour)
                    {
                        behaviour.enabled = false;
                    }
                }
            }
        }
        catch { }
    }
    
    void EnhanceAllGraphics()
    {
        Debug.Log("🎨 Enhancing graphics...");
        
        // Enhance walls
        GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
        Debug.Log($"🧱 Enhancing {walls.Length} walls...");
        foreach (GameObject wall in walls)
        {
            SpriteRenderer sr = wall.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sprite = CreateStoneBrickSprite();
                sr.color = new Color(0.4f, 0.35f, 0.3f);
            }
        }
        
        // Enhance player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Debug.Log("👤 Enhancing player...");
            SpriteRenderer sr = player.GetComponent<SpriteRenderer>();
            if (sr == null) sr = player.AddComponent<SpriteRenderer>();
            
            sr.sprite = CreatePlayerSprite();
            sr.color = new Color(0.2f, 0.6f, 0.95f);
            sr.sortingOrder = 10;
            player.transform.localScale = Vector3.one * 0.8f;
            
            Debug.Log("✅ Player enhanced");
        }
        
        // Enhance spiders
        GameObject[] spiders = GameObject.FindGameObjectsWithTag("Spider");
        Debug.Log($"🕷️ Enhancing {spiders.Length} spiders...");
        foreach (GameObject spider in spiders)
        {
            SpriteRenderer sr = spider.GetComponent<SpriteRenderer>();
            if (sr == null) sr = spider.AddComponent<SpriteRenderer>();
            
            sr.sprite = CreateSpiderSprite();
            sr.color = new Color(0.15f, 0.1f, 0.12f);
            sr.sortingOrder = 5;
        }
        
        // Enhance power-ups
        GameObject[] powerUps = GameObject.FindGameObjectsWithTag("PowerUp");
        Debug.Log($"⭐ Enhancing {powerUps.Length} power-ups...");
        foreach (GameObject powerUp in powerUps)
        {
            SpriteRenderer sr = powerUp.GetComponent<SpriteRenderer>();
            if (sr == null) sr = powerUp.AddComponent<SpriteRenderer>();
            
            PowerUp script = powerUp.GetComponent<PowerUp>();
            if (script != null)
            {
                switch (script.type)
                {
                    case PowerUp.PowerUpType.HealthPack:
                        sr.sprite = CreateHealthPackSprite();
                        sr.color = new Color(1f, 0.3f, 0.3f);
                        break;
                    case PowerUp.PowerUpType.SpeedBoost:
                        sr.sprite = CreateSpeedBoostSprite();
                        sr.color = new Color(0.3f, 1f, 0.3f);
                        break;
                    case PowerUp.PowerUpType.Invisibility:
                        sr.sprite = CreateShieldSprite();
                        sr.color = new Color(0.3f, 0.6f, 1f);
                        break;
                    case PowerUp.PowerUpType.SpiderRepellent:
                        sr.sprite = CreateShieldSprite();
                        sr.color = new Color(1f, 0.2f, 0.2f);
                        break;
                    case PowerUp.PowerUpType.ScoreMultiplier:
                        sr.sprite = CreateCoinSprite();
                        sr.color = new Color(1f, 0.8f, 0.2f);
                        break;
                }
            }
            sr.sortingOrder = 8;
        }
        
        Debug.Log("✅ All graphics enhanced!");
    }
    
    void FixPowerUps()
    {
        Debug.Log("⭐ Fixing power-ups...");
        GameObject[] powerUps = GameObject.FindGameObjectsWithTag("PowerUp");
        
        foreach (GameObject powerUp in powerUps)
        {
            CircleCollider2D col = powerUp.GetComponent<CircleCollider2D>();
            if (col == null) col = powerUp.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = 0.6f;
            
            if (powerUp.GetComponent<PowerUp>() == null)
            {
                powerUp.AddComponent<PowerUp>();
            }
        }
        
        Debug.Log($"✅ Fixed {powerUps.Length} power-ups");
    }
    
    void SetupRadar()
    {
        Debug.Log("📡 Setting up radar...");
        
        if (FindObjectOfType<WorkingRadar>() == null)
        {
            GameObject radarObj = new GameObject("WorkingRadar");
            radarObj.AddComponent<WorkingRadar>();
            Debug.Log("✅ Radar created");
        }
    }
    
    void UpgradeSpiderAI()
    {
        Debug.Log("🤖 Upgrading spider AI...");
        
        SpiderAI[] oldAI = FindObjectsOfType<SpiderAI>();
        int upgraded = 0;
        
        foreach (SpiderAI spider in oldAI)
        {
            spider.enabled = false;
            if (spider.GetComponent<AdvancedAISystem>() == null)
            {
                spider.gameObject.AddComponent<AdvancedAISystem>();
                upgraded++;
            }
        }
        
        if (upgraded > 0)
        {
            Debug.Log($"✅ Upgraded {upgraded} spiders to Advanced AI");
        }
    }
    
    void SetupUI()
    {
        Debug.Log("🎮 Setting up UI...");
        
        // Ensure RealisticUISystem exists
        if (FindObjectOfType<RealisticUISystem>() == null)
        {
            GameObject uiObj = new GameObject("RealisticUISystem");
            uiObj.AddComponent<RealisticUISystem>();
            Debug.Log("✅ RealisticUISystem created");
        }
        
        // Ensure BeautifulMenuSystem exists
        if (FindObjectOfType<BeautifulMenuSystem>() == null)
        {
            GameObject menuObj = new GameObject("BeautifulMenuSystem");
            menuObj.AddComponent<BeautifulMenuSystem>();
            Debug.Log("✅ BeautifulMenuSystem created");
        }
    }
    
    // === SPRITE CREATION ===
    
    Sprite CreateStoneBrickSprite()
    {
        int size = 64;
        Texture2D tex = new Texture2D(size, size);
        
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float noise = Mathf.PerlinNoise(x * 0.1f, y * 0.1f);
                Color baseColor = new Color(0.5f + noise * 0.2f, 0.45f + noise * 0.15f, 0.4f + noise * 0.1f);
                
                if (y % 16 == 0 || x % 32 == 0)
                {
                    baseColor *= 0.6f;
                }
                
                tex.SetPixel(x, y, baseColor);
            }
        }
        
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }
    
    Sprite CreatePlayerSprite()
    {
        int size = 128;
        Texture2D tex = new Texture2D(size, size);
        Vector2 center = new Vector2(size / 2, size / 2);
        float radius = size / 2.2f;
        
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), center);
                
                if (dist < radius)
                {
                    float gradientFactor = 1f - (dist / radius) * 0.4f;
                    Color bodyColor = new Color(1f, 1f, 1f, gradientFactor);
                    
                    if (x < center.x) bodyColor *= 0.75f;
                    if (x > center.x && y > center.y) bodyColor *= 1.2f;
                    
                    tex.SetPixel(x, y, bodyColor);
                }
                else
                {
                    tex.SetPixel(x, y, Color.clear);
                }
            }
        }
        
        // Eyes
        int eyeY = (int)(center.y + radius * 0.25f);
        int eyeLeftX = (int)(center.x - radius * 0.25f);
        int eyeRightX = (int)(center.x + radius * 0.25f);
        int eyeSize = 5;
        
        for (int i = -eyeSize; i <= eyeSize; i++)
        {
            for (int j = -eyeSize; j <= eyeSize; j++)
            {
                if (i * i + j * j <= eyeSize * eyeSize)
                {
                    tex.SetPixel(eyeLeftX + i, eyeY + j, new Color(0.1f, 0.1f, 0.15f, 1f));
                    tex.SetPixel(eyeRightX + i, eyeY + j, new Color(0.1f, 0.1f, 0.15f, 1f));
                }
            }
        }
        
        // Eye glints
        tex.SetPixel(eyeLeftX + 2, eyeY + 2, Color.white);
        tex.SetPixel(eyeRightX + 2, eyeY + 2, Color.white);
        
        // Smile
        int mouthY = (int)(center.y - radius * 0.15f);
        for (int i = -15; i <= 15; i++)
        {
            int curveY = mouthY - (int)(Mathf.Abs(i) * 0.15f);
            tex.SetPixel((int)center.x + i, curveY, new Color(0.1f, 0.1f, 0.15f, 1f));
            tex.SetPixel((int)center.x + i, curveY - 1, new Color(0.1f, 0.1f, 0.15f, 1f));
        }
        
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }
    
    Sprite CreateSpiderSprite()
    {
        int size = 64;
        Texture2D tex = new Texture2D(size, size);
        Vector2 center = new Vector2(size / 2, size / 2);
        
        for (int x = 0; x < size; x++)
            for (int y = 0; y < size; y++)
                tex.SetPixel(x, y, Color.clear);
        
        // Body
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
        
        // Legs
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
        
        for (int x = 0; x < size; x++)
            for (int y = 0; y < size; y++)
                tex.SetPixel(x, y, Color.clear);
        
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
        
        // Lightning bolt
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
                float dx = x - center.x;
                float dy = y - center.y;
                
                bool inShield = false;
                if (dy > 0)
                {
                    inShield = (dx * dx + dy * dy < 300);
                }
                else
                {
                    inShield = (Mathf.Abs(dx) < -dy * 0.8f);
                }
                
                tex.SetPixel(x, y, inShield ? Color.white : Color.clear);
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
}

