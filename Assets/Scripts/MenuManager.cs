using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

// Used in the MainMenu, GameOver and Win scenes.
// Put it on an empty object called "MenuManager" and connect the buttons to its functions.
public class MenuManager : MonoBehaviour
{
    public GameObject instructionsPanel;  // only in MainMenu
    public TMP_Text finalScoreText;       // only in GameOver and Win

    void Start()
    {
        Time.timeScale = 1f;
        if (instructionsPanel != null) instructionsPanel.SetActive(false);
        if (finalScoreText != null) finalScoreText.text = "Score: " + GameState.score;
    }

    // Play button and Retry button
    public void Play()
    {
        GameState.NewGame();
        SceneManager.LoadScene("Level1");
    }

    public void ShowInstructions() { instructionsPanel.SetActive(true); }
    public void HideInstructions() { instructionsPanel.SetActive(false); }

    public void BackToMenu() { SceneManager.LoadScene("MainMenu"); }

    public void Quit()
    {
        Application.Quit();   // works in the built game, not in the editor
    }
}
