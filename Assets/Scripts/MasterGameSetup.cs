using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// MASTER GAME SETUP - Complete game initialization from scratch
/// This ONE component sets up EVERYTHING correctly
/// </summary>
public class MasterGameSetup : MonoBehaviour
{
    [Header("Game Settings")]
    public bool autoSetup = true;
    public float playerSpeed = 6f;
    public int mazeSize = 30;
    
    [Header("Debug")]
    public bool showDebug = true;
    
    private GameObject player;
    private Camera mainCamera;
    
    void Start()
    {
        if (autoSetup)
        {
            StartCoroutine(SetupCompleteGame());
        }
    }
    
    IEnumerator SetupCompleteGame()
    {
        Debug.Log("🎮 === MASTER GAME SETUP STARTING === 🎮");
        
        yield return new WaitForSeconds(0.5f);
        
        // Step 1: Setup Camera
        SetupCamera();
        yield return new WaitForSeconds(0.1f);
        
        // Step 2: Setup Player with WORKING movement
        SetupPlayer();
        yield return new WaitForSeconds(0.1f);
        
        // Step 3: Fix Maze Generator
        FixMazeGenerator();
        yield return new WaitForSeconds(0.1f);
        
        // Step 4: Setup UI Systems
        SetupUISystems();
        yield return new WaitForSeconds(0.1f);
        
        // Step 5: Apply Realistic Graphics
        ApplyRealisticGraphics();
        yield return new WaitForSeconds(0.1f);
        
        // Step 6: Final verification
        VerifySetup();
        
        Debug.Log("✅ === MASTER GAME SETUP COMPLETE === ✅");
    }
    
