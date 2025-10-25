using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Clash of Clans style fog of war system - clouds that fade away as you explore
/// </summary>
public class ClashStyleFogSystem : MonoBehaviour
{
    [Header("Fog Settings")]
    public bool enableFog = true;
    public Color fogColor = new Color(0.95f, 0.95f, 1f, 0.95f); // More opaque!
    public float cloudSize = 3f; // Bigger clouds!
    public int gridResolution = 1; // Denser coverage!
    
    [Header("Reveal Settings")]
    public float revealRadius = 4f;
    public float revealSpeed = 2f;
    public float fadeSpeed = 1.5f;
    
    [Header("Visual Effects")]
    public bool animateClouds = true;
    public float cloudFloatSpeed = 0.3f;
    public float cloudFloatAmount = 0.1f;
    public bool pulsateClouds = true;
    public float pulsateSpeed = 1f;
    public float pulsateAmount = 0.1f;
    
    private GameObject fogLayer;
    private List<FogCloud> fogClouds = new List<FogCloud>();
    private Transform player;
    private MazeGenerator mazeGenerator;
    private Camera mainCamera;
    
    private class FogCloud
    {
        public GameObject gameObject;
        public SpriteRenderer spriteRenderer;
        public Vector3 originalPosition;
        public float originalAlpha;
        public float currentAlpha;
        public bool isRevealed;
        public float randomOffset;
    }
    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        mazeGenerator = FindFirstObjectByType<MazeGenerator>();
        mainCamera = Camera.main;
        
        Debug.Log($"🌫 ClashStyleFogSystem initialized! EnableFog: {enableFog}");
        
        // Check if GameInitializer exists and game has started
        GameInitializer gameInit = GameInitializer.Instance;
        
