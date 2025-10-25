using UnityEngine;
using System.Collections.Generic;

public class MazeGenerator : MonoBehaviour
{
    [Header("Maze Generation")]
    public GameObject wallPrefab;
    public int mazeWidth = 60;  // BIGGER MAZE!
    public int mazeHeight = 60; // BIGGER MAZE!
    public float cellSize = 1f;
    public int gladeSize = 10;  // Bigger starting area

    [Header("Spider Settings")]
    public GameObject spiderPrefab;
    public int spiderCount = 8; // More spiders for bigger maze

    [Header("Environment")]
    public GameObject treePrefab;
    public GameObject bushPrefab;

    [Header("Fog of War")]
    public GameObject cloudPrefab;
    public float fogRevealRadius = 6f;
    public bool enableFogOfWar = true;
    
    [Header("Power-ups")]
    public GameObject powerUpPrefab;
    public int powerUpCount = 15; // More power-ups for bigger maze

    public int[,] mazeGrid; // Made public for MazeValidator
    private GameObject fogSystem; // Old square fog - disabled

    void Start()
    {
        Debug.Log("Generating maze with spiders...");
        GenerateCompleteMaze();
        Debug.Log("Maze ready with spider enemies!");
    }

    void Update()
    {
        // Press R to regenerate maze
        if (Input.GetKeyDown(KeyCode.R))
        {
            RegenerateMaze();
        }

        // Press F to toggle fog of war
        if (Input.GetKeyDown(KeyCode.F))
        {
            ToggleFogOfWar();
        }
    }

    void GenerateCompleteMaze()
    {
        // Initialize grid
        mazeGrid = new int[mazeWidth, mazeHeight];

        // Fill with walls
        for (int x = 0; x < mazeWidth; x++)
        {
            for (int y = 0; y < mazeHeight; y++)
            {
                mazeGrid[x, y] = 1;
            }
        }

        // DEBUG: Check coordinate system BEFORE any generation
        DebugCoordinateSystem();

        // Generate maze FIRST
        int entranceX = (mazeWidth + gladeSize) / 2;  // Right side of glade
        int entranceY = mazeHeight / 2;

        // Clear entrance path
        mazeGrid[entranceX, entranceY] = 0;
        mazeGrid[entranceX + 1, entranceY] = 0;
        mazeGrid[entranceX + 2, entranceY] = 0;

        // Generate maze from entrance
        GenerateMazeFromPoint(entranceX + 2, entranceY);

        // NOW clear the Glade area (this will override any walls placed by maze gen)
        int gladeStartX = (mazeWidth - gladeSize) / 2;
        int gladeEndX = gladeStartX + gladeSize;
        int gladeStartY = (mazeHeight - gladeSize) / 2;
        int gladeEndY = gladeStartY + gladeSize;

        for (int x = gladeStartX; x < gladeEndX; x++)
        {
            for (int y = gladeStartY; y < gladeEndY; y++)
            {
                mazeGrid[x, y] = 0;  // Force clear Glade area
            }
        }

        // REMOVE THIS DUPLICATE LINE:
        // GenerateMazeFromPoint(entranceX + 2, entranceY); // DELETE THIS!

        // Re-clear the entrance path (in case maze gen messed with it)
        mazeGrid[entranceX, entranceY] = 0;
        mazeGrid[entranceX + 1, entranceY] = 0;
        mazeGrid[entranceX + 2, entranceY] = 0;

        // Create boundaries
        CreateBoundaries();

        // Create random exit far from entrance
        CreateRandomExit();

        // Build all visual elements
        BuildAllWalls();
        BuildGladeWalls();

        // SPAWN SPIDERS
        SpawnSpidersInMaze();

        // Add environment
        AddEnvironmentToMaze();
        
        // Spawn power-ups
        SpawnPowerUps();

        // Initialize Fog of War
        // Old square fog disabled - use ClashStyleFogSystem instead
        // if (enableFogOfWar && cloudPrefab != null)
        // {
        //     InitializeFogOfWar();
        // }
    }

