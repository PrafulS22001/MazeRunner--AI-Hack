using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenEffectsManager : MonoBehaviour
{
    [Header("Screen Effects")]
    public Image fadeOverlay;
    public Image damageOverlay;
    public Image healOverlay;
    public Image speedBoostOverlay;
    public Image invisibilityOverlay;
    
    [Header("Effect Settings")]
    public float fadeDuration = 1f;
    public float damageFlashDuration = 0.3f;
    public float healGlowDuration = 1f;
    public float speedBoostDuration = 2f;
    public float invisibilityDuration = 5f;
    
    [Header("Camera Effects")]
    public Camera mainCamera;
    public float cameraShakeIntensity = 0.1f;
    public float cameraShakeDuration = 0.5f;
    
    private static ScreenEffectsManager instance;
    private Vector3 originalCameraPosition;
    
    public static ScreenEffectsManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ScreenEffectsManager>();
                if (instance == null)
                {
                    GameObject effectsManager = new GameObject("ScreenEffectsManager");
                    instance = effectsManager.AddComponent<ScreenEffectsManager>();
                }
            }
            return instance;
        }
    }
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeEffects();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        
        if (mainCamera != null)
        {
            originalCameraPosition = mainCamera.transform.position;
        }
    }
    
    void InitializeEffects()
    {
        // Create UI overlays if they don't exist
        CreateOverlays();
        
        // Hide all overlays initially
        HideAllOverlays();
    }
    
    void CreateOverlays()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasGO = new GameObject("Canvas");
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
        }
        
        // Create fade overlay
        if (fadeOverlay == null)
        {
            GameObject fadeGO = new GameObject("FadeOverlay");
            fadeGO.transform.SetParent(canvas.transform, false);
            fadeOverlay = fadeGO.AddComponent<Image>();
            fadeOverlay.color = Color.black;
            fadeOverlay.rectTransform.anchorMin = Vector2.zero;
            fadeOverlay.rectTransform.anchorMax = Vector2.one;
            fadeOverlay.rectTransform.offsetMin = Vector2.zero;
            fadeOverlay.rectTransform.offsetMax = Vector2.zero;
        }
        
        // Create damage overlay
        if (damageOverlay == null)
        {
            GameObject damageGO = new GameObject("DamageOverlay");
            damageGO.transform.SetParent(canvas.transform, false);
            damageOverlay = damageGO.AddComponent<Image>();
            damageOverlay.color = new Color(1f, 0f, 0f, 0f);
            damageOverlay.rectTransform.anchorMin = Vector2.zero;
            damageOverlay.rectTransform.anchorMax = Vector2.one;
            damageOverlay.rectTransform.offsetMin = Vector2.zero;
            damageOverlay.rectTransform.offsetMax = Vector2.zero;
        }
        
        // Create heal overlay
        if (healOverlay == null)
        {
            GameObject healGO = new GameObject("HealOverlay");
            healGO.transform.SetParent(canvas.transform, false);
            healOverlay = healGO.AddComponent<Image>();
            healOverlay.color = new Color(0f, 1f, 0f, 0f);
            healOverlay.rectTransform.anchorMin = Vector2.zero;
            healOverlay.rectTransform.anchorMax = Vector2.one;
            healOverlay.rectTransform.offsetMin = Vector2.zero;
            healOverlay.rectTransform.offsetMax = Vector2.zero;
        }
        
        // Create speed boost overlay
        if (speedBoostOverlay == null)
        {
            GameObject speedGO = new GameObject("SpeedBoostOverlay");
            speedGO.transform.SetParent(canvas.transform, false);
            speedBoostOverlay = speedGO.AddComponent<Image>();
            speedBoostOverlay.color = new Color(1f, 1f, 0f, 0f);
            speedBoostOverlay.rectTransform.anchorMin = Vector2.zero;
            speedBoostOverlay.rectTransform.anchorMax = Vector2.one;
            speedBoostOverlay.rectTransform.offsetMin = Vector2.zero;
            speedBoostOverlay.rectTransform.offsetMax = Vector2.zero;
        }
        
        // Create invisibility overlay
        if (invisibilityOverlay == null)
        {
            GameObject invisGO = new GameObject("InvisibilityOverlay");
            invisGO.transform.SetParent(canvas.transform, false);
            invisibilityOverlay = invisGO.AddComponent<Image>();
            invisibilityOverlay.color = new Color(0f, 1f, 1f, 0f);
            invisibilityOverlay.rectTransform.anchorMin = Vector2.zero;
            invisibilityOverlay.rectTransform.anchorMax = Vector2.one;
            invisibilityOverlay.rectTransform.offsetMin = Vector2.zero;
            invisibilityOverlay.rectTransform.offsetMax = Vector2.zero;
        }
    }
    
    void HideAllOverlays()
    {
        if (fadeOverlay != null) fadeOverlay.color = new Color(0f, 0f, 0f, 0f);
        if (damageOverlay != null) damageOverlay.color = new Color(1f, 0f, 0f, 0f);
        if (healOverlay != null) healOverlay.color = new Color(0f, 1f, 0f, 0f);
        if (speedBoostOverlay != null) speedBoostOverlay.color = new Color(1f, 1f, 0f, 0f);
        if (invisibilityOverlay != null) invisibilityOverlay.color = new Color(0f, 1f, 1f, 0f);
    }
    
    public void FadeIn()
    {
        StartCoroutine(FadeEffect(true));
    }
    
    public void FadeOut()
    {
        StartCoroutine(FadeEffect(false));
    }
    
    IEnumerator FadeEffect(bool fadeIn)
    {
        if (fadeOverlay == null) yield break;
        
        Color startColor = fadeOverlay.color;
        Color endColor = fadeIn ? new Color(0f, 0f, 0f, 0f) : new Color(0f, 0f, 0f, 1f);
        
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float progress = t / fadeDuration;
            fadeOverlay.color = Color.Lerp(startColor, endColor, progress);
            yield return null;
        }
        
        fadeOverlay.color = endColor;
    }
    
    public void DamageFlash()
    {
        StartCoroutine(DamageFlashEffect());
    }
    
    IEnumerator DamageFlashEffect()
    {
        if (damageOverlay == null) yield break;
        
        Color startColor = new Color(1f, 0f, 0f, 0f);
        Color flashColor = new Color(1f, 0f, 0f, 0.3f);
        
        // Flash in
        for (float t = 0; t < damageFlashDuration / 2; t += Time.deltaTime)
        {
            float progress = t / (damageFlashDuration / 2);
            damageOverlay.color = Color.Lerp(startColor, flashColor, progress);
            yield return null;
        }
        
        // Flash out
        for (float t = 0; t < damageFlashDuration / 2; t += Time.deltaTime)
        {
            float progress = t / (damageFlashDuration / 2);
            damageOverlay.color = Color.Lerp(flashColor, startColor, progress);
            yield return null;
        }
        
        damageOverlay.color = startColor;
    }
    
    public void HealGlow()
    {
        StartCoroutine(HealGlowEffect());
    }
    
    IEnumerator HealGlowEffect()
    {
        if (healOverlay == null) yield break;
        
        Color startColor = new Color(0f, 1f, 0f, 0f);
        Color glowColor = new Color(0f, 1f, 0f, 0.2f);
        
        // Glow in
        for (float t = 0; t < healGlowDuration / 2; t += Time.deltaTime)
        {
            float progress = t / (healGlowDuration / 2);
            healOverlay.color = Color.Lerp(startColor, glowColor, progress);
            yield return null;
        }
        
        // Glow out
        for (float t = 0; t < healGlowDuration / 2; t += Time.deltaTime)
        {
            float progress = t / (healGlowDuration / 2);
            healOverlay.color = Color.Lerp(glowColor, startColor, progress);
            yield return null;
        }
        
        healOverlay.color = startColor;
    }
    
    public void SpeedBoostEffect()
    {
        StartCoroutine(SpeedBoostEffectCoroutine());
    }
    
    IEnumerator SpeedBoostEffectCoroutine()
    {
        if (speedBoostOverlay == null) yield break;
        
        Color startColor = new Color(1f, 1f, 0f, 0f);
        Color boostColor = new Color(1f, 1f, 0f, 0.1f);
        
        for (float t = 0; t < speedBoostDuration; t += Time.deltaTime)
        {
            float intensity = Mathf.Sin(t * 5f) * 0.5f + 0.5f;
            speedBoostOverlay.color = Color.Lerp(startColor, boostColor, intensity);
            yield return null;
        }
        
        speedBoostOverlay.color = startColor;
    }
    
    public void InvisibilityEffect()
    {
        StartCoroutine(InvisibilityEffectCoroutine());
    }
    
    IEnumerator InvisibilityEffectCoroutine()
    {
        if (invisibilityOverlay == null) yield break;
        
        Color startColor = new Color(0f, 1f, 1f, 0f);
        Color invisColor = new Color(0f, 1f, 1f, 0.15f);
        
        // Fade in
        for (float t = 0; t < 0.5f; t += Time.deltaTime)
        {
            float progress = t / 0.5f;
            invisibilityOverlay.color = Color.Lerp(startColor, invisColor, progress);
            yield return null;
        }
        
        // Stay visible for duration
        yield return new WaitForSeconds(invisibilityDuration - 1f);
        
        // Fade out
        for (float t = 0; t < 0.5f; t += Time.deltaTime)
        {
            float progress = t / 0.5f;
            invisibilityOverlay.color = Color.Lerp(invisColor, startColor, progress);
            yield return null;
        }
        
        invisibilityOverlay.color = startColor;
    }
    
    public void CameraShake()
    {
        StartCoroutine(CameraShakeEffect());
    }
    
    IEnumerator CameraShakeEffect()
    {
        if (mainCamera == null) yield break;
        
        Vector3 originalPos = mainCamera.transform.position;
        float elapsed = 0f;
        
        while (elapsed < cameraShakeDuration)
        {
            float x = Random.Range(-1f, 1f) * cameraShakeIntensity;
            float y = Random.Range(-1f, 1f) * cameraShakeIntensity;
            
            mainCamera.transform.position = originalPos + new Vector3(x, y, 0f);
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        mainCamera.transform.position = originalPos;
    }
    
    public void ScreenFlash(Color color, float duration)
    {
        StartCoroutine(ScreenFlashEffect(color, duration));
    }
    
    IEnumerator ScreenFlashEffect(Color color, float duration)
    {
        if (fadeOverlay == null) yield break;
        
        Color startColor = new Color(color.r, color.g, color.b, 0f);
        Color flashColor = new Color(color.r, color.g, color.b, 0.5f);
        
        // Flash in
        for (float t = 0; t < duration / 2; t += Time.deltaTime)
        {
            float progress = t / (duration / 2);
            fadeOverlay.color = Color.Lerp(startColor, flashColor, progress);
            yield return null;
        }
        
        // Flash out
        for (float t = 0; t < duration / 2; t += Time.deltaTime)
        {
            float progress = t / (duration / 2);
            fadeOverlay.color = Color.Lerp(flashColor, startColor, progress);
            yield return null;
        }
        
        fadeOverlay.color = startColor;
    }
}
