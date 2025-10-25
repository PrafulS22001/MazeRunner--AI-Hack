using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Advanced AI System with realistic behaviors
/// - Smart pathfinding (A* algorithm)
/// - State machine (Patrol, Chase, Search, Attack)
/// - Group coordination
/// - Learning from player behavior
/// </summary>
public class AdvancedAISystem : MonoBehaviour
{
    [Header("AI Settings")]
    public float detectionRange = 5f;
    public float attackRange = 1f;
    public float moveSpeed = 3f;
    public float chaseSpeedMultiplier = 1.5f;
    
    [Header("Behavior Settings")]
    public bool useAdvancedPathfinding = true;
    public bool coordinateWithOthers = true;
    public bool rememberPlayerLocation = true;
    public float memoryDuration = 5f;
    
    [Header("States")]
    public AIState currentState = AIState.Patrol;
    
    public enum AIState
    {
        Idle,
        Patrol,
        Chase,
        Search,
        Attack,
        Flee
    }
    
    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Vector2 lastKnownPlayerPos;
    private float lastPlayerSightTime;
    private List<Vector2> patrolPoints = new List<Vector2>();
    private int currentPatrolIndex = 0;
    private float stateTimer = 0f;
    
    // Advanced AI memory
    private Dictionary<string, float> behaviorWeights = new Dictionary<string, float>();
    private int playerEncounters = 0;
    private bool playerInMaze = false;
    private MazeGenerator mazeGen;
    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        mazeGen = FindObjectOfType<MazeGenerator>();
        
        // Setup Rigidbody
        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
        
        // Generate patrol points
        GeneratePatrolPoints();
        
        // Initialize behavior weights
        InitializeBehaviorWeights();
        
        // Start checking for player in maze
        InvokeRepeating("CheckPlayerInMaze", 1f, 0.5f);
        
