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
        mazeGen = FindObjectOfType<MazeGenerator>();
        Invoke("ValidateMaze", 2f); // Validate after maze generates
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
        
        Debug.Log("🔍 Validating maze connectivity...");
        
        // Find glade center
        int gladeCenterX = mazeGen.mazeWidth / 2;
        int gladeCenterY = mazeGen.mazeHeight / 2;
        
        // Find exit position
        ExitTrigger exit = FindObjectOfType<ExitTrigger>();
        if (exit == null)
        {
            Debug.LogWarning("⚠️ No exit found in maze");
            return;
        }
        
        Vector3 exitWorldPos = exit.transform.position;
        Vector2Int exitGridPos = WorldToGrid(exitWorldPos);
        
        // Check if path exists from glade to exit
        bool pathExists = HasPath(gladeCenterX, gladeCenterY, exitGridPos.x, exitGridPos.y);
        
        if (pathExists)
        {
            Debug.Log("✅ Maze is valid - Path exists from glade to exit!");
        }
        else
        {
            Debug.LogWarning("❌ MAZE HAS NO PATH! Creating emergency path...");
            CreateEmergencyPath(gladeCenterX, gladeCenterY, exitGridPos.x, exitGridPos.y);
        }
    }
    
    bool HasPath(int startX, int startY, int endX, int endY)
    {
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
        
        // Find and destroy any wall at this position
        GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
        foreach (GameObject wall in walls)
        {
            if (Vector2.Distance(wall.transform.position, worldPos) < 0.5f)
            {
                Destroy(wall);
            }
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
        
        ExitTrigger exit = FindObjectOfType<ExitTrigger>();
        if (exit != null)
        {
            Vector2Int exitPos = WorldToGrid(exit.transform.position);
            CreateEmergencyPath(gladeCenterX, gladeCenterY, exitPos.x, exitPos.y);
        }
    }
}

