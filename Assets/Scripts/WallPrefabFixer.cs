using UnityEngine;

/// <summary>
/// Automatically adds "Wall" tag to the Wall prefab and all existing wall clones
/// </summary>
public class WallPrefabFixer : MonoBehaviour
{
    [Header("Auto-Fix Settings")]
    public bool fixOnStart = true;
    public string wallPrefabName = "Wall";
    
    [Header("Status")]
    public int wallsTagged = 0;
    public bool hasPrefabBeenFixed = false;
    
    void Start()
    {
        Debug.Log("🔧 WALL PREFAB FIXER ACTIVATED!");
        
        if (fixOnStart)
        {
            FixWallTagsNow();
            
            // Try again after a delay in case walls are created later
            Invoke("FixWallTagsNow", 2f);
            Invoke("FixWallTagsNow", 4f);
        }
    }
    
    [ContextMenu("🔧 FIX WALL TAGS NOW")]
    public void FixWallTagsNow()
    {
        Debug.Log("🔧 FIXING WALL TAGS...");
        
        wallsTagged = 0;
        
        // METHOD 1: Find and fix the Wall Prefab in Resources/Prefabs
        FixWallPrefab();
        
        // METHOD 2: Find all existing walls in scene and tag them
        FixExistingWalls();
        
        Debug.Log($"🔧 WALL TAG FIX COMPLETE! Tagged {wallsTagged} walls");
        
        if (wallsTagged > 0)
        {
            Debug.Log("   ✅ Now run EmergencyWallFixer to style them!");
            
            // Automatically trigger wall styling
            EmergencyWallFixer fixer = FindFirstObjectByType<EmergencyWallFixer>();
            if (fixer != null)
            {
                Debug.Log("   🚀 Auto-triggering EmergencyWallFixer...");
                fixer.ForceFixWallsNow();
            }
        }
    }
    
    void FixWallPrefab()
    {
        Debug.Log("   📦 Checking Wall prefab...");
        
        // Try to find MazeGenerator to get wall prefab reference
        MazeGenerator mazeGen = FindFirstObjectByType<MazeGenerator>();
        if (mazeGen != null)
        {
            // Access the wall prefab through reflection or public field
            var wallPrefabField = mazeGen.GetType().GetField("wallPrefab");
            if (wallPrefabField != null)
            {
                GameObject wallPrefab = wallPrefabField.GetValue(mazeGen) as GameObject;
                if (wallPrefab != null)
                {
                    if (!wallPrefab.CompareTag("Wall"))
                    {
                        // Check if "Wall" tag exists first
                        try
                        {
                            wallPrefab.tag = "Wall";
                            hasPrefabBeenFixed = true;
                            Debug.Log($"      ✅ Set Wall prefab tag to 'Wall'");
                        }
                        catch (UnityException e)
                        {
                            Debug.LogError($"      ❌ Failed to set tag: {e.Message}");
                            Debug.LogError("      💡 FIX: Edit → Project Settings → Tags & Layers → Add 'Wall' tag");
                        }
                    }
                    else
                    {
                        Debug.Log("      ✅ Wall prefab already has 'Wall' tag");
                        hasPrefabBeenFixed = true;
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning("      ⚠️ MazeGenerator not found, can't access prefab");
        }
    }
    
    void FixExistingWalls()
    {
        Debug.Log("   🔍 Finding existing walls in scene...");
        
        // Find all objects that look like walls
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        
        foreach (GameObject obj in allObjects)
        {
            if (obj == null) continue;
            
            // Check if it looks like a wall
            bool hasWallInName = obj.name.Contains("Wall");
            bool hasSquareScale = Mathf.Abs(obj.transform.localScale.x - 1f) < 0.3f;
            bool hasSpriteRenderer = obj.GetComponent<SpriteRenderer>() != null;
            bool hasBoxCollider = obj.GetComponent<BoxCollider2D>() != null;
            
            // If it looks like a wall and doesn't have Wall tag
            if (hasWallInName && hasSpriteRenderer && hasBoxCollider && hasSquareScale)
            {
                if (!obj.CompareTag("Wall"))
                {
                    try
                    {
                        obj.tag = "Wall";
                        wallsTagged++;
                        
                        if (wallsTagged <= 5) // Log first 5
                        {
                            Debug.Log($"      ✅ Tagged '{obj.name}' as 'Wall'");
                        }
                    }
                    catch (UnityException e)
                    {
                        if (wallsTagged == 0) // Only log once
                        {
                            Debug.LogError($"      ❌ Can't set tag: {e.Message}");
                            Debug.LogError("      💡 SOLUTION: Create 'Wall' tag first!");
                            Debug.LogError("      📍 Edit → Project Settings → Tags & Layers → Tags → Click '+' → Type 'Wall'");
                        }
                        break;
                    }
                }
            }
        }
        
        if (wallsTagged > 5)
        {
            Debug.Log($"      ... and {wallsTagged - 5} more");
        }
        
        if (wallsTagged > 0)
        {
            Debug.Log($"      ✅ Tagged {wallsTagged} walls total!");
        }
        else
        {
            Debug.LogWarning("      ⚠️ No walls found to tag. Possible reasons:");
            Debug.LogWarning("         1. Maze not generated yet (wait a few seconds)");
            Debug.LogWarning("         2. Walls don't have 'Wall' in their name");
            Debug.LogWarning("         3. Wall objects missing SpriteRenderer or BoxCollider2D");
        }
    }
    
    [ContextMenu("🏷️ Create Wall Tag")]
    public void CreateWallTag()
    {
        Debug.Log("📝 To create the 'Wall' tag:");
        Debug.Log("   1. Edit → Project Settings");
        Debug.Log("   2. Click 'Tags & Layers' on the left");
        Debug.Log("   3. Under 'Tags' section, click the '+' button");
        Debug.Log("   4. Type 'Wall' in the text field");
        Debug.Log("   5. Press Enter or click outside the field");
        Debug.Log("   6. Close Project Settings");
        Debug.Log("   7. Run this script again!");
    }
    
    [ContextMenu("🔍 Check Wall Tag Exists")]
    public void CheckWallTagExists()
    {
        try
        {
            // Try to create a test object with Wall tag
            GameObject test = new GameObject("TagTest");
            test.tag = "Wall";
            Destroy(test);
            Debug.Log("✅ 'Wall' tag EXISTS!");
        }
        catch (UnityException)
        {
            Debug.LogError("❌ 'Wall' tag DOES NOT EXIST!");
            Debug.LogError("   💡 Create it: Edit → Project Settings → Tags & Layers → Add 'Wall'");
        }
    }
}


