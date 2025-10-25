using UnityEngine;

/// <summary>
/// EMERGENCY FIX - Forces wall styling immediately on every frame until walls are styled
/// Add this to ANY GameObject, press Play, and it will aggressively fix your walls!
/// </summary>
public class EmergencyWallFixer : MonoBehaviour
{
    [Header("Emergency Settings")]
    public bool fixOnStart = true;
    public bool fixEveryFrame = true;
    public Color targetWallColor = new Color(0.6f, 0.55f, 0.5f); // Stone gray
    
    [Header("Status")]
    public int totalWallsFound = 0;
    public int wallsStyled = 0;
    public bool hasFixedWalls = false;
    
    private int frameCount = 0;
    
    void Start()
    {
        Debug.Log("🚨 EMERGENCY WALL FIXER ACTIVATED!");
        
        // Check if game is blocked by menu
        CheckGameStatus();
        
        if (fixOnStart)
        {
            ForceFixWallsNow();
            
            // Also fix after longer delays (maze takes time to generate)
            Invoke("ForceFixWallsNow", 2f);
            Invoke("ForceFixWallsNow", 4f);
            Invoke("ForceFixWallsNow", 6f);
            Invoke("ForceFixWallsNow", 8f);
            Invoke("ForceFixWallsNow", 10f);
        }
    }
    
    void CheckGameStatus()
    {
        // Check if MazeGenerator exists
        MazeGenerator mazeGen = FindFirstObjectByType<MazeGenerator>();
        if (mazeGen == null)
        {
            Debug.LogError("❌ MazeGenerator NOT FOUND in scene!");
            Debug.LogError("   💡 Add a GameObject with MazeGenerator component!");
            return;
        }
        else
        {
            Debug.Log("✅ MazeGenerator found");
        }
        
        // Check if GameInitializer is blocking
        GameInitializer gameInit = GameInitializer.Instance;
        if (gameInit != null)
        {
            if (!gameInit.IsGameStarted())
            {
                Debug.LogWarning("⚠️ GAME HASN'T STARTED YET!");
                Debug.LogWarning("   🎮 GameInitializer is waiting for you to start the game");
                Debug.LogWarning("   💡 SOLUTION: Press 'START' in the menu, or set 'Skip Menu' to true in GameInitializer");
                Debug.LogWarning("   📍 Find GameInitializer on 'GameSetup' GameObject → Inspector → Check 'Skip Menu'");
            }
            else
            {
                Debug.Log("✅ Game has started");
            }
        }
        
        // Check time scale
        if (Time.timeScale == 0f)
        {
            Debug.LogWarning("⚠️ TIME IS PAUSED (Time.timeScale = 0)!");
            Debug.LogWarning("   💡 This means the game is frozen (probably at menu)");
            Debug.LogWarning("   🎮 Press START in the menu to unpause");
        }
    }
    
    void Update()
    {
        frameCount++;
        
        // Fix every 30 frames (0.5 seconds) until walls are styled
        if (fixEveryFrame && frameCount % 30 == 0 && !hasFixedWalls)
        {
            ForceFixWallsNow();
        }
    }
    
    [ContextMenu("🚨 EMERGENCY FIX WALLS NOW")]
    public void ForceFixWallsNow()
    {
        Debug.Log("🚨 EMERGENCY WALL FIX STARTING...");
        
        int styled = 0;
        totalWallsFound = 0;
        
        // METHOD 1: Find by "Wall" tag
        Debug.Log("   Method 1: Finding walls by 'Wall' tag...");
        try
        {
            GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
            totalWallsFound = walls.Length;
            Debug.Log($"      Found {walls.Length} objects with 'Wall' tag");
            
            foreach (GameObject wall in walls)
            {
                if (wall == null) continue;
                
                SpriteRenderer sr = wall.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    Color oldColor = sr.color;
                    sr.color = targetWallColor;
                    styled++;
                    
                    if (styled <= 3) // Log first 3 for debugging
                    {
                        Debug.Log($"         ✅ '{wall.name}': ({oldColor.r:F2}, {oldColor.g:F2}, {oldColor.b:F2}) → ({targetWallColor.r}, {targetWallColor.g}, {targetWallColor.b})");
                    }
                }
            }
            
            Debug.Log($"      ✅ Styled {styled} walls by tag!");
        }
        catch (UnityException e)
        {
            Debug.LogError($"      ❌ 'Wall' tag error: {e.Message}");
            Debug.LogError("      💡 FIX: Go to Tags & Layers and create 'Wall' tag!");
        }
        