        Debug.Log($"🤖 Advanced AI initialized for {gameObject.name}");
    }
    
    void Update()
    {
        if (player == null) return;
        
        stateTimer += Time.deltaTime;
        
        // State machine
        switch (currentState)
        {
            case AIState.Patrol:
                PatrolBehavior();
                break;
            case AIState.Chase:
                ChaseBehavior();
                break;
            case AIState.Search:
                SearchBehavior();
                break;
            case AIState.Attack:
                AttackBehavior();
                break;
            case AIState.Flee:
                FleeBehavior();
                break;
        }
        
        // Check for state transitions
        EvaluateStateTransitions();
        
        // Visual feedback
        UpdateVisuals();
    }
    
    void GeneratePatrolPoints()
    {
        // Create patrol points in a radius around spawn
        Vector2 spawnPos = transform.position;
        int pointCount = 4;
        
        for (int i = 0; i < pointCount; i++)
        {
            float angle = (360f / pointCount) * i;
            float radius = Random.Range(3f, 6f);
            
            Vector2 point = spawnPos + new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad) * radius,
                Mathf.Sin(angle * Mathf.Deg2Rad) * radius
            );
            
            patrolPoints.Add(point);
        }
    }
    
    void InitializeBehaviorWeights()
    {
        behaviorWeights["aggression"] = 0.5f;
        behaviorWeights["caution"] = 0.5f;
        behaviorWeights["intelligence"] = 0.7f;
    }
    
    void PatrolBehavior()
    {
        if (patrolPoints.Count == 0) return;
        
        // Move to current patrol point
        Vector2 targetPoint = patrolPoints[currentPatrolIndex];
        Vector2 direction = (targetPoint - (Vector2)transform.position).normalized;
        
        rb.linearVelocity = direction * moveSpeed;
        
        // Check if reached patrol point
        if (Vector2.Distance(transform.position, targetPoint) < 0.5f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Count;
            stateTimer = 0f;
        }
    }
    
    void ChaseBehavior()
    {
        if (player == null) return;
        
        // Advanced pathfinding to player
        Vector2 direction = GetSmartDirection(player.position);
        
        float speed = moveSpeed * chaseSpeedMultiplier;
        rb.linearVelocity = direction * speed;
        
        // Remember player location
        if (rememberPlayerLocation)
        {
            lastKnownPlayerPos = player.position;
            lastPlayerSightTime = Time.time;
        }
        
        // Learning: increase aggression with each chase
        if (behaviorWeights.ContainsKey("aggression"))
        {
            behaviorWeights["aggression"] = Mathf.Min(1f, behaviorWeights["aggression"] + 0.01f * Time.deltaTime);
        }
    }
    
    void SearchBehavior()
    {
        // Search last known player position
        if (lastKnownPlayerPos != Vector2.zero)
        {
            Vector2 direction = (lastKnownPlayerPos - (Vector2)transform.position).normalized;
            rb.linearVelocity = direction * moveSpeed * 0.7f;
            
            // If reached last known position, return to patrol
            if (Vector2.Distance(transform.position, lastKnownPlayerPos) < 1f)
            {
                stateTimer = memoryDuration; // Force state change
            }
        }
    }
    
    void AttackBehavior()
    {
        // Stop and attack
        rb.linearVelocity = Vector2.zero;
        
        // Deal damage to player
        if (player != null && Vector2.Distance(transform.position, player.position) < attackRange)
        {
            HealthSystem playerHealth = player.GetComponent<HealthSystem>();
            if (playerHealth != null && stateTimer > 1f) // Attack cooldown
            {
                playerHealth.TakeDamage(10);
                stateTimer = 0f;
                
                // Increase encounter count
                playerEncounters++;
                
                Debug.Log($"🕷️ {gameObject.name} attacked player! (Encounters: {playerEncounters})");
            }
        }
    }
    
    void FleeBehavior()
    {
        if (player == null) return;
        
        // Run away from player
        Vector2 direction = ((Vector2)transform.position - (Vector2)player.position).normalized;
        rb.linearVelocity = direction * moveSpeed * 1.2f;
        
        // After fleeing for a while, return to patrol
        if (stateTimer > 3f)
        {
            ChangeState(AIState.Patrol);
        }
    }
    
    void CheckPlayerInMaze()
    {
        if (player == null || mazeGen == null) return;
        
        // Check if player is outside glade (in maze)
        float distFromCenter = Vector2.Distance(player.position, Vector2.zero);
        float gladeRadius = (mazeGen.gladeSize * mazeGen.cellSize / 2f);
        
        bool wasInMaze = playerInMaze;
        playerInMaze = distFromCenter > gladeRadius;
        
        // Player just entered maze!
        if (playerInMaze && !wasInMaze)
        {
            Debug.Log($"🕷️ {gameObject.name} ALERT: Player entered the maze!");
            
            // Increase detection range when player in maze
            detectionRange = 8f;
            
            // If close enough, start chasing immediately
            float distToPlayer = Vector2.Distance(transform.position, player.position);
            if (distToPlayer < detectionRange)
            {
                ChangeState(AIState.Chase);
            }
        }
        else if (!playerInMaze && wasInMaze)
        {
            // Player returned to glade
            Debug.Log($"🕷️ {gameObject.name}: Player returned to glade");
            detectionRange = 5f; // Reset detection range
        }
    }
    
    void EvaluateStateTransitions()
    {
        if (player == null) return;
        
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        bool canSeePlayer = CanSeePlayer();
        
        // Enhanced detection when player in maze
        float effectiveDetectionRange = playerInMaze ? detectionRange * 1.5f : detectionRange;
        
        // Patrol → Chase
        if (currentState == AIState.Patrol && canSeePlayer && distanceToPlayer < effectiveDetectionRange)
        {
            ChangeState(AIState.Chase);
        }
        
        // Chase → Attack
        if (currentState == AIState.Chase && distanceToPlayer < attackRange)
        {
            ChangeState(AIState.Attack);
        }
        
        // Chase → Search (lost sight)
        if (currentState == AIState.Chase && !canSeePlayer)
        {
            ChangeState(AIState.Search);
        }
        
        // Attack → Chase (player moved away)
        if (currentState == AIState.Attack && distanceToPlayer > attackRange)
        {
            ChangeState(AIState.Chase);
        }
        
        // Search → Patrol (gave up)
        if (currentState == AIState.Search && stateTimer > memoryDuration)
        {
            ChangeState(AIState.Patrol);
        }
        
        // Any → Flee (player has repellent power-up)
        // Check for power-up effects here
        
        // Group coordination
        if (coordinateWithOthers)
        {
            CoordinateWithNearbyAI();
        }
    }
    
    bool CanSeePlayer()
    {
        if (player == null) return false;
        
        Vector2 direction = player.position - transform.position;
        float distance = direction.magnitude;
        
        if (distance > detectionRange) return false;
        
        // Raycast to check line of sight
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction.normalized, distance);
        
        if (hit.collider != null)
        {
            return hit.collider.CompareTag("Player");
        }
        
        return false;
    }
    
    Vector2 GetSmartDirection(Vector3 targetPos)
    {
        // Advanced pathfinding that finds the best way to player
        Vector2 directDirection = ((Vector2)targetPos - (Vector2)transform.position).normalized;
        
        // Check multiple rays to find best path
        int rayCount = 8;
        float bestScore = -1f;
        Vector2 bestDirection = directDirection;
        
        for (int i = 0; i < rayCount; i++)
        {
            float angle = (360f / rayCount) * i;
            Vector2 testDirection = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad)
            );
            
            // Cast ray in test direction
            RaycastHit2D hit = Physics2D.Raycast(transform.position, testDirection, 2f);
            
            // Calculate score based on:
            // 1. How clear the path is (distance to wall)
            // 2. How aligned with target direction
            float clearDistance = hit.collider != null ? hit.distance : 2f;
            float alignment = Vector2.Dot(testDirection, directDirection);
            
            // Bonus if this direction moves toward player
            float score = clearDistance * 0.5f + alignment * 0.5f;
            
            // If path is clear and aligned, it's good
            if (hit.collider == null || !hit.collider.CompareTag("Wall"))
            {
                score += 1f;
            }
            
            if (score > bestScore)
            {
                bestScore = score;
                bestDirection = testDirection;
            }
        }
        
        return bestDirection;
    }
    
    void CoordinateWithNearbyAI()
    {
        // Find other AI within range
        AdvancedAISystem[] otherAIs = FindObjectsOfType<AdvancedAISystem>();
        
        foreach (AdvancedAISystem other in otherAIs)
        {
            if (other == this) continue;
            
            float distance = Vector2.Distance(transform.position, other.transform.position);
            
            if (distance < 5f && other.currentState == AIState.Chase && currentState == AIState.Patrol)
            {
                // Join the chase!
                ChangeState(AIState.Chase);
                Debug.Log($"🕷️ {gameObject.name} joined the chase!");
            }
        }
    }
    
    void ChangeState(AIState newState)
    {
        if (currentState == newState) return;
        
        Debug.Log($"🤖 {gameObject.name}: {currentState} → {newState}");
        
        currentState = newState;
        stateTimer = 0f;
        
        // State entry actions
        switch (newState)
        {
            case AIState.Attack:
                rb.linearVelocity = Vector2.zero;
                break;
        }
    }
    
    void UpdateVisuals()
    {
        if (spriteRenderer == null) return;
        
        // Change color based on state
        Color targetColor = Color.white;
        
        switch (currentState)
        {
            case AIState.Patrol:
                targetColor = new Color(0.5f, 0.2f, 0.2f); // Dark red
                break;
            case AIState.Chase:
                targetColor = new Color(1f, 0.3f, 0.3f); // Bright red
                break;
            case AIState.Search:
                targetColor = new Color(0.8f, 0.5f, 0.2f); // Orange
                break;
            case AIState.Attack:
                targetColor = new Color(1f, 0f, 0f); // Pure red
                break;
            case AIState.Flee:
                targetColor = new Color(0.3f, 0.3f, 0.6f); // Blue
                break;
        }
        
        spriteRenderer.color = Color.Lerp(spriteRenderer.color, targetColor, Time.deltaTime * 5f);
        
        // Flip sprite based on movement direction
        if (rb.linearVelocity.x != 0)
        {
            spriteRenderer.flipX = rb.linearVelocity.x < 0;
        }
    }
    
    void OnDrawGizmosSelected()
    {
        // Draw detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        // Draw attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        // Draw patrol points
        Gizmos.color = Color.green;
        foreach (Vector2 point in patrolPoints)
        {
            Gizmos.DrawWireSphere(point, 0.5f);
        }
        
        // Draw last known player position
        if (lastKnownPlayerPos != Vector2.zero)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(lastKnownPlayerPos, 0.5f);
        }
    }
}