    void DebugCoordinateSystem()
    {
        Debug.Log("=== COORDINATE SYSTEM DEBUG ===");
        Debug.Log($"Maze Dimensions: {mazeWidth}x{mazeHeight} cells, Cell Size: {cellSize}");

        // Test key positions
        Vector2 centerPos = GridToWorldPosition(mazeWidth / 2, mazeHeight / 2);
        Vector2 topLeftPos = GridToWorldPosition(0, 0);
        Vector2 topRightPos = GridToWorldPosition(mazeWidth - 1, 0);
        Vector2 bottomLeftPos = GridToWorldPosition(0, mazeHeight - 1);
        Vector2 bottomRightPos = GridToWorldPosition(mazeWidth - 1, mazeHeight - 1);

        Debug.Log($"Center (Grid {mazeWidth / 2},{mazeHeight / 2}) -> World ({centerPos.x:F1}, {centerPos.y:F1})");
        Debug.Log($"Top-Left (Grid 0,0) -> World ({topLeftPos.x:F1}, {topLeftPos.y:F1})");
        Debug.Log($"Top-Right (Grid {mazeWidth - 1},0) -> World ({topRightPos.x:F1}, {topRightPos.y:F1})");
        Debug.Log($"Bottom-Left (Grid 0,{mazeHeight - 1}) -> World ({bottomLeftPos.x:F1}, {bottomLeftPos.y:F1})");
        Debug.Log($"Bottom-Right (Grid {mazeWidth - 1},{mazeHeight - 1}) -> World ({bottomRightPos.x:F1}, {bottomRightPos.y:F1})");

        // Calculate expected range
        float expectedMinX = -(mazeWidth * cellSize) / 2f;
        float expectedMaxX = (mazeWidth * cellSize) / 2f;
        float expectedMinY = -(mazeHeight * cellSize) / 2f;
        float expectedMaxY = (mazeHeight * cellSize) / 2f;

        Debug.Log($"Expected World Range: X[{expectedMinX:F1} to {expectedMaxX:F1}], Y[{expectedMinY:F1} to {expectedMaxY:F1}]");

        // Test Glade positions
        int gladeStartX = (mazeWidth - gladeSize) / 2;
        int gladeStartY = (mazeHeight - gladeSize) / 2;
        int gladeCenterX = gladeStartX + gladeSize / 2;
        int gladeCenterY = gladeStartY + gladeSize / 2;

        Vector2 gladeCenterWorld = GridToWorldPosition(gladeCenterX, gladeCenterY);
        Vector2 gladeStartWorld = GridToWorldPosition(gladeStartX, gladeStartY);
        Vector2 gladeEndWorld = GridToWorldPosition(gladeStartX + gladeSize - 1, gladeStartY + gladeSize - 1);

        Debug.Log($"Glade Center (Grid {gladeCenterX},{gladeCenterY}) -> World ({gladeCenterWorld.x:F1}, {gladeCenterWorld.y:F1})");
        Debug.Log($"Glade Start (Grid {gladeStartX},{gladeStartY}) -> World ({gladeStartWorld.x:F1}, {gladeStartWorld.y:F1})");
        Debug.Log($"Glade End (Grid {gladeStartX + gladeSize - 1},{gladeStartY + gladeSize - 1}) -> World ({gladeEndWorld.x:F1}, {gladeEndWorld.y:F1})");

        Debug.Log("=== END COORDINATE DEBUG ===");
    }

    void GenerateMazeFromPoint(int startX, int startY)
    {
        Stack<Vector2> stack = new Stack<Vector2>();
        stack.Push(new Vector2(startX, startY));

        while (stack.Count > 0)
        {
            Vector2 current = stack.Pop();
            int x = (int)current.x;
            int y = (int)current.y;

            // Mark current cell as path
            mazeGrid[x, y] = 0;

            // Define possible directions (moving 2 cells)
            List<Vector2> directions = new List<Vector2>
            {
                new Vector2(2, 0),   // Right
                new Vector2(-2, 0),  // Left
                new Vector2(0, 2),   // Up
                new Vector2(0, -2)   // Down
            };

            // Shuffle directions for randomness
            ShuffleList(directions);

            foreach (Vector2 dir in directions)
            {
                int newX = x + (int)dir.x;
                int newY = y + (int)dir.y;

                // Check if new position is valid and unvisited
                if (newX > 0 && newX < mazeWidth - 1 &&
                    newY > 0 && newY < mazeHeight - 1 &&
                    mazeGrid[newX, newY] == 1)
                {
                    // Carve through the wall between cells
                    mazeGrid[x + (int)dir.x / 2, y + (int)dir.y / 2] = 0;

                    // Mark new cell and add to stack
                    mazeGrid[newX, newY] = 0;
                    stack.Push(new Vector2(newX, newY));
                }
            }
        }
    }

