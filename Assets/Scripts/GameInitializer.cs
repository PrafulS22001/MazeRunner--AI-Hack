using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Master game initialization script - ensures everything loads in the correct order
/// This script should execute FIRST (Script Execution Order: -100)
/// </summary>
public class GameInitializer : MonoBehaviour
{
    [Header("Initialization Settings")]
    public bool startWithMenu = true;
    public bool skipMenu = false; // For testing
    
    [Header("System References")]
    private BeautifulMenuSystem menuSystem;
    private MazeGenerator mazeGenerator;
    private ClashStyleFogSystem fogSystem;
    private GameManager gameManager;
    private RealisticUISystem realisticUI;
    
    [Header("Status")]
    public bool isInitialized = false;
    public bool gameStarted = false;
    
    private static GameInitializer instance;
    
    void Awake()
    {
        // Singleton pattern
        if (instance == null)
        {
            instance = this;
            Debug.Log("🎮 GameInitializer starting...");
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        // Set initial game state
        if (startWithMenu && !skipMenu)
        {
            Time.timeScale = 0f; // Pause immediately
        }
    }
    
    void Start()
    {
        InitializeGame();
    }
    
    void InitializeGame()
    {
        Debug.Log("=== GAME INITIALIZATION START ===");
        
        // Phase 1: Find all systems
        FindAllSystems();
        
        // Phase 2: Initialize in correct order
        if (startWithMenu && !skipMenu)
        {
            InitializeMenuMode();
        }
        else
        {
            InitializeDirectPlayMode();
        }
        
        isInitialized = true;
        Debug.Log("=== GAME INITIALIZATION COMPLETE ===");
    }
    
    void FindAllSystems()
    {
        Debug.Log("📦 Finding game systems...");
        
        menuSystem = FindFirstObjectByType<BeautifulMenuSystem>();
        mazeGenerator = FindFirstObjectByType<MazeGenerator>();
        fogSystem = FindFirstObjectByType<ClashStyleFogSystem>();
        gameManager = FindFirstObjectByType<GameManager>();
        realisticUI = FindFirstObjectByType<RealisticUISystem>();
        
        Debug.Log($"Menu System: {(menuSystem != null ? "✅" : "❌")}");
        Debug.Log($"Maze Generator: {(mazeGenerator != null ? "✅" : "❌")}");
        Debug.Log($"Fog System: {(fogSystem != null ? "✅" : "❌")}");
        Debug.Log($"Game Manager: {(gameManager != null ? "✅" : "❌")}");
        Debug.Log($"Realistic UI: {(realisticUI != null ? "✅" : "❌")}");
    }
    
    void InitializeMenuMode()
    {
        Debug.Log("🎮 Initializing MENU MODE...");
        
        // 1. Hide gameplay UI (but don't disable it completely)
        // Note: We keep it active but behind the menu
        if (realisticUI != null)
        {
            // Don't fully hide it - just lower its canvas order
            Canvas uiCanvas = realisticUI.GetComponent<Canvas>();
            if (uiCanvas == null)
            {
                uiCanvas = realisticUI.GetComponentInChildren<Canvas>();
            }
            if (uiCanvas != null)
            {
                uiCanvas.sortingOrder = 50; // Below menu (which is 100+)
            }
            Debug.Log("   ✅ Gameplay UI ready (behind menu)");
        }
        
        // 2. Disable game systems temporarily
        if (mazeGenerator != null)
        {
            mazeGenerator.enabled = false;
            Debug.Log("   ✅ Maze generation paused");
        }
        
        if (fogSystem != null)
        {
            fogSystem.enabled = false;
            Debug.Log("   ✅ Fog system paused");
        }
        
        if (gameManager != null)
        {
            gameManager.enabled = false;
            Debug.Log("   ✅ Game manager paused");
        }
        
        // 3. Ensure menu system is active and shows start screen
        if (menuSystem == null)
        {
            Debug.LogError("❌ BeautifulMenuSystem not found! Creating one...");
            GameObject menuGO = new GameObject("BeautifulMenuSystem");
            menuSystem = menuGO.AddComponent<BeautifulMenuSystem>();
        }
        
        // Menu system will show start screen automatically in its Start()
        Debug.Log("   ✅ Menu system initialized");
        
        // 4. Freeze time
        Time.timeScale = 0f;
        
        Debug.Log("🎮 Menu mode ready! Press PLAY to start game.");
    }
    
    void InitializeDirectPlayMode()
    {
        Debug.Log("⚡ Initializing DIRECT PLAY MODE...");
        
        // Hide menu
        if (menuSystem != null)
        {
            menuSystem.gameObject.SetActive(false);
        }
        
        // Show gameplay UI
        if (realisticUI != null)
        {
            realisticUI.gameObject.SetActive(true);
        }
        
        // Enable all game systems
        EnableGameSystems();
        
        Time.timeScale = 1f;
        gameStarted = true;
        
        Debug.Log("⚡ Direct play mode ready!");
    }
    
    /// <summary>
    /// Called by BeautifulMenuSystem when player clicks START GAME
    /// </summary>
    public void OnGameStart()
    {
        if (gameStarted)
        {
            Debug.LogWarning("Game already started!");
            return;
        }
        
        Debug.Log("🎮 STARTING GAME...");
        
        // 1. Re-find RealisticUISystem in case it was created after initialization
        if (realisticUI == null)
        {
            Debug.Log("   🔍 RealisticUISystem was null, searching again...");
            
            // Search including inactive objects
            realisticUI = FindFirstObjectByType<RealisticUISystem>(FindObjectsInactive.Include);
            
            if (realisticUI != null)
            {
                Debug.Log($"   ✅ Found RealisticUISystem on '{realisticUI.gameObject.name}'");
            }
            else
            {
                Debug.LogWarning("   ⚠️ Still not found! Checking SystemsAutoEnabler...");
                
                // Last resort: Let SystemsAutoEnabler create it
                SystemsAutoEnabler autoEnabler = FindFirstObjectByType<SystemsAutoEnabler>();
                if (autoEnabler != null)
                {
                    autoEnabler.CheckAndEnableSystems();
                    realisticUI = FindFirstObjectByType<RealisticUISystem>(FindObjectsInactive.Include);
                }
            }
        }
        
        // 2. Show gameplay UI properly
        if (realisticUI != null)
        {
            realisticUI.gameObject.SetActive(true);
            realisticUI.enabled = true;
            
            // Ensure canvas is on top
            Canvas uiCanvas = realisticUI.GetComponent<Canvas>();
            if (uiCanvas == null)
            {
                uiCanvas = realisticUI.GetComponentInChildren<Canvas>();
            }
            if (uiCanvas != null)
            {
                uiCanvas.sortingOrder = 200; // Above most things
            }
            
            Debug.Log("   ✅ Gameplay UI shown and enabled");
        }
        else
        {
            Debug.LogWarning("   ⚠️ RealisticUISystem not found!");
            Debug.LogWarning("   💡 SystemsAutoEnabler should create it automatically");
        }
        
        // 2. Enable game systems
        EnableGameSystems();
        
        // 3. Unpause time
        Time.timeScale = 1f;
        gameStarted = true;
        
        Debug.Log("🎮 GAME STARTED!");
    }
    
    void EnableGameSystems()
    {
        Debug.Log("🔧 Enabling game systems...");
        
        if (mazeGenerator != null)
        {
            mazeGenerator.enabled = true;
            Debug.Log("   ✅ Maze generator enabled");
        }
        
        if (fogSystem != null)
        {
            fogSystem.enabled = true;
            Debug.Log("   ✅ Fog system enabled");
        }
        
        if (gameManager != null)
        {
            gameManager.enabled = true;
            gameManager.OnGameStarted(); // Notify GameManager game has started
            Debug.Log("   ✅ Game manager enabled and notified");
        }
        
        // Find and enable player movement
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerMovement pm = player.GetComponent<PlayerMovement>();
            if (pm != null)
            {
                pm.enabled = true;
                Debug.Log("   ✅ Player movement enabled");
            }
        }
    }
    
    public void OnGamePause()
    {
        Time.timeScale = 0f;
        Debug.Log("⏸ Game paused");
    }
    
    public void OnGameResume()
    {
        if (gameStarted)
        {
            Time.timeScale = 1f;
            Debug.Log("▶ Game resumed");
        }
    }
    
    public void RestartGame()
    {
        Debug.Log("🔄 Restarting game...");
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public static GameInitializer Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<GameInitializer>();
            }
            return instance;
        }
    }
    
    public bool IsGameStarted()
    {
        return gameStarted;
    }
    
    public bool HasMenu()
    {
        return startWithMenu && !skipMenu;
    }
}



