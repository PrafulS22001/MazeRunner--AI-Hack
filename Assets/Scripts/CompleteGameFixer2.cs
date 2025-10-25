using UnityEngine;

/// <summary>
/// Ultimate fixer - Removes ALL old systems and ensures only new ones work
/// Run this ONCE to clean everything up
/// </summary>
public class CompleteGameFixer2 : MonoBehaviour
{
    void Start()
    {
        Invoke("FixEverything", 0.5f);
    }
    
    [ContextMenu("Fix Everything Now")]
    public void FixEverything()
    {
        Debug.Log("🔧 === COMPLETE GAME FIXER STARTING === 🔧");
        
        // Fix 1: Remove old fog system
        RemoveOldFogSystem();
        
        // Fix 2: Remove duplicate health bars
        RemoveDuplicateHealthBars();
        
        // Fix 3: Ensure only new systems active
        EnableNewSystems();
        
        // Fix 4: Clean up old canvases
        CleanupOldCanvases();
        
        // Fix 5: Apply realistic graphics
        ApplyRealisticGraphics();
        
        Debug.Log("✅ === COMPLETE FIX DONE === ✅");
    }
    
    void RemoveOldFogSystem()
    {
        Debug.Log("🌫️ Removing old fog systems...");
        
        // Find and destroy CloudSystem (old fog)
        GameObject cloudSystem = GameObject.Find("CloudSystem");
        if (cloudSystem != null)
        {
            Destroy(cloudSystem);
            Debug.Log("✅ Destroyed old CloudSystem");
        }
        
        // Find and disable ClashStyleCloudFog components using reflection
        DisableOldFogComponents("ClashStyleCloudFog");
        
        // Find any GameObject with "Cloud" in name (old fog)
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("Cloud") && !obj.name.Contains("Fog"))
            {
                if (obj.GetComponent<SpriteRenderer>() != null)
                {
                    SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
                    if (sr.sortingOrder < 500) // Old clouds have low sorting order
                    {
                        Destroy(obj);
                    }
                }
            }
        }
        
        // Disable old fog components using reflection (safer - no compile errors)
        DisableOldFogComponents("FogOfWar");
        DisableOldFogComponents("AdvancedFogOfWar");
        
        Debug.Log("✅ Old fog systems removed!");
    }
    
    void DisableOldFogComponents(string typeName)
    {
        try
        {
            // Use reflection to find components by name
            Object[] components = FindObjectsOfType(typeof(Component));
            int foundCount = 0;
            
            foreach (Object obj in components)
            {
                if (obj is Component comp && comp.GetType().Name == typeName)
                {
                    // Destroy the entire GameObject (for old fog systems)
                    if (typeName.Contains("Fog") || typeName.Contains("Cloud"))
                    {
                        Destroy(comp.gameObject);
                        foundCount++;
                    }
                    // Just disable the component (for other systems)
                    else if (comp is Behaviour behaviour)
                    {
                        behaviour.enabled = false;
                        foundCount++;
                    }
                }
            }
            
            if (foundCount > 0)
            {
                Debug.Log($"✅ Removed {foundCount} old {typeName} component(s)");
            }
        }
        catch (System.Exception e)
        {
            Debug.Log($"ℹ️ No {typeName} components found (this is OK): {e.Message}");
        }
    }
    
    void RemoveDuplicateHealthBars()
    {
        Debug.Log("❤️ Removing duplicate health bars...");
        
        // Find all canvases
        Canvas[] allCanvases = FindObjectsOfType<Canvas>(true);
        
        foreach (Canvas canvas in allCanvases)
        {
            // Disable GameplayUICanvas (old system)
            if (canvas.name.Contains("GameplayUICanvas") || canvas.name.Contains("GameplayUI"))
            {
                canvas.gameObject.SetActive(false);
                Debug.Log($"✅ Disabled old canvas: {canvas.name}");
            }
        }
        
        // Disable all old GameplayUI components using reflection
        DisableOldUIComponents("GameplayUI");
        
        Debug.Log("✅ Duplicate health bars removed!");
    }
    
    void DisableOldUIComponents(string typeName)
    {
        try
        {
            // Use reflection to find and disable old UI components
            Object[] components = FindObjectsOfType(typeof(Component), true);
            int foundCount = 0;
            
            foreach (Object obj in components)
            {
                if (obj is Component comp && comp.GetType().Name == typeName)
                {
                    if (comp is Behaviour behaviour)
                    {
                        behaviour.enabled = false;
                        foundCount++;
                    }
                }
            }
            
            if (foundCount > 0)
            {
                Debug.Log($"✅ Disabled {foundCount} old {typeName} component(s)");
            }
        }
        catch (System.Exception e)
        {
            Debug.Log($"ℹ️ No {typeName} components found (this is OK): {e.Message}");
        }
    }
    
    void EnableNewSystems()
    {
        Debug.Log("🎮 Ensuring new systems are active...");
        
        // Ensure ClashStyleFogSystem is active
        ClashStyleFogSystem newFog = FindObjectOfType<ClashStyleFogSystem>();
        if (newFog != null)
        {
            newFog.enabled = true;
            Debug.Log("✅ ClashStyleFogSystem active");
        }
        else
        {
            Debug.LogWarning("⚠️ ClashStyleFogSystem not found! Add it to GameSetup.");
        }
        
        // Ensure RealisticUISystem is active
        RealisticUISystem newUI = FindObjectOfType<RealisticUISystem>();
        if (newUI != null)
        {
            newUI.enabled = true;
            Debug.Log("✅ RealisticUISystem active");
        }
        else
        {
            Debug.LogWarning("⚠️ RealisticUISystem not found! Add it to GameSetup.");
        }
        
        // Ensure AdvancedAISystem on spiders
        SpiderAI[] oldSpiders = FindObjectsOfType<SpiderAI>();
        int upgradedSpiders = 0;
        
        foreach (SpiderAI spider in oldSpiders)
        {
            spider.enabled = false;
            
            AdvancedAISystem advAI = spider.GetComponent<AdvancedAISystem>();
            if (advAI == null)
            {
                spider.gameObject.AddComponent<AdvancedAISystem>();
                upgradedSpiders++;
            }
        }
        
        if (upgradedSpiders > 0)
        {
            Debug.Log($"✅ Upgraded {upgradedSpiders} spiders to Advanced AI");
        }
        
        Debug.Log("✅ New systems verified!");
    }
    
    void CleanupOldCanvases()
    {
        Debug.Log("🧹 Cleaning up old UI elements...");
        
        // Find all canvases and check for duplicates
        Canvas[] allCanvases = FindObjectsOfType<Canvas>(true);
        int healthBarCount = 0;
        
        foreach (Canvas canvas in allCanvases)
        {
            // Count health bars
            UnityEngine.UI.Slider[] sliders = canvas.GetComponentsInChildren<UnityEngine.UI.Slider>(true);
            foreach (var slider in sliders)
            {
                if (slider.name.Contains("Health"))
                {
                    healthBarCount++;
                    
                    // If this is not from RealisticUISystem, disable it
                    if (!canvas.name.Contains("RealisticUI"))
                    {
                        slider.gameObject.SetActive(false);
                        Debug.Log($"✅ Disabled old health bar in {canvas.name}");
                    }
                }
            }
        }
        
        Debug.Log($"ℹ️ Found {healthBarCount} health bars (should be 1)");
        Debug.Log("✅ Cleanup complete!");
    }
    
    void ApplyRealisticGraphics()
    {
        Debug.Log("🎨 Applying realistic graphics...");
        
        // Use reflection to find or create RealisticGraphicsSystem
        System.Type graphicsType = System.Type.GetType("RealisticGraphicsSystem");
        
        if (graphicsType == null)
        {
            // Try to find it in all assemblies
            foreach (System.Reflection.Assembly assembly in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                graphicsType = assembly.GetType("RealisticGraphicsSystem");
                if (graphicsType != null) break;
            }
        }
        
        if (graphicsType != null)
        {
            // Find existing instance
            Component graphics = FindObjectOfType(graphicsType) as Component;
            
            if (graphics == null)
            {
                // Create new instance
                GameObject graphicsObj = new GameObject("RealisticGraphicsSystem");
                graphics = graphicsObj.AddComponent(graphicsType);
                Debug.Log("✅ Created RealisticGraphicsSystem");
            }
            else
            {
                // Call EnhanceAllGraphics method if it exists
                System.Reflection.MethodInfo enhanceMethod = graphicsType.GetMethod("EnhanceAllGraphics");
                if (enhanceMethod != null)
                {
                    enhanceMethod.Invoke(graphics, null);
                }
            }
            
            Debug.Log("✅ Realistic graphics system ready!");
        }
        else
        {
            Debug.LogWarning("⚠️ RealisticGraphicsSystem.cs not found or not compiled yet. It will be applied on next Play.");
        }
    }
    
    void Update()
    {
        // Press F12 to manually run fix
        if (Input.GetKeyDown(KeyCode.F12))
        {
            FixEverything();
        }
    }
}

