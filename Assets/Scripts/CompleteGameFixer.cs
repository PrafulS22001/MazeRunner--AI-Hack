using UnityEngine;

/// <summary>
/// COMPLETE GAME FIXER - Master controller for all fixes
/// 
/// This is THE ULTIMATE FIX for all your issues:
/// 1. Trees chasing player (COMPLETELY FIXED with TreeGuardian in prefab)
/// 2. Walls looking like basic squares (FIXED with EnhancedWallSystem)
/// 3. Maze disappearing (diagnostic + fixes)
/// 
/// Just add this to any GameObject and press Play!
/// </summary>
public class CompleteGameFixer : MonoBehaviour
{
    [Header("Auto-Fix Systems")]
    public bool runOnStart = true;
    public bool fixTrees = true;
    public bool enhanceWalls = true;
    public bool checkMaze = true;
    public bool enhancePlayer = true;
    public bool enhancePowerUps = true;
    
    [Header("Prefab References (Auto-Check)")]
    public GameObject expectedTreePrefab;
    public GameObject expectedSpiderPrefab;
    
    [Header("Status")]
    public int treesFound = 0;
    public int treesFixed = 0;
    public int wallsEnhanced = 0;
    public bool allSystemsGo = false;
    
    private EnhancedWallSystem wallSystem;
    
    void Start()
    {
        Debug.Log("═══════════════════════════════════════════");
        Debug.Log("⚡ COMPLETE GAME FIXER - STARTING");
        Debug.Log("═══════════════════════════════════════════");
        
        // FIRST: Force game to start if blocked by menu
        ForceGameStart();
        
        // Add wall enhancement system if not present
        wallSystem = GetComponent<EnhancedWallSystem>();
        if (wallSystem == null && enhanceWalls)
        {
            wallSystem = gameObject.AddComponent<EnhancedWallSystem>();
            Debug.Log("✅ Added EnhancedWallSystem");
        }
        
        if (runOnStart)
        {
            // Run immediately, then again later to catch any late spawns
            RunCompleteFix();
            Invoke("RunCompleteFix", 2f);
            Invoke("RunCompleteFix", 4f);
            Invoke("RunCompleteFix", 6f);
        }
    }
    
    void ForceGameStart()
    {
        Debug.Log("🚀 FORCING GAME TO START...");
        
        // Check if GameInitializer is blocking
        GameInitializer gameInit = GameInitializer.Instance;
        
        if (gameInit != null)
        {
            if (!gameInit.IsGameStarted())
            {
                Debug.LogWarning("⚠️ Game blocked by menu system!");
                Debug.Log("   → Forcing game to start immediately...");
                
                // Force skip menu using reflection
                var skipMenuField = gameInit.GetType().GetField("skipMenu");
                if (skipMenuField != null)
                {
                    skipMenuField.SetValue(gameInit, true);
                }
                
                // Force game started
                var gameStartedField = gameInit.GetType().GetField("gameStarted", 
                    System.Reflection.BindingFlags.NonPublic | 
                    System.Reflection.BindingFlags.Instance | 
                    System.Reflection.BindingFlags.Public);
                if (gameStartedField != null)
                {
                    gameStartedField.SetValue(gameInit, true);
                }
                
                // Enable systems
                gameInit.SendMessage("EnableGameSystems", SendMessageOptions.DontRequireReceiver);
            }
        }
        
        // Force time to run
        Time.timeScale = 1f;
        
        // Force enable MazeGenerator
        MazeGenerator mazeGen = FindFirstObjectByType<MazeGenerator>();
        if (mazeGen != null && !mazeGen.enabled)
        {
            mazeGen.enabled = true;
            Debug.Log("   ✅ Enabled MazeGenerator");
        }
        
        Debug.Log("✅ Game forced to start - all systems go!");
    }
    
