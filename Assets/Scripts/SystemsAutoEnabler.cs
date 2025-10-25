using UnityEngine;

/// <summary>
/// Ensures critical game systems are always active and visible
/// This fixes missing fog, health bar, and wall visuals
/// </summary>
public class SystemsAutoEnabler : MonoBehaviour
{
    [Header("Auto-Enable Settings")]
    public bool ensureFogSystem = true;
    public bool ensureRealisticUI = true;
    public bool ensureWallVisuals = true;
    
    [Header("Status (Read Only)")]
    public bool fogSystemActive = false;
    public bool uiSystemActive = false;
    public int wallsStyled = 0;
    
    private ClashStyleFogSystem fogSystem;
    private RealisticUISystem realisticUI;
    private bool hasChecked = false;
    
    void Start()
    {
        // Check immediately, then check again to be sure
        Debug.Log("🔧 SystemsAutoEnabler started!");
        CheckAndEnableSystems(); // Immediate check!
        Invoke("CheckAndEnableSystems", 1f);
        Invoke("CheckAndEnableSystems", 3f);
        Invoke("StyleWalls", 2f); // Force wall styling after maze is built
        Invoke("StyleWalls", 4f); // And again to be sure
    }
    
    void Update()
    {
        // Continuously monitor after game starts
        if (Time.time > 5f && !hasChecked)
        {
            hasChecked = true;
            CheckAndEnableSystems();
        }
        
        // Check FREQUENTLY for wall styling (every 30 frames = 0.5 seconds)
        if (ensureWallVisuals && Time.frameCount % 30 == 0)
        {
            StyleWalls();
        }
        
        // Check other systems less frequently
        if (Time.frameCount % 300 == 0) // Every 5 seconds at 60fps
        {
            CheckAndEnableSystems();
        }
    }
    
    public void CheckAndEnableSystems()
    {
        Debug.Log("🔧 SystemsAutoEnabler: Checking game systems...");
        
        if (ensureFogSystem)
        {
            EnableFogSystem();
        }
        
        if (ensureRealisticUI)
        {
            EnableRealisticUI();
        }
        
        if (ensureWallVisuals)
        {
            StyleWalls();
        }
        
        Debug.Log($"✅ Systems check complete: Fog={fogSystemActive}, UI={uiSystemActive}, Walls={wallsStyled}");
        
        // Help user find the UI system
        if (realisticUI != null)
        {
            Debug.Log($"   📍 RealisticUISystem is on GameObject: '{realisticUI.gameObject.name}'");
        }
    }
    
    void EnableFogSystem()
    {
        // Find fog system
        if (fogSystem == null)
        {
            fogSystem = FindFirstObjectByType<ClashStyleFogSystem>();
        }
        
        if (fogSystem != null)
        {
            // Ensure it's enabled
            if (!fogSystem.enabled)
            {
                fogSystem.enabled = true;
                Debug.Log("   ✅ ClashStyleFogSystem enabled");
            }
            
            // Ensure fog is turned on
            if (!fogSystem.enableFog)
            {
                fogSystem.enableFog = true;
                Debug.Log("   ✅ Fog enabled");
            }
            
            // Force create fog if it doesn't exist yet
            if (fogSystem.enableFog)
            {
                fogSystem.ForceCreateFog();
            }
            
            fogSystemActive = true;
        }
        else
        {
            Debug.LogWarning("   ⚠️ ClashStyleFogSystem not found - creating it now...");
            CreateClashStyleFogSystem();
        }
    }
    
    void CreateClashStyleFogSystem()
    {
        // Create the GameObject
        GameObject fogObj = new GameObject("ClashStyleFogSystem");
        fogSystem = fogObj.AddComponent<ClashStyleFogSystem>();
        
        if (fogSystem != null)
        {
            fogSystem.enableFog = true;
            Debug.Log("   ✅ ClashStyleFogSystem created successfully!");
            fogSystemActive = true;
        }
        else
        {
            Debug.LogError("   ❌ Failed to create ClashStyleFogSystem");
            fogSystemActive = false;
        }
    }
    
    void EnableRealisticUI()
    {
        // Find UI system
        if (realisticUI == null)
        {
            realisticUI = FindFirstObjectByType<RealisticUISystem>();
        }
        
        if (realisticUI != null)
        {
            // Ensure GameObject is active
            if (!realisticUI.gameObject.activeSelf)
            {
                realisticUI.gameObject.SetActive(true);
                Debug.Log("   ✅ RealisticUISystem GameObject activated");
            }
            
            // Ensure component is enabled
            if (!realisticUI.enabled)
            {
                realisticUI.enabled = true;
                Debug.Log("   ✅ RealisticUISystem component enabled");
            }
            
            uiSystemActive = true;
        }
        else
        {
            // Create RealisticUISystem if it doesn't exist
            Debug.LogWarning("   ⚠️ RealisticUISystem not found - creating it now...");
            CreateRealisticUISystem();
        }
    }
    
