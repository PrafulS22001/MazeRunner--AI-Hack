using UnityEngine;
using UnityEngine.SceneManagement;

public class GameIntegration : MonoBehaviour
{
    [Header("Integration Settings")]
    public bool autoInitialize = true;
    public bool enableDebugMode = false;
    public bool enablePerformanceOptimization = true;
    
    [Header("Component References")]
    public HealthSystem healthSystem;
    public GameManager gameManager;
    public MazeGenerator mazeGenerator;
    public AudioManager audioManager;
    public RealisticUISystem realisticUI; // Replaced UIManager
    public LevelManager levelManager;
    public PlayerProgress playerProgress;
    public PerformanceManager performanceManager;
    public ScreenEffectsManager screenEffectsManager;
    public ParticleEffectManager particleEffectManager;
    
    private bool isInitialized = false;
    
    void Start()
    {
        if (autoInitialize)
        {
            InitializeGame();
        }
    }
    
    public void InitializeGame()
    {
        if (isInitialized) return;
        
        Debug.Log("Initializing game integration...");
        
        // Initialize core systems
        InitializeCoreSystems();
        
        // Initialize managers
        InitializeManagers();
        
        // Setup event connections
        SetupEventConnections();
        
        // Apply optimizations
        if (enablePerformanceOptimization)
        {
            ApplyOptimizations();
        }
        
        // Setup debug mode
        if (enableDebugMode)
        {
            SetupDebugMode();
        }
        
        isInitialized = true;
        Debug.Log("Game integration completed successfully!");
    }
    
    void InitializeCoreSystems()
    {
        // Find or create core systems
        if (healthSystem == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                healthSystem = player.GetComponent<HealthSystem>();
                if (healthSystem == null)
                {
                    healthSystem = player.AddComponent<HealthSystem>();
                }
                
                // Add player animator
                if (player.GetComponent<PlayerAnimator>() == null)
                {
                    player.AddComponent<PlayerAnimator>();
                }
            }
        }
        
        if (gameManager == null)
        {
            GameObject gameManagerGO = GameObject.Find("GameManager");
            if (gameManagerGO == null)
            {
                gameManagerGO = new GameObject("GameManager");
            }
            
            gameManager = gameManagerGO.GetComponent<GameManager>();
            if (gameManager == null)
            {
                gameManager = gameManagerGO.AddComponent<GameManager>();
            }
        }
        