    void CreateRandomExit()
    {
        // Calculate Glade position for distance check
        int gladeStartX = (mazeWidth - gladeSize) / 2;
        int gladeEndX = gladeStartX + gladeSize;
        int gladeStartY = (mazeHeight - gladeSize) / 2;
        int gladeEndY = gladeStartY + gladeSize;

        // Define search priorities (far from Glade entrance)
        List<Vector2Int> searchPriorities = new List<Vector2Int>
        {
            new Vector2Int(0, mazeHeight / 2),           // Left middle
            new Vector2Int(mazeWidth / 2, 0),            // Bottom middle
            new Vector2Int(mazeWidth / 2, mazeHeight - 1), // Top middle
            new Vector2Int(mazeWidth - 1, 0),            // Bottom right
            new Vector2Int(mazeWidth - 1, mazeHeight - 1), // Top right
            new Vector2Int(0, 0),                        // Bottom left
            new Vector2Int(0, mazeHeight - 1)            // Top left
        };

        // Randomize search order
        ShuffleList(searchPriorities);

        foreach (Vector2Int searchPoint in searchPriorities)
        {
            // Search around this point
            for (int radius = 0; radius < 6; radius++)
            {
                for (int x = Mathf.Max(1, searchPoint.x - radius); x <= Mathf.Min(mazeWidth - 2, searchPoint.x + radius); x++)
                {
                    for (int y = Mathf.Max(1, searchPoint.y - radius); y <= Mathf.Min(mazeHeight - 2, searchPoint.y + radius); y++)
                    {
                        // Ensure exit is far from Glade entrance
                        bool isFarFromEntrance = (x < gladeStartX - 6 || x > gladeEndX + 6 ||
                                                y < gladeStartY - 6 || y > gladeEndY + 6);

                        if (mazeGrid[x, y] == 0 && isFarFromEntrance)
                        {
                            CreateExitAtNearestEdge(x, y);
                            return;
                        }
                    }
                }
            }
        }

        // Fallback exit
        CreateEmergencyExit();
    }

    void CreateExitAtNearestEdge(int pathX, int pathY)
    {
        // Calculate distances to each edge
        int distToLeft = pathX;
        int distToRight = mazeWidth - 1 - pathX;
        int distToBottom = pathY;
        int distToTop = mazeHeight - 1 - pathY;

        // Find closest edge
        int minDist = Mathf.Min(distToLeft, distToRight, distToBottom, distToTop);

        if (minDist == distToLeft)
        {
            mazeGrid[0, pathY] = 0;
            CreateExitMarker(0, pathY, Color.green);
        }
        else if (minDist == distToRight)
        {
            mazeGrid[mazeWidth - 1, pathY] = 0;
            CreateExitMarker(mazeWidth - 1, pathY, Color.green);
        }
        else if (minDist == distToBottom)
        {
            mazeGrid[pathX, 0] = 0;
            CreateExitMarker(pathX, 0, Color.green);
        }
        else
        {
            mazeGrid[pathX, mazeHeight - 1] = 0;
            CreateExitMarker(pathX, mazeHeight - 1, Color.green);
        }
    }

    void CreateEmergencyExit()
    {
        // Create exit anywhere as last resort
        for (int x = 1; x < mazeWidth - 1; x++)
        {
            for (int y = 1; y < mazeHeight - 1; y++)
            {
                if (mazeGrid[x, y] == 0)
                {
                    if (x < mazeWidth / 2)
                    {
                        mazeGrid[0, y] = 0;
                        CreateExitMarker(0, y, Color.red);
                    }
                    else
                    {
                        mazeGrid[mazeWidth - 1, y] = 0;
                        CreateExitMarker(mazeWidth - 1, y, Color.red);
                    }
                    return;
                }
            }
        }
    }