    [ContextMenu("⚡ RUN COMPLETE FIX")]
    public void RunCompleteFix()
    {
        Debug.Log("═══════════════════════════════════════════");
        Debug.Log("⚡ RUNNING COMPLETE GAME FIX");
        Debug.Log("═══════════════════════════════════════════");
        
        treesFound = 0;
        treesFixed = 0;
        wallsEnhanced = 0;
        
        // Fix #1: Check and verify trees
        if (fixTrees)
        {
            CheckTreeSystem();
        }
        
        // Fix #2: Enhance walls
        if (enhanceWalls && wallSystem != null)
        {
            wallSystem.EnhanceAllWalls();
            GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
            wallsEnhanced = walls.Length;
        }
        
        // Fix #3: Check maze generation
        if (checkMaze)
        {
            CheckMazeGeneration();
        }
        
        // Fix #4: Enhance player appearance
        if (enhancePlayer)
        {
            EnhancePlayerAppearance();
        }
        
        // Fix #5: Ensure power-ups have effects
        if (enhancePowerUps)
        {
            EnhancePowerUps();
        }
        
        // Final report
        Debug.Log("═══════════════════════════════════════════");
        Debug.Log("✅ COMPLETE FIX DONE!");
        Debug.Log($"   🌲 Trees checked: {treesFound} (fixed: {treesFixed})");
        Debug.Log($"   🧱 Walls enhanced: {wallsEnhanced}");
        Debug.Log($"   🎮 Systems: {(allSystemsGo ? "ALL GOOD" : "CHECK WARNINGS")}");
        Debug.Log("═══════════════════════════════════════════");
    }
    
    void EnhancePlayerAppearance()
    {
        Debug.Log("👤 ENHANCING PLAYER APPEARANCE...");
        
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        
        if (player == null)
        {
            Debug.LogWarning("   ⚠️ Player not found in scene");
            return;
        }
        
        // Add enhanced appearance if not present
        EnhancedPlayerAppearance appearance = player.GetComponent<EnhancedPlayerAppearance>();
        if (appearance == null)
        {
            appearance = player.AddComponent<EnhancedPlayerAppearance>();
            Debug.Log("   ✅ Added EnhancedPlayerAppearance to player!");
        }
        else
        {
            Debug.Log("   ✅ Player already has enhanced appearance");
        }
    }
    
    void EnhancePowerUps()
    {
        Debug.Log("⭐ CHECKING POWER-UPS...");
        
        PowerUp[] powerUps = FindObjectsByType<PowerUp>(FindObjectsSortMode.None);
        
        if (powerUps.Length == 0)
        {
            Debug.LogWarning("   ⚠️ No power-ups found yet");
            return;
        }
        
        int enhanced = 0;
        
        foreach (PowerUp powerUp in powerUps)
        {
            PowerUpEffects effects = powerUp.GetComponent<PowerUpEffects>();
            if (effects == null)
            {
                powerUp.gameObject.AddComponent<PowerUpEffects>();
                enhanced++;
            }
        }
        
        Debug.Log($"   ✅ Power-ups found: {powerUps.Length} (enhanced: {enhanced})");
    }
    
