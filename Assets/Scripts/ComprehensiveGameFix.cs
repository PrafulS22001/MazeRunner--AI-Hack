using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// COMPREHENSIVE GAME FIX - Fixes ALL reported issues
/// 1. Bigger maze (60x60)
/// 2. Power-up interaction fixed
/// 3. Maze path validation
/// 4. Better character graphics (not square)
/// 5. More organic maze shape
/// 6. Working radar
/// </summary>
public class ComprehensiveGameFix : MonoBehaviour
{
    void Start()
    {
        Invoke("FixAllIssues", 1f);
    }
    
    [ContextMenu("Fix All Issues Now")]
    public void FixAllIssues()
    {
        Debug.Log("🔧 === COMPREHENSIVE FIX STARTING === 🔧");
        
        StartCoroutine(FixSequence());
    }
    
    IEnumerator FixSequence()
    {
        // Fix 1: Ensure maze is bigger
        FixMazeSize();
        yield return new WaitForSeconds(0.2f);
        
        // Fix 2: Fix power-up interactions
        FixPowerUpInteractions();
        yield return new WaitForSeconds(0.2f);
        
        // Fix 3: Validate maze has path
        ValidateMazePath();
        yield return new WaitForSeconds(0.2f);
        
        // Fix 4: Enhance player graphics
        EnhancePlayerGraphics();
        yield return new WaitForSeconds(0.2f);
        
        // Fix 5: Make maze more organic
        MakeMazeOrganic();
        yield return new WaitForSeconds(0.2f);
        
        // Fix 6: Fix radar
        FixRadar();
        
        Debug.Log("✅ === ALL FIXES COMPLETE === ✅");
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
            
            Debug.Log("✅ Maze size updated to 60x60");
        }
    }
    
    void FixPowerUpInteractions()
    {
        Debug.Log("⭐ Fixing power-up interactions...");
        
        GameObject[] powerUps = GameObject.FindGameObjectsWithTag("PowerUp");
        foreach (GameObject powerUp in powerUps)
        {
            // Ensure collider exists
            CircleCollider2D collider = powerUp.GetComponent<CircleCollider2D>();
            if (collider == null)
            {
                collider = powerUp.AddComponent<CircleCollider2D>();
            }
            collider.isTrigger = true;
            collider.radius = 0.6f;
            
            // Ensure sprite renderer
            SpriteRenderer sr = powerUp.GetComponent<SpriteRenderer>();
            if (sr == null)
            {
                sr = powerUp.AddComponent<SpriteRenderer>();
            }
            
            // Ensure PowerUp script
            if (powerUp.GetComponent<PowerUp>() == null)
            {
                powerUp.AddComponent<PowerUp>();
            }
        }
        
        Debug.Log($"✅ Fixed {powerUps.Length} power-ups");
    }
    
    void ValidateMazePath()
    {
        Debug.Log("🗺️ Validating maze path...");
        
        MazeValidator validator = FindObjectOfType<MazeValidator>();
        if (validator != null)
        {
            validator.enabled = true;
            Debug.Log("✅ Maze validator enabled");
        }
        else
        {
            // Create validator
            GameObject validatorObj = new GameObject("MazeValidator");
            validatorObj.AddComponent<MazeValidator>();
            Debug.Log("✅ Maze validator created");
        }
    }
    
    void EnhancePlayerGraphics()
    {
        Debug.Log("👤 Enhancing player graphics...");
        
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            SpriteRenderer sr = player.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                // Create a better player sprite (adventurer, not square)
                sr.sprite = CreateAdvancedPlayerSprite();
                sr.color = new Color(0.2f, 0.6f, 0.95f); // Bright blue
                
                // Ensure proper scale
                player.transform.localScale = Vector3.one * 0.8f;
                
                Debug.Log("✅ Player graphics enhanced");
            }
        }
    }
    
    Sprite CreateAdvancedPlayerSprite()
    {
        int size = 128; // Higher resolution
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
                    // Main body with gradient
                    float gradientFactor = 1f - (dist / radius) * 0.4f;
                    Color bodyColor = new Color(1f, 1f, 1f, gradientFactor);
                    
                    // Add shading (darker on left side)
                    if (x < center.x)
                    {
                        bodyColor *= 0.75f;
                    }
                    
                    // Add highlight (brighter on top-right)
                    if (x > center.x && y > center.y)
                    {
                        bodyColor *= 1.2f;
                    }
                    
                    tex.SetPixel(x, y, bodyColor);
                }
                else
                {
                    tex.SetPixel(x, y, Color.clear);
                }
            }
        }
        
        // Add detailed face
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
    
    void MakeMazeOrganic()
    {
        Debug.Log("🌿 Making maze more organic...");
        
        // Add variation to wall positions to make it less grid-like
        GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
        int modifiedCount = 0;
        
        foreach (GameObject wall in walls)
        {
            // Randomly offset some walls slightly
            if (Random.value < 0.3f) // 30% of walls get variation
            {
                Vector3 offset = new Vector3(
                    Random.Range(-0.1f, 0.1f),
                    Random.Range(-0.1f, 0.1f),
                    0f
                );
                wall.transform.position += offset;
                
                // Slight rotation for more organic feel
                wall.transform.rotation = Quaternion.Euler(0, 0, Random.Range(-2f, 2f));
                
                modifiedCount++;
            }
        }
        
        Debug.Log($"✅ Made maze more organic ({modifiedCount} walls modified)");
    }
    
    void FixRadar()
    {
        Debug.Log("📡 Fixing radar...");
        
        // Find or create working radar system
        WorkingRadar radar = FindObjectOfType<WorkingRadar>();
        if (radar == null)
        {
            GameObject radarObj = new GameObject("WorkingRadar");
            radar = radarObj.AddComponent<WorkingRadar>();
            Debug.Log("✅ Created working radar system");
        }
        else
        {
            radar.enabled = true;
            Debug.Log("✅ Enabled existing radar");
        }
    }
}

