using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Validates and fixes maze generation issues
/// Ensures there's always a path from glade to exit
/// </summary>
public class MazeValidator : MonoBehaviour
{
    private MazeGenerator mazeGen;
    
    void Start()
    {
        mazeGen = FindFirstObjectByType<MazeGenerator>();
        
        // Validate multiple times to ensure path exists
        Invoke("ValidateMaze", 2f);  // First check
        Invoke("ValidateMaze", 4f);  // Second check (in case of timing issues)
        Invoke("ValidateMaze", 6f);  // Third check for safety
    }
    
    void Update()
    {
        // Re-validate when R is pressed
        if (Input.GetKeyDown(KeyCode.R))
        {
            Invoke("ValidateMaze", 1f);
        }
    }
    
    void ValidateMaze()
    {
        if (mazeGen == null || mazeGen.mazeGrid == null)
        {
            Debug.LogWarning("⚠️ Cannot validate maze - MazeGenerator not found");
            return;
        }
        
        // Check if maze grid is properly initialized
        if (mazeGen.mazeWidth <= 0 || mazeGen.mazeHeight <= 0)
        {
            Debug.LogWarning("⚠️ Cannot validate maze - Invalid maze dimensions");
            return;
        }
        
        Debug.Log("🔍 Validating maze connectivity...");
        
        // Find glade center
        int gladeCenterX = mazeGen.mazeWidth / 2;
        int gladeCenterY = mazeGen.mazeHeight / 2;
        
        // Find exit position
        ExitTrigger exit = FindFirstObjectByType<ExitTrigger>();
        if (exit == null)
        {
            Debug.LogWarning("⚠️ No exit found in maze - skipping validation");
            return;
        }
        
        Vector3 exitWorldPos = exit.transform.position;
        Vector2Int exitGridPos = WorldToGrid(exitWorldPos);
        
        Debug.Log($"📍 Exit found at world: {exitWorldPos} → grid: ({exitGridPos.x}, {exitGridPos.y})");
        Debug.Log($"📍 Glade center at grid: ({gladeCenterX}, {gladeCenterY})");
        
        // Validate coordinates before checking path
        if (!IsValidPosition(gladeCenterX, gladeCenterY))
        {
            Debug.LogWarning($"⚠️ Glade center out of bounds: ({gladeCenterX}, {gladeCenterY})");
            return;
        }
        
        if (!IsValidPosition(exitGridPos.x, exitGridPos.y))
        {
            Debug.LogWarning($"⚠️ Exit position out of bounds: ({exitGridPos.x}, {exitGridPos.y})");
            return;
        }
        
        // Check if path exists from glade to exit
        bool pathExists = HasPath(gladeCenterX, gladeCenterY, exitGridPos.x, exitGridPos.y);
        
        if (pathExists)
        {
            Debug.Log("✅ Maze is valid - Path exists from glade to exit!");
            Debug.Log($"   🎯 Exit is at {exit.transform.position}");
            Debug.Log($"   🗺️ Exit grid: ({exitGridPos.x}, {exitGridPos.y})");
        }
        else
        {
            Debug.LogWarning("❌ MAZE HAS NO PATH! Creating emergency path...");
            Debug.LogWarning($"   📍 From: Glade ({gladeCenterX}, {gladeCenterY}) → Exit ({exitGridPos.x}, {exitGridPos.y})");
            CreateEmergencyPath(gladeCenterX, gladeCenterY, exitGridPos.x, exitGridPos.y);
            
            // Verify the emergency path worked
            Invoke("VerifyEmergencyPath", 1f);
        }
    }
    
    bool HasPath(int startX, int startY, int endX, int endY)
    {
        // Validate coordinates are within bounds
        if (!IsValidPosition(startX, startY) || !IsValidPosition(endX, endY))
        {
            Debug.LogWarning($"⚠️ Path coordinates out of bounds: Start({startX},{startY}) End({endX},{endY})");
            return false;
        }
        
        if (mazeGen.mazeGrid[startX, startY] == 1 || mazeGen.mazeGrid[endX, endY] == 1)
        {
            return false; // Start or end is in wall
        }
        
        // BFS pathfinding
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        
        queue.Enqueue(new Vector2Int(startX, startY));
        visited.Add(new Vector2Int(startX, startY));
        
        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            
            // Found the exit!
            if (current.x == endX && current.y == endY)
            {
                return true;
            }
            
            // Check neighbors
            Vector2Int[] neighbors = new Vector2Int[]
            {
                new Vector2Int(current.x + 1, current.y),
                new Vector2Int(current.x - 1, current.y),
                new Vector2Int(current.x, current.y + 1),
                new Vector2Int(current.x, current.y - 1)
            };
            
            foreach (Vector2Int neighbor in neighbors)
            {
                if (IsValidPosition(neighbor.x, neighbor.y) && 
                    !visited.Contains(neighbor) && 
                    mazeGen.mazeGrid[neighbor.x, neighbor.y] == 0)
                {
                    queue.Enqueue(neighbor);
                    visited.Add(neighbor);
                }
            }
        }
        
