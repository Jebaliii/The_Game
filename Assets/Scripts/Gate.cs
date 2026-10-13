using UnityEngine;
using UnityEngine.SceneManagement;

// The gate at the end of level 1. If the hero has the key, it loads the next level.
// Needs a normal Collider 2D (NOT trigger) so it blocks the hero.
public class Gate : MonoBehaviour
{
    public string nextScene = "Level2";

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player") && GameState.hasKey)
        {
            GameState.hasKey = false;   // the key is used
            SceneManager.LoadScene(nextScene);
        }
    }
}
