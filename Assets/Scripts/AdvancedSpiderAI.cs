using UnityEngine;
using System.Collections.Generic;

public class AdvancedSpiderAI : MonoBehaviour
{
    [Header("Spider Settings")]
    public float moveSpeed = 2f;
    public float chaseRadius = 5f;
    public float patrolSpeed = 1f;
    public float detectionRadius = 8f;
    
    [Header("Pathfinding")]
    public float pathfindingUpdateInterval = 0.5f;
    public LayerMask obstacleLayer = 1; // Default layer
    
    [Header("Behavior States")]
    public SpiderState currentState = SpiderState.Patrol;
    
    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Vector2 patrolPoint;
    private List<Vector2> currentPath = new List<Vector2>();
    private int currentPathIndex = 0;
    private float lastPathUpdate = 0f;
    private bool ignorePlayer = false;
    private bool repellentMode = false;
    private Vector2 lastKnownPlayerPosition;
    private float lastSeenPlayerTime = 0f;
    private float searchDuration = 3f;
    
    public enum SpiderState
    {
        Patrol,
        Chase,
        Search,
        Flee
    }
    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        
        SetRandomPatrolPoint();
        SetState(SpiderState.Patrol);
        
        Debug.Log("Advanced Spider AI initialized");
    }
    
    void Update()
    {
        if (player == null) return;
        
        UpdateState();
        ExecuteCurrentState();
        UpdateVisuals();
    }
    
    void UpdateState()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        bool canSeePlayer = CanSeePlayer();
        
        switch (currentState)
        {
            case SpiderState.Patrol:
                if (repellentMode)
                {
                    SetState(SpiderState.Flee);
                }
                else if (!ignorePlayer && canSeePlayer && distanceToPlayer <= detectionRadius)
                {
                    SetState(SpiderState.Chase);
                    lastKnownPlayerPosition = player.position;
                }
                break;
                
            case SpiderState.Chase:
                if (repellentMode)
                {
                    SetState(SpiderState.Flee);
                }
                else if (canSeePlayer)
                {
                    lastKnownPlayerPosition = player.position;
                    lastSeenPlayerTime = Time.time;
                }
                else if (Time.time - lastSeenPlayerTime > 1f)
                {
                    SetState(SpiderState.Search);
                }
                break;
                
            case SpiderState.Search:
                if (repellentMode)
                {
                    SetState(SpiderState.Flee);
                }
                else if (canSeePlayer)
                {
                    SetState(SpiderState.Chase);
                    lastKnownPlayerPosition = player.position;
                }
                else if (Time.time - lastSeenPlayerTime > searchDuration)
                {
                    SetState(SpiderState.Patrol);
                }
                break;
                
            case SpiderState.Flee:
                if (!repellentMode)
                {
                    SetState(SpiderState.Patrol);
                }
                break;
        }
    }
    
    void ExecuteCurrentState()
    {
        switch (currentState)
        {
            case SpiderState.Patrol:
                ExecutePatrol();
                break;
            case SpiderState.Chase:
                ExecuteChase();
                break;
            case SpiderState.Search:
                ExecuteSearch();
                break;
            case SpiderState.Flee:
                ExecuteFlee();
                break;
        }
    }
    
    void ExecutePatrol()
    {
        if (Vector2.Distance(transform.position, patrolPoint) < 0.5f || rb.linearVelocity.magnitude < 0.1f)
        {
            SetRandomPatrolPoint();
        }
        
        MoveTowardsTarget(patrolPoint, patrolSpeed);
    }
    
    void ExecuteChase()
    {
        if (Time.time - lastPathUpdate > pathfindingUpdateInterval)
        {
            UpdatePathToTarget(player.position);
            lastPathUpdate = Time.time;
        }
        
        FollowPath(moveSpeed);
    }
    
    void ExecuteSearch()
    {
        if (Time.time - lastPathUpdate > pathfindingUpdateInterval)
        {
            UpdatePathToTarget(lastKnownPlayerPosition);
            lastPathUpdate = Time.time;
        }
        
        FollowPath(patrolSpeed);
    }
    
    void ExecuteFlee()
    {
        Vector2 fleeDirection = (transform.position - (Vector3)player.position).normalized;
        rb.linearVelocity = fleeDirection * moveSpeed * 1.5f;
    }
    
    void MoveTowardsTarget(Vector2 target, float speed)
    {
        Vector2 direction = (target - (Vector2)transform.position).normalized;
        rb.linearVelocity = direction * speed;
    }
    
    void FollowPath(float speed)
    {
        if (currentPath.Count == 0) return;
        
        Vector2 targetPoint = currentPath[currentPathIndex];
        Vector2 direction = (targetPoint - (Vector2)transform.position).normalized;
        
        rb.linearVelocity = direction * speed;
        
        if (Vector2.Distance(transform.position, targetPoint) < 0.3f)
        {
            currentPathIndex++;
            if (currentPathIndex >= currentPath.Count)
            {
                currentPath.Clear();
                currentPathIndex = 0;
            }
        }
    }
    
    void UpdatePathToTarget(Vector2 target)
    {
        currentPath = FindPath(transform.position, target);
        currentPathIndex = 0;
    }
    
    List<Vector2> FindPath(Vector2 start, Vector2 end)
    {
        // Simple pathfinding - move towards target while avoiding walls
        List<Vector2> path = new List<Vector2>();
        
        Vector2 current = start;
        Vector2 direction = (end - start).normalized;
        
        // Add intermediate waypoints to avoid obstacles
        float distance = Vector2.Distance(start, end);
        int steps = Mathf.CeilToInt(distance / 1f); // Check every 1 unit
        
        for (int i = 0; i <= steps; i++)
        {
            Vector2 waypoint = Vector2.Lerp(start, end, (float)i / steps);
            
            // Check if waypoint is clear
            if (IsPositionClear(waypoint))
            {
                path.Add(waypoint);
            }
            else
            {
                // Try to find alternative path around obstacle
                Vector2 alternative = FindAlternativePath(current, waypoint);
                if (alternative != Vector2.zero)
                {
                    path.Add(alternative);
                    current = alternative;
                }
            }
        }
        
        path.Add(end);
        return path;
    }
    
    Vector2 FindAlternativePath(Vector2 start, Vector2 target)
    {
        Vector2 direction = (target - start).normalized;
        
        // Try perpendicular directions
        Vector2[] alternatives = {
            new Vector2(-direction.y, direction.x), // Left
            new Vector2(direction.y, -direction.x), // Right
            new Vector2(-direction.x, -direction.y), // Back
        };
        
        foreach (Vector2 alt in alternatives)
        {
            Vector2 testPos = start + alt * 1f;
            if (IsPositionClear(testPos))
            {
                return testPos;
            }
        }
        
        return Vector2.zero;
    }
    
    bool IsPositionClear(Vector2 position)
    {
        Collider2D hit = Physics2D.OverlapCircle(position, 0.3f, obstacleLayer);
        return hit == null || hit.CompareTag("Player");
    }
    
    bool CanSeePlayer()
    {
        if (player == null) return false;
        
        Vector2 direction = (player.position - transform.position).normalized;
        float distance = Vector2.Distance(transform.position, player.position);
        
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance, obstacleLayer);
        
        return hit.collider == null || hit.collider.CompareTag("Player");
    }
    
    void SetRandomPatrolPoint()
    {
        // Generate random patrol point within reasonable bounds
        patrolPoint = new Vector2(
            Random.Range(-12f, 12f),
            Random.Range(-12f, 12f)
        );
    }
    
    void SetState(SpiderState newState)
    {
        if (currentState != newState)
        {
            currentState = newState;
            Debug.Log($"Spider state changed to: {newState}");
        }
    }
    
    void UpdateVisuals()
    {
        if (spriteRenderer == null) return;
        
        // Flip sprite based on movement direction
        if (rb.linearVelocity.x > 0.1f)
            spriteRenderer.flipX = false;
        else if (rb.linearVelocity.x < -0.1f)
            spriteRenderer.flipX = true;
        
        // Color based on state
        switch (currentState)
        {
            case SpiderState.Patrol:
                spriteRenderer.color = Color.white;
                break;
            case SpiderState.Chase:
                spriteRenderer.color = Color.red;
                break;
            case SpiderState.Search:
                spriteRenderer.color = Color.yellow;
                break;
            case SpiderState.Flee:
                spriteRenderer.color = Color.blue;
                break;
        }
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Spider caught the player!");
            
            HealthSystem playerHealth = collision.gameObject.GetComponent<HealthSystem>();
            if (playerHealth != null && !playerHealth.IsInvulnerable())
            {
                playerHealth.TakeDamage(25);
                
                GameManager gameManager = FindObjectOfType<GameManager>();
                if (gameManager != null)
                {
                    gameManager.OnSpiderAvoided();
                }
            }
        }
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
    
    void OnDrawGizmosSelected()
    {
        // Draw detection radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        
        // Draw chase radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRadius);
        
        // Draw current path
        Gizmos.color = Color.green;
        for (int i = 0; i < currentPath.Count - 1; i++)
        {
            Gizmos.DrawLine(currentPath[i], currentPath[i + 1]);
        }
        
        // Draw patrol point
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(patrolPoint, 0.5f);
    }
}
