using UnityEngine;

/// <summary>
/// INSTANT GAME STARTER - Bypasses menu and starts everything immediately
/// This ensures ALL fixes work right away without needing to press START button
/// DISABLE THIS if you want the menu to show!
/// </summary>
[DefaultExecutionOrder(-200)] // Run BEFORE GameInitializer
public class InstantGameStarter : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Force game to start immediately (skip menu) - UNCHECK to show menu!")]
    public bool forceInstantStart = false;  // Changed to FALSE by default!
    
    [Tooltip("Show debug messages")]
    public bool showDebug = true;
    
    void Awake()
    {
        if (!forceInstantStart) return;
        
        if (showDebug)
        {
            Debug.Log("⚡⚡⚡ INSTANT GAME STARTER - ACTIVATED! ⚡⚡⚡");
        }
        
        // Find GameInitializer and force it to skip menu
        GameInitializer gameInit = FindFirstObjectByType<GameInitializer>();
        
        if (gameInit != null)
        {
            // Use reflection to force skipMenu = true
            var skipMenuField = gameInit.GetType().GetField("skipMenu");
            if (skipMenuField != null)
            {
                skipMenuField.SetValue(gameInit, true);
                if (showDebug)
                {
                    Debug.Log("✅ Forced GameInitializer.skipMenu = true");
                }
            }
            
            var startWithMenuField = gameInit.GetType().GetField("startWithMenu");
            if (startWithMenuField != null)
            {
                startWithMenuField.SetValue(gameInit, false);
                if (showDebug)
                {
                    Debug.Log("✅ Forced GameInitializer.startWithMenu = false");
                }
            }
        }
        else
        {
            if (showDebug)
            {
                Debug.Log("⚠️ GameInitializer not found - game will start normally");
            }
        }
        
        // Ensure time is running
        Time.timeScale = 1f;
        
        if (showDebug)
        {
            Debug.Log("⚡ INSTANT START COMPLETE - Game will run immediately!");
        }
    }
    
    void Start()
    {
        // Double-check after a frame
        if (forceInstantStart)
        {
            Time.timeScale = 1f;
            
            // Force-enable MazeGenerator if it exists
            MazeGenerator mazeGen = FindFirstObjectByType<MazeGenerator>();
            if (mazeGen != null && !mazeGen.enabled)
            {
                mazeGen.enabled = true;
                if (showDebug)
                {
                    Debug.Log("✅ Force-enabled MazeGenerator");
                }
            }
            
            // Force game started status
            GameInitializer gameInit = FindFirstObjectByType<GameInitializer>();
            if (gameInit != null)
            {
                var gameStartedField = gameInit.GetType().GetField("gameStarted", 
                    System.Reflection.BindingFlags.NonPublic | 
                    System.Reflection.BindingFlags.Instance | 
                    System.Reflection.BindingFlags.Public);
                
                if (gameStartedField != null)
                {
                    gameStartedField.SetValue(gameInit, true);
                    if (showDebug)
                    {
                        Debug.Log("✅ Set gameStarted = true");
                    }
                }
            }
        }
    }
}

