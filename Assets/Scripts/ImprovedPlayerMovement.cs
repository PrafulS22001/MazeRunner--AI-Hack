using UnityEngine;

/// <summary>
/// Improved player movement that doesn't get stuck
/// Replaces the old PlayerMovement script
/// </summary>
public class ImprovedPlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5f;
    public float acceleration = 50f;
    public bool usePhysicsMovement = false; // Toggle between physics and direct movement
    
    [Header("Anti-Stuck Features")]
    public float unstuckForce = 10f;
    public float wallPushDistance = 0.1f;
    
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 lastValidPosition;
    private float stuckTimer = 0f;
    private const float STUCK_THRESHOLD = 0.5f;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("No Rigidbody2D found on player!");
            return;
        }
        
        // Setup Rigidbody for smooth movement
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.linearDamping = 0f;
        
        // Store starting position
        lastValidPosition = transform.position;
        
        Debug.Log("✅ Improved Player Movement initialized");
    }
    
    void Update()
    {
        // Get input every frame
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        
        // Normalize diagonal movement
        if (moveInput.magnitude > 1f)
        {
            moveInput.Normalize();
        }
        
        // Check if player is stuck
        CheckIfStuck();
        
        // Debug info (optional - press L to see)
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log($"Player Pos: {transform.position}, Velocity: {rb.linearVelocity}, Input: {moveInput}");
        }
    }
    
    void FixedUpdate()
    {
        if (rb == null) return;
        
        if (usePhysicsMovement)
        {
            MoveWithPhysics();
        }
        else
        {
            MoveDirectly();
        }
    }
    
    void MoveWithPhysics()
    {
        // Smooth acceleration-based movement
        Vector2 targetVelocity = moveInput * speed;
        Vector2 velocityChange = targetVelocity - rb.linearVelocity;
        velocityChange = Vector2.ClampMagnitude(velocityChange, acceleration * Time.fixedDeltaTime);
        
        rb.linearVelocity += velocityChange;
    }
    
    void MoveDirectly()
    {
        // Direct position-based movement (more reliable, no sticking)
        Vector2 movement = moveInput * speed * Time.fixedDeltaTime;
        Vector2 newPosition = rb.position + movement;
        
        // Use MovePosition for collision-aware movement
        rb.MovePosition(newPosition);
    }
    
    void CheckIfStuck()
    {
        // Check if player hasn't moved significantly
        float distanceMoved = Vector2.Distance(transform.position, lastValidPosition);
        
        if (moveInput.magnitude > 0.1f) // Player is trying to move
        {
            if (distanceMoved < 0.01f) // But not moving
            {
                stuckTimer += Time.deltaTime;
                
                if (stuckTimer > STUCK_THRESHOLD)
                {
                    UnstuckPlayer();
                    stuckTimer = 0f;
                }
            }
            else
            {
                stuckTimer = 0f;
                lastValidPosition = transform.position;
            }
        }
        else
        {
            stuckTimer = 0f;
            lastValidPosition = transform.position;
        }
    }
    
    void UnstuckPlayer()
    {
        Debug.LogWarning("Player appears stuck! Applying unstuck force...");
        
        // Try to push player away from walls
        Vector2 pushDirection = moveInput;
        
        if (pushDirection.magnitude < 0.1f)
        {
            // If no input, push in a random direction
            pushDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        }
        
        // Small push away from obstacle
        Vector2 newPos = rb.position + pushDirection * wallPushDistance;
        rb.MovePosition(newPos);
        
        // Clear velocity
        rb.linearVelocity = Vector2.zero;
        
        Debug.Log($"Applied unstuck force in direction {pushDirection}");
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        // When hitting a wall, ensure velocity is zeroed on that axis
        if (collision.gameObject.CompareTag("Wall"))
        {
            Vector2 normal = collision.contacts[0].normal;
            
            // Remove velocity component in the direction of the wall normal
            Vector2 velocity = rb.linearVelocity;
            velocity -= Vector2.Dot(velocity, normal) * normal;
            rb.linearVelocity = velocity;
        }
    }
    
    void OnCollisionStay2D(Collision2D collision)
    {
        // Continuously push slightly away from walls when in contact
        if (collision.gameObject.CompareTag("Wall"))
        {
            Vector2 pushAway = collision.contacts[0].normal * wallPushDistance;
            rb.MovePosition(rb.position + pushAway * Time.fixedDeltaTime);
        }
    }
    
    [ContextMenu("Unstuck Player Now")]
    public void ManualUnstuck()
    {
        UnstuckPlayer();
    }
    
    [ContextMenu("Toggle Movement Mode")]
    public void ToggleMovementMode()
    {
        usePhysicsMovement = !usePhysicsMovement;
        Debug.Log($"Movement mode: {(usePhysicsMovement ? "Physics" : "Direct")}");
    }
}