        return false; // No path found
    }
    
    void CreateEmergencyPath(int startX, int startY, int endX, int endY)
    {
        // Create a simple straight path with some turns
        int currentX = startX;
        int currentY = startY;
        
        int pathsCreated = 0;
        
        while (currentX != endX || currentY != endY)
        {
            // Clear current position
            if (IsValidPosition(currentX, currentY))
            {
                mazeGen.mazeGrid[currentX, currentY] = 0;
                
                // Remove wall if it exists
                RemoveWallAt(currentX, currentY);
                pathsCreated++;
            }
            
            // Move towards exit
            if (currentX < endX) currentX++;
            else if (currentX > endX) currentX--;
            else if (currentY < endY) currentY++;
            else if (currentY > endY) currentY--;
        }
        
        // Clear exit position
        mazeGen.mazeGrid[endX, endY] = 0;
        RemoveWallAt(endX, endY);
        
        Debug.Log($"✅ Created emergency path ({pathsCreated} cells cleared)");
    }
    
    void RemoveWallAt(int gridX, int gridY)
    {
        Vector2 worldPos = GridToWorld(gridX, gridY);
        
        try
        {
            // Find and destroy any wall at this position
            GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
            foreach (GameObject wall in walls)
            {
                if (wall != null && Vector2.Distance(wall.transform.position, worldPos) < 0.5f)
                {
                    Destroy(wall);
                }
            }
        }
        catch (UnityException)
        {
            // Wall tag doesn't exist, that's okay
            Debug.LogWarning("⚠️ Wall tag not defined - cannot remove walls");
        }
    }
    
    bool IsValidPosition(int x, int y)
    {
        return x >= 0 && x < mazeGen.mazeWidth && y >= 0 && y < mazeGen.mazeHeight;
    }
    
    Vector2Int WorldToGrid(Vector3 worldPos)
    {
        int gridX = Mathf.RoundToInt(worldPos.x / mazeGen.cellSize + mazeGen.mazeWidth / 2f);
        int gridY = Mathf.RoundToInt(worldPos.y / mazeGen.cellSize + mazeGen.mazeHeight / 2f);
        
        // Clamp to valid grid bounds
        gridX = Mathf.Clamp(gridX, 0, mazeGen.mazeWidth - 1);
        gridY = Mathf.Clamp(gridY, 0, mazeGen.mazeHeight - 1);
        
        return new Vector2Int(gridX, gridY);
    }
    
    Vector2 GridToWorld(int gridX, int gridY)
    {
        float worldX = (gridX - mazeGen.mazeWidth / 2) * mazeGen.cellSize;
        float worldY = (gridY - mazeGen.mazeHeight / 2) * mazeGen.cellSize;
        return new Vector2(worldX, worldY);
    }
    
    [ContextMenu("Validate Maze Now")]
    public void ValidateMazeNow()
    {
        ValidateMaze();
    }
    
    [ContextMenu("Force Create Path")]
    public void ForceCreatePath()
    {
        if (mazeGen == null) return;
        
        int gladeCenterX = mazeGen.mazeWidth / 2;
        int gladeCenterY = mazeGen.mazeHeight / 2;
        
        ExitTrigger exit = FindFirstObjectByType<ExitTrigger>();
        if (exit != null)
        {
            Vector2Int exitPos = WorldToGrid(exit.transform.position);
            CreateEmergencyPath(gladeCenterX, gladeCenterY, exitPos.x, exitPos.y);
        }
    }
    
    void VerifyEmergencyPath()
    {
        Debug.Log("🔍 Verifying emergency path was created...");
        
        ExitTrigger exit = FindFirstObjectByType<ExitTrigger>();
        if (exit == null) return;
        
        int gladeCenterX = mazeGen.mazeWidth / 2;
        int gladeCenterY = mazeGen.mazeHeight / 2;
        Vector2Int exitGridPos = WorldToGrid(exit.transform.position);
        
        bool pathExists = HasPath(gladeCenterX, gladeCenterY, exitGridPos.x, exitGridPos.y);
        
        if (pathExists)
        {
            Debug.Log("✅ Emergency path VERIFIED - Exit is now reachable!");
        }
        else
        {
            Debug.LogError("❌ CRITICAL: Emergency path failed! Creating wider path...");
            CreateWiderEmergencyPath(gladeCenterX, gladeCenterY, exitGridPos.x, exitGridPos.y);
        }
    }
    
    void CreateWiderEmergencyPath(int startX, int startY, int endX, int endY)
    {
        // Create a 3-cell wide path to ensure accessibility
        CreateEmergencyPath(startX, startY, endX, endY);
        CreateEmergencyPath(startX + 1, startY, endX, endY);
        CreateEmergencyPath(startX, startY + 1, endX, endY);
        
        Debug.Log("✅ Created wider emergency path (3 cells wide)");
    }
}


