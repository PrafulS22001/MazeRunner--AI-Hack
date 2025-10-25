using UnityEngine;
using System.Collections.Generic;
using System;

[System.Serializable]
public class PlayerProgressData
{
    public int highestLevelCompleted = 0;
    public int totalScore = 0;
    public int totalPlayTime = 0;
    public List<int> unlockedLevels = new List<int>();
    public List<int> levelScores = new List<int>();
    public List<float> levelTimes = new List<float>();
    public int spidersAvoided = 0;
    public int powerUpsCollected = 0;
    public int deaths = 0;
    public DateTime lastPlayed;
    
    public PlayerProgressData()
    {
        unlockedLevels.Add(0); // Level 1 is always unlocked
        lastPlayed = DateTime.Now;
    }
}

public class PlayerProgress : MonoBehaviour
{
    [Header("Progress Settings")]
    public string saveFileName = "PlayerProgress.json";
    
    private PlayerProgressData progressData;
    private static PlayerProgress instance;
    
    public static PlayerProgress Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<PlayerProgress>();
                if (instance == null)
                {
                    GameObject progressManager = new GameObject("PlayerProgress");
                    instance = progressManager.AddComponent<PlayerProgress>();
                }
            }
            return instance;
        }
    }
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadProgress();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        // Ensure level 1 is always unlocked
        if (!progressData.unlockedLevels.Contains(0))
        {
            progressData.unlockedLevels.Add(0);
        }
    }
    
    void LoadProgress()
    {
        string filePath = System.IO.Path.Combine(Application.persistentDataPath, saveFileName);
        
        if (System.IO.File.Exists(filePath))
        {
            try
            {
                string json = System.IO.File.ReadAllText(filePath);
                progressData = JsonUtility.FromJson<PlayerProgressData>(json);
                Debug.Log("Player progress loaded successfully");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load progress: {e.Message}");
                CreateNewProgress();
            }
        }
        else
        {
            CreateNewProgress();
        }
    }
    
    void SaveProgress()
    {
        string filePath = System.IO.Path.Combine(Application.persistentDataPath, saveFileName);
        
        try
        {
            progressData.lastPlayed = DateTime.Now;
            string json = JsonUtility.ToJson(progressData, true);
            System.IO.File.WriteAllText(filePath, json);
            Debug.Log("Player progress saved successfully");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save progress: {e.Message}");
        }
    }
    
    void CreateNewProgress()
    {
        progressData = new PlayerProgressData();
        Debug.Log("Created new player progress");
    }
    
    public void CompleteLevel(int levelIndex)
    {
        if (levelIndex > progressData.highestLevelCompleted)
        {
            progressData.highestLevelCompleted = levelIndex;
        }
        
        // Unlock next level
        if (!progressData.unlockedLevels.Contains(levelIndex + 1))
        {
            progressData.unlockedLevels.Add(levelIndex + 1);
        }
        
        // Record level score and time
        GameManager gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager != null)
        {
            // Ensure we have enough entries in the lists
            while (progressData.levelScores.Count <= levelIndex)
            {
                progressData.levelScores.Add(0);
            }
            while (progressData.levelTimes.Count <= levelIndex)
            {
                progressData.levelTimes.Add(0f);
            }
            
            // Update with current score/time if better
            int currentScore = gameManager.GetScore();
            float currentTime = gameManager.GetGameTime();
            
            if (currentScore > progressData.levelScores[levelIndex])
            {
                progressData.levelScores[levelIndex] = currentScore;
            }
            
            if (currentTime > progressData.levelTimes[levelIndex])
            {
                progressData.levelTimes[levelIndex] = currentTime;
            }
            
            progressData.totalScore += currentScore;
            progressData.totalPlayTime += Mathf.RoundToInt(currentTime);
        }
        
        SaveProgress();
        Debug.Log($"Level {levelIndex + 1} completed!");
    }
    
    public void UnlockLevel(int levelIndex)
    {
        if (!progressData.unlockedLevels.Contains(levelIndex))
        {
            progressData.unlockedLevels.Add(levelIndex);
            SaveProgress();
            Debug.Log($"Level {levelIndex + 1} unlocked!");
        }
    }
    
    public bool IsLevelUnlocked(int levelIndex)
    {
        return progressData.unlockedLevels.Contains(levelIndex);
    }
    
    public void RecordSpiderAvoided()
    {
        progressData.spidersAvoided++;
        SaveProgress();
    }
    
    public void RecordPowerUpCollected()
    {
        progressData.powerUpsCollected++;
        SaveProgress();
    }
    
    public void RecordDeath()
    {
        progressData.deaths++;
        SaveProgress();
    }
    
    public void ResetProgress()
    {
        progressData = new PlayerProgressData();
        SaveProgress();
        Debug.Log("Player progress reset");
    }
    
    // Getters for UI and other systems
    public int GetHighestLevelCompleted()
    {
        return progressData.highestLevelCompleted;
    }
    
    public int GetTotalScore()
    {
        return progressData.totalScore;
    }
    
    public int GetTotalPlayTime()
    {
        return progressData.totalPlayTime;
    }
    
    public int GetSpidersAvoided()
    {
        return progressData.spidersAvoided;
    }
    
    public int GetPowerUpsCollected()
    {
        return progressData.powerUpsCollected;
    }
    
    public int GetDeaths()
    {
        return progressData.deaths;
    }
    
    public int GetLevelScore(int levelIndex)
    {
        if (levelIndex < progressData.levelScores.Count)
        {
            return progressData.levelScores[levelIndex];
        }
        return 0;
    }
    
    public float GetLevelTime(int levelIndex)
    {
        if (levelIndex < progressData.levelTimes.Count)
        {
            return progressData.levelTimes[levelIndex];
        }
        return 0f;
    }
    
    public List<int> GetUnlockedLevels()
    {
        return new List<int>(progressData.unlockedLevels);
    }
    
    public DateTime GetLastPlayed()
    {
        return progressData.lastPlayed;
    }
    
    public float GetCompletionPercentage()
    {
        // Simple completion percentage based on highest level
        // Assuming 10 levels for now (can be adjusted)
        int totalLevels = 10;
        return (float)progressData.highestLevelCompleted / totalLevels;
    }
    
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveProgress();
        }
    }
    
    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            SaveProgress();
        }
    }
    
    void OnDestroy()
    {
        SaveProgress();
    }
}

