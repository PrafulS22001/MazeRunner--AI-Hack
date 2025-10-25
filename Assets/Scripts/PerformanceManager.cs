using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class PerformanceManager : MonoBehaviour
{
    [Header("Performance Settings")]
    public int targetFrameRate = 60;
    public float objectPoolSize = 100;
    public float cullingDistance = 20f;
    public bool enableObjectPooling = true;
    public bool enableLODSystem = true;
    
    [Header("Debug Info")]
    public bool showPerformanceStats = false;
    public Text performanceText;
    
    private Dictionary<string, Queue<GameObject>> objectPools = new Dictionary<string, Queue<GameObject>>();
    private List<GameObject> activeObjects = new List<GameObject>();
    private float frameTime = 0f;
    private int frameCount = 0;
    private float fps = 0f;
    private Camera mainCamera;
    
    private static PerformanceManager instance;
    
    public static PerformanceManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PerformanceManager>();
                if (instance == null)
                {
                    GameObject perfManager = new GameObject("PerformanceManager");
                    instance = perfManager.AddComponent<PerformanceManager>();
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
            InitializePerformance();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        Application.targetFrameRate = targetFrameRate;
        
        if (enableObjectPooling)
        {
            InitializeObjectPools();
        }
        
        if (showPerformanceStats)
        {
            CreatePerformanceUI();
        }
    }
    
    void Update()
    {
        UpdatePerformanceStats();
        
        if (showPerformanceStats && performanceText != null)
        {
            UpdatePerformanceUI();
        }
        
        // Optimize active objects
        OptimizeActiveObjects();
    }
    
    void InitializePerformance()
    {
        // Set quality settings for better performance
        QualitySettings.vSyncCount = 0;
        QualitySettings.antiAliasing = 0;
        QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
        
        Debug.Log("Performance Manager initialized");
    }
    
    void InitializeObjectPools()
    {
        // Pre-create pools for common objects
        CreateObjectPool("Particle", 50);
        CreateObjectPool("PowerUp", 20);
        CreateObjectPool("Spider", 10);
        CreateObjectPool("Wall", 200);
        
        Debug.Log("Object pools initialized");
    }
    
    void CreateObjectPool(string objectType, int poolSize)
    {
        if (!objectPools.ContainsKey(objectType))
        {
            objectPools[objectType] = new Queue<GameObject>();
        }
        
        // Pre-instantiate objects for the pool
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = new GameObject($"{objectType}_Pooled");
            obj.SetActive(false);
            objectPools[objectType].Enqueue(obj);
        }
    }
    
    public GameObject GetPooledObject(string objectType)
    {
        if (!enableObjectPooling || !objectPools.ContainsKey(objectType))
        {
            return null;
        }
        
        Queue<GameObject> pool = objectPools[objectType];
        
        if (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            obj.SetActive(true);
            activeObjects.Add(obj);
            return obj;
        }
        
        // If pool is empty, create a new object
        GameObject newObj = new GameObject($"{objectType}_New");
        activeObjects.Add(newObj);
        return newObj;
    }
    
    public void ReturnToPool(GameObject obj, string objectType)
    {
        if (!enableObjectPooling || !objectPools.ContainsKey(objectType))
        {
            Destroy(obj);
            return;
        }
        
        obj.SetActive(false);
        activeObjects.Remove(obj);
        objectPools[objectType].Enqueue(obj);
    }
    
    void OptimizeActiveObjects()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        
        if (mainCamera == null) return;
        
        // Cull objects that are too far from camera
        for (int i = activeObjects.Count - 1; i >= 0; i--)
        {
            if (activeObjects[i] == null)
            {
                activeObjects.RemoveAt(i);
                continue;
            }
            
            float distance = Vector3.Distance(mainCamera.transform.position, activeObjects[i].transform.position);
            
            if (distance > cullingDistance)
            {
                // Disable renderer if it has one
                Renderer renderer = activeObjects[i].GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.enabled = false;
                }
            }
            else
            {
                // Re-enable renderer
                Renderer renderer = activeObjects[i].GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.enabled = true;
                }
            }
        }
    }
    
    void UpdatePerformanceStats()
    {
        frameTime += Time.deltaTime;
        frameCount++;
        
        if (frameTime >= 1f)
        {
            fps = frameCount / frameTime;
            frameTime = 0f;
            frameCount = 0;
        }
    }
    
    void UpdatePerformanceUI()
    {
        if (performanceText == null) return;
        
        string stats = $"FPS: {fps:F1}\n";
        stats += $"Active Objects: {activeObjects.Count}\n";
        stats += $"Memory: {System.GC.GetTotalMemory(false) / 1024 / 1024:F1} MB\n";
        string renderPipeline = UnityEngine.Rendering.GraphicsSettings.defaultRenderPipeline != null ? "URP" : "Built-in";
        stats += $"Draw Calls: {renderPipeline}\n";
        
        performanceText.text = stats;
    }
    
    void CreatePerformanceUI()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasGO = new GameObject("PerformanceCanvas");
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
        }
        
        GameObject textGO = new GameObject("PerformanceText");
        textGO.transform.SetParent(canvas.transform, false);
        performanceText = textGO.AddComponent<Text>();
        
        // Setup text component
        try
        {
            performanceText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        }
        catch
        {
            performanceText.font = Resources.Load<Font>("Arial");
        }
        performanceText.fontSize = 14;
        performanceText.color = Color.white;
        
        // Position in top-left corner
        RectTransform rectTransform = performanceText.rectTransform;
        rectTransform.anchorMin = new Vector2(0f, 1f);
        rectTransform.anchorMax = new Vector2(0f, 1f);
        rectTransform.pivot = new Vector2(0f, 1f);
        rectTransform.anchoredPosition = new Vector2(10f, -10f);
        rectTransform.sizeDelta = new Vector2(200f, 100f);
    }
    
    public void OptimizeMazeGeneration()
    {
        // Optimize maze generation by reducing unnecessary operations
        MazeGenerator mazeGen = FindObjectOfType<MazeGenerator>();
        if (mazeGen != null)
        {
            // Reduce debug logging in release builds
            #if !UNITY_EDITOR
            Debug.unityLogger.logEnabled = false;
            #endif
        }
    }
    
    public void OptimizeSpiderAI()
    {
        // Reduce AI update frequency for better performance
        SpiderAI[] spiders = FindObjectsOfType<SpiderAI>();
        foreach (SpiderAI spider in spiders)
        {
            // Add update interval to spider AI
            if (spider.GetComponent<OptimizedSpiderAI>() == null)
            {
                spider.gameObject.AddComponent<OptimizedSpiderAI>();
            }
        }
        
        AdvancedSpiderAI[] advancedSpiders = FindObjectsOfType<AdvancedSpiderAI>();
        foreach (AdvancedSpiderAI spider in advancedSpiders)
        {
            // Reduce pathfinding update frequency
            spider.pathfindingUpdateInterval = Mathf.Max(0.5f, spider.pathfindingUpdateInterval);
        }
    }
    
    public void CleanupUnusedObjects()
    {
        // Clean up unused objects
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("(Clone)") && !obj.activeInHierarchy)
            {
                Destroy(obj);
            }
        }
        
        // Force garbage collection
        System.GC.Collect();
        
        Debug.Log("Cleaned up unused objects");
    }
    
    public void SetTargetFrameRate(int fps)
    {
        targetFrameRate = fps;
        Application.targetFrameRate = fps;
    }
    
    public float GetFPS()
    {
        return fps;
    }
    
    public int GetActiveObjectCount()
    {
        return activeObjects.Count;
    }
    
    public void TogglePerformanceStats()
    {
        showPerformanceStats = !showPerformanceStats;
        
        if (performanceText != null)
        {
            performanceText.gameObject.SetActive(showPerformanceStats);
        }
    }
    
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            // Reduce performance when paused
            SetTargetFrameRate(30);
        }
        else
        {
            // Restore normal performance
            SetTargetFrameRate(targetFrameRate);
        }
    }
    
    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            // Reduce performance when not focused
            SetTargetFrameRate(30);
        }
        else
        {
            // Restore normal performance
            SetTargetFrameRate(targetFrameRate);
        }
    }
}