    void CreateExitMarker(int gridX, int gridY, Color color)
    {
        Vector2 exitPos = GridToWorldPosition(gridX, gridY);
        GameObject exit = Instantiate(wallPrefab, exitPos, Quaternion.identity);

        // Setup as exit trigger instead of wall
        try
        {
            exit.tag = "Exit";
        }
        catch
        {
            // Exit tag doesn't exist, that's okay
            Debug.LogWarning("Exit tag not defined. Exit will work without tag.");
        }
        exit.GetComponent<SpriteRenderer>().color = color;
        exit.transform.localScale = Vector3.one * 1.3f;
        
        // Remove wall collider and add trigger
        BoxCollider2D wallCollider = exit.GetComponent<BoxCollider2D>();
        if (wallCollider != null)
        {
            wallCollider.isTrigger = true;
        }
        
        // Add exit trigger component
        if (exit.GetComponent<ExitTrigger>() == null)
        {
            exit.AddComponent<ExitTrigger>();
        }
        
        Debug.Log($"Exit created at grid ({gridX}, {gridY})");
    }

    void BuildGladeWalls()
    {
        // Calculate Glade center in grid coordinates
        int gladeCenterX = mazeWidth / 2;
        int gladeCenterY = mazeHeight / 2;
        Vector2 gladeCenterWorld = GridToWorldPosition(gladeCenterX, gladeCenterY);

        int wallRadius = gladeSize / 2;

        // Build Glade walls around the actual Glade center
        for (int x = -wallRadius; x <= wallRadius; x++)
        {
            for (int y = -wallRadius; y <= wallRadius; y++)
            {
                // Only build perimeter walls
                if (Mathf.Abs(x) == wallRadius || Mathf.Abs(y) == wallRadius)
                {
                    // Create entrance on right side (3 units wide)
                    bool isEntrance = (x == wallRadius && y >= -1 && y <= 1);

                    if (!isEntrance)
                    {
                        Vector2 pos = gladeCenterWorld + new Vector2(x * cellSize, y * cellSize);
                        CreateWallAtPosition(pos);
                    }
                }
            }
        }

        // Position player in the actual Glade center
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = gladeCenterWorld;
        }