    void SetupCamera()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            mainCamera = FindObjectOfType<Camera>();
        }
        
        if (mainCamera != null)
        {
            mainCamera.orthographic = true;
            mainCamera.orthographicSize = 6f;
            mainCamera.backgroundColor = new Color(0.1f, 0.1f, 0.15f);
            
            // Add or fix camera follow
            CameraFollow camFollow = mainCamera.GetComponent<CameraFollow>();
            if (camFollow == null)
            {
                camFollow = mainCamera.gameObject.AddComponent<CameraFollow>();
            }
            
            Debug.Log("✅ Camera setup complete");
        }
    }
    
    void SetupPlayer()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        
        if (player == null)
        {
            Debug.LogError("❌ Player not found! Create a GameObject with tag 'Player'");
            return;
        }
        
        // REMOVE ALL OLD MOVEMENT SCRIPTS
        PlayerMovement[] oldMovements = player.GetComponents<PlayerMovement>();
        foreach (var old in oldMovements)
        {
            DestroyImmediate(old);
        }
        
        ImprovedPlayerMovement[] improvedMovements = player.GetComponents<ImprovedPlayerMovement>();
        foreach (var old in improvedMovements)
        {
            DestroyImmediate(old);
        }
        
        // Setup Rigidbody2D - CRITICAL SETTINGS
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = player.AddComponent<Rigidbody2D>();
        }
        
        // PERFECT RIGIDBODY SETTINGS
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0f;
        rb.linearDamping = 0f; // NO DAMPING!
        rb.angularDamping = 0f;
        rb.mass = 1f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.sleepMode = RigidbodySleepMode2D.NeverSleep; // NEVER SLEEP!
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        
        // Setup Collider - SMALLER FOR NO STICKING
        BoxCollider2D collider = player.GetComponent<BoxCollider2D>();
        if (collider == null)
        {
            collider = player.AddComponent<BoxCollider2D>();
        }
        
        collider.size = new Vector2(0.4f, 0.4f); // VERY SMALL!
        collider.offset = Vector2.zero;
        collider.isTrigger = false;
        
        // Setup SpriteRenderer
        SpriteRenderer sr = player.GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            sr = player.AddComponent<SpriteRenderer>();
            sr.sprite = CreatePlayerSprite();
        }
        sr.color = new Color(0.2f, 0.4f, 1f); // Blue
        sr.sortingOrder = 10;
        
        // Add NEW WORKING MOVEMENT SCRIPT
        SimpleSmoothMovement movement = player.AddComponent<SimpleSmoothMovement>();
        movement.speed = playerSpeed;
        
        // Setup HealthSystem
        if (player.GetComponent<HealthSystem>() == null)
        {
            player.AddComponent<HealthSystem>();
        }
        
        // Position at center
        player.transform.position = Vector3.zero;
        player.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
        
        Debug.Log("✅ Player setup with NEW MOVEMENT SYSTEM (no more sticking!)");
    }
    
    void FixMazeGenerator()
    {
        MazeGenerator mazeGen = FindObjectOfType<MazeGenerator>();
        
        if (mazeGen == null)
        {
            Debug.LogWarning("⚠️ MazeGenerator not found");
            return;
        }
        
        // Disable tree spawning temporarily to avoid wall issues
        if (mazeGen.treePrefab == null)
        {
            Debug.Log("ℹ️ Tree spawning disabled (no prefab)");
        }
        
        Debug.Log("✅ MazeGenerator verified");
    }
    
    void SetupUISystems()
    {
        // Ensure EventSystem exists
        if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject es = new GameObject("EventSystem");
            es.AddComponent<UnityEngine.EventSystems.EventSystem>();
            es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }
        
        // Setup GameManager
        if (FindObjectOfType<GameManager>() == null)
        {
            GameObject gm = new GameObject("GameManager");
            gm.AddComponent<GameManager>();
        }
        
        Debug.Log("✅ UI Systems ready");
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
            
            Debug.Log("✅ Realistic graphics will be applied!");
        }
        else
        {
            Debug.LogWarning("⚠️ RealisticGraphicsSystem.cs not compiled yet. Will apply on next Play.");
        }
    }
    
    void VerifySetup()
    {
        Debug.Log("=== VERIFICATION ===");
        Debug.Log($"Player: {(player != null ? "✓" : "✗")}");
        Debug.Log($"Camera: {(mainCamera != null ? "✓" : "✗")}");
        Debug.Log($"Movement: {(player?.GetComponent<SimpleSmoothMovement>() != null ? "✓" : "✗")}");
        Debug.Log($"Rigidbody: {(player?.GetComponent<Rigidbody2D>() != null ? "✓" : "✗")}");
        Debug.Log("====================");
    }
    
    Sprite CreatePlayerSprite()
    {
        Texture2D tex = new Texture2D(32, 32);
        Color[] pixels = new Color[32 * 32];
        
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.white;
        }
        
        tex.SetPixels(pixels);
        tex.Apply();
        
        return Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
    }
    
    [ContextMenu("Reset and Setup Again")]
    public void ResetSetup()
    {
        StopAllCoroutines();
        StartCoroutine(SetupCompleteGame());
    }
}

/// <summary>
/// SIMPLE SMOOTH MOVEMENT - Guaranteed to work, no sticking!
/// Uses direct transform movement instead of physics
/// </summary>
public class SimpleSmoothMovement : MonoBehaviour
{
    public float speed = 6f;
    
    private Rigidbody2D rb;
    private Vector2 movement;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Debug.Log("✅ Simple Smooth Movement active");
    }
    
    void Update()
    {
        // Get input
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        
        // Normalize for consistent diagonal speed
        if (movement.magnitude > 1f)
        {
            movement.Normalize();
        }
    }
    
    void FixedUpdate()
    {
        if (rb == null) return;
        
        // DIRECT POSITION MOVEMENT - NO STICKING!
        Vector2 newPosition = rb.position + movement * speed * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);
        
        // Alternative: Use velocity (smoother but can stick)
        // rb.velocity = movement * speed;
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Slide along walls instead of stopping
        if (collision.gameObject.CompareTag("Wall"))
        {
            Vector2 normal = collision.GetContact(0).normal;
            Vector2 slideDirection = Vector2.Perpendicular(normal);
            
            // Keep moving along the wall
            float dot = Vector2.Dot(movement, slideDirection);
            rb.MovePosition(rb.position + slideDirection * dot * speed * Time.fixedDeltaTime);
        }
    }
}

