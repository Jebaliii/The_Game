using UnityEngine;

// The key of the level. Needs a Collider 2D with "Is Trigger" ticked.
public class KeyPickup : MonoBehaviour
{
    public int value = 100;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameState.hasKey = true;
            GameState.score += value;
            Destroy(gameObject);
        }
    }
}
