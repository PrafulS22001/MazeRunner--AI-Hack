using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Realistic, modern UI/UX system with animations and effects
/// Features: Smooth transitions, particles, tooltips, realistic HUD
/// </summary>
public class RealisticUISystem : MonoBehaviour
{
    [Header("UI Elements")]
    private Canvas mainCanvas;
    private GameObject hudPanel;
    private GameObject minimapPanel;
    private GameObject stealthIndicator;
    private GameObject dangerIndicator;
    
    [Header("Health Bar")]
    private Slider healthSlider;
    private Image healthFill;
    private Text healthText;
    private GameObject healthWarning;
    
    [Header("Minimap")]
    private RawImage minimapImage;
    private Camera minimapCamera;
    private RenderTexture minimapTexture;
    
    [Header("Danger System")]
    private float dangerLevel = 0f;
    private int nearbyEnemies = 0;
    
    private Transform player;
    private HealthSystem playerHealth;
    private GameManager gameManager;
    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        playerHealth = player?.GetComponent<HealthSystem>();
        gameManager = FindFirstObjectByType<GameManager>();
        
        CreateRealisticUI();
        StartCoroutine(UpdateDangerLevel());
        
        Debug.Log("🎨 Realistic UI System initialized!");
    }
    
    void CreateRealisticUI()
    {
        // Create main canvas
        GameObject canvasGO = new GameObject("RealisticUICanvas");
        mainCanvas = canvasGO.AddComponent<Canvas>();
        mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        mainCanvas.sortingOrder = 200; // Top layer
        
        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        
        canvasGO.AddComponent<GraphicRaycaster>();
        
        // Create HUD
        CreateAdvancedHUD();
        CreateMinimap();
        CreateDangerIndicators();
        CreateStealthIndicator();
    }
    
    void CreateAdvancedHUD()
    {
        // HUD Panel with glass effect
        hudPanel = new GameObject("HUDPanel");
        hudPanel.transform.SetParent(mainCanvas.transform, false);
        
        RectTransform hudRT = hudPanel.AddComponent<RectTransform>();
        hudRT.anchorMin = new Vector2(0, 0.85f);
        hudRT.anchorMax = new Vector2(0.4f, 1f);
        hudRT.offsetMin = new Vector2(20, -20);
        hudRT.offsetMax = new Vector2(-20, -20);
        
        Image hudBG = hudPanel.AddComponent<Image>();
        hudBG.color = new Color(0.05f, 0.05f, 0.1f, 0.7f);
        
        // Advanced Health Bar
        GameObject healthBarContainer = new GameObject("HealthBarContainer");
        healthBarContainer.transform.SetParent(hudPanel.transform, false);
        
        RectTransform healthRT = healthBarContainer.AddComponent<RectTransform>();
        healthRT.anchorMin = new Vector2(0.1f, 0.3f);
        healthRT.anchorMax = new Vector2(0.9f, 0.6f);
        healthRT.offsetMin = Vector2.zero;
        healthRT.offsetMax = Vector2.zero;
        
        // Health slider with glow effect
        GameObject sliderGO = new GameObject("HealthSlider");
        sliderGO.transform.SetParent(healthBarContainer.transform, false);
        
        healthSlider = sliderGO.AddComponent<Slider>();
        healthSlider.minValue = 0;
        healthSlider.maxValue = 100;
        healthSlider.value = 100;
        
        RectTransform sliderRT = sliderGO.GetComponent<RectTransform>();
        sliderRT.anchorMin = Vector2.zero;
        sliderRT.anchorMax = Vector2.one;
        sliderRT.offsetMin = Vector2.zero;
        sliderRT.offsetMax = Vector2.zero;
        
        // Background
        GameObject bgGO = new GameObject("Background");
        bgGO.transform.SetParent(sliderGO.transform, false);
        RectTransform bgRT = bgGO.AddComponent<RectTransform>();
        bgRT.anchorMin = Vector2.zero;
        bgRT.anchorMax = Vector2.one;
        bgRT.offsetMin = Vector2.zero;
        bgRT.offsetMax = Vector2.zero;
        Image bgImage = bgGO.AddComponent<Image>();
        bgImage.color = new Color(0.2f, 0.1f, 0.1f);
        
        // Fill with gradient
        GameObject fillGO = new GameObject("Fill");
        fillGO.transform.SetParent(bgGO.transform, false);
        RectTransform fillRT = fillGO.AddComponent<RectTransform>();
        fillRT.anchorMin = Vector2.zero;
        fillRT.anchorMax = new Vector2(1, 1);
        fillRT.offsetMin = new Vector2(5, 5);
        fillRT.offsetMax = new Vector2(-5, -5);
        
        healthFill = fillGO.AddComponent<Image>();
        healthFill.color = new Color(0.2f, 1f, 0.3f);
        healthFill.type = Image.Type.Filled;
        healthFill.fillMethod = Image.FillMethod.Horizontal;
        
        healthSlider.fillRect = fillRT;
        
        // Health text with shadow
        GameObject textGO = new GameObject("HealthText");
        textGO.transform.SetParent(healthBarContainer.transform, false);
        RectTransform textRT = textGO.AddComponent<RectTransform>();
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.offsetMin = Vector2.zero;
        textRT.offsetMax = Vector2.zero;
        
        healthText = textGO.AddComponent<Text>();
        healthText.text = "100 / 100";
        healthText.fontSize = 20;
        healthText.fontStyle = FontStyle.Bold;
        healthText.color = Color.white;
        healthText.alignment = TextAnchor.MiddleCenter;
        
        try
        {
            healthText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        }
        catch
        {
            healthText.font = Resources.Load<Font>("Arial");
        }
        
        Shadow shadow = textGO.AddComponent<Shadow>();
        shadow.effectColor = Color.black;
        shadow.effectDistance = new Vector2(2, -2);
        
        // Heart icon
        GameObject heartIcon = new GameObject("HeartIcon");
        heartIcon.transform.SetParent(hudPanel.transform, false);
        RectTransform heartRT = heartIcon.AddComponent<RectTransform>();
        heartRT.anchorMin = new Vector2(0.05f, 0.5f);
        heartRT.anchorMax = new Vector2(0.05f, 0.5f);
        heartRT.pivot = new Vector2(0, 0.5f);
        heartRT.anchoredPosition = Vector2.zero;
        heartRT.sizeDelta = new Vector2(50, 50);
        
        Text heartText = heartIcon.AddComponent<Text>();
        heartText.text = "❤";
        heartText.fontSize = 40;
        heartText.color = Color.red;
        heartText.alignment = TextAnchor.MiddleCenter;
        try
        {
            heartText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        }
        catch { }
        
        // Pulsing animation
        heartIcon.AddComponent<PulseAnimation>();
    }
    
    void CreateMinimap()
    {
        // Minimap panel (top-right)
        minimapPanel = new GameObject("MinimapPanel");
        minimapPanel.transform.SetParent(mainCanvas.transform, false);
        
        RectTransform mapRT = minimapPanel.AddComponent<RectTransform>();
        mapRT.anchorMin = new Vector2(0.75f, 0.75f);
        mapRT.anchorMax = new Vector2(0.98f, 0.98f);
        mapRT.offsetMin = Vector2.zero;
        mapRT.offsetMax = Vector2.zero;
        
        Image mapBG = minimapPanel.AddComponent<Image>();
        mapBG.color = new Color(0.05f, 0.05f, 0.1f, 0.8f);
        
        // Border
        Outline outline = minimapPanel.AddComponent<Outline>();
        outline.effectColor = new Color(0.3f, 0.5f, 0.7f);
        outline.effectDistance = new Vector2(3, -3);
        
        // Minimap text
        GameObject mapLabel = new GameObject("Label");
        mapLabel.transform.SetParent(minimapPanel.transform, false);
        RectTransform labelRT = mapLabel.AddComponent<RectTransform>();
        labelRT.anchorMin = new Vector2(0, 0.85f);
        labelRT.anchorMax = new Vector2(1, 1);
        labelRT.offsetMin = Vector2.zero;
        labelRT.offsetMax = Vector2.zero;
        
        Text labelText = mapLabel.AddComponent<Text>();
        labelText.text = "RADAR";
        labelText.fontSize = 14;
        labelText.fontStyle = FontStyle.Bold;
        labelText.color = new Color(0.5f, 0.7f, 1f);
        labelText.alignment = TextAnchor.MiddleCenter;
        try
        {
            labelText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        }
        catch { }
        
        // Add visual radar elements
        CreateRadarElements();
    }
    
    void CreateRadarElements()
    {
        // Create simple radar visualization
        for (int i = 0; i < 3; i++)
        {
            GameObject circle = new GameObject($"RadarCircle{i}");
            circle.transform.SetParent(minimapPanel.transform, false);
            
            RectTransform circleRT = circle.AddComponent<RectTransform>();
            circleRT.anchorMin = new Vector2(0.5f, 0.4f);
            circleRT.anchorMax = new Vector2(0.5f, 0.4f);
            circleRT.pivot = new Vector2(0.5f, 0.5f);
            float size = 40f + (i * 20f);
            circleRT.sizeDelta = new Vector2(size, size);
            
            Image circleImage = circle.AddComponent<Image>();
            circleImage.color = new Color(0.2f, 0.8f, 0.4f, 0.3f - (i * 0.1f));
            
            // Add outline to make it look like radar
            Outline circleOutline = circle.AddComponent<Outline>();
            circleOutline.effectColor = new Color(0.3f, 1f, 0.5f, 0.5f);
            circleOutline.effectDistance = new Vector2(1, -1);
        }
        
        // Player dot in center
        GameObject playerDot = new GameObject("PlayerDot");
        playerDot.transform.SetParent(minimapPanel.transform, false);
        
        RectTransform dotRT = playerDot.AddComponent<RectTransform>();
        dotRT.anchorMin = new Vector2(0.5f, 0.4f);
        dotRT.anchorMax = new Vector2(0.5f, 0.4f);
        dotRT.pivot = new Vector2(0.5f, 0.5f);
        dotRT.sizeDelta = new Vector2(8, 8);
        
        Image dotImage = playerDot.AddComponent<Image>();
        dotImage.color = new Color(0.3f, 0.8f, 1f);
    }
    
    void CreateDangerIndicators()
    {
        // Danger indicator (screen edges)
        dangerIndicator = new GameObject("DangerIndicator");
        dangerIndicator.transform.SetParent(mainCanvas.transform, false);
        
        RectTransform dangerRT = dangerIndicator.AddComponent<RectTransform>();
        dangerRT.anchorMin = Vector2.zero;
        dangerRT.anchorMax = Vector2.one;
        dangerRT.offsetMin = Vector2.zero;
        dangerRT.offsetMax = Vector2.zero;
        
        Image dangerImage = dangerIndicator.AddComponent<Image>();
        dangerImage.color = new Color(1f, 0f, 0f, 0f); // Start transparent
        dangerImage.raycastTarget = false;
        
        dangerIndicator.AddComponent<DangerVignette>();
    }
    
    void CreateStealthIndicator()
    {
        // Stealth indicator (bottom-center)
        stealthIndicator = new GameObject("StealthIndicator");
        stealthIndicator.transform.SetParent(mainCanvas.transform, false);
        
        RectTransform stealthRT = stealthIndicator.AddComponent<RectTransform>();
        stealthRT.anchorMin = new Vector2(0.4f, 0.02f);
        stealthRT.anchorMax = new Vector2(0.6f, 0.08f);
        stealthRT.offsetMin = Vector2.zero;
        stealthRT.offsetMax = Vector2.zero;
        
        Image stealthBG = stealthIndicator.AddComponent<Image>();
        stealthBG.color = new Color(0.1f, 0.1f, 0.2f, 0.7f);
        
        GameObject stealthText = new GameObject("Text");
        stealthText.transform.SetParent(stealthIndicator.transform, false);
        RectTransform textRT = stealthText.AddComponent<RectTransform>();
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.offsetMin = Vector2.zero;
        textRT.offsetMax = Vector2.zero;
        
        Text text = stealthText.AddComponent<Text>();
        text.text = "👁 UNDETECTED";
        text.fontSize = 18;
        text.fontStyle = FontStyle.Bold;
        text.color = new Color(0.3f, 0.8f, 0.3f);
        text.alignment = TextAnchor.MiddleCenter;
        try
        {
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        }
        catch { }
        
        stealthIndicator.SetActive(false);
    }
    
    void Update()
    {
        UpdateHealthDisplay();
        UpdateStealthStatus();
    }
    
    void UpdateHealthDisplay()
    {
        if (playerHealth == null || healthSlider == null) return;
        
        float targetValue = playerHealth.currentHealth;
        healthSlider.value = Mathf.Lerp(healthSlider.value, targetValue, Time.deltaTime * 5f);
        
        healthText.text = $"{Mathf.RoundToInt(healthSlider.value)} / {playerHealth.maxHealth}";
        
        // Change color based on health
        float healthPercent = healthSlider.value / playerHealth.maxHealth;
        
        if (healthPercent > 0.6f)
        {
            healthFill.color = Color.Lerp(healthFill.color, new Color(0.2f, 1f, 0.3f), Time.deltaTime * 5f);
        }
        else if (healthPercent > 0.3f)
        {
            healthFill.color = Color.Lerp(healthFill.color, new Color(1f, 0.8f, 0.2f), Time.deltaTime * 5f);
        }
        else
        {
            healthFill.color = Color.Lerp(healthFill.color, new Color(1f, 0.2f, 0.2f), Time.deltaTime * 5f);
        }
    }
    
    void UpdateStealthStatus()
    {
        // Check for nearby enemies (using SpiderAI)
        SpiderAI[] enemies = FindObjectsByType<SpiderAI>(FindObjectsSortMode.None);
        nearbyEnemies = 0;
        bool detected = false;
        
        foreach (SpiderAI enemy in enemies)
        {
            if (enemy == null) continue;
            
            float distance = Vector2.Distance(player.position, enemy.transform.position);
            if (distance < 5f)
            {
                nearbyEnemies++;
                // Spider is chasing if within chase radius (8 units)
                if (distance <= 8f)
                {
                    detected = true;
                }
            }
        }
        
        // Update stealth indicator
        if (stealthIndicator != null)
        {
            if (detected)
            {
                stealthIndicator.SetActive(true);
                Text text = stealthIndicator.GetComponentInChildren<Text>();
                if (text != null)
                {
                    text.text = "⚠ DETECTED!";
                    text.color = new Color(1f, 0.3f, 0.3f);
                }
            }
            else if (nearbyEnemies > 0)
            {
                stealthIndicator.SetActive(true);
                Text text = stealthIndicator.GetComponentInChildren<Text>();
                if (text != null)
                {
                    text.text = $"⚠ {nearbyEnemies} ENEMY NEARBY";
                    text.color = new Color(1f, 0.8f, 0.2f);
                }
            }
            else
            {
                stealthIndicator.SetActive(false);
            }
        }
    }
    
    IEnumerator UpdateDangerLevel()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            
            // Calculate danger based on nearby enemies and health
            float healthFactor = 1f - (playerHealth != null ? (float)playerHealth.currentHealth / playerHealth.maxHealth : 1f);
            float enemyFactor = nearbyEnemies * 0.2f;
            
            dangerLevel = Mathf.Clamp01(healthFactor + enemyFactor);
            
            // Update danger vignette
            if (dangerIndicator != null)
            {
                Image image = dangerIndicator.GetComponent<Image>();
                if (image != null)
                {
                    Color targetColor = new Color(1f, 0f, 0f, dangerLevel * 0.3f);
                    image.color = Color.Lerp(image.color, targetColor, Time.deltaTime * 2f);
                }
            }
        }
    }
}

// Animation components
public class PulseAnimation : MonoBehaviour
{
    private RectTransform rt;
    private Vector3 originalScale;
    
    void Start()
    {
        rt = GetComponent<RectTransform>();
        originalScale = rt.localScale;
    }
    
    void Update()
    {
        float scale = 1f + Mathf.Sin(Time.time * 3f) * 0.1f;
        rt.localScale = originalScale * scale;
    }
}

public class DangerVignette : MonoBehaviour
{
    private Image image;
    
    void Start()
    {
        image = GetComponent<Image>();
    }
    
    void Update()
    {
        if (image != null && image.color.a > 0.05f)
        {
            // Pulse effect
            float pulse = Mathf.Sin(Time.time * 5f) * 0.5f + 0.5f;
            Color color = image.color;
            float baseAlpha = color.a;
            color.a = baseAlpha * (0.7f + pulse * 0.3f);
            image.color = color;
        }
    }
}



