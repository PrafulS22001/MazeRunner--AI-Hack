using UnityEngine;

public class OptimizedSpiderAI : MonoBehaviour
{
    [Header("Optimization Settings")]
    public float updateInterval = 0.1f; // Update every 0.1 seconds instead of every frame
    public float pathfindingInterval = 0.5f; // Pathfinding every 0.5 seconds
    public bool enableLOD = true;
    public float lodDistance = 10f;
    
    private SpiderAI spiderAI;
    private AdvancedSpiderAI advancedSpiderAI;
    private Transform player;
    private Camera mainCamera;
    private float lastUpdate = 0f;
    private float lastPathfindingUpdate = 0f;
    private bool isInLODRange = true;
    
    void Start()
    {
        spiderAI = GetComponent<SpiderAI>();
        advancedSpiderAI = GetComponent<AdvancedSpiderAI>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        mainCamera = Camera.main;
        
        if (mainCamera == null)
        {
            mainCamera = FindObjectOfType<Camera>();
        }
    }
    
    void Update()
    {
        if (Time.time - lastUpdate < updateInterval) return;
        
        lastUpdate = Time.time;
        
        // Check LOD (Level of Detail)
        if (enableLOD)
        {
            CheckLOD();
        }
        
        // Only update AI if in LOD range
        if (isInLODRange)
        {
            UpdateAI();
        }
    }
    
    void CheckLOD()
    {
        if (mainCamera == null || player == null) return;
        
        float distanceToCamera = Vector3.Distance(mainCamera.transform.position, transform.position);
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        // Use the closer distance for LOD calculation
        float closestDistance = Mathf.Min(distanceToCamera, distanceToPlayer);
        
        bool wasInLODRange = isInLODRange;
        isInLODRange = closestDistance <= lodDistance;
        
        // If LOD status changed, update AI behavior
        if (wasInLODRange != isInLODRange)
        {
            OnLODStatusChanged(isInLODRange);
        }
    }
    
    void OnLODStatusChanged(bool inRange)
    {
        if (spiderAI != null)
        {
            // Enable/disable spider AI based on LOD
            spiderAI.enabled = inRange;
        }
        
        if (advancedSpiderAI != null)
        {
            // Adjust update frequency based on LOD
            advancedSpiderAI.pathfindingUpdateInterval = inRange ? 0.5f : 2f;
        }
        
        // Disable/enable renderer for distant spiders
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.enabled = inRange;
        }
        
        // Disable/enable collider for distant spiders
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = inRange;
        }
    }
    
    void UpdateAI()
    {
        // This method can be overridden by specific AI behaviors
        // For now, it just ensures the AI components are active
        
        if (spiderAI != null && !spiderAI.enabled)
        {
            spiderAI.enabled = true;
        }
        
        if (advancedSpiderAI != null && !advancedSpiderAI.enabled)
        {
            advancedSpiderAI.enabled = true;
        }
    }
    
    public void ForceUpdate()
    {
        // Force an immediate update (useful for important events)
        lastUpdate = 0f;
        lastPathfindingUpdate = 0f;
    }
    
    public void SetUpdateInterval(float interval)
    {
        updateInterval = Mathf.Max(0.01f, interval);
    }
    
    public void SetLODDistance(float distance)
    {
        lodDistance = Mathf.Max(1f, distance);
    }
    
    public bool IsInLODRange()
    {
        return isInLODRange;
    }
}
