using UnityEngine;

/// <summary>
/// Diagnostic tool to help identify why visuals aren't working
/// </summary>
public class DiagnosticHelper : MonoBehaviour
{
    [ContextMenu("🔍 Diagnose All Systems")]
    public void DiagnoseAllSystems()
    {
        Debug.Log("=== 🔍 DIAGNOSTIC REPORT ===");
        
        // Check SystemsAutoEnabler
        SystemsAutoEnabler autoEnabler = FindFirstObjectByType<SystemsAutoEnabler>();
        if (autoEnabler == null)
        {
            Debug.LogError("❌ SystemsAutoEnabler NOT FOUND in scene!");
            Debug.LogError("   💡 FIX: Create an empty GameObject named 'GameSetup' and add SystemsAutoEnabler component");
        }
        else
        {
            Debug.Log($"✅ SystemsAutoEnabler found on '{autoEnabler.gameObject.name}'");
            Debug.Log($"   - Fog System Active: {autoEnabler.fogSystemActive}");
            Debug.Log($"   - UI System Active: {autoEnabler.uiSystemActive}");
            Debug.Log($"   - Walls Styled: {autoEnabler.wallsStyled}");
        }
        
        // Check for walls
        int wallCount = 0;
        int whiteWalls = 0;
        int grayWalls = 0;
        
        GameObject[] taggedWalls = new GameObject[0];
        try
        {
            taggedWalls = GameObject.FindGameObjectsWithTag("Wall");
            wallCount = taggedWalls.Length;
            
            foreach (GameObject wall in taggedWalls)
            {
                SpriteRenderer sr = wall.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    if (sr.color == Color.white)
                        whiteWalls++;
                    else if (sr.color.r > 0.5f && sr.color.g > 0.5f && sr.color.b > 0.4f)
                        grayWalls++;
                }
            }
        }
        catch (UnityException)
        {
            Debug.LogWarning("⚠️ 'Wall' tag doesn't exist - walls won't be found!");
        }
        
        // Check all sprites
        SpriteRenderer[] allSprites = FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None);
        int totalSprites = allSprites.Length;
        int whiteSprites = 0;
        
        foreach (SpriteRenderer sr in allSprites)
        {
            if (sr.color == Color.white)
                whiteSprites++;
        }
        
        Debug.Log($"📊 WALL STATISTICS:");
        Debug.Log($"   - Tagged walls: {wallCount}");
        Debug.Log($"   - White walls: {whiteWalls}");
        Debug.Log($"   - Gray/styled walls: {grayWalls}");
        Debug.Log($"   - Total sprites: {totalSprites}");
        Debug.Log($"   - White sprites: {whiteSprites}");
        
        if (whiteWalls > 0)
        {
            Debug.LogWarning($"⚠️ {whiteWalls} white walls detected - these need styling!");
        }
        
        // Check player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Debug.Log($"✅ Player found: '{player.name}'");
            SpriteRenderer playerSr = player.GetComponent<SpriteRenderer>();
            if (playerSr != null)
            {
                Debug.Log($"   - Player color: {playerSr.color}");
                Debug.Log($"   - Player scale: {player.transform.localScale}");
            }
        }
        else
        {
            Debug.LogError("❌ Player not found!");
        }
        
        // Check fog
        ClashStyleFogSystem fog = FindFirstObjectByType<ClashStyleFogSystem>();
        if (fog != null)
        {
            Debug.Log($"✅ ClashStyleFogSystem found on '{fog.gameObject.name}'");
            Debug.Log($"   - Fog enabled: {fog.enableFog}");
        }
        else
        {
            Debug.LogWarning("⚠️ ClashStyleFogSystem not found!");
        }
        
        // Check UI
        RealisticUISystem ui = FindFirstObjectByType<RealisticUISystem>();
        if (ui != null)
        {
            Debug.Log($"✅ RealisticUISystem found on '{ui.gameObject.name}'");
        }
        else
        {
            Debug.LogWarning("⚠️ RealisticUISystem not found!");
        }
        
        Debug.Log("=== END DIAGNOSTIC REPORT ===");
    }
    
    [ContextMenu("🎨 Force Style All Walls NOW")]
    public void ForceStyleWalls()
    {
        Debug.Log("🎨 FORCE STYLING ALL WALLS...");
        
        int styled = 0;
        Color targetColor = new Color(0.6f, 0.55f, 0.5f); // Stone gray
        
        // Method 1: Style by Wall tag
        try
        {
            GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
            Debug.Log($"   Found {walls.Length} objects with 'Wall' tag");
            
            foreach (GameObject wall in walls)
            {
                if (wall == null) continue;
                
                SpriteRenderer sr = wall.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    Color oldColor = sr.color;
                    sr.color = targetColor;
                    styled++;
                    
                    if (styled <= 5) // Log first 5 for debugging
                    {
                        Debug.Log($"      Styled '{wall.name}' from color ({oldColor.r:F2}, {oldColor.g:F2}, {oldColor.b:F2}) to ({targetColor.r:F2}, {targetColor.g:F2}, {targetColor.b:F2})");
                    }
                }
            }
        }
        catch (UnityException)
        {
            Debug.LogWarning("   ⚠️ 'Wall' tag doesn't exist!");
        }
        
        // Method 2: Style by characteristics (if tag didn't work)
        if (styled == 0)
        {
            Debug.Log("   Trying to find walls by characteristics...");
            SpriteRenderer[] allSprites = FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None);
            
            foreach (SpriteRenderer sr in allSprites)
            {
                if (sr == null) continue;
                
                GameObject obj = sr.gameObject;
                
                // Check if it's likely a wall
                bool hasCorrectScale = sr.transform.localScale.x > 0.8f && sr.transform.localScale.x < 1.3f;
                bool hasSprite = sr.sprite != null;
                bool isWallLayer = obj.layer == 0 || obj.layer == 8;
                bool notPlayer = !obj.name.Contains("Player") && !obj.CompareTag("Player");
                bool notExit = !obj.name.Contains("Exit") && !obj.name.Contains("EXIT_MARKER");
                bool notSpider = !obj.name.Contains("Spider");
                bool notEnv = !obj.name.Contains("Bush") && !obj.name.Contains("Tree");
                
                if (hasCorrectScale && hasSprite && isWallLayer && notPlayer && notExit && notSpider && notEnv)
                {
                    sr.color = targetColor;
                    styled++;
                }
            }
        }
        
        Debug.Log($"✅ Styled {styled} walls!");
        
        if (styled == 0)
        {
            Debug.LogWarning("⚠️ No walls styled! Possible reasons:");
            Debug.LogWarning("   1. Walls haven't been created yet (run after maze generation)");
            Debug.LogWarning("   2. Wall tag doesn't exist in project");
            Debug.LogWarning("   3. Walls have unusual scale or layer settings");
        }
    }
    
    [ContextMenu("🔄 Enable All Systems")]
    public void EnableAllSystems()
    {
        SystemsAutoEnabler autoEnabler = FindFirstObjectByType<SystemsAutoEnabler>();
        if (autoEnabler != null)
        {
            autoEnabler.CheckAndEnableSystems();
        }
        else
        {
            Debug.LogError("❌ SystemsAutoEnabler not found!");
            Debug.LogError("   💡 Create an empty GameObject and add SystemsAutoEnabler component");
        }
    }
}

