using UnityEngine;

// Keeps the game data between scenes (score, hearts, key).
// You don't put this script on any object.
public static class GameState
{
    public static int score = 0;
    public static int hearts = 3;
    public static bool hasKey = false;

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
