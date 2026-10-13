using UnityEngine;

// A coin: gives points when the hero touches it.
// Needs a Collider 2D with "Is Trigger" ticked.
public class Coin : MonoBehaviour
{
    public int value = 10;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameState.score += value;
            Destroy(gameObject);
        }
    }
}
