using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class LevelData
{
    public int levelNumber;
    public int mazeWidth = 30;
    public int mazeHeight = 30;
    public int spiderCount = 3;
    public int powerUpCount = 5;
    public float levelTime = 300f; // 5 minutes
    public float spiderSpeed = 2f;
    public float spiderChaseRadius = 5f;
    public int gladeSize = 10;
    public bool enableFogOfWar = true;
    public float fogRevealRadius = 6f;
    public string levelName = "Maze Level";
    public string levelDescription = "Navigate through the maze and avoid the spiders!";
}

public class LevelManager : MonoBehaviour
{
    [Header("Level Settings")]
    public List<LevelData> levels = new List<LevelData>();
    public int currentLevelIndex = 0;
    public bool unlockAllLevels = false;
    
    [Header("Progression Settings")]
    public int scoreRequiredForNextLevel = 1000;
    public int maxLevels = 10;
    
    [Header("Difficulty Scaling")]
    public float spiderSpeedIncrease = 0.2f;
    public float mazeSizeIncrease = 5f;
    public float timeDecrease = 30f;
    
    private GameManager gameManager;
    private MazeGenerator mazeGenerator;
    private PlayerProgress playerProgress;
    
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        mazeGenerator = FindObjectOfType<MazeGenerator>();
        playerProgress = FindObjectOfType<PlayerProgress>();
        
        InitializeLevels();
        LoadCurrentLevel();
        
