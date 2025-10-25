using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    public int score = 0;
    public int survivalBonus = 10; // Points per second survived
    public int exitBonus = 1000; // Points for reaching exit
    public int spiderAvoidBonus = 50; // Points for avoiding spiders
    
    [Header("UI References")]
    public Text scoreText;
    public Text timeText;
    public Text levelText;
    public GameObject pausePanel;
    public Button pauseButton;
    public Button resumeButton;
    public Button settingsButton;
    
    [Header("Level Settings")]
    public int currentLevel = 1;
    public float levelTime = 300f; // 5 minutes per level
    public bool isPaused = false;
    
    private float gameTime = 0f;
    private float survivalTimer = 0f;
    private bool gameStarted = false;
    private HealthSystem playerHealth;
    private MazeGenerator mazeGenerator;
    
    // Events
    public System.Action<int> OnScoreChanged;
    public System.Action<int> OnLevelChanged;
    public System.Action OnGamePaused;
    public System.Action OnGameResumed;
    public System.Action OnPlayerDeath;
    public System.Action OnPlayerReachedExit;
    
    void Start()
    {
        // Find components
        playerHealth = FindObjectOfType<HealthSystem>();
        mazeGenerator = FindObjectOfType<MazeGenerator>();
        
        // Setup UI
        SetupUI();
        
        // Start game
        StartGame();
        
        Debug.Log("GameManager initialized");
    }
    
    void Update()
    {
        if (!isPaused && gameStarted && !playerHealth.IsDead())
        {
            UpdateGameTime();
            UpdateSurvivalScore();
            CheckLevelCompletion();
            
            // Pause with Escape key
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePause();
            }
        }
    }
    
    void SetupUI()
    {
        // Setup pause panel
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
        
        // Setup buttons
        if (pauseButton != null)
        {
            pauseButton.onClick.AddListener(TogglePause);
        }
        
        if (resumeButton != null)
        {
            resumeButton.onClick.AddListener(ResumeGame);
        }
        
        if (settingsButton != null)
        {
            settingsButton.onClick.AddListener(OpenSettings);
        }
        
        UpdateUI();
    }
    
    void StartGame()
    {
        gameStarted = true;
        gameTime = 0f;
        survivalTimer = 0f;
        score = 0;
        
        UpdateUI();
        Debug.Log("Game started!");
    }
    
    void UpdateGameTime()
    {
        gameTime += Time.deltaTime;
        survivalTimer += Time.deltaTime;
        
        if (timeText != null)
        {
            int minutes = Mathf.FloorToInt(gameTime / 60f);
            int seconds = Mathf.FloorToInt(gameTime % 60f);
            timeText.text = $"Time: {minutes:00}:{seconds:00}";
        }
    }
    
    void UpdateSurvivalScore()
    {
        if (survivalTimer >= 1f) // Add points every second
        {
            AddScore(survivalBonus);
            survivalTimer = 0f;
        }
    }
    
    void CheckLevelCompletion()
    {
        if (gameTime >= levelTime)
        {
            CompleteLevel();
        }
    }
    
    public void AddScore(int points)
    {
        score += points;
        OnScoreChanged?.Invoke(score);
        UpdateUI();
        
        Debug.Log($"Score added: +{points}. Total: {score}");
    }
    
    public void HandlePlayerReachedExit()
    {
        AddScore(exitBonus);
        Debug.Log("Player reached exit! Bonus points awarded!");
    }
    
    public void OnSpiderAvoided()
    {
        AddScore(spiderAvoidBonus);
        Debug.Log("Spider avoided! Bonus points!");
    }
    
    public void CompleteLevel()
    {
        Debug.Log($"Level {currentLevel} completed!");
        
        // Calculate final score for this level
        int timeBonus = Mathf.RoundToInt((levelTime - gameTime) * 10);
        AddScore(timeBonus);
        
        // Move to next level
        currentLevel++;
        OnLevelChanged?.Invoke(currentLevel);
        
        // Regenerate maze for next level
        if (mazeGenerator != null)
        {
            mazeGenerator.RegenerateMaze();
        }
        
        // Reset timer
        gameTime = 0f;
        survivalTimer = 0f;
        
        UpdateUI();
    }
    
    public void HandlePlayerDeath()
    {
        gameStarted = false;
        Debug.Log("Game Over! Final Score: " + score);
    }
    
    public void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }
    
    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        
        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }
        
        OnGamePaused?.Invoke();
        Debug.Log("Game paused");
    }
    
    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
        
        OnGameResumed?.Invoke();
        Debug.Log("Game resumed");
    }
    
    public void OpenSettings()
    {
        Debug.Log("Settings opened");
        // We'll implement settings later
    }
    
    void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }
        
        if (levelText != null)
        {
            levelText.text = $"Level: {currentLevel}";
        }
    }
    
    public int GetScore()
    {
        return score;
    }
    
    public int GetCurrentLevel()
    {
        return currentLevel;
    }
    
    public float GetGameTime()
    {
        return gameTime;
    }
    
    public bool IsGamePaused()
    {
        return isPaused;
    }
    
    public bool IsGameStarted()
    {
        return gameStarted;
    }
}
