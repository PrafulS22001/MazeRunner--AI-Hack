using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [Header("Power-Up Settings")]
    public PowerUpType type; // Changed from powerUpType to match usage
    public float duration = 10f;
    public int healAmount = 25;
    public float speedMultiplier = 1.5f;
    public float invisibilityDuration = 5f;
    
    [Header("Visual Effects")]
    public GameObject collectEffect;
    public AudioClip collectSound;
    
    private bool isCollected = false;
    
    public enum PowerUpType
    {
        HealthPack,
        SpeedBoost,
        Invisibility,
        SpiderRepellent,
        ScoreMultiplier
    }
    
    void Start()
    {
        // Ensure collider exists and is trigger
        CircleCollider2D collider = GetComponent<CircleCollider2D>();
        if (collider == null)
        {
            collider = gameObject.AddComponent<CircleCollider2D>();
        }
        collider.isTrigger = true;
        collider.radius = 0.5f;
        
        // Ensure tag is set
        if (!gameObject.CompareTag("PowerUp"))
        {
            try { gameObject.tag = "PowerUp"; }
            catch { Debug.LogWarning("PowerUp tag not defined!"); }
        }
        
        // Add new effects system
        if (GetComponent<PowerUpEffects>() == null)
        {
            gameObject.AddComponent<PowerUpEffects>();
        }
        
        // Setup visual based on type
        SetupVisuals();
        
        Debug.Log($"✅ PowerUp {type} ready at {transform.position}");
    }
    
    void SetupVisuals()
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        if (renderer == null) return;
        
        switch (type)
        {
            case PowerUpType.HealthPack:
                renderer.color = Color.green;
                break;
            case PowerUpType.SpeedBoost:
                renderer.color = Color.yellow;
                break;
            case PowerUpType.Invisibility:
                renderer.color = Color.cyan;
                break;
            case PowerUpType.SpiderRepellent:
                renderer.color = Color.red;
                break;
            case PowerUpType.ScoreMultiplier:
                renderer.color = Color.magenta;
                break;
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isCollected)
        {
            CollectPowerUp(other.gameObject);
        }
    }
    
    void CollectPowerUp(GameObject player)
    {
        isCollected = true;
        
        Debug.Log($"Player collected {type} power-up!");
        
        // Apply power-up effect
        ApplyPowerUpEffect(player);
        
        // Trigger beautiful animation
        PowerUpEffects effects = GetComponent<PowerUpEffects>();
        if (effects != null)
        {
            effects.OnCollected(player);
        }
        else
        {
            // Fallback: old effects
            PlayCollectEffects();
            Destroy(gameObject);
        }
        
        // Award points
        GameManager gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager != null)
        {
            gameManager.AddScore(100); // Base points for collecting power-up
        }
    }
    
    void ApplyPowerUpEffect(GameObject player)
    {
        switch (type)
        {
            case PowerUpType.HealthPack:
                HealthSystem health = player.GetComponent<HealthSystem>();
                if (health != null)
                {
                    health.Heal(healAmount);
                }
                break;
                
            case PowerUpType.SpeedBoost:
                PlayerMovement movement = player.GetComponent<PlayerMovement>();
                if (movement != null)
                {
                    StartCoroutine(SpeedBoostEffect(movement));
                }
                break;
                
            case PowerUpType.Invisibility:
                StartCoroutine(InvisibilityEffect(player));
                break;
                
            case PowerUpType.SpiderRepellent:
                StartCoroutine(SpiderRepellentEffect());
                break;
                
            case PowerUpType.ScoreMultiplier:
                GameManager gameManager = FindFirstObjectByType<GameManager>();
                if (gameManager != null)
                {
                    StartCoroutine(ScoreMultiplierEffect(gameManager));
                }
                break;
        }
    }
    
    System.Collections.IEnumerator SpeedBoostEffect(PlayerMovement movement)
    {
        float originalSpeed = movement.speed;
        movement.speed *= speedMultiplier;
        
        Debug.Log($"Speed boost activated! Speed: {movement.speed}");
        
        yield return new WaitForSeconds(duration);
        
        movement.speed = originalSpeed;
        Debug.Log("Speed boost expired");
    }
    
    System.Collections.IEnumerator InvisibilityEffect(GameObject player)
    {
        SpriteRenderer renderer = player.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            Color originalColor = renderer.color;
            renderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.3f);
            
            // Make spiders ignore player
            SpiderAI[] spiders = FindObjectsByType<SpiderAI>(FindObjectsSortMode.None);
            foreach (SpiderAI spider in spiders)
            {
                spider.SetIgnorePlayer(true);
            }
            
            Debug.Log("Invisibility activated!");
            
            yield return new WaitForSeconds(invisibilityDuration);
            
            // Restore visibility
            renderer.color = originalColor;
            
            // Restore spider behavior
            foreach (SpiderAI spider in spiders)
            {
                spider.SetIgnorePlayer(false);
            }
            
            Debug.Log("Invisibility expired");
        }
    }
    
    System.Collections.IEnumerator SpiderRepellentEffect()
    {
        SpiderAI[] spiders = FindObjectsByType<SpiderAI>(FindObjectsSortMode.None);
        
        // Make spiders run away from player
        foreach (SpiderAI spider in spiders)
        {
            spider.SetRepellentMode(true);
        }
        
        Debug.Log("Spider repellent activated!");
        
        yield return new WaitForSeconds(duration);
        
        // Restore normal spider behavior
        foreach (SpiderAI spider in spiders)
        {
            spider.SetRepellentMode(false);
        }
        
        Debug.Log("Spider repellent expired");
    }
    
    System.Collections.IEnumerator ScoreMultiplierEffect(GameManager gameManager)
    {
        // This would require modifying GameManager to support score multipliers
        // For now, just award bonus points
        gameManager.AddScore(500);
        Debug.Log("Score multiplier activated!");
        
        yield return new WaitForSeconds(duration);
        Debug.Log("Score multiplier expired");
    }
    
    void PlayCollectEffects()
    {
        // Play particle effect
        if (collectEffect != null)
        {
            Instantiate(collectEffect, transform.position, Quaternion.identity);
        }
        
        // Play sound
        if (collectSound != null)
        {
            AudioSource.PlayClipAtPoint(collectSound, transform.position);
        }
    }
}


