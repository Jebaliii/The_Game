using UnityEngine;
using TMPro;

// Shows score, hearts and key on the screen.
// Put this script on the Canvas of each level.
public class HUD : MonoBehaviour
{
    public TMP_Text scoreText;
    public TMP_Text heartsText;
    public TMP_Text keyText;   // optional

    void Update()
    {
        if (scoreText != null) scoreText.text = "Score: " + GameState.score;
        if (heartsText != null) heartsText.text = "Hearts: " + GameState.hearts;
        if (keyText != null) keyText.text = GameState.hasKey ? "Key: YES" : "Key: NO";
    }
}
