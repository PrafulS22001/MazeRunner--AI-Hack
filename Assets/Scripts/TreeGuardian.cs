using UnityEngine;

/// <summary>
/// TreeGuardian - Ensures trees NEVER move or chase the player
/// Attach this to Tree prefab to guarantee static behavior
/// </summary>
public class TreeGuardian : MonoBehaviour
{
    void Awake()
    {
        // Immediately destroy any movement components that shouldn't be here
        SpiderAI spiderAI = GetComponent<SpiderAI>();
        if (spiderAI != null)
        {
            Debug.LogError($"❌ TREE '{name}' HAD SPIDER AI! Destroying it!");
            Destroy(spiderAI);
        }
        
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Debug.LogError($"❌ TREE '{name}' HAD RIGIDBODY2D! Destroying it!");
            Destroy(rb);
        }
        
        // Ensure collider is solid, not trigger
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        if (col != null && col.isTrigger)
        {
            Debug.LogWarning($"⚠️ TREE '{name}' collider was trigger! Fixing...");
            col.isTrigger = false;
        }
        
        // Make absolutely sure this tree is on correct layer
        if (gameObject.layer != 7) // Layer 7 = Environment
        {
            gameObject.layer = 7;
        }
        
        // Tag it properly
        if (!gameObject.CompareTag("Tree"))
        {
            try
            {
                gameObject.tag = "Tree";
            }
            catch (System.Exception)
            {
                // Tag doesn't exist, that's okay
            }
        }
    }
    
    void Update()
    {
        // Paranoid check: If any movement components get added at runtime, destroy them
        SpiderAI spiderCheck = GetComponent<SpiderAI>();
        if (spiderCheck != null)
        {
            Destroy(spiderCheck);
        }
        
        Rigidbody2D rbCheck = GetComponent<Rigidbody2D>();
        if (rbCheck != null)
        {
            Destroy(rbCheck);
        }
    }
}

