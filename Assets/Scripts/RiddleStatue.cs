using UnityEngine;
using TMPro;

// PUZZLE 2: a statue asks a riddle. Correct answer -> the door opens.
// Wrong answer -> the hero loses 1 heart (walk away and come back to try again).
// Needs a Collider 2D with "Is Trigger" ticked (make it a bit bigger than the statue).
public class RiddleStatue : MonoBehaviour
{
    [Header("UI")]
    public GameObject riddlePanel;     // the panel with the question and 3 buttons
    public TMP_Text questionText;
    public TMP_Text[] answerTexts;     // the texts of the 3 buttons

    [Header("Riddle")]
    [TextArea] public string question = "I have keys but I can't open any door. What am I?";
    public string[] answers = { "A tree", "A piano", "A river" };
    public int correctAnswer = 1;      // 0 = first button, 1 = second, 2 = third

    [Header("Result")]
    public GameObject doorToOpen;      // the door that disappears when solved
    public int scoreReward = 200;

    private bool solved = false;
    private PlayerHealth player;

    void Start()
    {
        if (riddlePanel != null) riddlePanel.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (solved) return;
        if (other.CompareTag("Player"))
        {
            player = other.GetComponent<PlayerHealth>();
            OpenRiddle();
        }
    }

    void OpenRiddle()
    {
        questionText.text = question;
        for (int i = 0; i < answerTexts.Length && i < answers.Length; i++)
            answerTexts[i].text = answers[i];

        riddlePanel.SetActive(true);
        Time.timeScale = 0f;   // pause the game while answering
    }

    // Connect each button: button 1 -> Answer(0), button 2 -> Answer(1), button 3 -> Answer(2)
    public void Answer(int index)
    {
        riddlePanel.SetActive(false);
        Time.timeScale = 1f;

        if (index == correctAnswer)
        {
            solved = true;
            GameState.score += scoreReward;
            if (doorToOpen != null) doorToOpen.SetActive(false);
        }
        else
        {
            if (player != null) player.TakeDamage(1);
        }
    }
}
