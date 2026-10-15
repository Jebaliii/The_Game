using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Shows score, hearts and key on the screen.
// Put this script on the Canvas of each level.
// Level 1 and Level 2 use the HUD bar fields (the old text fields are not used anymore).
// Any field left empty is simply skipped.
public class HUD : MonoBehaviour
{
    [Header("Old texts (not used)")]
    public TMP_Text scoreText;
    public TMP_Text heartsText;
    public TMP_Text keyText;   // optional

    [Header("HUD bar (Level 1)")]
    public TMP_Text barScoreText;   // "SCORE 120"
    public Image[] heartImages;     // the 3 hearts, left to right
    public Sprite fullHeart;
    public Sprite emptyHeart;
    public TMP_Text barKeyText;     // "KEY: NO" / "KEY: YES"

    private Color keyNoColor = new Color32(0xA9, 0xB9, 0xA8, 0xFF);   // grey
    private Color keyYesColor = new Color32(0xE5, 0xA9, 0x2F, 0xFF);  // gold

    void Update()
    {
        // Old texts
        if (scoreText != null) scoreText.text = "Score: " + GameState.score;
        if (heartsText != null) heartsText.text = "Hearts: " + GameState.hearts;
        if (keyText != null) keyText.text = GameState.hasKey ? "Key: YES" : "Key: NO";

        // HUD bar: the number is bold
        if (barScoreText != null) barScoreText.text = "SCORE <b>" + GameState.score + "</b>";

        // Full heart for each heart we still have, empty heart for the lost ones
        if (heartImages != null)
        {
            for (int i = 0; i < heartImages.Length; i++)
            {
                if (heartImages[i] != null)
                    heartImages[i].sprite = (i < GameState.hearts) ? fullHeart : emptyHeart;
            }
        }

        if (barKeyText != null)
        {
            barKeyText.text = GameState.hasKey ? "KEY: YES" : "KEY: NO";
            barKeyText.color = GameState.hasKey ? keyYesColor : keyNoColor;
        }
    }
}