    void CreateRealisticUISystem()
    {
        // Create the GameObject
        GameObject uiObj = new GameObject("RealisticUISystem");
        realisticUI = uiObj.AddComponent<RealisticUISystem>();
        
        if (realisticUI != null)
        {
            Debug.Log("   ✅ RealisticUISystem created successfully!");
            Debug.Log($"   📍 Location: GameObject named '{uiObj.name}' in Hierarchy");
            Debug.Log($"   🎯 To find it: Look in Hierarchy for 'RealisticUISystem'");
            uiSystemActive = true;
        }
        else
        {
            Debug.LogError("   ❌ Failed to create RealisticUISystem");
            uiSystemActive = false;
        }
    }
    
    void StyleWalls()
    {
        GameObject[] walls;
        int styled = 0;
        
        try
        {
            walls = GameObject.FindGameObjectsWithTag("Wall");
            
            // Style tagged walls - CHECK ANY COLOR, NOT JUST WHITE!
            foreach (GameObject wall in walls)
            {
                if (wall == null) continue;
                
                SpriteRenderer sr = wall.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    // Target color: Stone gray
                    Color targetColor = new Color(0.6f, 0.55f, 0.5f);
                    
                    // Only style if not already styled
                    if (!IsColorSimilar(sr.color, targetColor))
                    {
                        sr.color = targetColor;
                        styled++;
                    }
                }
            }
        }
        catch (UnityException)
        {
            // Wall tag doesn't exist, try without tag
            walls = new GameObject[0];
        }
        
        // If no tagged walls found, search all sprites
        if (walls.Length == 0)
        {
            SpriteRenderer[] allSprites = FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None);
            
            foreach (SpriteRenderer sr in allSprites)
            {
                if (sr == null) continue;
                
                // Check if it's a wall (square sprite, scale near 1, not the player/exit)
                bool hasCorrectScale = sr.transform.localScale.x > 0.8f && sr.transform.localScale.x < 1.3f;
                bool hasSprite = sr.sprite != null;
                bool isDefaultLayer = sr.gameObject.layer == 0 || sr.gameObject.layer == 8; // Layer 8 is Wall layer
                bool notPlayer = !sr.gameObject.name.Contains("Player") && !sr.gameObject.CompareTag("Player");
                bool notExit = !sr.gameObject.name.Contains("Exit") && !sr.gameObject.name.Contains("EXIT_MARKER");
                bool notSpider = !sr.gameObject.name.Contains("Spider");
                bool notEnv = !sr.gameObject.name.Contains("Bush") && !sr.gameObject.name.Contains("Tree");
                
                if (hasCorrectScale && hasSprite && isDefaultLayer && notPlayer && notExit && notSpider && notEnv)
                {
                    // Style it as a stone wall
                    Color targetColor = new Color(0.6f, 0.55f, 0.5f);
                    if (!IsColorSimilar(sr.color, targetColor))
                    {
                        sr.color = targetColor;
                        styled++;
                    }
                }
            }
        }
        
        // Update counter (only log when actually styling)
        if (styled > 0)
        {
            wallsStyled += styled;
            Debug.Log($"   ✅ Styled {styled} walls (total: {wallsStyled})");
        }
    }
    
    bool IsColorSimilar(Color a, Color b, float threshold = 0.05f)
    {
        return Mathf.Abs(a.r - b.r) < threshold &&
               Mathf.Abs(a.g - b.g) < threshold &&
               Mathf.Abs(a.b - b.b) < threshold;
    }
    
    [ContextMenu("Force Enable All Systems")]
    public void ForceEnableAll()
    {
        Debug.Log("🔧 Force enabling all systems...");
        CheckAndEnableSystems();
    }
    
    [ContextMenu("Find RealisticUISystem")]
    public void FindRealisticUI()
    {
        RealisticUISystem[] allUI = FindObjectsByType<RealisticUISystem>(FindObjectsInactive.Include, FindObjectsSortMode.None); // Include inactive
        
        if (allUI.Length == 0)
        {
            Debug.LogWarning("❌ No RealisticUISystem found anywhere in scene!");
        }
        else
        {
            Debug.Log($"✅ Found {allUI.Length} RealisticUISystem(s):");
            for (int i = 0; i < allUI.Length; i++)
            {
                Debug.Log($"   {i + 1}. GameObject: '{allUI[i].gameObject.name}' | Active: {allUI[i].gameObject.activeSelf} | Enabled: {allUI[i].enabled}");
                
                #if UNITY_EDITOR
                // Highlight it in hierarchy (Editor only)
                UnityEditor.Selection.activeGameObject = allUI[i].gameObject;
                UnityEditor.EditorGUIUtility.PingObject(allUI[i].gameObject);
                #endif
            }
        }
    }
    
    [ContextMenu("List All GameObjects")]
    public void ListAllGameObjects()
    {
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        Debug.Log($"📋 Total GameObjects in scene: {allObjects.Length}");
        Debug.Log("Root GameObjects:");
        
        foreach (GameObject obj in allObjects)
        {
            if (obj.transform.parent == null) // Root level only
            {
                Debug.Log($"   - {obj.name}");
            }
        }
    }
    
    [ContextMenu("Reset and Re-Enable")]
    public void ResetAndReEnable()
    {
        fogSystem = null;
        realisticUI = null;
        hasChecked = false;
        CheckAndEnableSystems();
    }
}




