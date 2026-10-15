using UnityEngine;
using UnityEngine.SceneManagement;

// The gate at the end of level 1. It opens when all the coins are collected,
// all the monsters are killed and the hero has the key. Then it loads the next level.
// Needs a normal Collider 2D (NOT trigger) so it blocks the hero.
public class Gate : MonoBehaviour
{
    public string nextScene = "Level2";

    private int totalCoins;
    private int totalMonsters;

    void Start()
    {
        // Count everything at the start of the level
        totalCoins = LevelGoals.CoinCount();
        totalMonsters = LevelGoals.MonsterCount();
        GameState.level1Monsters = totalMonsters;   // the Level 2 door asks this number
        MessageUI.Show("Kill all the monsters and collect all the coins!");
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Player")) return;
        if (!LevelGoals.CheckDone(totalCoins, totalMonsters)) return;   // shows the progress message

        if (GameState.hasKey)
        {
            GameState.hasKey = false;   // the key is used
            SceneManager.LoadScene(nextScene);
        }
        else MessageUI.Show("Locked! Find the key first.");
    }
}
