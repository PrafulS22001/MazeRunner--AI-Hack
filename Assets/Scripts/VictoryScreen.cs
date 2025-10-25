using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Beautiful victory screen that shows when player reaches the exit
/// </summary>
public class VictoryScreen : MonoBehaviour
{
    private Canvas victoryCanvas;
    private GameObject victoryPanel;
    private bool isShowing = false;
    
    void Awake()
    {
        CreateVictoryScreen();
    }
    
    void Start()
    {
        HideVictoryScreen(); // Start hidden
    }
    
    void CreateVictoryScreen()
    {
        // Create canvas
        GameObject canvasGO = new GameObject("VictoryCanvas");
        victoryCanvas = canvasGO.AddComponent<Canvas>();
        victoryCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        victoryCanvas.sortingOrder = 1000; // Top of everything!
        
        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        
        canvasGO.AddComponent<GraphicRaycaster>();
        
        // Dark overlay
        GameObject overlay = new GameObject("Overlay");
        overlay.transform.SetParent(canvasGO.transform, false);
        RectTransform overlayRT = overlay.AddComponent<RectTransform>();
        overlayRT.anchorMin = Vector2.zero;
        overlayRT.anchorMax = Vector2.one;
        overlayRT.sizeDelta = Vector2.zero;
        
        Image overlayImg = overlay.AddComponent<Image>();
        overlayImg.color = new Color(0, 0, 0, 0.8f);
        
        // Victory panel
        victoryPanel = new GameObject("VictoryPanel");
        victoryPanel.transform.SetParent(canvasGO.transform, false);
        
        RectTransform panelRT = victoryPanel.AddComponent<RectTransform>();
        panelRT.sizeDelta = new Vector2(800, 600);
        panelRT.anchoredPosition = Vector2.zero;
        
        Image panelImg = victoryPanel.AddComponent<Image>();
        panelImg.color = new Color(0.1f, 0.1f, 0.15f, 0.95f);
        
        // Add glow effect
        var shadow = victoryPanel.AddComponent<Shadow>();
        shadow.effectColor = new Color(0, 1, 0, 0.5f);
        shadow.effectDistance = new Vector2(5, -5);
        
        // Title text
        CreateText("YOU ESCAPED!", 
                   new Vector2(0, 200), 
                   80, 
                   Color.green,
                   FontStyle.Bold,
                   victoryPanel.transform);
        
        // Subtitle
        CreateText("Congratulations! You made it out of the maze!", 
                   new Vector2(0, 120), 
                   32, 
                   Color.white,
                   FontStyle.Normal,
                   victoryPanel.transform);
        
        // Stats (we'll populate these dynamically)
        CreateText("Time: 0:00", 
                   new Vector2(0, 40), 
                   28, 
                   new Color(0.8f, 0.8f, 1f),
                   FontStyle.Normal,
                   victoryPanel.transform).name = "TimeText";
        
        // Buttons
        CreateButton("Play Again", new Vector2(0, -80), new Vector2(300, 70), RestartGame);
        CreateButton("Main Menu", new Vector2(0, -180), new Vector2(300, 70), GoToMenu);
        
        Debug.Log("✨ Victory screen created!");
    }
    
    GameObject CreateText(string content, Vector2 position, int fontSize, Color color, FontStyle style, Transform parent)
    {
        GameObject textGO = new GameObject("Text_" + content.Substring(0, Mathf.Min(10, content.Length)));
        textGO.transform.SetParent(parent, false);
        
        RectTransform rt = textGO.AddComponent<RectTransform>();
        rt.anchoredPosition = position;
        rt.sizeDelta = new Vector2(750, fontSize + 20);
        
        Text text = textGO.AddComponent<Text>();
        text.text = content;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = fontSize;
        text.color = color;
        text.fontStyle = style;
        text.alignment = TextAnchor.MiddleCenter;
        
        // Add outline for readability
        Outline outline = textGO.AddComponent<Outline>();
        outline.effectColor = Color.black;
        outline.effectDistance = new Vector2(2, -2);
        
        return textGO;
    }
    