        if (mazeGenerator == null)
        {
            mazeGenerator = FindObjectOfType<MazeGenerator>();
        }
    }
    
    void InitializeManagers()
    {
        // Initialize Audio Manager
        if (audioManager == null)
        {
            audioManager = AudioManager.Instance;
        }
        
        // Initialize Realistic UI System
        if (realisticUI == null)
        {
            realisticUI = FindObjectOfType<RealisticUISystem>();
            if (realisticUI == null)
            {
                Debug.LogWarning("⚠️ RealisticUISystem not found. It should be created by setup scripts.");
            }
        }
        
        // Initialize Level Manager
        if (levelManager == null)
        {
            GameObject levelManagerGO = GameObject.Find("LevelManager");
            if (levelManagerGO == null)
            {
                levelManagerGO = new GameObject("LevelManager");
            }
            
            levelManager = levelManagerGO.GetComponent<LevelManager>();
            if (levelManager == null)
            {
                levelManager = levelManagerGO.AddComponent<LevelManager>();
            }
        }
        
        // Initialize Player Progress
        if (playerProgress == null)
        {
            playerProgress = PlayerProgress.Instance;
        }
        
        // Initialize Performance Manager
        if (performanceManager == null)
        {
            performanceManager = PerformanceManager.Instance;
        }
        
        // Initialize Screen Effects Manager
        if (screenEffectsManager == null)
        {
            screenEffectsManager = ScreenEffectsManager.Instance;
        }
        
        // Initialize Particle Effect Manager
        if (particleEffectManager == null)
        {
            particleEffectManager = ParticleEffectManager.Instance;
        }
    }
    
    void SetupEventConnections()
    {
        // Connect health system events
        if (healthSystem != null)
        {
            // Health system events are handled internally
        }
        
        // Connect game manager events
        if (gameManager != null)
        {
            gameManager.OnPlayerDeath += HandlePlayerDeath;
            gameManager.OnPlayerReachedExit += HandlePlayerReachedExit;
        }
        
        // Connect UI events
        if (realisticUI != null)
        {
            // UI events are handled internally by RealisticUISystem
        }
        
        // Connect level manager events
        if (levelManager != null)
        {
            // Level manager events are handled internally
        }
        
        Debug.Log("Event connections established");
    }
    
    void ApplyOptimizations()
    {
        if (performanceManager != null)
        {
            performanceManager.OptimizeMazeGeneration();
            performanceManager.OptimizeSpiderAI();
        }
        
        Debug.Log("Performance optimizations applied");
    }
    
    void SetupDebugMode()
    {
        if (performanceManager != null)
        {
            performanceManager.showPerformanceStats = true;
        }
        
        Debug.Log("Debug mode enabled");
    }
    
    // Event handlers
    void HandlePlayerDeath()
    {
        Debug.Log("Player death event received");
        
        // Play death effects
        if (audioManager != null)
        {
            audioManager.PlayGameOverMusic();
        }
        
        if (screenEffectsManager != null)
        {
            screenEffectsManager.ScreenFlash(Color.red, 1f);
        }
        
        if (particleEffectManager != null)
        {
            if (healthSystem != null)
            {
                particleEffectManager.PlayExplosionEffect(healthSystem.transform.position);
            }
        }
        
        // Record death in progress
        if (playerProgress != null)
        {
            playerProgress.RecordDeath();
        }
        
        // Show game over UI
        if (realisticUI != null)
        {
            // RealisticUISystem handles this automatically via health system
            Debug.Log("🎮 Player died - UI system notified");
        }
    }
    
    void HandlePlayerReachedExit()
    {
        Debug.Log("Player reached exit event received");
        
        // Play victory effects
        if (audioManager != null)
        {
            audioManager.PlayVictoryMusic();
        }
        
        if (screenEffectsManager != null)
        {
            screenEffectsManager.ScreenFlash(Color.green, 1f);
        }
        
        if (particleEffectManager != null)
        {
            if (healthSystem != null)
            {
                particleEffectManager.PlayExitGlowEffect(healthSystem.transform.position);
            }
        }
        
        // Complete level
        if (levelManager != null)
        {
            levelManager.CompleteCurrentLevel();
        }
        
        // Show victory UI
        if (realisticUI != null)
        {
            // RealisticUISystem handles this automatically
            Debug.Log("🎮 Player reached exit - UI system notified");
        }
    }
    
    // Public methods for external access
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    
    public void PauseGame()
    {
        Time.timeScale = Time.timeScale == 0 ? 1 : 0;
        Debug.Log($"🎮 Game {(Time.timeScale == 0 ? "paused" : "resumed")}");
        
        // BeautifulMenuSystem handles pause UI
    }
    
    public void ToggleDebugMode()
    {
        enableDebugMode = !enableDebugMode;
        
        if (performanceManager != null)
        {
            performanceManager.TogglePerformanceStats();
        }
        
        Debug.Log($"Debug mode: {(enableDebugMode ? "ON" : "OFF")}");
    }
    
    public void CleanupAndRestart()
    {
        if (performanceManager != null)
        {
            performanceManager.CleanupUnusedObjects();
        }
        
        RestartGame();
    }
    
    // Getters for other scripts
    public HealthSystem GetHealthSystem() => healthSystem;
    public GameManager GetGameManager() => gameManager;
    public MazeGenerator GetMazeGenerator() => mazeGenerator;
    public AudioManager GetAudioManager() => audioManager;
    public RealisticUISystem GetRealisticUI() => realisticUI; // Replaced GetUIManager
    public LevelManager GetLevelManager() => levelManager;
    public PlayerProgress GetPlayerProgress() => playerProgress;
    public PerformanceManager GetPerformanceManager() => performanceManager;
    public ScreenEffectsManager GetScreenEffectsManager() => screenEffectsManager;
    public ParticleEffectManager GetParticleEffectManager() => particleEffectManager;
    
    void OnDestroy()
    {
        // Cleanup event connections
        if (gameManager != null)
        {
            gameManager.OnPlayerDeath -= HandlePlayerDeath;
            gameManager.OnPlayerReachedExit -= HandlePlayerReachedExit;
        }
    }
}
