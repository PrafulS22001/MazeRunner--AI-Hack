using UnityEngine;

public class SpiderAI : MonoBehaviour
{
    [Header("Spider Settings")]
    public float moveSpeed = 2f;
    public float chaseRadius = 5f;
    public float patrolSpeed = 1f;

    private Transform player;
    private Rigidbody2D rb;
    private Vector2 patrolPoint;
    private bool isChasing = false;
    private SpriteRenderer spriteRenderer;
    private bool ignorePlayer = false;
    private bool repellentMode = false;

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
        }
        else if (!ignorePlayer && distanceToPlayer <= chaseRadius)
        {
            isChasing = true;
            ChasePlayer();
        }
        else
        {
            isChasing = false;
            Patrol();
        }
    }

    void ChasePlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed;

        // Flip spider to face player
        if (direction.x > 0)
            spriteRenderer.flipX = false;
        else if (direction.x < 0)
            spriteRenderer.flipX = true;
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
            Debug.Log("Spider caught the player!");
            
            // Damage the player
            HealthSystem playerHealth = collision.gameObject.GetComponent<HealthSystem>();
            if (playerHealth != null && !playerHealth.IsInvulnerable())
            {
                playerHealth.TakeDamage(25); // Spider deals 25 damage
                
                // Award points for avoiding spider (if they survive)
                GameManager gameManager = FindObjectOfType<GameManager>();
                if (gameManager != null)
                {
                    gameManager.OnSpiderAvoided();
                }
            }
        }
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