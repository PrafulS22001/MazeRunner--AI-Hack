using UnityEngine;

public class SimpleParticle : MonoBehaviour
{
    [Header("Particle Settings")]
    public float lifetime = 2f;
    public float speed = 2f;
    public Vector2 direction = Vector2.up;
    public bool useGravity = false;
    public float gravity = 9.8f;
    public bool fadeOut = true;
    public bool shrinkOverTime = true;
    
    private Vector2 velocity;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Vector3 originalScale;
    private float elapsedTime = 0f;
    
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
        
        originalScale = transform.localScale;
        
        // Set initial velocity
        velocity = direction.normalized * speed;
        
        // Add random variation
        velocity += new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));
        
        // Random rotation
        transform.Rotate(0, 0, Random.Range(0f, 360f));
    }
    
    void Update()
    {
        elapsedTime += Time.deltaTime;
        
        // Move particle
        if (useGravity)
        {
            velocity.y -= gravity * Time.deltaTime;
        }
        
        transform.position += (Vector3)velocity * Time.deltaTime;
        
        // Update visual effects
        if (fadeOut && spriteRenderer != null)
        {
            float alpha = Mathf.Lerp(originalColor.a, 0f, elapsedTime / lifetime);
            Color newColor = originalColor;
            newColor.a = alpha;
            spriteRenderer.color = newColor;
        }
        
        if (shrinkOverTime)
        {
            float scale = Mathf.Lerp(1f, 0f, elapsedTime / lifetime);
            transform.localScale = originalScale * scale;
        }
        
        // Destroy when lifetime expires
        if (elapsedTime >= lifetime)
        {
            Destroy(gameObject);
        }
    }
    
    public void SetDirection(Vector2 newDirection)
    {
        direction = newDirection;
        velocity = direction.normalized * speed;
    }
    
    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
        velocity = direction.normalized * speed;
    }
    
    public void SetColor(Color color)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = color;
            originalColor = color;
        }
    }
}
