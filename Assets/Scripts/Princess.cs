using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

// The princess: when the hero reaches her, the player wins.
// If a win pop-up is set (Level 2), it is shown. Otherwise the Win scene is loaded.
// Needs a Collider 2D with "Is Trigger" ticked.
public class Princess : MonoBehaviour
{
    public GameObject winPanel;      // the "You win!" pop-up (hidden at the start)
    public TMP_Text winScoreText;    // "SCORE 750" inside the pop-up

    private bool saved = false;

    void Start()
    {
        if (winPanel != null) winPanel.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (saved || !other.CompareTag("Player")) return;
        saved = true;

        if (winPanel == null)
        {
            SceneManager.LoadScene("Win");
            return;
        }

        if (winScoreText != null) winScoreText.text = "SCORE <b>" + GameState.score + "</b>";
        winPanel.SetActive(true);
        SoundFX.PlayKey();
        Time.timeScale = 0f;   // stop the monsters behind the pop-up
    }

    // "Play again" button of the pop-up
    public void PlayAgain()
    {
        Time.timeScale = 1f;
        GameState.NewGame();
        SceneManager.LoadScene("Level1");
    }
}
