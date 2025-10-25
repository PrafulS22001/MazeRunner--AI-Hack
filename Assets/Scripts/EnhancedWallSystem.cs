using UnityEngine;

/// <summary>
/// Enhanced Wall System - Makes walls look interesting, not just basic squares
/// Adds visual variation: different shades, slight scale variations, roughness
/// </summary>
public class EnhancedWallSystem : MonoBehaviour
{
    [Header("Visual Enhancement")]
    [Tooltip("Add random color variation to walls")]
    public bool addColorVariation = true;
    
    [Tooltip("Add random scale variation to walls")]
    public bool addScaleVariation = true;
    
    [Tooltip("Add random rotation for roughness")]
    public bool addRotationVariation = true;
    
    [Header("Stone Wall Colors")]
    public Color baseStoneColor = new Color(0.6f, 0.55f, 0.5f);
    public Color darkStoneColor = new Color(0.5f, 0.45f, 0.4f);
    public Color lightStoneColor = new Color(0.7f, 0.65f, 0.6f);
    
    [Header("Variation Amount")]
    [Range(0f, 0.2f)]
    public float colorVariationAmount = 0.15f;
    
    [Range(0f, 0.3f)]
    public float scaleVariationAmount = 0.15f;
    
    [Range(0f, 10f)]
    public float rotationVariationDegrees = 5f;
    
    [Header("Auto-Run")]
    public bool enhanceOnStart = true;
    
    void Start()
    {
        if (enhanceOnStart)
        {
            Invoke("EnhanceAllWalls", 2f);
            Invoke("EnhanceAllWalls", 4f);
        }
    }
    
    [ContextMenu("🎨 Enhance All Walls")]
    public void EnhanceAllWalls()
    {
        Debug.Log("🎨 ENHANCING WALLS - Making them look awesome!");
        
        GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
        
        if (walls.Length == 0)
        {
            Debug.LogWarning("   ⚠️ No walls found with 'Wall' tag");
            return;
        }
        
        int enhanced = 0;
        
        foreach (GameObject wall in walls)
        {
            if (wall == null) continue;
            
            // Skip exit markers
            if (wall.name.Contains("EXIT")) continue;
            
            EnhanceWall(wall);
            enhanced++;
        }
        
        Debug.Log($"   ✅ Enhanced {enhanced} walls!");
        Debug.Log("   Walls now have varied colors, scales, and rotations");
    }
    
    void EnhanceWall(GameObject wall)
    {
        SpriteRenderer sr = wall.GetComponent<SpriteRenderer>();
        if (sr == null) return;
        
        // Apply color variation
        if (addColorVariation)
        {
            // Choose random variation of stone color
            float random = Random.value;
            Color chosenColor;
            
            if (random < 0.33f)
                chosenColor = darkStoneColor;
            else if (random < 0.66f)
                chosenColor = baseStoneColor;
            else
                chosenColor = lightStoneColor;
            
            // Add slight random variation
            float rVariation = Random.Range(-colorVariationAmount, colorVariationAmount);
            float gVariation = Random.Range(-colorVariationAmount, colorVariationAmount);
            float bVariation = Random.Range(-colorVariationAmount, colorVariationAmount);
            
            sr.color = new Color(
                Mathf.Clamp01(chosenColor.r + rVariation),
                Mathf.Clamp01(chosenColor.g + gVariation),
                Mathf.Clamp01(chosenColor.b + bVariation),
                1f
            );
        }
        
        // Apply scale variation
        if (addScaleVariation)
        {
            float scaleX = 1f + Random.Range(-scaleVariationAmount, scaleVariationAmount);
            float scaleY = 1f + Random.Range(-scaleVariationAmount, scaleVariationAmount);
            
            wall.transform.localScale = new Vector3(scaleX, scaleY, 1f);
        }
        
        // Apply rotation variation (makes walls look less uniform)
        if (addRotationVariation)
        {
            float rotation = Random.Range(-rotationVariationDegrees, rotationVariationDegrees);
            wall.transform.rotation = Quaternion.Euler(0f, 0f, rotation);
        }
    }
    
    [ContextMenu("🔄 Reset All Walls to Default")]
    public void ResetAllWalls()
    {
        Debug.Log("🔄 Resetting all walls to default appearance");
        
        GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
        
        foreach (GameObject wall in walls)
        {
            if (wall == null) continue;
            if (wall.name.Contains("EXIT")) continue;
            
            SpriteRenderer sr = wall.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.color = baseStoneColor;
            }
            
            wall.transform.localScale = Vector3.one;
            wall.transform.rotation = Quaternion.identity;
        }
        
        Debug.Log("   ✅ All walls reset to default");
    }
}