        Debug.Log($"Level Manager initialized. Current level: {GetCurrentLevel().levelNumber}");
    }
    
    void InitializeLevels()
    {
        if (levels.Count == 0)
        {
            GenerateDefaultLevels();
        }
    }
    
    void GenerateDefaultLevels()
    {
        levels.Clear();
        
        for (int i = 1; i <= maxLevels; i++)
        {
            LevelData level = new LevelData();
            level.levelNumber = i;
            level.levelName = $"Maze Level {i}";
            level.levelDescription = $"Navigate through the maze and avoid the spiders! Level {i}";
            
            // Progressive difficulty
            level.mazeWidth = 25 + (i * 2);
            level.mazeHeight = 25 + (i * 2);
            level.spiderCount = 2 + (i / 2);
            level.powerUpCount = 3 + (i / 3);
            level.levelTime = 300f - (i * 20f); // Decrease time each level
            level.spiderSpeed = 1.5f + (i * 0.2f);
            level.spiderChaseRadius = 4f + (i * 0.3f);
            level.gladeSize = Mathf.Max(8, 12 - (i / 2));
            level.fogRevealRadius = Mathf.Max(3f, 6f - (i * 0.2f));
            
            // Special level modifications
            if (i % 3 == 0) // Every 3rd level
            {
                level.enableFogOfWar = false; // Disable fog for easier levels
            }
            
            if (i >= 5) // Level 5+
            {
                level.spiderCount += 1; // Extra spider
            }
            
            if (i >= 8) // Level 8+
            {
                level.levelTime -= 30f; // Less time
                level.fogRevealRadius -= 1f; // Less visibility
            }
            
            levels.Add(level);
        }
        
        Debug.Log($"Generated {levels.Count} levels");
    }
    
    public void LoadCurrentLevel()
    {
        if (currentLevelIndex >= levels.Count)
        {
            Debug.LogWarning("No more levels available!");
            return;
        }
        
        LevelData currentLevel = levels[currentLevelIndex];
        ApplyLevelSettings(currentLevel);
        
        Debug.Log($"Loaded level {currentLevel.levelNumber}: {currentLevel.levelName}");
    }
    
    void ApplyLevelSettings(LevelData level)
    {
        if (mazeGenerator != null)
        {
            mazeGenerator.mazeWidth = level.mazeWidth;
            mazeGenerator.mazeHeight = level.mazeHeight;
            mazeGenerator.spiderCount = level.spiderCount;
            mazeGenerator.powerUpCount = level.powerUpCount;
            mazeGenerator.gladeSize = level.gladeSize;
            mazeGenerator.enableFogOfWar = level.enableFogOfWar;
            mazeGenerator.fogRevealRadius = level.fogRevealRadius;
        }
        
        if (gameManager != null)
        {
            gameManager.levelTime = level.levelTime;
        }
        
        // Update spider AI settings
        UpdateSpiderSettings(level);
    }
    
    void UpdateSpiderSettings(LevelData level)
    {
        SpiderAI[] spiders = FindObjectsOfType<SpiderAI>();
        foreach (SpiderAI spider in spiders)
        {
            spider.moveSpeed = level.spiderSpeed;
            spider.chaseRadius = level.spiderChaseRadius;
        }
        
        AdvancedSpiderAI[] advancedSpiders = FindObjectsOfType<AdvancedSpiderAI>();
        foreach (AdvancedSpiderAI spider in advancedSpiders)
        {
            spider.moveSpeed = level.spiderSpeed;
            spider.chaseRadius = level.spiderChaseRadius;
            spider.detectionRadius = level.spiderChaseRadius + 2f;
        }
    }
    
    public void CompleteCurrentLevel()
    {
        LevelData completedLevel = GetCurrentLevel();
        Debug.Log($"Level {completedLevel.levelNumber} completed!");
        
        // Save progress
        if (playerProgress != null)
        {
            playerProgress.CompleteLevel(currentLevelIndex);
        }
        
        // Check if player can advance
        if (CanAdvanceToNextLevel())
        {
            AdvanceToNextLevel();
        }
        else
        {
            Debug.Log("Not enough score to advance to next level!");
            ShowLevelCompleteScreen();
        }
    }
    
    public void AdvanceToNextLevel()
    {
        currentLevelIndex++;
        
        if (currentLevelIndex < levels.Count)
        {
            LoadCurrentLevel();
            
            // Regenerate maze for new level
            if (mazeGenerator != null)
            {
                mazeGenerator.RegenerateMaze();
            }
            
            Debug.Log($"Advanced to level {GetCurrentLevel().levelNumber}");
        }
        else
        {
            Debug.Log("All levels completed! Game finished!");
            ShowGameCompleteScreen();
        }
    }
    
    bool CanAdvanceToNextLevel()
    {
        if (unlockAllLevels) return true;
        
        if (gameManager != null)
        {
            return gameManager.GetScore() >= scoreRequiredForNextLevel;
        }
        
        return true;
    }
    
    public LevelData GetCurrentLevel()
    {
        if (currentLevelIndex < levels.Count)
        {
            return levels[currentLevelIndex];
        }
        
        return levels[levels.Count - 1]; // Return last level if out of bounds
    }
    
    public LevelData GetLevel(int index)
    {
        if (index >= 0 && index < levels.Count)
        {
            return levels[index];
        }
        
        return null;
    }
    
    public bool IsLevelUnlocked(int levelIndex)
    {
        if (unlockAllLevels) return true;
        
        if (playerProgress != null)
        {
            return playerProgress.IsLevelUnlocked(levelIndex);
        }
        
        return levelIndex <= currentLevelIndex;
    }
    
    public void UnlockLevel(int levelIndex)
    {
        if (playerProgress != null)
        {
            playerProgress.UnlockLevel(levelIndex);
        }
    }
    
    public void ResetProgress()
    {
        currentLevelIndex = 0;
        
        if (playerProgress != null)
        {
            playerProgress.ResetProgress();
        }
        
        LoadCurrentLevel();
        Debug.Log("Progress reset to level 1");
    }
    
    void ShowLevelCompleteScreen()
    {
        // This would show a level complete UI
        Debug.Log("Level Complete! Press any key to continue...");
    }
    
    void ShowGameCompleteScreen()
    {
        // This would show a game complete UI
        Debug.Log("Congratulations! You've completed all levels!");
    }
    
    public int GetTotalLevels()
    {
        return levels.Count;
    }
    
    public int GetCurrentLevelIndex()
    {
        return currentLevelIndex;
    }
    
    public float GetLevelProgress()
    {
        return (float)currentLevelIndex / levels.Count;
    }
}