        Debug.Log($"Glade walls built around: {gladeCenterWorld}");
    }

    void CreateBoundaries()
    {
        // Create solid outer walls
        for (int x = 0; x < mazeWidth; x++)
        {
            mazeGrid[x, 0] = 1;
            mazeGrid[x, mazeHeight - 1] = 1;
        }
        for (int y = 0; y < mazeHeight; y++)
        {
            mazeGrid[0, y] = 1;
            mazeGrid[mazeWidth - 1, y] = 1;
        }
    }

    void BuildAllWalls()
    {
        // Clear existing walls
        GameObject[] oldWalls = GameObject.FindGameObjectsWithTag("Wall");
        foreach (GameObject wall in oldWalls)
        {
            Destroy(wall);
        }

        // Build walls from grid
        for (int x = 0; x < mazeWidth; x++)
        {
            for (int y = 0; y < mazeHeight; y++)
            {
                if (mazeGrid[x, y] == 1)
                {
                    Vector2 pos = GridToWorldPosition(x, y);
                    CreateWallAtPosition(pos);
                }
            }
        }
    }

    // ========== SPIDER SPAWNING METHODS ==========

    void SpawnSpidersInMaze()
    {
        if (spiderPrefab == null)
        {
            Debug.LogError("Spider Prefab is not assigned!");
            return;
        }

        int spawnedCount = 0;

        for (int i = 0; i < spiderCount; i++)
        {
            Vector2 spawnPos = GetRandomSpiderSpawnPosition();
            if (spawnPos != Vector2.zero)
            {
                Instantiate(spiderPrefab, spawnPos, Quaternion.identity);
                spawnedCount++;
                Debug.Log($"Spider {i + 1} spawned at {spawnPos}");
            }
        }

        Debug.Log($"Successfully spawned {spawnedCount}/{spiderCount} spiders");
    }

    Vector2 GetRandomSpiderSpawnPosition()
    {
        // First try: strict conditions (far from glade, good spawn)
        for (int attempts = 0; attempts < 50; attempts++)
        {
            int x = Random.Range(5, mazeWidth - 5);
            int y = Random.Range(5, mazeHeight - 5);

            if (mazeGrid[x, y] == 0 && IsFarFromGlade(x, y) && IsValidSpiderSpawn(x, y))
            {
                return GridToWorldPosition(x, y);
            }
        }
        
        // Second try: relaxed conditions (just far from glade)
        for (int attempts = 0; attempts < 50; attempts++)
        {
            int x = Random.Range(3, mazeWidth - 3);
            int y = Random.Range(3, mazeHeight - 3);

            if (mazeGrid[x, y] == 0 && IsFarFromGlade(x, y))
            {
                return GridToWorldPosition(x, y);
            }
        }
        
        // Third try: any open space not in glade
        for (int attempts = 0; attempts < 50; attempts++)
        {
            int x = Random.Range(2, mazeWidth - 2);
            int y = Random.Range(2, mazeHeight - 2);

            if (mazeGrid[x, y] == 0)
            {
                float distToGlade = Vector2.Distance(
                    new Vector2(x, y),
                    new Vector2(mazeWidth / 2f, mazeHeight / 2f)
                );
                
                if (distToGlade > gladeSize + 2)
                {
                    return GridToWorldPosition(x, y);
                }
            }
        }
        
        // Last resort: any open space
        for (int attempts = 0; attempts < 100; attempts++)
        {
            int x = Random.Range(1, mazeWidth - 1);
            int y = Random.Range(1, mazeHeight - 1);

            if (mazeGrid[x, y] == 0)
            {
                Debug.LogWarning($"Spider spawned at fallback position ({x}, {y})");
                return GridToWorldPosition(x, y);
            }
        }

        Debug.LogWarning("Could not find valid spider spawn position - spawning at safe location");
        // Ultimate fallback: spawn near edge
        return GridToWorldPosition(mazeWidth - 10, mazeHeight - 10);
    }

    bool IsValidSpiderSpawn(int gridX, int gridY)
    {
        // Check if spider has at least one open direction to move
        int openDirections = 0;

        if (gridX + 1 < mazeWidth && mazeGrid[gridX + 1, gridY] == 0) openDirections++;
        if (gridX - 1 >= 0 && mazeGrid[gridX - 1, gridY] == 0) openDirections++;
        if (gridY + 1 < mazeHeight && mazeGrid[gridX, gridY + 1] == 0) openDirections++;
        if (gridY - 1 >= 0 && mazeGrid[gridX, gridY - 1] == 0) openDirections++;

        return openDirections >= 1; // Changed from 2 to 1 for more flexibility
    }

    bool IsFarFromGlade(int gridX, int gridY)
    {
        int gladeCenterX = mazeWidth / 2;
        int gladeCenterY = mazeHeight / 2;

        float distance = Vector2.Distance(new Vector2(gridX, gridY),
                                         new Vector2(gladeCenterX, gladeCenterY));
        return distance > gladeSize + 5;
    }

    // ========== ENVIRONMENT METHODS ==========

    void AddEnvironmentToMaze()
    {
        Debug.Log("Adding environment to maze...");
        AddTreesToGlade();
        AddBushesToMaze();
    }

    void AddTreesToGlade()
    {
        Debug.Log("Adding trees to Glade...");

        if (treePrefab == null)
        {
            Debug.LogError("Tree Prefab is not assigned!");
            return;
        }

        // Calculate Glade bounds in grid coordinates
        int gladeStartX = (mazeWidth - gladeSize) / 2;
        int gladeEndX = gladeStartX + gladeSize;
        int gladeStartY = (mazeHeight - gladeSize) / 2;
        int gladeEndY = gladeStartY + gladeSize;

        Debug.Log($"Placing trees in Glade area: Grid({gladeStartX},{gladeStartY}) to ({gladeEndX},{gladeEndY})");

        // Place trees only in open areas within the Glade
        int treesPlaced = 0;
        int maxAttempts = 50;

        while (treesPlaced < 7 && maxAttempts > 0) // Try to place 7 trees
        {
            // Random position within Glade (excluding very center)
            int gridX = Random.Range(gladeStartX + 1, gladeEndX - 1);
            int gridY = Random.Range(gladeStartY + 1, gladeEndY - 1);

            // Check if this is a path cell (not wall) and not too close to center
            if (mazeGrid[gridX, gridY] == 0 && IsValidTreePosition(gridX, gridY))
            {
                Vector2 worldPos = GridToWorldPosition(gridX, gridY);
                Instantiate(treePrefab, worldPos, Quaternion.identity);
                treesPlaced++;
                Debug.Log($"Tree {treesPlaced} created at grid:({gridX},{gridY}) world:({worldPos.x:F1},{worldPos.y:F1})");
            }

            maxAttempts--;
        }

        Debug.Log($"Added {treesPlaced} trees to Glade");
    }

    bool IsValidTreePosition(int gridX, int gridY)
    {
        // Don't place trees too close to the center (player spawn)
        int gladeCenterX = mazeWidth / 2;
        int gladeCenterY = mazeHeight / 2;

        float distanceToCenter = Vector2.Distance(
            new Vector2(gridX, gridY),
            new Vector2(gladeCenterX, gladeCenterY)
        );

        return distanceToCenter > 2f; // Minimum distance from center
    }

    void AddBushesToMaze()
    {
        Debug.Log("Adding bushes to maze...");

        if (bushPrefab == null)
        {
            Debug.LogError("Bush Prefab is not assigned!");
            return;
        }

        int bushCount = 0;

        for (int x = 0; x < mazeWidth; x++)
        {
            for (int y = 0; y < mazeHeight; y++)
            {
                // Place bushes in path cells, avoiding Glade area
                if (mazeGrid[x, y] == 0 && IsFarFromGladeForBush(x, y))
                {
                    // 25% chance to place bush in path cells
                    if (Random.Range(0, 4) == 0)
                    {
                        Vector2 worldPos = GridToWorldPosition(x, y);
                        Instantiate(bushPrefab, worldPos, Quaternion.identity);
                        bushCount++;
                    }
                }
            }
        }

        Debug.Log($"Added {bushCount} bushes to maze");
    }

    bool IsFarFromGladeForBush(int gridX, int gridY)
    {
        int gladeCenterX = mazeWidth / 2;
        int gladeCenterY = mazeHeight / 2;

        float distance = Vector2.Distance(new Vector2(gridX, gridY),
                                         new Vector2(gladeCenterX, gladeCenterY));
        return distance > gladeSize + 2;
    }

    // ========== FOG OF WAR METHODS ==========

    // OLD FOG SYSTEM DISABLED - Use ClashStyleFogSystem instead
    // void InitializeFogOfWar()
    // {
    //     // This old fog system has been replaced
    // }

    void ToggleFogOfWar()
    {
        enableFogOfWar = !enableFogOfWar;

        if (fogSystem != null)
        {
            fogSystem.SetActive(enableFogOfWar);
        }

        Debug.Log($"Fog of War: {(enableFogOfWar ? "ENABLED" : "DISABLED")}");
    }

    // ========== POWER-UP METHODS ==========
    
    void SpawnPowerUps()
    {
        if (powerUpPrefab == null)
        {
            Debug.LogError("Power-up Prefab is not assigned!");
            return;
        }
        
        Debug.Log("Spawning power-ups...");
        
        int spawnedCount = 0;
        
        for (int i = 0; i < powerUpCount; i++)
        {
            Vector2 spawnPos = GetRandomPowerUpSpawnPosition();
            if (spawnPos != Vector2.zero)
            {
                GameObject powerUp = Instantiate(powerUpPrefab, spawnPos, Quaternion.identity);
                
                // Randomize power-up type
                PowerUp powerUpScript = powerUp.GetComponent<PowerUp>();
                if (powerUpScript != null)
                {
                    System.Array types = System.Enum.GetValues(typeof(PowerUp.PowerUpType));
                    powerUpScript.type = (PowerUp.PowerUpType)types.GetValue(Random.Range(0, types.Length));
                }
                
                spawnedCount++;
                Debug.Log($"Power-up {i + 1} spawned at {spawnPos}");
            }
        }
        
        Debug.Log($"Successfully spawned {spawnedCount}/{powerUpCount} power-ups");
    }
    
    Vector2 GetRandomPowerUpSpawnPosition()
    {
        for (int attempts = 0; attempts < 50; attempts++)
        {
            int x = Random.Range(2, mazeWidth - 2);
            int y = Random.Range(2, mazeHeight - 2);
            
            if (mazeGrid[x, y] == 0 && IsValidPowerUpSpawn(x, y))
            {
                return GridToWorldPosition(x, y);
            }
        }
        
        Debug.LogWarning("Could not find valid power-up spawn position");
        return Vector2.zero;
    }
    
    bool IsValidPowerUpSpawn(int gridX, int gridY)
    {
        // Don't spawn too close to Glade
        int gladeCenterX = mazeWidth / 2;
        int gladeCenterY = mazeHeight / 2;
        
        float distanceToGlade = Vector2.Distance(
            new Vector2(gridX, gridY),
            new Vector2(gladeCenterX, gladeCenterY)
        );
        
        return distanceToGlade > gladeSize + 3;
    }

    // ========== UTILITY METHODS ==========

    GameObject CreateWallAtPosition(Vector2 position)
    {
        GameObject wall = Instantiate(wallPrefab, position, Quaternion.identity);

        // Ensure collider exists
        if (wall.GetComponent<BoxCollider2D>() == null)
        {
            wall.AddComponent<BoxCollider2D>();
        }

        return wall;
    }

    public void RegenerateMaze()
    {
        Debug.Log("Regenerating maze...");

        // Clear all walls
        GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
        foreach (GameObject wall in walls)
        {
            Destroy(wall);
        }

        // Clear spiders
        SpiderAI[] spiders = FindObjectsOfType<SpiderAI>();
        foreach (SpiderAI spider in spiders)
        {
            Destroy(spider.gameObject);
        }

        // Clear environment (bushes)
        BushScript[] bushes = FindObjectsOfType<BushScript>();
        foreach (BushScript bush in bushes)
        {
            Destroy(bush.gameObject);
        }
        
        // Clear trees (by tag)
        try
        {
            GameObject[] trees = GameObject.FindGameObjectsWithTag("Tree");
            foreach (GameObject tree in trees)
            {
                Destroy(tree);
            }
        }
        catch
        {
            // Tree tag doesn't exist
        }
        
        // Clear trees (by script) - only if TreeScript exists
        Object[] treeScripts = FindObjectsOfType(typeof(Component));
        foreach (Object obj in treeScripts)
        {
            if (obj is Component comp && comp.GetType().Name == "TreeScript")
            {
                Destroy(comp.gameObject);
            }
        }
        
        // Clear any GameObject with "Tree" in name (fallback)
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("Tree") && !obj.name.Contains("TreeScript") && !obj.name.Contains("MazeGenerator"))
            {
                Destroy(obj);
            }
        }
        
        // Clear power-ups
        PowerUp[] powerUps = FindObjectsOfType<PowerUp>();
        foreach (PowerUp powerUp in powerUps)
        {
            Destroy(powerUp.gameObject);
        }
        
        // Clear exits
        try
        {
            GameObject[] exits = GameObject.FindGameObjectsWithTag("Exit");
            foreach (GameObject exit in exits)
            {
                Destroy(exit);
            }
        }
        catch
        {
            // Exit tag doesn't exist, find exits another way
            ExitTrigger[] exitTriggers = FindObjectsOfType<ExitTrigger>();
            foreach (ExitTrigger exitTrigger in exitTriggers)
            {
                Destroy(exitTrigger.gameObject);
            }
        }

        // Reset fog system (works with any fog system using reflection)
        if (enableFogOfWar)
        {
            // Find ClashStyleFogSystem (new system)
            ClashStyleFogSystem newFog = FindObjectOfType<ClashStyleFogSystem>();
            if (newFog != null)
            {
                // Recreate fog for the new maze
                Invoke("RecreateFog", 0.5f);
            }
            
            // Also try to reset old fog system if it exists (using reflection)
            if (fogSystem != null)
            {
                Component fogComponent = fogSystem.GetComponent(fogSystem.GetType());
                if (fogComponent != null)
                {
                    System.Reflection.MethodInfo resetMethod = fogComponent.GetType().GetMethod("ResetFog");
                    if (resetMethod != null)
                    {
                        resetMethod.Invoke(fogComponent, null);
                    }
                }
            }
        }

        // Generate new maze
        GenerateCompleteMaze();
    }
    
    void RecreateFog()
    {
        // Recreate fog for the new maze layout
        ClashStyleFogSystem fogSystem = FindObjectOfType<ClashStyleFogSystem>();
        if (fogSystem != null)
        {
            // Call Start() again to recreate fog
            fogSystem.SendMessage("CreateFogSystem", SendMessageOptions.DontRequireReceiver);
        }
    }

    void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    Vector2 GridToWorldPosition(int gridX, int gridY)
    {
        float worldX = (gridX - mazeWidth / 2) * cellSize;
        float worldY = (gridY - mazeHeight / 2) * cellSize;
        return new Vector2(worldX, worldY);
    }
}