using UnityEngine;

// Keeps the game data between scenes (score, hearts, key).
// You don't put this script on any object.
public static class GameState
{
    public static int score = 0;
    public static int hearts = 3;
    public static bool hasKey = false;
    public static int level1Monsters = 0;   // counted when Level 1 starts (for the question at the Level 2 door)

    // Chosen in the main menu. It is NOT reset by NewGame, so "Play again" keeps it.
    public const int Easy = 0, Medium = 1, Hard = 2;
    public static int difficulty = Medium;

    // Called when the player clicks Play or Retry
    public static void NewGame()
    {
        score = 0;
        hearts = 3;
        hasKey = false;
    }

    // Unity calls this automatically every time you press Play in the editor
    // (and when the built game starts), so old values never carry over.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetOnPlay()
    {
        NewGame();
    }
}