        // METHOD 2: Find ALL SpriteRenderers and style likely walls
        Debug.Log("   Method 2: Finding walls by characteristics...");
        SpriteRenderer[] allSprites = FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None);
        int fallbackStyled = 0;
        
        Debug.Log($"      Checking {allSprites.Length} sprite renderers...");
        
        foreach (SpriteRenderer sr in allSprites)
        {
            if (sr == null) continue;
            
            GameObject obj = sr.gameObject;
            
            // Check if it looks like a wall
            bool hasSquareScale = Mathf.Abs(sr.transform.localScale.x - 1f) < 0.3f; // Scale near 1.0
            bool hasSprite = sr.sprite != null;
            bool isWallLayer = obj.layer == 0 || obj.layer == 8; // Default or Wall layer
            bool hasWallTag = obj.CompareTag("Wall");
            bool notPlayer = !obj.name.Contains("Player") && !obj.CompareTag("Player");
            bool notExit = !obj.name.Contains("Exit") && !obj.name.Contains("EXIT");
            bool notSpider = !obj.name.Contains("Spider");
            bool notEnv = !obj.name.Contains("Bush") && !obj.name.Contains("Tree") && !obj.name.Contains("Cloud") && !obj.name.Contains("Fog");
            
            // Style if it looks like a wall OR has Wall tag
            if (hasSprite && isWallLayer && notPlayer && notExit && notSpider && notEnv && (hasSquareScale || hasWallTag))
            {
                if (!IsColorSimilar(sr.color, targetWallColor))
                {
                    sr.color = targetWallColor;
                    fallbackStyled++;
                }
            }
        }
        
        Debug.Log($"      ✅ Styled {fallbackStyled} additional walls by characteristics");
        
        wallsStyled = styled + fallbackStyled;
        
        Debug.Log($"🚨 EMERGENCY FIX COMPLETE!");
        Debug.Log($"   📊 Total walls found by tag: {totalWallsFound}");
        Debug.Log($"   🎨 Total walls styled: {wallsStyled}");
        
        if (wallsStyled > 100)
        {
            hasFixedWalls = true;
            Debug.Log("   ✅ Walls successfully fixed! Emergency fixer will stop auto-fixing.");
        }
        else if (wallsStyled == 0)
        {
            Debug.LogWarning("   ⚠️ NO WALLS STYLED! Checking why...");
            
            // Check if maze generator exists
            MazeGenerator mazeGen = FindFirstObjectByType<MazeGenerator>();
            if (mazeGen == null)
            {
                Debug.LogError("      ❌ MazeGenerator NOT in scene! Add it first!");
            }
            else
            {
                Debug.Log("      ✅ MazeGenerator exists");
            }
            
            // Check if game has started
            GameInitializer gameInit = GameInitializer.Instance;
            if (gameInit != null && !gameInit.IsGameStarted())
            {
                Debug.LogError("      ❌ GAME HASN'T STARTED - Maze won't generate until you press START!");
                Debug.LogError("      💡 SOLUTION 1: Press 'START' button in the menu");
                Debug.LogError("      💡 SOLUTION 2: Find 'GameSetup' → GameInitializer → Check ☑ 'Skip Menu'");
            }
            else
            {
                Debug.Log("      ✅ Game has started (or no GameInitializer)");
            }
            
            // Check time scale
            if (Time.timeScale == 0f)
            {
                Debug.LogError("      ❌ TIME IS PAUSED! Game is frozen (probably at menu)");
                Debug.LogError("      💡 Press START in menu to unpause and generate maze");
            }
            else
            {
                Debug.Log("      ✅ Time is running normally");
            }
            
            Debug.LogWarning("      Other possible issues:");
            Debug.LogWarning("      • 'Wall' tag doesn't exist in project");
            Debug.LogWarning("      • Wall prefab doesn't have 'Wall' tag");
            Debug.LogWarning("      • Maze generation failed (check for errors above)");
        }
    }
    
    bool IsColorSimilar(Color a, Color b, float threshold = 0.05f)
    {
        return Mathf.Abs(a.r - b.r) < threshold &&
               Mathf.Abs(a.g - b.g) < threshold &&
               Mathf.Abs(a.b - b.b) < threshold;
    }
    
    [ContextMenu("📊 Show Wall Statistics")]
    public void ShowWallStats()
    {
        Debug.Log("=== 📊 WALL STATISTICS ===");
        
        // Check tagged walls
        try
        {
            GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
            Debug.Log($"Walls with 'Wall' tag: {walls.Length}");
            
            int darkBrown = 0;
            int stoneGray = 0;
            int white = 0;
            int other = 0;
            
            foreach (GameObject wall in walls)
            {
                SpriteRenderer sr = wall.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    Color c = sr.color;
                    
                    if (IsColorSimilar(c, new Color(0.6f, 0.55f, 0.5f)))
                        stoneGray++;
                    else if (IsColorSimilar(c, new Color(0.215f, 0.201f, 0.201f)))
                        darkBrown++;
                    else if (IsColorSimilar(c, Color.white))
                        white++;
                    else
                        other++;
                }
            }
            
            Debug.Log($"   Stone Gray (✅ styled): {stoneGray}");
            Debug.Log($"   Dark Brown (❌ not styled): {darkBrown}");
            Debug.Log($"   White: {white}");
            Debug.Log($"   Other colors: {other}");
        }
        catch (UnityException)
        {
            Debug.LogError("'Wall' tag doesn't exist!");
        }
        
        Debug.Log("=========================");
    }
}

