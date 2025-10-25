using UnityEngine;

/// <summary>
/// One-click fix for common visual issues
/// </summary>
public class QuickFixer : MonoBehaviour
{
    [Header("Auto-Fix on Start")]
    public bool autoFixOnStart = true;
    
    void Start()
    {
        if (autoFixOnStart)
        {
            Invoke("FixEverything", 2f); // Wait for maze to generate
            Invoke("FixEverything", 4f); // And again to be sure
        }
    }
    
    [ContextMenu("🚀 FIX EVERYTHING NOW")]
    public void FixEverything()
    {
        Debug.Log("🚀 QUICK FIX: Fixing all visual issues...");
        
        // 1. Ensure SystemsAutoEnabler exists
        EnsureSystemsAutoEnabler();
        
        // 2. Force style walls
        ForceStyleAllWalls();
        
        // 3. Enable fog
        EnableFogSystem();
        
        // 4. Enable UI
        EnableUISystem();
        
        // 5. Fix player if needed
        FixPlayerVisuals();
        
        // 6. Fix exit
        FixExitVisuals();
        
        Debug.Log("✅ QUICK FIX COMPLETE!");
    }
    
    void EnsureSystemsAutoEnabler()
    {
        SystemsAutoEnabler autoEnabler = FindFirstObjectByType<SystemsAutoEnabler>();
        
        if (autoEnabler == null)
        {
            Debug.LogWarning("⚠️ SystemsAutoEnabler not found - creating it now...");
            
            GameObject gameSetup = GameObject.Find("GameSetup");
            if (gameSetup == null)
            {
                gameSetup = new GameObject("GameSetup");
                Debug.Log("   ✅ Created GameSetup GameObject");
            }
            
            autoEnabler = gameSetup.AddComponent<SystemsAutoEnabler>();
            Debug.Log("   ✅ Added SystemsAutoEnabler component");
        }
        else
        {
            Debug.Log("   ✅ SystemsAutoEnabler already exists");
            autoEnabler.CheckAndEnableSystems();
        }
    }
    
    void ForceStyleAllWalls()
    {
        Debug.Log("🎨 Styling walls...");
        int styled = 0;
        Color targetColor = new Color(0.6f, 0.55f, 0.5f); // Stone gray
        
        // First: Style by tag (most reliable!)
        try
        {
            GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
            foreach (GameObject wall in walls)
            {
                if (wall == null) continue;
                
                SpriteRenderer sr = wall.GetComponent<SpriteRenderer>();
                if (sr != null && !IsColorSimilar(sr.color, targetColor))
                {
                    sr.color = targetColor;
                    styled++;
                }
            }
            Debug.Log($"   ✅ Styled {styled} tagged walls");
        }
        catch (UnityException)
        {
            Debug.LogWarning("   ⚠️ 'Wall' tag doesn't exist");
        }
        
        // Second: Style by characteristics (fallback)
        SpriteRenderer[] allSprites = FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None);
        int fallbackStyled = 0;
        
        foreach (SpriteRenderer sr in allSprites)
        {
            if (sr == null) continue;
            
            GameObject obj = sr.gameObject;
            
            // Check if it's likely a wall
            bool hasCorrectScale = sr.transform.localScale.x > 0.8f && sr.transform.localScale.x < 1.3f;
            bool hasSprite = sr.sprite != null;
            bool isWallLayer = obj.layer == 0 || obj.layer == 8; // Layer 8 is Wall layer
            bool notPlayer = !obj.name.Contains("Player") && !obj.CompareTag("Player");
            bool notExit = !obj.name.Contains("Exit") && !obj.name.Contains("EXIT_MARKER");
            bool notSpider = !obj.name.Contains("Spider");
            bool notBush = !obj.name.Contains("Bush");
            bool notTree = !obj.name.Contains("Tree");
            bool notFog = !obj.name.Contains("Fog") && !obj.name.Contains("Cloud");
            
            if (hasCorrectScale && hasSprite && isWallLayer && 
                notPlayer && notExit && notSpider && notBush && notTree && notFog)
            {
                // Style it as a stone wall (if not already styled)
                if (!IsColorSimilar(sr.color, targetColor))
                {
                    sr.color = targetColor;
                    fallbackStyled++;
                }
            }
        }
        
        if (fallbackStyled > 0)
        {
            Debug.Log($"   ✅ Styled {fallbackStyled} additional walls by characteristics");
        }
        
        Debug.Log($"   ✅ Total styled: {styled + fallbackStyled} walls");
    }
    
    bool IsColorSimilar(Color a, Color b, float threshold = 0.05f)
    {
        return Mathf.Abs(a.r - b.r) < threshold &&
               Mathf.Abs(a.g - b.g) < threshold &&
               Mathf.Abs(a.b - b.b) < threshold;
    }
    
    void EnableFogSystem()
    {
        ClashStyleFogSystem fog = FindFirstObjectByType<ClashStyleFogSystem>();
        if (fog != null)
        {
            fog.enabled = true;
            fog.enableFog = true;
            fog.ForceCreateFog();
            Debug.Log("   ✅ Fog system enabled");
        }
        else
        {
            Debug.LogWarning("   ⚠️ ClashStyleFogSystem not found in scene");
        }
    }
    
    void EnableUISystem()
    {
        RealisticUISystem ui = FindFirstObjectByType<RealisticUISystem>();
        if (ui != null)
        {
            ui.gameObject.SetActive(true);
            ui.enabled = true;
            Debug.Log("   ✅ UI system enabled");
        }
        else
        {
            Debug.LogWarning("   ⚠️ RealisticUISystem not found - will be created by SystemsAutoEnabler");
        }
    }
    
    void FixPlayerVisuals()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            SpriteRenderer sr = player.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                // If player is white, give it a color
                if (sr.color == Color.white)
                {
                    sr.color = new Color(0.3f, 0.7f, 1f); // Light blue
                    Debug.Log("   ✅ Set player color to light blue");
                }
                
                // Ensure player is visible
                sr.sortingOrder = 10;
            }
            
            Debug.Log($"   ✅ Player visuals fixed: {player.name}");
        }
        else
        {
            Debug.LogWarning("   ⚠️ Player not found");
        }
    }
    
    void FixExitVisuals()
    {
        ExitTrigger exit = FindFirstObjectByType<ExitTrigger>();
        if (exit != null)
        {
            SpriteRenderer sr = exit.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.color = new Color(0, 1, 0, 1); // Bright green
                sr.sortingOrder = 200;
                exit.transform.localScale = Vector3.one * 1.5f;
            }
            
            // Ensure it's a trigger
            BoxCollider2D col = exit.GetComponent<BoxCollider2D>();
            if (col != null)
            {
                col.isTrigger = true;
                col.size = Vector2.one * 1.2f;
            }
            
            Debug.Log("   ✅ Exit visuals fixed");
        }
    }
}

