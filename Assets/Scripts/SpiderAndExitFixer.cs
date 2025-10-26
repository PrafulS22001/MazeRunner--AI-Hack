using UnityEngine;

/// <summary>
/// Fixes spider chase behavior and exit accessibility issues
/// Run this automatically on game start
/// </summary>
public class SpiderAndExitFixer : MonoBehaviour
{
    [Header("Auto-Fix Settings")]
    [Tooltip("Automatically fix spiders on start")]
    public bool autoFixSpiders = true;
    
    [Tooltip("Automatically fix exit on start")]
    public bool autoFixExit = true;
    
    [Tooltip("WARNING: Creates a direct path to exit (for testing only!)")]
    public bool forceCreatePath = false;  // Disabled by default!
    
    void Start()
    {
        if (autoFixSpiders)
        {
            Invoke("FixAllSpiders", 2f);   // Fix spiders after they spawn
        }
        
        if (autoFixExit)
        {
            Invoke("FixExitAccess", 3f);   // Fix exit after it's created
        }
        
        // Only create path if explicitly enabled (for testing)
        if (forceCreatePath)
        {
            Invoke("ForceCreateExitPath", 4f);  // FORCE a path to exit
            Debug.LogWarning("⚠️ ForceCreatePath is ENABLED - Exit will be easy to reach (testing mode)");
        }
    }
    
