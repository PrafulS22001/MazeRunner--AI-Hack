using UnityEngine;
using System.Collections;

public class PlayerAnimator : MonoBehaviour
{
    [Header("Animation Settings")]
    public float walkAnimationSpeed = 8f;
    public float idleAnimationSpeed = 2f;
    public float damageAnimationSpeed = 10f;
    
    [Header("Visual Effects")]
    public Color normalColor = Color.white;
    public Color damageColor = Color.red;
    public Color healColor = Color.green;
    public Color speedBoostColor = Color.yellow;
    public Color invisibilityColor = new Color(1f, 1f, 1f, 0.3f);
    
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private PlayerMovement playerMovement;
    private HealthSystem healthSystem;
    private Vector3 originalScale;
    private bool isAnimating = false;
    
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        playerMovement = GetComponent<PlayerMovement>();
        healthSystem = GetComponent<HealthSystem>();
        
        originalScale = transform.localScale;
        
        if (spriteRenderer != null)
        {
            spriteRenderer.color = normalColor;
        }
    }
    
    void Update()
    {
        if (isAnimating) return;
        
        AnimateMovement();
        AnimateIdle();
    }
    
    void AnimateMovement()
    {
        if (rb == null || spriteRenderer == null) return;
        
        float speed = rb.linearVelocity.magnitude;
        
        if (speed > 0.1f)
        {
            // Walking animation - scale pulsing
            float walkPulse = Mathf.Sin(Time.time * walkAnimationSpeed) * 0.1f;
            transform.localScale = originalScale * (1f + walkPulse);
            
            // Flip sprite based on direction
            if (rb.linearVelocity.x > 0.1f)
            {
                spriteRenderer.flipX = false;
            }
            else if (rb.linearVelocity.x < -0.1f)
            {
                spriteRenderer.flipX = true;
            }
        }
        else
        {
            // Idle animation - gentle breathing
            float idlePulse = Mathf.Sin(Time.time * idleAnimationSpeed) * 0.05f;
            transform.localScale = originalScale * (1f + idlePulse);
        }
    }
    
    void AnimateIdle()
    {
        if (rb == null || rb.linearVelocity.magnitude > 0.1f) return;
        
        // Gentle color pulsing when idle
        if (spriteRenderer != null && spriteRenderer.color == normalColor)
        {
            float colorPulse = Mathf.Sin(Time.time * idleAnimationSpeed) * 0.1f;
            Color currentColor = normalColor;
            currentColor.a = normalColor.a + colorPulse;
            spriteRenderer.color = currentColor;
        }
    }
    
    public void PlayDamageAnimation()
    {
        if (isAnimating) return;
        StartCoroutine(DamageAnimation());
    }
    
    public void PlayHealAnimation()
    {
        if (isAnimating) return;
        StartCoroutine(HealAnimation());
    }
    
    public void PlaySpeedBoostAnimation()
    {
        if (isAnimating) return;
        StartCoroutine(SpeedBoostAnimation());
    }
    
    public void PlayInvisibilityAnimation()
    {
        if (isAnimating) return;
        StartCoroutine(InvisibilityAnimation());
    }
    
    IEnumerator DamageAnimation()
    {
        isAnimating = true;
        
        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;
            
            // Flash red multiple times
            for (int i = 0; i < 3; i++)
            {
                spriteRenderer.color = damageColor;
                yield return new WaitForSeconds(0.1f);
                spriteRenderer.color = originalColor;
                yield return new WaitForSeconds(0.1f);
            }
            
            // Shake effect
            Vector3 originalPos = transform.position;
            for (int i = 0; i < 5; i++)
            {
                transform.position = originalPos + new Vector3(
                    Random.Range(-0.1f, 0.1f),
                    Random.Range(-0.1f, 0.1f),
                    0f
                );
                yield return new WaitForSeconds(0.05f);
            }
            transform.position = originalPos;
        }
        
        isAnimating = false;
    }
    
    IEnumerator HealAnimation()
    {
        isAnimating = true;
        
        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;
            
            // Gentle green glow
            for (float t = 0; t < 1f; t += Time.deltaTime * 2f)
            {
                spriteRenderer.color = Color.Lerp(originalColor, healColor, t);
                yield return null;
            }
            
            for (float t = 0; t < 1f; t += Time.deltaTime * 2f)
            {
                spriteRenderer.color = Color.Lerp(healColor, originalColor, t);
                yield return null;
            }
            
            spriteRenderer.color = originalColor;
        }
        
        isAnimating = false;
    }
    
    IEnumerator SpeedBoostAnimation()
    {
        isAnimating = true;
        
        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;
            
            // Yellow trail effect
            for (float t = 0; t < 2f; t += Time.deltaTime)
            {
                float intensity = Mathf.Sin(t * 5f) * 0.5f + 0.5f;
                spriteRenderer.color = Color.Lerp(originalColor, speedBoostColor, intensity);
                yield return null;
            }
            
            spriteRenderer.color = originalColor;
        }
        
        isAnimating = false;
    }
    
    IEnumerator InvisibilityAnimation()
    {
        isAnimating = true;
        
        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;
            
            // Fade to invisibility
            for (float t = 0; t < 1f; t += Time.deltaTime * 3f)
            {
                spriteRenderer.color = Color.Lerp(originalColor, invisibilityColor, t);
                yield return null;
            }
            
            // Stay invisible for duration (handled by power-up)
            // Fade back to normal
            for (float t = 0; t < 1f; t += Time.deltaTime * 3f)
            {
                spriteRenderer.color = Color.Lerp(invisibilityColor, originalColor, t);
                yield return null;
            }
            
            spriteRenderer.color = originalColor;
        }
        
        isAnimating = false;
    }
    
    public void ResetAnimation()
    {
        isAnimating = false;
        transform.localScale = originalScale;
        
        if (spriteRenderer != null)
        {
            spriteRenderer.color = normalColor;
        }
    }
}
