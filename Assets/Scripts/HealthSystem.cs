using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HealthSystem : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;
    public float invulnerabilityTime = 1f;
    
    [Header("UI References")]
    public Slider healthBar;
    public Text healthText;
    public GameObject gameOverPanel;
    public Text finalScoreText;
    public Button restartButton;
    public Button mainMenuButton;
    
    [Header("Visual Feedback")]
    public Color normalColor = Color.white;
    public Color damageColor = Color.red;
    public float damageFlashTime = 0.2f;
    
    // Event callback for health changes
    public delegate void OnHealthChanged(int currentHealth, int maxHealth);
    public event OnHealthChanged onHealthChangedCallback;
    
    private bool isInvulnerable = false;
    private SpriteRenderer spriteRenderer;
    private GameManager gameManager;
    private bool isDead = false;
    
    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        gameManager = FindObjectOfType<GameManager>();
        
        // Setup UI
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
        
        UpdateHealthUI();
        
        // Setup game over panel buttons
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
            if (restartButton != null)
                restartButton.onClick.AddListener(RestartGame);
            if (mainMenuButton != null)
                mainMenuButton.onClick.AddListener(GoToMainMenu);
        }
        
        Debug.Log("Health system initialized");
    }
    
    public void TakeDamage(int damage)
    {
        if (isInvulnerable || isDead) return;
        
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);
        
        Debug.Log($"Player took {damage} damage. Health: {currentHealth}/{maxHealth}");
        
        // Visual feedback
        StartCoroutine(DamageFlash());
        
        // Invulnerability period
        StartCoroutine(InvulnerabilityPeriod());
        
        UpdateHealthUI();
        
        // Invoke health changed event
        onHealthChangedCallback?.Invoke(currentHealth, maxHealth);
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    public void Heal(int healAmount)
    {
        if (isDead) return;
        
        currentHealth += healAmount;
        currentHealth = Mathf.Min(maxHealth, currentHealth);
        
        Debug.Log($"Player healed {healAmount}. Health: {currentHealth}/{maxHealth}");
        
        UpdateHealthUI();
        
        // Invoke health changed event
        onHealthChangedCallback?.Invoke(currentHealth, maxHealth);
    }
    
    void UpdateHealthUI()
    {
        if (healthBar != null)
        {
            healthBar.value = currentHealth;
        }
        
        if (healthText != null)
        {
            healthText.text = $"{currentHealth}/{maxHealth}";
        }
    }
    
    void Die()
    {
        isDead = true;
        Debug.Log("Player died!");
        
        // Stop player movement
        PlayerMovement playerMovement = GetComponent<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }
        
        // Stop camera following
        CameraFollow cameraFollow = FindObjectOfType<CameraFollow>();
        if (cameraFollow != null)
        {
            cameraFollow.enabled = false;
        }
        
        // Show game over screen through BeautifulMenuSystem
        BeautifulMenuSystem menuSystem = FindObjectOfType<BeautifulMenuSystem>();
        if (menuSystem != null && gameManager != null)
        {
            menuSystem.ShowGameOver(gameManager.GetScore(), gameManager.GetGameTime());
        }
        else
        {
            ShowGameOverScreen();
        }
        
        // Notify game manager
        if (gameManager != null)
        {
            gameManager.HandlePlayerDeath();
            gameManager.OnPlayerDeath?.Invoke();
        }
    }
    
    void ShowGameOverScreen()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            
            // Display final score
            if (finalScoreText != null && gameManager != null)
            {
                finalScoreText.text = $"Final Score: {gameManager.GetScore()}";
            }
        }
    }
    
    System.Collections.IEnumerator DamageFlash()
    {
        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;
            spriteRenderer.color = damageColor;
            yield return new WaitForSeconds(damageFlashTime);
            spriteRenderer.color = originalColor;
        }
    }
    
    System.Collections.IEnumerator InvulnerabilityPeriod()
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(invulnerabilityTime);
        isInvulnerable = false;
    }
    
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu"); // We'll create this later
    }
    
    public bool IsDead()
    {
        return isDead;
    }
    
    public bool IsInvulnerable()
    {
        return isInvulnerable;
    }
    
    public float GetHealthPercentage()
    {
        return (float)currentHealth / maxHealth;
    }
}
