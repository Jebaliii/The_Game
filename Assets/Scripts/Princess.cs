using UnityEngine;
using UnityEngine.SceneManagement;

// The princess: when the hero reaches her, the player wins.
// Needs a Collider 2D with "Is Trigger" ticked.
public class Princess : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene("Win");
        }
    }
}
