using UnityEngine;

public class BushScript : MonoBehaviour
{
    public bool playerInside = false;
    private SpriteRenderer playerRenderer;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            playerRenderer = other.GetComponent<SpriteRenderer>();
            if (playerRenderer != null)
            {
                // Make player semi-transparent when hiding
                playerRenderer.color = new Color(1, 1, 1, 0.4f);
            }
            Debug.Log("Player entered bush - hiding!");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            if (playerRenderer != null)
            {
                // Restore player visibility
                playerRenderer.color = Color.white;
            }
            Debug.Log("Player left bush - visible!");
        }
    }
}