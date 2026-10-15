using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

// Used in the MainMenu, GameOver and Win scenes.
// Put it on an empty object called "MenuManager" and connect the buttons to its functions.
// The MainMenu has 4 pages: main, difficulty, how to play, settings. Only one is shown at a time.
public class MenuManager : MonoBehaviour
{
    [Header("Pages (only in MainMenu)")]
    public GameObject mainPanel;
    public GameObject difficultyPanel;
    public GameObject instructionsPanel;
    public GameObject settingsPanel;

    [Header("Settings texts (only in MainMenu)")]
    public TMP_Text volumeText;
    public TMP_Text shakeText;
    public TMP_Text fullscreenText;

    [Header("Only in GameOver and Win")]
    public TMP_Text finalScoreText;

    void Start()
    {
        Time.timeScale = 1f;
        if (mainPanel != null) ShowPanel(mainPanel);
        else if (instructionsPanel != null) instructionsPanel.SetActive(false);
        if (finalScoreText != null) finalScoreText.text = "Score: " + GameState.score;
        RefreshSettings();
    }

    // ---------- Pages ----------

    public void ShowMain() { ShowPanel(mainPanel); }
    public void ShowDifficulty() { ShowPanel(difficultyPanel); }
    public void ShowSettings() { ShowPanel(settingsPanel); RefreshSettings(); }
    public void ShowInstructions() { ShowPanel(instructionsPanel); }
    public void HideInstructions() { ShowPanel(mainPanel); }

    // Show one page and hide the others
    void ShowPanel(GameObject panel)
    {
        GameObject[] pages = { mainPanel, difficultyPanel, instructionsPanel, settingsPanel };
        foreach (GameObject page in pages)
            if (page != null) page.SetActive(page == panel);
    }

    // ---------- Play ----------

    public void PlayEasy() { StartGame(GameState.Easy); }
    public void PlayMedium() { StartGame(GameState.Medium); }
    public void PlayHard() { StartGame(GameState.Hard); }

    void StartGame(int difficulty)
    {
        GameState.difficulty = difficulty;
        Play();
    }

    // Retry button (GameOver / Win): new game with the same difficulty
    public void Play()
    {
        GameState.NewGame();
        SceneManager.LoadScene("Level1");
    }

    public void BackToMenu() { SceneManager.LoadScene("MainMenu"); }

    public void Quit()
    {
        Application.Quit();   // works in the built game, not in the editor
    }

    // ---------- Settings ----------

    public void VolumeDown() { Settings.Volume -= 0.1f; RefreshSettings(); }
    public void VolumeUp() { Settings.Volume += 0.1f; RefreshSettings(); }
    public void ToggleShake() { Settings.ScreenShake = !Settings.ScreenShake; RefreshSettings(); }
    public void ToggleFullscreen()
    {
        // Unity changes the screen at the end of the frame, so show the new value ourselves
        bool on = !Settings.Fullscreen;
        Settings.Fullscreen = on;
        if (fullscreenText != null) fullscreenText.text = Settings.FullscreenText(on);
    }

    void RefreshSettings()
    {
        if (volumeText != null) volumeText.text = Settings.VolumeText();
        if (shakeText != null) shakeText.text = Settings.ShakeText();
        if (fullscreenText != null) fullscreenText.text = Settings.FullscreenText(Settings.Fullscreen);
    }
}
