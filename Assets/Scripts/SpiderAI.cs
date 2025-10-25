using UnityEngine;

public class SpiderAI : MonoBehaviour
{
    [Header("Spider Settings")]
    public float moveSpeed = 3f; // Faster chase!
    public float chaseRadius = 8f; // Larger detection range!
    public float patrolSpeed = 1f;
    public int damageAmount = 25;
    
    [Header("Visual Feedback")]
    public Color normalColor = Color.white;
    public Color chaseColor = Color.red;
    public float colorChangeSpeed = 5f;

    private Transform player;
    private Rigidbody2D rb;
    private Vector2 patrolPoint;
    private SpriteRenderer spriteRenderer;
    private bool ignorePlayer = false;
    private bool repellentMode = false;
    private float chaseTimer = 0f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        SetRandomPatrolPoint();

        Debug.Log("Spider spawned and ready!");
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (repellentMode)
        {
            RunAwayFromPlayer();
            UpdateVisuals(false);
        }
        else if (!ignorePlayer && distanceToPlayer <= chaseRadius)
        {
            chaseTimer = 2f; // Keep chasing for 2 seconds even if player escapes
            ChasePlayer();
            UpdateVisuals(true);
            
            // Audio feedback
            if (chaseTimer >= 1.9f) // Just started chasing
            {
                Debug.Log($"🕷️ SPIDER CHASING! Distance: {distanceToPlayer:F1}");
            }
        }
        else if (chaseTimer > 0f)
        {
            // Continue chasing for a bit even after player leaves radius
            chaseTimer -= Time.deltaTime;
            ChasePlayer();
            UpdateVisuals(true);
        }
        else
        {
            Patrol();
            UpdateVisuals(false);
        }
    }
    
    void UpdateVisuals(bool chasing)
    {
        if (spriteRenderer == null) return;
        
        Color targetColor = chasing ? chaseColor : normalColor;
        spriteRenderer.color = Color.Lerp(spriteRenderer.color, targetColor, Time.deltaTime * colorChangeSpeed);
        
        // Scale up when chasing
        float targetScale = chasing ? 1.2f : 1.0f;
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * targetScale, Time.deltaTime * 5f);
    }

    void ChasePlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        
        // Faster chase speed when player is close
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        float speedMultiplier = distanceToPlayer < 3f ? 1.5f : 1f; // Sprint when very close!
        
        rb.linearVelocity = direction * moveSpeed * speedMultiplier;

        // Flip spider to face player
        if (spriteRenderer != null)
        {
            if (direction.x > 0)
                spriteRenderer.flipX = false;
            else if (direction.x < 0)
                spriteRenderer.flipX = true;
        }
    }

    void Patrol()
    {
        // Move toward patrol point
        Vector2 direction = (patrolPoint - (Vector2)transform.position).normalized;
        rb.linearVelocity = direction * patrolSpeed;

        // If reached patrol point or stuck, get new one
        if (Vector2.Distance(transform.position, patrolPoint) < 0.5f || rb.linearVelocity.magnitude < 0.1f)
        {
            SetRandomPatrolPoint();
        }
    }

    void SetRandomPatrolPoint()
    {
        // Patrol within maze bounds
        patrolPoint = new Vector2(Random.Range(-10f, 10f), Random.Range(-10f, 10f));
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("🕷️ SPIDER CAUGHT THE PLAYER!");
            
            // Damage the player
            HealthSystem playerHealth = collision.gameObject.GetComponent<HealthSystem>();
            if (playerHealth != null && !playerHealth.IsInvulnerable())
            {
                playerHealth.TakeDamage(damageAmount);
                
                // Visual feedback - make spider flash
                if (spriteRenderer != null)
                {
                    StartCoroutine(FlashOnHit());
                }
                
                // Push player back slightly
                Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    Vector2 pushDirection = (collision.transform.position - transform.position).normalized;
                    playerRb.AddForce(pushDirection * 5f, ForceMode2D.Impulse);
                }
            }
        }
    }
    
    System.Collections.IEnumerator FlashOnHit()
    {
        Color original = spriteRenderer.color;
        spriteRenderer.color = Color.yellow;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = original;
    }

    void OnDrawGizmosSelected()
    {
        // Visualize chase radius in Scene view
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRadius);
    }
    
    void RunAwayFromPlayer()
    {
        Vector2 direction = (transform.position - player.position).normalized;
        rb.linearVelocity = direction * moveSpeed * 1.5f; // Run faster when scared
        
        // Flip spider to face away from player
        if (direction.x > 0)
            spriteRenderer.flipX = true;
        else if (direction.x < 0)
            spriteRenderer.flipX = false;
    }
    
    public void SetIgnorePlayer(bool ignore)
    {
        ignorePlayer = ignore;
        Debug.Log($"Spider ignore player: {ignore}");
    }
    
    public void SetRepellentMode(bool repellent)
    {
        repellentMode = repellent;
        Debug.Log($"Spider repellent mode: {repellent}");
    }
}