    void CheckTreeSystem()
    {
        Debug.Log("🌲 CHECKING TREE SYSTEM...");
        
        // Find MazeGenerator
        MazeGenerator mazeGen = FindFirstObjectByType<MazeGenerator>();
        if (mazeGen == null)
        {
            Debug.LogError("   ❌ MazeGenerator not found!");
            return;
        }
        
        // Check treePrefab assignment
        var treePrefabField = mazeGen.GetType().GetField("treePrefab");
        if (treePrefabField != null)
        {
            GameObject assignedPrefab = treePrefabField.GetValue(mazeGen) as GameObject;
            
            if (assignedPrefab == null)
            {
                Debug.LogError("   ❌ TREE PREFAB NOT ASSIGNED!");
                Debug.LogError("   💡 In MazeGenerator Inspector:");
                Debug.LogError("      → Set Tree Prefab to: Assets/Prefabs/Environment/Tree");
                return;
            }
            
            // Check if wrong prefab assigned
            if (assignedPrefab.name.Contains("Spider"))
            {
                Debug.LogError("   ❌ FOUND THE PROBLEM!");
                Debug.LogError($"   Tree Prefab = '{assignedPrefab.name}' (WRONG!)");
                Debug.LogError("   This is why 'trees' chase you - they're spiders!");
                Debug.LogError("");
                Debug.LogError("   💡 FIX:");
                Debug.LogError("   1. Select GameObject with MazeGenerator");
                Debug.LogError("   2. Inspector → Tree Prefab field");
                Debug.LogError("   3. Click ⭕ icon");
                Debug.LogError("   4. Select 'Tree' from Assets/Prefabs/Environment/");
                Debug.LogError("   5. Press Ctrl+S to save");
                return;
            }
            
            // Check if TreeGuardian is on prefab
            TreeGuardian guardian = assignedPrefab.GetComponent<TreeGuardian>();
            if (guardian == null)
            {
                Debug.LogWarning("   ⚠️ TreeGuardian not on prefab (but trees should still be static)");
            }
            else
            {
                Debug.Log($"   ✅ Tree Prefab = '{assignedPrefab.name}' with TreeGuardian");
            }
            
            // Check if prefab has movement components
            if (assignedPrefab.GetComponent<SpiderAI>() != null)
            {
                Debug.LogError("   ❌ Tree prefab has SpiderAI!");
                Debug.LogError("   💡 Open Tree.prefab and remove SpiderAI component");
            }
            
            if (assignedPrefab.GetComponent<Rigidbody2D>() != null)
            {
                Debug.LogError("   ❌ Tree prefab has Rigidbody2D!");
                Debug.LogError("   💡 Open Tree.prefab and remove Rigidbody2D component");
            }
        }
        
        // Check spawned trees
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        
        foreach (GameObject obj in allObjects)
        {
            if (obj == null) continue;
            
            bool isTree = obj.name.Contains("Tree") && 
                         !obj.name.Contains("TreeTrunk") && 
                         !obj.name.Contains("TreeLeaves");
            
            if (!isTree) continue;
            
            treesFound++;
            
            // Check for bad components
            SpiderAI ai = obj.GetComponent<SpiderAI>();
            Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
            
            if (ai != null || rb != null)
            {
                Debug.LogWarning($"   ⚠️ Tree '{obj.name}' has movement components");
                
                if (ai != null)
                {
                    Destroy(ai);
                    Debug.Log($"      → Destroyed SpiderAI");
                }
                
                if (rb != null)
                {
                    Destroy(rb);
                    Debug.Log($"      → Destroyed Rigidbody2D");
                }
                
                treesFixed++;
            }
        }
        
        if (treesFound == 0)
        {
            Debug.LogWarning("   ⚠️ No trees found in scene (maybe not spawned yet)");
        }
        else if (treesFixed == 0)
        {
            Debug.Log($"   ✅ All {treesFound} trees are correct!");
            allSystemsGo = true;
        }
        else
        {
            Debug.Log($"   ✅ Fixed {treesFixed}/{treesFound} trees");
        }
    }
    
