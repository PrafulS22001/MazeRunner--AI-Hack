using UnityEngine;

public class ExitTrigger : MonoBehaviour
{
    [Header("Exit Settings")]
    public Color exitColor = Color.green;
    public float glowIntensity = 0.5f;
    
    private GameManager gameManager;
    private SpriteRenderer spriteRenderer;
    private bool playerReachedExit = false;
    
    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Setup visual
        if (spriteRenderer != null)
        {
            spriteRenderer.color = exitColor;
        }
        
        // Add glowing effect
        if (GetComponent<ExitGlow>() == null)
        {
            gameObject.AddComponent<ExitGlow>();
        }
        
        Debug.Log("Exit trigger initialized");
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !playerReachedExit)
        {
            playerReachedExit = true;
            Debug.Log("🎉 Player reached the exit!");
            
            // Show victory screen!
            VictoryScreen.TriggerVictory();
            
            // Notify game manager
            if (gameManager != null)
            {
                gameManager.HandlePlayerReachedExit();
                gameManager.OnPlayerReachedExit?.Invoke();
            }
            
            // Visual feedback
            StartCoroutine(ExitSequence());
        }
    }
    
    System.Collections.IEnumerator ExitSequence()
    {
        // Flash effect
        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;
            
            for (int i = 0; i < 3; i++)
            {
                spriteRenderer.color = Color.white;
                yield return new WaitForSeconds(0.1f);
                spriteRenderer.color = originalColor;
                yield return new WaitForSeconds(0.1f);
            }
        }
        
        // Play exit sound (we'll add this later)
        // AudioSource.PlayClipAtPoint(exitSound, transform.position);
        
        Debug.Log("Exit sequence completed!");
    }
}