    void Update()
    {
        // Press F to force fix everything NOW
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("🔧 FORCE FIXING EVERYTHING!");
            ForceFixAll();
            ForceCreateExitPath();
        }
    }
    
    void FixAllSpiders()
    {
        Debug.Log("🕷️ Fixing all spiders...");
        
        // Find all spiders (try multiple ways)
        GameObject[] spiders = GameObject.FindGameObjectsWithTag("Spider");
        
        if (spiders.Length == 0)
        {
            // Try finding by component
            SpiderAI[] spiderAIs = FindObjectsByType<SpiderAI>(FindObjectsSortMode.None);
            Debug.Log($"   📍 Found {spiderAIs.Length} spiders by component");
            
            foreach (SpiderAI spider in spiderAIs)
            {
                FixSpider(spider.gameObject);
            }
        }
        else
        {
            Debug.Log($"   📍 Found {spiders.Length} spiders by tag");
            foreach (GameObject spider in spiders)
            {
                FixSpider(spider);
            }
        }
    }
    
    void FixSpider(GameObject spider)
    {
        if (spider == null) return;
        
        Debug.Log($"   🔧 Fixing spider: {spider.name}");
        
        // 1. Ensure Rigidbody2D exists and is configured correctly
        Rigidbody2D rb = spider.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = spider.AddComponent<Rigidbody2D>();
            Debug.Log("      ✅ Added Rigidbody2D");
        }
        
        // Configure Rigidbody2D for proper movement
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0f;  // No gravity in 2D top-down
        rb.linearDamping = 0f;  // No drag
        rb.angularDamping = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;  // Don't rotate
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        
        // 2. Ensure CircleCollider2D exists
        CircleCollider2D col = spider.GetComponent<CircleCollider2D>();
        if (col == null)
        {
            col = spider.AddComponent<CircleCollider2D>();
            Debug.Log("      ✅ Added CircleCollider2D");
        }
        
        col.isTrigger = false;  // NOT a trigger (for collision with player)
        col.radius = 0.4f;  // Smaller than cell size
        
        // 3. Ensure SpiderAI component exists and is enabled
        SpiderAI ai = spider.GetComponent<SpiderAI>();
        if (ai == null)
        {
            ai = spider.AddComponent<SpiderAI>();
            Debug.Log("      ✅ Added SpiderAI");
        }
        else
        {
            ai.enabled = true;
        }
        
        // 4. Ensure spider has proper layer (for collision)
        spider.layer = 0;  // Default layer
        
        Debug.Log($"      ✅ Spider fixed! Can now chase player");
    }
    
    void FixExitAccess()
    {
        Debug.Log("🚪 Fixing exit accessibility...");
        
        // Find exit
        ExitTrigger[] exits = FindObjectsByType<ExitTrigger>(FindObjectsSortMode.None);
        
        if (exits.Length == 0)
        {
            // Try finding by name
            GameObject exitObj = GameObject.Find("EXIT_MARKER");
            if (exitObj != null)
            {
                Debug.Log("   📍 Found exit by name");
                FixExit(exitObj);
            }
            else
            {
                Debug.LogWarning("   ⚠️ No exit found!");
            }
        }
        else
        {
            Debug.Log($"   📍 Found {exits.Length} exit(s)");
            foreach (ExitTrigger exit in exits)
            {
                FixExit(exit.gameObject);
            }
        }
    }
    
    void FixExit(GameObject exit)
    {
        if (exit == null) return;
        
        Debug.Log($"   🔧 Fixing exit: {exit.name} at {exit.transform.position}");
        
        // 1. Ensure BoxCollider2D exists and is a TRIGGER
        BoxCollider2D boxCol = exit.GetComponent<BoxCollider2D>();
        if (boxCol == null)
        {
            boxCol = exit.AddComponent<BoxCollider2D>();
            Debug.Log("      ✅ Added BoxCollider2D");
        }
        
        boxCol.isTrigger = true;  // MUST be trigger!
        boxCol.size = Vector2.one * 1.5f;  // Larger trigger area
        
        // 2. Remove any CircleCollider2D (might conflict)
        CircleCollider2D circleCol = exit.GetComponent<CircleCollider2D>();
        if (circleCol != null && !circleCol.isTrigger)
        {
            Destroy(circleCol);
            Debug.Log("      🗑️ Removed non-trigger CircleCollider2D");
        }
        
        // 3. Ensure ExitTrigger component exists
        ExitTrigger exitTrigger = exit.GetComponent<ExitTrigger>();
        if (exitTrigger == null)
        {
            exitTrigger = exit.AddComponent<ExitTrigger>();
            Debug.Log("      ✅ Added ExitTrigger");
        }
        
        // 4. Clear any walls near exit (2 unit radius)
        Vector3 exitPos = exit.transform.position;
        Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(exitPos, 2f);
        
        int wallsRemoved = 0;
        foreach (Collider2D col in nearbyColliders)
        {
            if (col.gameObject != exit && col.gameObject.CompareTag("Wall"))
            {
                Debug.Log($"      🗑️ Removing wall blocking exit at {col.transform.position}");
                Destroy(col.gameObject);
                wallsRemoved++;
            }
        }
        
        if (wallsRemoved > 0)
        {
            Debug.Log($"      ✅ Cleared {wallsRemoved} walls near exit");
        }
        
        // 5. Make exit more visible
        SpriteRenderer sr = exit.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = Color.green;  // Bright green
            sr.sortingOrder = 50;  // Above walls
            exit.transform.localScale = Vector3.one * 1.5f;  // Bigger!
        }
        
        Debug.Log($"      ✅ Exit fixed! Position: {exitPos}");
    }
    
    void ForceCreateExitPath()
    {
        Debug.Log("🛣️ FORCING EXIT PATH CREATION...");
        
        // Find player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("   ⚠️ Player not found!");
            return;
        }
        
        // Find exit
        ExitTrigger exit = FindFirstObjectByType<ExitTrigger>();
        if (exit == null)
        {
            Debug.LogWarning("   ⚠️ Exit not found!");
            return;
        }
        
        Vector3 playerPos = player.transform.position;
        Vector3 exitPos = exit.transform.position;
        
        Debug.Log($"   📍 Player at: {playerPos}");
        Debug.Log($"   📍 Exit at: {exitPos}");
        
        // Create WIDE path by removing ALL walls between player and exit
        Vector3 direction = (exitPos - playerPos).normalized;
        float distance = Vector3.Distance(playerPos, exitPos);
        
        int wallsRemoved = 0;
        
        // Create path 3 units wide!
        for (float d = 0; d < distance + 3f; d += 0.5f)
        {
            for (float offset = -1.5f; offset <= 1.5f; offset += 0.5f)
            {
                Vector3 perpendicular = new Vector3(-direction.y, direction.x, 0);
                Vector3 checkPos = playerPos + direction * d + perpendicular * offset;
                
                // Remove walls at this position
                Collider2D[] colliders = Physics2D.OverlapCircleAll(checkPos, 0.6f);
                foreach (Collider2D col in colliders)
                {
                    if (col.gameObject.CompareTag("Wall"))
                    {
                        Destroy(col.gameObject);
                        wallsRemoved++;
                    }
                }
            }
        }
        
        Debug.Log($"   ✅ Blasted through {wallsRemoved} walls!");
        Debug.Log($"   🛣️ Created wide path from player to exit!");
        
        // Make exit SUPER visible
        SpriteRenderer exitSR = exit.GetComponent<SpriteRenderer>();
        if (exitSR != null)
        {
            exitSR.color = new Color(0, 1, 0, 1);  // Bright green, fully opaque
            exitSR.sortingOrder = 200;  // Above EVERYTHING
            exit.transform.localScale = Vector3.one * 3f;  // HUGE!
            
            Debug.Log("   ✅ Made exit HUGE and bright green!");
        }
    }
    
    [ContextMenu("Force Fix Now")]
    public void ForceFixAll()
    {
        FixAllSpiders();
        FixExitAccess();
        ForceCreateExitPath();
        
        // Also check player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
            if (playerRb == null)
            {
                Debug.LogError("❌ Player has no Rigidbody2D! Exit trigger won't work!");
            }
            else
            {
                Debug.Log("✅ Player has Rigidbody2D - exit trigger will work");
            }
        }
        else
        {
            Debug.LogWarning("⚠️ Player not found!");
        }
    }
}