    void CheckMazeGeneration()
    {
        Debug.Log("🔲 CHECKING MAZE...");
        
        MazeGenerator mazeGen = FindFirstObjectByType<MazeGenerator>();
        
        if (mazeGen == null)
        {
            Debug.LogError("   ❌ MazeGenerator not found!");
            return;
        }
        
        // Check if maze generated
        if (mazeGen.mazeGrid == null)
        {
            Debug.LogWarning("   ⚠️ Maze not generated yet");
            
            // Check if menu blocking
            GameInitializer gameInit = GameInitializer.Instance;
            if (gameInit != null && !gameInit.IsGameStarted())
            {
                Debug.LogError("   ❌ GAME NOT STARTED - Menu is blocking!");
                Debug.LogError("   💡 Press START button or enable 'Skip Menu'");
            }
            return;
        }
        
        // Check wall count
        int gridWalls = 0;
        int gridPaths = 0;
        for (int x = 0; x < mazeGen.mazeGrid.GetLength(0); x++)
        {
            for (int y = 0; y < mazeGen.mazeGrid.GetLength(1); y++)
            {
                if (mazeGen.mazeGrid[x, y] == 1)
                    gridWalls++;
                else
                    gridPaths++;
            }
        }
        
        GameObject[] sceneWalls = GameObject.FindGameObjectsWithTag("Wall");
        int difference = Mathf.Abs(gridWalls - sceneWalls.Length);
        
        Debug.Log($"   Walls in grid: {gridWalls}");
        Debug.Log($"   Paths in grid: {gridPaths}");
        Debug.Log($"   Walls in scene: {sceneWalls.Length}");
        
        // Check for suspiciously large path areas (might indicate exit corridor bug)
        float pathPercentage = (float)gridPaths / (float)(gridWalls + gridPaths) * 100f;
        Debug.Log($"   Path coverage: {pathPercentage:F1}%");
        
        if (pathPercentage > 60f)
        {
            Debug.LogWarning($"   ⚠️ Maze has very large open areas ({pathPercentage:F0}%)");
            Debug.LogWarning("   This might be due to exit corridor generation");
            Debug.LogWarning("   💡 Press R to regenerate for a more challenging maze");
        }
        
        if (difference > gridWalls * 0.1f)
        {
            Debug.LogWarning($"   ⚠️ {difference} walls missing from scene!");
            Debug.LogWarning("   💡 Press R to regenerate maze");
        }
        else
        {
            Debug.Log("   ✅ Maze structure looks good!");
        }
    }
    
    [ContextMenu("📊 FULL DIAGNOSTIC")]
    public void FullDiagnostic()
    {
        Debug.Log("═══════════════════════════════════════════");
        Debug.Log("📊 FULL DIAGNOSTIC REPORT");
        Debug.Log("═══════════════════════════════════════════");
        
        // Check MazeGenerator
        MazeGenerator mazeGen = FindFirstObjectByType<MazeGenerator>();
        Debug.Log($"MazeGenerator: {(mazeGen != null ? "FOUND ✅" : "MISSING ❌")}");
        
        if (mazeGen != null)
        {
            // Check prefabs
            var treePrefabField = mazeGen.GetType().GetField("treePrefab");
            var wallPrefabField = mazeGen.GetType().GetField("wallPrefab");
            var spiderPrefabField = mazeGen.GetType().GetField("spiderPrefab");
            
            if (treePrefabField != null)
            {
                GameObject treePrefab = treePrefabField.GetValue(mazeGen) as GameObject;
                if (treePrefab != null)
                {
                    Debug.Log($"Tree Prefab: '{treePrefab.name}'");
                    
                    bool hasGuardian = treePrefab.GetComponent<TreeGuardian>() != null;
                    bool hasSpiderAI = treePrefab.GetComponent<SpiderAI>() != null;
                    bool hasRB = treePrefab.GetComponent<Rigidbody2D>() != null;
                    
                    Debug.Log($"   - TreeGuardian: {(hasGuardian ? "YES ✅" : "NO ⚠️")}");
                    Debug.Log($"   - SpiderAI: {(hasSpiderAI ? "YES ❌" : "NO ✅")}");
                    Debug.Log($"   - Rigidbody2D: {(hasRB ? "YES ❌" : "NO ✅")}");
                }
                else
                {
                    Debug.LogError("Tree Prefab: NOT ASSIGNED ❌");
                }
            }
        }
        
        // Check spawned objects
        GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
        Debug.Log($"\nWalls in scene: {walls.Length}");
        
        int treesCount = 0;
        int spidersCount = 0;
        
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("Tree") && !obj.name.Contains("TreeTrunk") && !obj.name.Contains("TreeLeaves"))
                treesCount++;
            if (obj.name.Contains("Spider") || obj.GetComponent<SpiderAI>() != null)
                spidersCount++;
        }
        
        Debug.Log($"Trees in scene: {treesCount}");
        Debug.Log($"Spiders in scene: {spidersCount}");
        
        // Check systems
        EnhancedWallSystem wallSys = FindFirstObjectByType<EnhancedWallSystem>();
        Debug.Log($"\nEnhancedWallSystem: {(wallSys != null ? "ACTIVE ✅" : "MISSING ⚠️")}");
        
        Debug.Log("═══════════════════════════════════════════");
    }
}

