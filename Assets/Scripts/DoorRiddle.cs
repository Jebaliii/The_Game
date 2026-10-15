using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

// The locked door of level 2. The hero must collect ALL the coins and kill ALL the monsters first.
// Then, when the hero touches it, a pop-up asks a question.
// Correct answer -> the door opens (the princess is behind it).
// Wrong answer -> the game restarts from the beginning (Level 1, new game).
// Needs a normal Collider 2D (NOT trigger) so it blocks the hero.
public class DoorRiddle : MonoBehaviour
{
    [Header("UI")]
    public GameObject riddlePanel;     // the pop-up (hidden at the start)
    public TMP_Text questionText;
    public Button[] answerButtons;     // the 3 buttons, left to right
    public TMP_Text[] answerTexts;     // the texts of the 3 buttons
    public TMP_Text feedbackText;      // "Wrong answer!" (empty at the start)

    [Header("Question")]
    [TextArea] public string question = "How many monsters were in Level 1?";
    public string[] answers = { "3", "4", "5" };
    public int correctAnswer = 1;      // 0 = first button, 1 = second, 2 = third
                                       // (both are set by the game when you come from Level 1)

    [Header("Result")]
    public int scoreReward = 200;
    public string restartScene = "Level1";
    public float restartDelay = 2f;    // seconds to read "Wrong answer!" before the restart

    private bool asking = false;
    private int totalCoins;
    private int totalMonsters;

    void Start()
    {
        if (riddlePanel != null) riddlePanel.SetActive(false);

        // Count everything at the start of the level
        totalCoins = LevelGoals.CoinCount();
        totalMonsters = LevelGoals.MonsterCount();
        MessageUI.Show("Kill all the monsters and collect all the coins to open the door!");
    }

    // Called every frame while the hero pushes against the door
    void OnCollisionStay2D(Collision2D collision)
    {
        if (asking || !collision.collider.CompareTag("Player")) return;
        if (!LevelGoals.CheckDone(totalCoins, totalMonsters)) return;   // shows the progress message
        OpenRiddle();
    }

    void OpenRiddle()
    {
        asking = true;

        // The real number of monsters of Level 1 (counted when Level 1 started).
        // The right answer is put on a random button, the 2 others are the numbers just around it.
        // If Level 2 was started directly (no Level 1 before), the answers of the Inspector are used.
        int n = GameState.level1Monsters;
        if (n > 0)
        {
            correctAnswer = Random.Range(0, 3);
            int first = Mathf.Max(0, n - correctAnswer);
            correctAnswer = n - first;
            answers = new string[] { first.ToString(), (first + 1).ToString(), (first + 2).ToString() };
        }

        questionText.text = question;
        for (int i = 0; i < answerTexts.Length && i < answers.Length; i++)
            answerTexts[i].text = answers[i];
        if (feedbackText != null) feedbackText.text = "";

        riddlePanel.SetActive(true);
        Time.timeScale = 0f;   // pause the game while answering
    }

    // Connect each button: button 1 -> Answer(0), button 2 -> Answer(1), button 3 -> Answer(2)
    public void Answer(int index)
    {
        if (index == correctAnswer)
        {
            riddlePanel.SetActive(false);
            Time.timeScale = 1f;
            GameState.score += scoreReward;
            SoundFX.PlayKey();
            MessageUI.Show("Correct! The door is open. Save the princess!");
            gameObject.SetActive(false);   // the door disappears
        }
        else
        {
            StartCoroutine(WrongAnswer());
        }
    }

    IEnumerator WrongAnswer()
    {
        // Lock the buttons so the player can't click again
        foreach (Button b in answerButtons)
            if (b != null) b.interactable = false;

        if (feedbackText != null) feedbackText.text = "Wrong answer! Back to the beginning...";
        SoundFX.PlayHurt();

        // Realtime because the game is paused (timeScale = 0)
        yield return new WaitForSecondsRealtime(restartDelay);

        Time.timeScale = 1f;
        GameState.NewGame();
        SceneManager.LoadScene(restartScene);
    }
}
