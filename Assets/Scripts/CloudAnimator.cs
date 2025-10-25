using UnityEngine;

public class CloudAnimator : MonoBehaviour
{
    private Vector3 startPosition;
    private Vector3 startScale;
    private float randomOffset;
    private SpriteRenderer spriteRenderer;

    [Header("Floating Animation")]
    public float floatSpeed = 0.3f;
    public float floatAmount = 0.02f;
    public float pulseSpeed = 0.5f;
    public float pulseAmount = 0.05f;
    public float alphaVariation = 0.1f;

    void Start()
    {
        startPosition = transform.position;
        startScale = transform.localScale;
        randomOffset = Random.Range(0f, 10f);
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (spriteRenderer == null) return;

        // Gentle floating movement
        float floatX = Mathf.Sin(Time.time * floatSpeed + randomOffset) * floatAmount;
        float floatY = Mathf.Cos(Time.time * 0.7f + randomOffset) * floatAmount;
        transform.position = startPosition + new Vector3(floatX, floatY, 0);

        // Subtle pulsing scale
        float pulse = Mathf.Sin(Time.time * pulseSpeed + randomOffset) * pulseAmount;
        transform.localScale = startScale * (1f + pulse);

        // Gentle alpha breathing
        float alpha = 0.7f + (Mathf.Sin(Time.time * 0.8f + randomOffset) * alphaVariation);
        Color color = spriteRenderer.color;
        color.a = alpha;
        spriteRenderer.color = color;

        // NO ROTATION - Clouds stay upright
    }
}