    void CreateButton(string label, Vector2 position, Vector2 size, UnityEngine.Events.UnityAction onClick)
    {
        GameObject buttonGO = new GameObject("Button_" + label);
        buttonGO.transform.SetParent(victoryPanel.transform, false);
        
        RectTransform rt = buttonGO.AddComponent<RectTransform>();
        rt.anchoredPosition = position;
        rt.sizeDelta = size;
        
        Image img = buttonGO.AddComponent<Image>();
        img.color = new Color(0.2f, 0.8f, 0.2f, 1f); // Green button
        
        Button button = buttonGO.AddComponent<Button>();
        button.targetGraphic = img;
        button.onClick.AddListener(onClick);
        
        // Button hover effect
        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.2f, 0.8f, 0.2f, 1f);
        colors.highlightedColor = new Color(0.3f, 1f, 0.3f, 1f);
        colors.pressedColor = new Color(0.1f, 0.6f, 0.1f, 1f);
        button.colors = colors;
        
        // Button text
        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(buttonGO.transform, false);
        
        RectTransform textRT = textGO.AddComponent<RectTransform>();
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.sizeDelta = Vector2.zero;
        
        Text text = textGO.AddComponent<Text>();
        text.text = label;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 32;
        text.color = Color.white;
        text.fontStyle = FontStyle.Bold;
        text.alignment = TextAnchor.MiddleCenter;
        
        // Text shadow
        Shadow shadow = textGO.AddComponent<Shadow>();
        shadow.effectColor = Color.black;
        shadow.effectDistance = new Vector2(2, -2);
    }
    
    public void ShowVictoryScreen()
    {
        if (isShowing) return;
        
        // Ensure the screen is created
        if (victoryCanvas == null || victoryPanel == null)
        {
            Debug.Log("📦 Creating victory screen...");
            CreateVictoryScreen();
        }
        
        isShowing = true;
        
        if (victoryCanvas != null)
        {
            victoryCanvas.gameObject.SetActive(true);
        }
        
        // Update stats
        if (victoryPanel != null)
        {
            Text timeText = victoryPanel.transform.Find("TimeText")?.GetComponent<Text>();
            if (timeText != null)
            {
                float playTime = Time.timeSinceLevelLoad;
                int minutes = Mathf.FloorToInt(playTime / 60);
                int seconds = Mathf.FloorToInt(playTime % 60);
                timeText.text = $"Time: {minutes}:{seconds:00}";
            }
        }
        
        // Pause the game
        Time.timeScale = 0f;
        
        // Animate in
        StartCoroutine(AnimateIn());
        
        Debug.Log("🎉 VICTORY! Player escaped the maze!");
    }
    
    IEnumerator AnimateIn()
    {
        if (victoryPanel == null) yield break;
        
        victoryPanel.transform.localScale = Vector3.zero;
        
        float duration = 0.5f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;
            float scale = Mathf.Lerp(0f, 1f, EaseOutBack(t));
            victoryPanel.transform.localScale = Vector3.one * scale;
            yield return null;
        }
        
        victoryPanel.transform.localScale = Vector3.one;
    }
    
    float EaseOutBack(float t)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
    }
    
    void HideVictoryScreen()
    {
        if (victoryCanvas != null)
        {
            victoryCanvas.gameObject.SetActive(false);
        }
        isShowing = false;
    }
    
    void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    void GoToMenu()
    {
        Time.timeScale = 1f;
        // Just restart for now (menu will show on restart)
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    // Static method to be called from ExitTrigger
    public static void TriggerVictory()
    {
        VictoryScreen screen = FindFirstObjectByType<VictoryScreen>();
        if (screen != null)
        {
            screen.ShowVictoryScreen();
        }
        else
        {
            Debug.LogWarning("⚠️ VictoryScreen not found! Creating it now...");
            GameObject go = new GameObject("VictoryScreen");
            screen = go.AddComponent<VictoryScreen>();
            screen.ShowVictoryScreen();
        }
    }
}


