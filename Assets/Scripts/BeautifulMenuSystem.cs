using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Beautiful, modern menu system with animations and stunning visuals
/// </summary>
public class BeautifulMenuSystem : MonoBehaviour
{
    [Header("Menu Panels")]
    private GameObject startScreen;
    private GameObject pauseScreen;
    private GameObject gameOverScreen;
    private GameObject victoryScreen;
    private GameObject howToPlayScreen;
    private GameObject settingsScreen;
    private GameObject gameModeScreen;
    
    [Header("Canvas Settings")]
    private Canvas menuCanvas;
    private CanvasScaler canvasScaler;
    
    private bool isPaused = false;
    private GameManager gameManager;
    private AudioManager audioManager;
    
    void Start()
    {
        CreateBeautifulCanvas();
        CreateAllMenus();
        ShowStartScreen();
        
        gameManager = FindObjectOfType<GameManager>();
        audioManager = AudioManager.Instance;
        
        Debug.Log("✨ Beautiful Menu System loaded!");
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }
    
    void CreateBeautifulCanvas()
    {
        // Create main canvas
        GameObject canvasGO = new GameObject("BeautifulMenuCanvas");
        menuCanvas = canvasGO.AddComponent<Canvas>();
        menuCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        menuCanvas.sortingOrder = 100; // Ensure it's on top
        
        canvasScaler = canvasGO.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(1920, 1080);
        canvasScaler.matchWidthOrHeight = 0.5f;
        
        canvasGO.AddComponent<GraphicRaycaster>(); // IMPORTANT for clicking!
        
        // Ensure EventSystem exists
        if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            Debug.Log("✅ EventSystem created for menu interaction!");
        }
    }
    
    void CreateAllMenus()
    {
        startScreen = CreateStartScreen();
        pauseScreen = CreatePauseScreen();
        gameOverScreen = CreateGameOverScreen();
        victoryScreen = CreateVictoryScreen();
        howToPlayScreen = CreateHowToPlayScreen();
        settingsScreen = CreateSettingsScreen();
        gameModeScreen = CreateGameModeScreen();
        
        HideAllScreens();
    }
    
    GameObject CreateStartScreen()
    {
        GameObject screen = CreateAnimatedPanel("StartScreen", new Color(0.05f, 0.05f, 0.1f, 0.98f));
        
        // Animated background gradient
        GameObject bgGradient = CreateGradientBackground(screen, 
            new Color(0.1f, 0.05f, 0.2f), 
            new Color(0.05f, 0.1f, 0.3f));
        
        // Glowing particles effect
        CreateParticleEffect(screen);
        
        // Animated title with glow
        GameObject titleGO = CreateAnimatedTitle(screen, "MAZE RUNNER 2D", new Vector2(0, 300));
        
        // Subtitle with typing effect
        CreateText(screen, "~ Escape the Maze, Conquer Your Fears ~", 28, new Vector2(0, 220), 
            new Color(0.8f, 0.9f, 1f, 0.9f), FontStyle.Italic);
        
        // Beautiful buttons with hover effects
        float yPos = 80;
        float spacing = 85;
        
        CreateBeautifulButton(screen, "▶ PLAY", new Vector2(0, yPos), new Vector2(400, 70), 
            new Color(0.2f, 0.6f, 0.3f), Color.white, StartGame);
        
        CreateBeautifulButton(screen, "🎮 GAME MODE", new Vector2(0, yPos - spacing), new Vector2(400, 70),
            new Color(0.3f, 0.4f, 0.7f), Color.white, ShowGameModeScreen);
        
        CreateBeautifulButton(screen, "📖 HOW TO PLAY", new Vector2(0, yPos - spacing * 2), new Vector2(400, 70),
            new Color(0.5f, 0.3f, 0.6f), Color.white, ShowHowToPlay);
        
        CreateBeautifulButton(screen, "⚙ SETTINGS", new Vector2(0, yPos - spacing * 3), new Vector2(400, 70),
            new Color(0.6f, 0.5f, 0.2f), Color.white, ShowSettings);
        
        CreateBeautifulButton(screen, "✖ QUIT", new Vector2(0, yPos - spacing * 4), new Vector2(400, 70),
            new Color(0.6f, 0.2f, 0.2f), Color.white, QuitGame);
        
        // Footer text
        CreateText(screen, "Unity Game © 2025 | Press ESC to Pause", 16, new Vector2(0, -450),
            new Color(0.5f, 0.5f, 0.6f, 0.7f), FontStyle.Normal);
        
        return screen;
    }
    
    GameObject CreatePauseScreen()
    {
        GameObject screen = CreateAnimatedPanel("PauseScreen", new Color(0, 0, 0, 0.85f));
        
        // Title with glow
        CreateAnimatedTitle(screen, "PAUSED", new Vector2(0, 200));
        
        float yPos = 50;
        float spacing = 85;
        
        CreateBeautifulButton(screen, "▶ RESUME", new Vector2(0, yPos), new Vector2(400, 70),
            new Color(0.2f, 0.6f, 0.3f), Color.white, ResumeGame);
        
        CreateBeautifulButton(screen, "🔄 RESTART", new Vector2(0, yPos - spacing), new Vector2(400, 70),
            new Color(0.4f, 0.4f, 0.6f), Color.white, RestartGame);
        
        CreateBeautifulButton(screen, "⚙ SETTINGS", new Vector2(0, yPos - spacing * 2), new Vector2(400, 70),
            new Color(0.6f, 0.5f, 0.2f), Color.white, ShowSettings);
        
        CreateBeautifulButton(screen, "🏠 MAIN MENU", new Vector2(0, yPos - spacing * 3), new Vector2(400, 70),
            new Color(0.5f, 0.3f, 0.5f), Color.white, GoToMainMenu);
        
        return screen;
    }
    
    GameObject CreateGameOverScreen()
    {
        GameObject screen = CreateAnimatedPanel("GameOverScreen", new Color(0.15f, 0.05f, 0.05f, 0.95f));
        
        CreateAnimatedTitle(screen, "GAME OVER", new Vector2(0, 250), Color.red);
        
        CreateText(screen, "The spiders caught you!", 32, new Vector2(0, 150),
            new Color(1f, 0.7f, 0.7f), FontStyle.Bold);
        
        // Stats panel
        GameObject statsPanel = CreateGlassPanel(screen, new Vector2(0, 10), new Vector2(500, 150));
        CreateText(statsPanel, "Score: 0", 36, new Vector2(0, 30), Color.yellow, FontStyle.Bold);
        CreateText(statsPanel, "Time: 00:00", 28, new Vector2(0, -20), new Color(0.8f, 0.9f, 1f), FontStyle.Normal);
        
        CreateBeautifulButton(screen, "🔄 TRY AGAIN", new Vector2(0, -150), new Vector2(350, 70),
            new Color(0.3f, 0.6f, 0.3f), Color.white, RestartGame);
        
        CreateBeautifulButton(screen, "🏠 MAIN MENU", new Vector2(0, -240), new Vector2(350, 70),
            new Color(0.4f, 0.4f, 0.6f), Color.white, GoToMainMenu);
        
        return screen;
    }
    
    GameObject CreateVictoryScreen()
    {
        GameObject screen = CreateAnimatedPanel("VictoryScreen", new Color(0.05f, 0.15f, 0.05f, 0.95f));
        
        CreateAnimatedTitle(screen, "VICTORY!", new Vector2(0, 250), Color.green);
        
        CreateText(screen, "🏆 You Escaped the Maze! 🏆", 32, new Vector2(0, 150),
            Color.yellow, FontStyle.Bold);
        
        // Stats panel
        GameObject statsPanel = CreateGlassPanel(screen, new Vector2(0, 10), new Vector2(500, 180));
        CreateText(statsPanel, "Score: 0", 36, new Vector2(0, 50), Color.yellow, FontStyle.Bold);
        CreateText(statsPanel, "Time: 00:00", 28, new Vector2(0, 0), new Color(0.8f, 0.9f, 1f), FontStyle.Normal);
        CreateText(statsPanel, "Level: 1", 28, new Vector2(0, -40), new Color(0.8f, 1f, 0.8f), FontStyle.Normal);
        
        CreateBeautifulButton(screen, "➡ NEXT LEVEL", new Vector2(0, -160), new Vector2(350, 70),
            new Color(0.2f, 0.7f, 0.3f), Color.white, NextLevel);
        
        CreateBeautifulButton(screen, "🏠 MAIN MENU", new Vector2(0, -250), new Vector2(350, 70),
            new Color(0.4f, 0.4f, 0.6f), Color.white, GoToMainMenu);
        
        return screen;
    }
    
    GameObject CreateHowToPlayScreen()
    {
        GameObject screen = CreateAnimatedPanel("HowToPlayScreen", new Color(0.05f, 0.05f, 0.1f, 0.95f));
        
        CreateAnimatedTitle(screen, "HOW TO PLAY", new Vector2(0, 400), new Color(0.6f, 0.8f, 1f));
        
        // Create scrollable content
        GameObject contentPanel = CreateGlassPanel(screen, new Vector2(0, 0), new Vector2(800, 650));
        
        string helpText = 
            "<b><size=32>🎮 CONTROLS</size></b>\n" +
            "<color=#88DDFF>WASD / Arrow Keys</color> - Move Player\n" +
            "<color=#88DDFF>R</color> - Regenerate Maze\n" +
            "<color=#88DDFF>F</color> - Toggle Fog of War\n" +
            "<color=#88DDFF>ESC</color> - Pause Game\n\n" +
            
            "<b><size=32>🎯 OBJECTIVE</size></b>\n" +
            "Find the <color=#00FF00>glowing green exit</color> and escape!\n" +
            "Avoid the <color=#FF4444>red spiders</color> - they deal damage!\n\n" +
            
            "<b><size=32>⭐ POWER-UPS</size></b>\n" +
            "<color=#00FF00>🟢 Green</color> - Health Pack (+25 HP)\n" +
            "<color=#FFFF00>🟡 Yellow</color> - Speed Boost (10s)\n" +
            "<color=#00FFFF>🔵 Cyan</color> - Invisibility (5s)\n" +
            "<color=#FF0000>🔴 Red</color> - Spider Repellent (10s)\n" +
            "<color=#FF00FF>🟣 Magenta</color> - Score Multiplier\n\n" +
            
            "<b><size=32>💡 TIPS</size></b>\n" +
            "• Hide in <color=#88FF88>bushes</color> to avoid spiders\n" +
            "• Collect power-ups strategically\n" +
            "• Watch your health bar (top-left)\n" +
            "• Plan your route to the exit";
        
        Text helpTextComp = CreateRichText(contentPanel, helpText, 22, new Vector2(0, 50), Color.white);
        helpTextComp.alignment = TextAnchor.UpperCenter;
        
        CreateBeautifulButton(screen, "◀ BACK", new Vector2(0, -420), new Vector2(250, 60),
            new Color(0.4f, 0.4f, 0.6f), Color.white, () => { screen.SetActive(false); startScreen.SetActive(true); });
        
        return screen;
    }
    
    GameObject CreateSettingsScreen()
    {
        GameObject screen = CreateAnimatedPanel("SettingsScreen", new Color(0.05f, 0.05f, 0.1f, 0.95f));
        
        CreateAnimatedTitle(screen, "SETTINGS", new Vector2(0, 350), new Color(0.9f, 0.7f, 0.3f));
        
        GameObject settingsPanel = CreateGlassPanel(screen, new Vector2(0, 0), new Vector2(700, 500));
        
        // Volume sliders with labels
        CreateSliderWithLabel(settingsPanel, "Master Volume", new Vector2(0, 150), SetMasterVolume);
        CreateSliderWithLabel(settingsPanel, "Music Volume", new Vector2(0, 50), SetMusicVolume);
        CreateSliderWithLabel(settingsPanel, "SFX Volume", new Vector2(0, -50), SetSFXVolume);
        
        // Fullscreen toggle
        CreateToggleWithLabel(settingsPanel, "Fullscreen", new Vector2(0, -150), SetFullscreen);
        
        CreateBeautifulButton(screen, "◀ BACK", new Vector2(0, -350), new Vector2(250, 60),
            new Color(0.4f, 0.4f, 0.6f), Color.white, HideSettings);
        
        return screen;
    }
    
    GameObject CreateGameModeScreen()
    {
        GameObject screen = CreateAnimatedPanel("GameModeScreen", new Color(0.05f, 0.05f, 0.1f, 0.95f));
        
        CreateAnimatedTitle(screen, "SELECT MODE", new Vector2(0, 350), new Color(0.7f, 0.5f, 0.9f));
        
        float yPos = 150;
        float spacing = 130;
        
        // Classic Mode
        CreateModeCard(screen, "🏆 CLASSIC MODE", 
            "Progressive difficulty\nComplete all levels", 
            new Vector2(0, yPos), new Color(0.2f, 0.5f, 0.7f), 
            () => SelectGameMode("Classic"));
        
        // Survival Mode
        CreateModeCard(screen, "⚔ SURVIVAL MODE", 
            "Survive as long as possible\nEndless waves", 
            new Vector2(0, yPos - spacing), new Color(0.7f, 0.3f, 0.3f),
            () => SelectGameMode("Survival"));
        
        // Time Challenge
        CreateModeCard(screen, "⏱ TIME CHALLENGE", 
            "Race against the clock\nBeat the timer", 
            new Vector2(0, yPos - spacing * 2), new Color(0.6f, 0.6f, 0.2f),
            () => SelectGameMode("TimeChallenge"));
        
        CreateBeautifulButton(screen, "◀ BACK", new Vector2(0, -370), new Vector2(250, 60),
            new Color(0.4f, 0.4f, 0.6f), Color.white, () => { screen.SetActive(false); startScreen.SetActive(true); });
        
        return screen;
    }
    
    // Helper methods for creating beautiful UI elements
    
    GameObject CreateAnimatedPanel(string name, Color color)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(menuCanvas.transform, false);
        
        RectTransform rt = panel.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        
        Image image = panel.AddComponent<Image>();
        image.color = color;
        
        // Add fade in animation
        CanvasGroup canvasGroup = panel.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        
        return panel;
    }
    
    GameObject CreateGradientBackground(GameObject parent, Color topColor, Color bottomColor)
    {
        GameObject gradient = new GameObject("Gradient");
        gradient.transform.SetParent(parent.transform, false);
        
        RectTransform rt = gradient.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        
        Image image = gradient.AddComponent<Image>();
        
        // Create gradient texture
        Texture2D gradientTexture = new Texture2D(1, 256);
        for (int y = 0; y < 256; y++)
        {
            Color color = Color.Lerp(bottomColor, topColor, y / 255f);
            gradientTexture.SetPixel(0, y, color);
        }
        gradientTexture.Apply();
        
        image.sprite = Sprite.Create(gradientTexture, new Rect(0, 0, 1, 256), new Vector2(0.5f, 0.5f));
        
        return gradient;
    }
    
    void CreateParticleEffect(GameObject parent)
    {
        // Create animated stars/particles in background
        for (int i = 0; i < 20; i++)
        {
            GameObject star = new GameObject($"Star{i}");
            star.transform.SetParent(parent.transform, false);
            
            RectTransform rt = star.AddComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(Random.Range(-960, 960), Random.Range(-540, 540));
            rt.sizeDelta = new Vector2(Random.Range(3, 8), Random.Range(3, 8));
            
            Image image = star.AddComponent<Image>();
            image.color = new Color(1, 1, 1, Random.Range(0.3f, 0.7f));
            
            // Add twinkling animation
            star.AddComponent<UITwinkle>();
        }
    }
    
    GameObject CreateAnimatedTitle(GameObject parent, string text, Vector2 position, Color? customColor = null)
    {
        GameObject titleGO = new GameObject("Title");
        titleGO.transform.SetParent(parent.transform, false);
        
        RectTransform rt = titleGO.AddComponent<RectTransform>();
        rt.anchoredPosition = position;
        rt.sizeDelta = new Vector2(1000, 120);
        
        Text titleText = titleGO.AddComponent<Text>();
        titleText.text = text;
        titleText.fontSize = 72;
        titleText.fontStyle = FontStyle.Bold;
        titleText.color = customColor ?? new Color(1f, 0.9f, 0.5f);
        titleText.alignment = TextAnchor.MiddleCenter;
        
        try
        {
            titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        }
        catch
        {
            titleText.font = Resources.Load<Font>("Arial");
        }
        
        // Add outline
        Outline outline = titleGO.AddComponent<Outline>();
        outline.effectColor = Color.black;
        outline.effectDistance = new Vector2(4, -4);
        
        // Add shadow
        Shadow shadow = titleGO.AddComponent<Shadow>();
        shadow.effectColor = new Color(0, 0, 0, 0.5f);
        shadow.effectDistance = new Vector2(6, -6);
        
        // Add pulse animation
        titleGO.AddComponent<UIPulse>();
        
        return titleGO;
    }
    
    void CreateBeautifulButton(GameObject parent, string text, Vector2 position, Vector2 size, 
        Color backgroundColor, Color textColor, UnityEngine.Events.UnityAction onClick)
    {
        GameObject buttonGO = new GameObject("Button_" + text);
        buttonGO.transform.SetParent(parent.transform, false);
        
        RectTransform rt = buttonGO.AddComponent<RectTransform>();
        rt.anchoredPosition = position;
        rt.sizeDelta = size;
        
        Image image = buttonGO.AddComponent<Image>();
        image.color = backgroundColor;
        
        Button button = buttonGO.AddComponent<Button>();
        
        // Beautiful color transitions
        ColorBlock colors = button.colors;
        colors.normalColor = backgroundColor;
        colors.highlightedColor = backgroundColor * 1.3f;
        colors.pressedColor = backgroundColor * 0.8f;
        colors.selectedColor = backgroundColor * 1.2f;
        colors.colorMultiplier = 1f;
        colors.fadeDuration = 0.2f;
        button.colors = colors;
        
        button.onClick.AddListener(onClick);
        
        // Button text
        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(buttonGO.transform, false);
        
        RectTransform textRT = textGO.AddComponent<RectTransform>();
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.offsetMin = Vector2.zero;
        textRT.offsetMax = Vector2.zero;
        
        Text buttonText = textGO.AddComponent<Text>();
        buttonText.text = text;
        buttonText.fontSize = 28;
        buttonText.fontStyle = FontStyle.Bold;
        buttonText.color = textColor;
        buttonText.alignment = TextAnchor.MiddleCenter;
        
        try
        {
            buttonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        }
        catch
        {
            buttonText.font = Resources.Load<Font>("Arial");
        }
        
        Outline outline = textGO.AddComponent<Outline>();
        outline.effectColor = new Color(0, 0, 0, 0.8f);
        outline.effectDistance = new Vector2(2, -2);
        
        // Add hover animation
        buttonGO.AddComponent<UIButtonHover>();
    }
    
    void CreateModeCard(GameObject parent, string title, string description, Vector2 position, 
        Color cardColor, UnityEngine.Events.UnityAction onClick)
    {
        GameObject card = CreateGlassPanel(parent, position, new Vector2(700, 100));
        
        Button button = card.AddComponent<Button>();
        ColorBlock colors = button.colors;
        colors.normalColor = cardColor;
        colors.highlightedColor = cardColor * 1.2f;
        colors.pressedColor = cardColor * 0.9f;
        button.colors = colors;
        button.onClick.AddListener(onClick);
        
        CreateText(card, title, 32, new Vector2(0, 20), Color.white, FontStyle.Bold);
        CreateText(card, description, 18, new Vector2(0, -20), new Color(0.9f, 0.9f, 0.9f, 0.8f), FontStyle.Italic);
        
        card.AddComponent<UIButtonHover>();
    }
    
    GameObject CreateGlassPanel(GameObject parent, Vector2 position, Vector2 size)
    {
        GameObject panel = new GameObject("GlassPanel");
        panel.transform.SetParent(parent.transform, false);
        
        RectTransform rt = panel.AddComponent<RectTransform>();
        rt.anchoredPosition = position;
        rt.sizeDelta = size;
        
        Image image = panel.AddComponent<Image>();
        image.color = new Color(0.1f, 0.1f, 0.15f, 0.7f);
        
        Outline outline = panel.AddComponent<Outline>();
        outline.effectColor = new Color(0.5f, 0.7f, 1f, 0.5f);
        outline.effectDistance = new Vector2(2, -2);
        
        return panel;
    }
    
    Text CreateText(GameObject parent, string text, int fontSize, Vector2 position, Color color, FontStyle style)
    {
        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(parent.transform, false);
        
        RectTransform rt = textGO.AddComponent<RectTransform>();
        rt.anchoredPosition = position;
        rt.sizeDelta = new Vector2(700, 50);
        
        Text textComp = textGO.AddComponent<Text>();
        textComp.text = text;
        textComp.fontSize = fontSize;
        textComp.fontStyle = style;
        textComp.color = color;
        textComp.alignment = TextAnchor.MiddleCenter;
        
        try
        {
            textComp.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        }
        catch
        {
            textComp.font = Resources.Load<Font>("Arial");
        }
        
        return textComp;
    }
    
    Text CreateRichText(GameObject parent, string text, int fontSize, Vector2 position, Color color)
    {
        GameObject textGO = new GameObject("RichText");
        textGO.transform.SetParent(parent.transform, false);
        
        RectTransform rt = textGO.AddComponent<RectTransform>();
        rt.anchoredPosition = position;
        rt.sizeDelta = new Vector2(750, 600);
        
        Text textComp = textGO.AddComponent<Text>();
        textComp.text = text;
        textComp.fontSize = fontSize;
        textComp.color = color;
        textComp.alignment = TextAnchor.UpperLeft;
        textComp.supportRichText = true;
        
        try
        {
            textComp.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        }
        catch
        {
            textComp.font = Resources.Load<Font>("Arial");
        }
        
        return textComp;
    }
    
    void CreateSliderWithLabel(GameObject parent, string label, Vector2 position, UnityEngine.Events.UnityAction<float> onValueChanged)
    {
        CreateText(parent, label, 24, position + new Vector2(-150, 0), Color.white, FontStyle.Normal);
        
        GameObject sliderGO = new GameObject("Slider_" + label);
        sliderGO.transform.SetParent(parent.transform, false);
        
        RectTransform rt = sliderGO.AddComponent<RectTransform>();
        rt.anchoredPosition = position + new Vector2(120, 0);
        rt.sizeDelta = new Vector2(350, 30);
        
        Slider slider = sliderGO.AddComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = 0.8f;
        slider.onValueChanged.AddListener(onValueChanged);
        
        // Background
        GameObject bg = new GameObject("Background");
        bg.transform.SetParent(sliderGO.transform, false);
        RectTransform bgRT = bg.AddComponent<RectTransform>();
        bgRT.anchorMin = Vector2.zero;
        bgRT.anchorMax = Vector2.one;
        bgRT.offsetMin = Vector2.zero;
        bgRT.offsetMax = Vector2.zero;
        Image bgImage = bg.AddComponent<Image>();
        bgImage.color = new Color(0.2f, 0.2f, 0.3f);
        
        // Fill
        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(bg.transform, false);
        RectTransform fillRT = fill.AddComponent<RectTransform>();
        fillRT.anchorMin = Vector2.zero;
        fillRT.anchorMax = Vector2.one;
        fillRT.offsetMin = Vector2.zero;
        fillRT.offsetMax = Vector2.zero;
        Image fillImage = fill.AddComponent<Image>();
        fillImage.color = new Color(0.3f, 0.7f, 0.4f);
        
        slider.fillRect = fillRT;
        slider.targetGraphic = fillImage;
    }
    
    void CreateToggleWithLabel(GameObject parent, string label, Vector2 position, UnityEngine.Events.UnityAction<bool> onValueChanged)
    {
        CreateText(parent, label, 24, position + new Vector2(-50, 0), Color.white, FontStyle.Normal);
        
        GameObject toggleGO = new GameObject("Toggle_" + label);
        toggleGO.transform.SetParent(parent.transform, false);
        
        RectTransform rt = toggleGO.AddComponent<RectTransform>();
        rt.anchoredPosition = position + new Vector2(150, 0);
        rt.sizeDelta = new Vector2(60, 30);
        
        Toggle toggle = toggleGO.AddComponent<Toggle>();
        toggle.isOn = Screen.fullScreen;
        toggle.onValueChanged.AddListener(onValueChanged);
        
        // Background
        GameObject bg = new GameObject("Background");
        bg.transform.SetParent(toggleGO.transform, false);
        RectTransform bgRT = bg.AddComponent<RectTransform>();
        bgRT.anchorMin = Vector2.zero;
        bgRT.anchorMax = Vector2.one;
        bgRT.offsetMin = Vector2.zero;
        bgRT.offsetMax = Vector2.zero;
        Image bgImage = bg.AddComponent<Image>();
        bgImage.color = new Color(0.2f, 0.2f, 0.3f);
        
        // Checkmark
        GameObject check = new GameObject("Checkmark");
        check.transform.SetParent(bg.transform, false);
        RectTransform checkRT = check.AddComponent<RectTransform>();
        checkRT.anchorMin = Vector2.zero;
        checkRT.anchorMax = Vector2.one;
        checkRT.offsetMin = new Vector2(5, 5);
        checkRT.offsetMax = new Vector2(-5, -5);
        Image checkImage = check.AddComponent<Image>();
        checkImage.color = new Color(0.3f, 0.8f, 0.4f);
        
        toggle.targetGraphic = bgImage;
        toggle.graphic = checkImage;
    }
    
    // Menu actions
    public void ShowStartScreen()
    {
        HideAllScreens();
        startScreen.SetActive(true);
        StartCoroutine(FadeIn(startScreen));
        Time.timeScale = 0f;
        
        // Hide gameplay UI (RealisticUISystem handles this automatically)
        RealisticUISystem realisticUI = FindObjectOfType<RealisticUISystem>();
        if (realisticUI != null)
        {
            realisticUI.gameObject.SetActive(false);
        }
    }
    
    public void StartGame()
    {
        HideAllScreens();
        Time.timeScale = 1f;
        isPaused = false;
        
        // Show gameplay UI (RealisticUISystem handles this automatically)
        RealisticUISystem realisticUI = FindObjectOfType<RealisticUISystem>();
        if (realisticUI != null)
        {
            realisticUI.gameObject.SetActive(true);
        }
    }
    
    public void PauseGame()
    {
        pauseScreen.SetActive(true);
        StartCoroutine(FadeIn(pauseScreen));
        Time.timeScale = 0f;
        isPaused = true;
    }
    
    public void ResumeGame()
    {
        pauseScreen.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }
    
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        ShowStartScreen();
    }
    
    public void ShowGameOver(int score, float time)
    {
        HideAllScreens();
        gameOverScreen.SetActive(true);
        StartCoroutine(FadeIn(gameOverScreen));
        Time.timeScale = 0f;
    }
    
    public void ShowVictory(int score, float time, int level)
    {
        HideAllScreens();
        victoryScreen.SetActive(true);
        StartCoroutine(FadeIn(victoryScreen));
        Time.timeScale = 0f;
    }
    
    public void ShowHowToPlay()
    {
        HideAllScreens();
        howToPlayScreen.SetActive(true);
        StartCoroutine(FadeIn(howToPlayScreen));
    }
    
    public void ShowSettings()
    {
        HideAllScreens();
        settingsScreen.SetActive(true);
        StartCoroutine(FadeIn(settingsScreen));
    }
    
    public void HideSettings()
    {
        settingsScreen.SetActive(false);
        startScreen.SetActive(true);
    }
    
    public void ShowGameModeScreen()
    {
        HideAllScreens();
        gameModeScreen.SetActive(true);
        StartCoroutine(FadeIn(gameModeScreen));
    }
    
    public void SelectGameMode(string mode)
    {
        Debug.Log($"Selected game mode: {mode}");
        PlayerPrefs.SetString("GameMode", mode);
        StartGame();
    }
    
    public void NextLevel()
    {
        LevelManager levelManager = FindObjectOfType<LevelManager>();
        if (levelManager != null)
        {
            levelManager.AdvanceToNextLevel();
        }
        HideAllScreens();
        Time.timeScale = 1f;
    }
    
    public void QuitGame()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
    
    void SetMasterVolume(float value)
    {
        if (audioManager != null) audioManager.SetMasterVolume(value);
        PlayerPrefs.SetFloat("MasterVolume", value);
    }
    
    void SetMusicVolume(float value)
    {
        if (audioManager != null) audioManager.SetMusicVolume(value);
        PlayerPrefs.SetFloat("MusicVolume", value);
    }
    
    void SetSFXVolume(float value)
    {
        if (audioManager != null) audioManager.SetSFXVolume(value);
        PlayerPrefs.SetFloat("SFXVolume", value);
    }
    
    void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
    }
    
    void HideAllScreens()
    {
        if (startScreen != null) startScreen.SetActive(false);
        if (pauseScreen != null) pauseScreen.SetActive(false);
        if (gameOverScreen != null) gameOverScreen.SetActive(false);
        if (victoryScreen != null) victoryScreen.SetActive(false);
        if (howToPlayScreen != null) howToPlayScreen.SetActive(false);
        if (settingsScreen != null) settingsScreen.SetActive(false);
        if (gameModeScreen != null) gameModeScreen.SetActive(false);
    }
    
    IEnumerator FadeIn(GameObject screen)
    {
        CanvasGroup canvasGroup = screen.GetComponent<CanvasGroup>();
        if (canvasGroup == null) yield break;
        
        canvasGroup.alpha = 0;
        float elapsed = 0f;
        float duration = 0.5f;
        
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, elapsed / duration);
            yield return null;
        }
        
        canvasGroup.alpha = 1;
    }
}

// Animation components
public class UIPulse : MonoBehaviour
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
        float scale = 1f + Mathf.Sin(Time.unscaledTime * 2f) * 0.05f;
        rt.localScale = originalScale * scale;
    }
}

public class UITwinkle : MonoBehaviour
{
    private Image image;
    private float randomOffset;
    
    void Start()
    {
        image = GetComponent<Image>();
        randomOffset = Random.Range(0f, 10f);
    }
    
    void Update()
    {
        Color color = image.color;
        color.a = 0.3f + Mathf.Sin(Time.unscaledTime * 2f + randomOffset) * 0.4f;
        image.color = color;
    }
}

public class UIButtonHover : MonoBehaviour
{
    private RectTransform rt;
    private Vector3 originalScale;
    
    void Start()
    {
        rt = GetComponent<RectTransform>();
        originalScale = rt.localScale;
    }
    
    public void OnPointerEnter()
    {
        rt.localScale = originalScale * 1.05f;
    }
    
    public void OnPointerExit()
    {
        rt.localScale = originalScale;
    }
    
    void Update()
    {
        // Smooth transition
        rt.localScale = Vector3.Lerp(rt.localScale, originalScale, Time.unscaledDeltaTime * 5f);
    }
}