/// <summary>
/// Working radar that actually tracks spiders
/// </summary>
public class WorkingRadar : MonoBehaviour
{
    private Transform player;
    private GameObject radarPanel;
    private float updateInterval = 0.2f;
    private float lastUpdate = 0f;
    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        FindRadarPanel();
    }
    
    void FindRadarPanel()
    {
        // Find the minimap panel
        Canvas[] canvases = FindObjectsOfType<Canvas>();
        foreach (Canvas canvas in canvases)
        {
            Transform minimap = canvas.transform.Find("MinimapPanel");
            if (minimap != null)
            {
                radarPanel = minimap.gameObject;
                Debug.Log("✅ Found radar panel");
                return;
            }
        }
    }
    
    void Update()
    {
        if (player == null || radarPanel == null) return;
        
        // Update at intervals for performance
        if (Time.time - lastUpdate < updateInterval) return;
        lastUpdate = Time.time;
        
        UpdateRadarBlips();
    }
    
    void UpdateRadarBlips()
    {
        // Clear old blips
        foreach (Transform child in radarPanel.transform)
        {
            if (child.name.StartsWith("SpiderBlip"))
            {
                Destroy(child.gameObject);
            }
        }
        
        // Find all spiders
        GameObject[] spiders = GameObject.FindGameObjectsWithTag("Spider");
        
        foreach (GameObject spider in spiders)
        {
            float distance = Vector2.Distance(player.position, spider.transform.position);
            
            // Only show on radar if within range
            if (distance < 20f)
            {
                CreateSpiderBlip(spider.transform.position);
            }
        }
    }
    
    void CreateSpiderBlip(Vector3 spiderWorldPos)
    {
        GameObject blip = new GameObject("SpiderBlip");
        blip.transform.SetParent(radarPanel.transform, false);
        
        // Convert world position to radar position
        Vector2 offset = spiderWorldPos - player.position;
        float radarScale = 3f; // Scale factor for radar
        
        RectTransform blipRT = blip.AddComponent<RectTransform>();
        blipRT.anchorMin = new Vector2(0.5f, 0.4f);
        blipRT.anchorMax = new Vector2(0.5f, 0.4f);
        blipRT.pivot = new Vector2(0.5f, 0.5f);
        blipRT.anchoredPosition = new Vector2(offset.x * radarScale, offset.y * radarScale);
        blipRT.sizeDelta = new Vector2(6, 6);
        
        Image blipImage = blip.AddComponent<Image>();
        blipImage.color = new Color(1f, 0.2f, 0.2f, 0.9f); // Red for danger
        
        // Add pulsing effect
        StartCoroutine(PulseBlip(blipImage));
    }
    
    System.Collections.IEnumerator PulseBlip(Image blipImage)
    {
        float time = 0f;
        Color originalColor = blipImage.color;
        
        while (blipImage != null && time < updateInterval)
        {
            time += Time.deltaTime;
            float pulse = 0.5f + Mathf.Sin(time * 10f) * 0.5f;
            blipImage.color = originalColor * pulse;
            yield return null;
        }
    }
}