        if (enableFog)
        {
            if (gameInit != null && gameInit.IsGameStarted())
            {
                // Game already started, create fog soon
                Debug.Log("   📍 Game started - creating fog in 1 second");
                Invoke("CreateFogSystem", 1f);
            }
            else if (gameInit != null)
            {
                // Game not started yet (menu mode), wait for game to start
                Debug.Log("   📍 Menu mode - creating fog in 2 seconds");
                Invoke("CreateFogSystem", 2f);
            }
            else
            {
                // No GameInitializer, create fog soon
                Debug.Log("   📍 No GameInitializer - creating fog in 1.5 seconds");
                Invoke("CreateFogSystem", 1.5f);
            }
        }
        else
        {
            Debug.LogWarning("   ⚠️ Fog is disabled (enableFog = false)");
        }
    }
    
    // Method to force fog creation (can be called externally)
    public void ForceCreateFog()
    {
        if (enableFog && fogLayer == null)
        {
            CreateFogSystem();
        }
    }
    
    void CreateFogSystem()
    {
        Debug.Log("🌫 CreateFogSystem called...");
        
        // Don't create fog if it already exists
        if (fogLayer != null)
        {
            Debug.LogWarning("   ⚠️ Fog layer already exists!");
            return;
        }
        
        // Wait for player to exist
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
            if (player == null)
            {
                Debug.LogWarning("   ⚠️ Player not found yet, retrying in 0.5s...");
                Invoke("CreateFogSystem", 0.5f);
                return;
            }
        }
        
        // Refresh maze generator reference
        if (mazeGenerator == null)
        {
            mazeGenerator = FindFirstObjectByType<MazeGenerator>();
        }
        
        // Create fog layer
        Debug.Log("   📦 Creating fog layer GameObject...");
        fogLayer = new GameObject("ClashStyleFogLayer");
        fogLayer.transform.position = Vector3.zero;
        
        if (mazeGenerator == null)
        {
            Debug.LogWarning("   ⚠️ MazeGenerator not found! Creating fog over camera view.");
            CreateFogOverCameraView();
        }
        else
        {
            Debug.Log("   📍 Creating fog over maze...");
            CreateFogOverMaze();
        }
        
        if (fogClouds.Count > 0)
        {
            Debug.Log($"✅ Clash-style fog created! {fogClouds.Count} clouds");
            Debug.Log($"   🎨 Fog color: {fogColor}");
            Debug.Log($"   ☁️ Cloud size: {cloudSize}");
        }
        else
        {
            Debug.LogError("❌ No fog clouds were created!");
        }
    }
    
    void CreateFogOverMaze()
    {
        // Get maze bounds
        int mazeWidth = mazeGenerator.mazeWidth;
        int mazeHeight = mazeGenerator.mazeHeight;
        float cellSize = mazeGenerator.cellSize;
        int gladeSize = mazeGenerator.gladeSize;
        
        // Calculate world bounds
        float startX = -mazeWidth * cellSize / 2f;
        float startY = -mazeHeight * cellSize / 2f;
        float endX = mazeWidth * cellSize / 2f;
        float endY = mazeHeight * cellSize / 2f;
        
        // Calculate glade bounds (center area to exclude)
        float gladeCenterX = 0f;
        float gladeCenterY = 0f;
        float gladeRadius = (gladeSize * cellSize / 2f) + 1f; // Slightly larger to ensure no fog
        
        // Create clouds in a grid, skipping glade area
        for (float x = startX; x < endX; x += cloudSize / gridResolution)
        {
            for (float y = startY; y < endY; y += cloudSize / gridResolution)
            {
                // Check if position is in glade
                float distToGlade = Vector2.Distance(new Vector2(x, y), new Vector2(gladeCenterX, gladeCenterY));
                
                if (distToGlade > gladeRadius)
                {
                    // Outside glade - create fog
                    Vector3 position = new Vector3(
                        x + Random.Range(-0.1f, 0.1f),
                        y + Random.Range(-0.1f, 0.1f),
                        0
                    );
                    
                    CreateFogCloud(position);
                }
            }
        }
        
        Debug.Log($"Fog created outside glade (radius: {gladeRadius})");
    }
    
    void CreateFogOverCameraView()
    {
        if (mainCamera == null) return;
        
        float height = mainCamera.orthographicSize * 2f;
        float width = height * mainCamera.aspect;
        
        Vector3 camPos = mainCamera.transform.position;
        
        for (float x = -width; x < width; x += cloudSize / gridResolution)
        {
            for (float y = -height; y < height; y += cloudSize / gridResolution)
            {
                Vector3 position = new Vector3(
                    camPos.x + x + Random.Range(-0.1f, 0.1f),
                    camPos.y + y + Random.Range(-0.1f, 0.1f),
                    0
                );
                
                CreateFogCloud(position);
            }
        }
    }
    
    void CreateFogCloud(Vector3 position)
    {
        GameObject cloudGO = new GameObject($"FogCloud_{fogClouds.Count}");
        cloudGO.transform.SetParent(fogLayer.transform);
        cloudGO.transform.position = position;
        cloudGO.transform.localScale = Vector3.one * cloudSize;
        
        // Create sprite renderer
        SpriteRenderer sr = cloudGO.AddComponent<SpriteRenderer>();
        sr.sprite = CreateCloudSprite();
        sr.color = fogColor;
        
        // Use Default sorting layer with high order (above walls/player)
        // This ensures fog appears even if custom sorting layers don't exist
        sr.sortingLayerName = "Default";
        sr.sortingOrder = 100; // Above walls (0), player (10), but below UI
        
        // Create fog cloud data
        FogCloud cloud = new FogCloud
        {
            gameObject = cloudGO,
            spriteRenderer = sr,
            originalPosition = position,
            originalAlpha = fogColor.a,
            currentAlpha = fogColor.a,
            isRevealed = false,
            randomOffset = Random.Range(0f, 10f)
        };
        
        fogClouds.Add(cloud);
    }
    
    Sprite CreateCloudSprite()
    {
        int resolution = 128;
        Texture2D texture = new Texture2D(resolution, resolution);
        Color[] pixels = new Color[resolution * resolution];
        
        Vector2 center = new Vector2(resolution / 2f, resolution / 2f);
        float maxRadius = resolution / 2f;
        
        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                Vector2 pos = new Vector2(x, y);
                float distance = Vector2.Distance(pos, center);
                
                // Create soft cloud shape with Perlin noise
                float noise = Mathf.PerlinNoise(x * 0.05f, y * 0.05f);
                float radius = maxRadius * (0.7f + noise * 0.3f);
                
                if (distance < radius)
                {
                    // Soft falloff at edges
                    float alpha = 1f - (distance / radius);
                    alpha = Mathf.Pow(alpha, 2f); // Smooth falloff
                    alpha *= (0.8f + noise * 0.2f); // Add variation
                    
                    pixels[y * resolution + x] = new Color(1f, 1f, 1f, alpha);
                }
                else
                {
                    pixels[y * resolution + x] = Color.clear;
                }
            }
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        texture.filterMode = FilterMode.Bilinear;
        
        return Sprite.Create(texture, new Rect(0, 0, resolution, resolution), 
            new Vector2(0.5f, 0.5f), resolution / cloudSize);
    }
    
    void Update()
    {
        if (!enableFog || player == null || fogClouds.Count == 0) return;
        
        Vector3 playerPos = player.position;
        
        foreach (FogCloud cloud in fogClouds)
        {
            if (cloud == null || cloud.gameObject == null) continue;
            
            float distance = Vector2.Distance(
                new Vector2(cloud.originalPosition.x, cloud.originalPosition.y),
                new Vector2(playerPos.x, playerPos.y)
            );
            
            // Check if player is close enough to reveal
            if (distance < revealRadius)
            {
                if (!cloud.isRevealed)
                {
                    cloud.isRevealed = true;
                }
                
                // Fade out cloud
                cloud.currentAlpha = Mathf.Lerp(cloud.currentAlpha, 0f, Time.deltaTime * fadeSpeed);
            }
            else
            {
                // Cloud is far, keep visible if not revealed
                if (!cloud.isRevealed)
                {
                    cloud.currentAlpha = Mathf.Lerp(cloud.currentAlpha, cloud.originalAlpha, 
                        Time.deltaTime * revealSpeed);
                }
            }
            
            // Apply alpha
            Color cloudColor = cloud.spriteRenderer.color;
            cloudColor.a = cloud.currentAlpha;
            cloud.spriteRenderer.color = cloudColor;
            
            // Animate cloud
            if (animateClouds || pulsateClouds)
            {
                AnimateCloud(cloud);
            }
            
            // Destroy cloud if fully transparent
            if (cloud.currentAlpha < 0.01f && cloud.isRevealed)
            {
                Destroy(cloud.gameObject);
            }
        }
        
        // Remove destroyed clouds from list
        fogClouds.RemoveAll(c => c == null || c.gameObject == null);
        
        // Toggle fog with F key
        if (Input.GetKeyDown(KeyCode.F))
        {
            ToggleFog();
        }
    }
    
    void AnimateCloud(FogCloud cloud)
    {
        Vector3 offset = Vector3.zero;
        float scale = 1f;
        
        // Floating animation
        if (animateClouds)
        {
            offset.x = Mathf.Sin(Time.time * cloudFloatSpeed + cloud.randomOffset) * cloudFloatAmount;
            offset.y = Mathf.Cos(Time.time * cloudFloatSpeed * 0.7f + cloud.randomOffset) * cloudFloatAmount;
        }
        
        // Pulsating animation
        if (pulsateClouds)
        {
            scale = 1f + Mathf.Sin(Time.time * pulsateSpeed + cloud.randomOffset) * pulsateAmount;
        }
        
        cloud.gameObject.transform.position = cloud.originalPosition + offset;
        cloud.gameObject.transform.localScale = Vector3.one * cloudSize * scale;
    }
    
    public void ToggleFog()
    {
        enableFog = !enableFog;
        
        if (fogLayer != null)
        {
            fogLayer.SetActive(enableFog);
        }
        
        Debug.Log($"Fog {(enableFog ? "enabled" : "disabled")}");
    }
    
    public void RevealAll()
    {
        foreach (FogCloud cloud in fogClouds)
        {
            if (cloud != null && cloud.gameObject != null)
            {
                cloud.isRevealed = true;
                cloud.currentAlpha = 0f;
            }
        }
        
        Debug.Log("All fog revealed!");
    }
    
    public void ResetFog()
    {
        foreach (FogCloud cloud in fogClouds)
        {
            if (cloud != null && cloud.gameObject != null)
            {
                cloud.isRevealed = false;
                cloud.currentAlpha = cloud.originalAlpha;
            }
        }
        
        Debug.Log("Fog reset!");
    }
    
    public void SetRevealRadius(float radius)
    {
        revealRadius = Mathf.Max(1f, radius);
    }
    
    public void SetFogColor(Color color)
    {
        fogColor = color;
        
        foreach (FogCloud cloud in fogClouds)
        {
            if (cloud != null && cloud.spriteRenderer != null)
            {
                Color cloudColor = cloud.spriteRenderer.color;
                cloudColor.r = color.r;
                cloudColor.g = color.g;
                cloudColor.b = color.b;
                cloud.spriteRenderer.color = cloudColor;
            }
        }
    }
    
    void OnDrawGizmosSelected()
    {
        if (player != null)
        {
            // Draw reveal radius
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(player.position, revealRadius);
        }
    }
    
    [ContextMenu("Reveal All Fog")]
    void ContextRevealAll()
    {
        RevealAll();
    }
    
    [ContextMenu("Reset Fog")]
    void ContextResetFog()
    {
        ResetFog();
    }
}



