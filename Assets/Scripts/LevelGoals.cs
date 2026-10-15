using UnityEngine;

// The goals of every level: collect all the coins and kill all the monsters.
// The gate (Level 1) and the door (Level 2) use it before they open.
// You don't put this script on any object.
public static class LevelGoals
{
    // How many coins / monsters are in the scene right now
    public static int CoinCount() { return Object.FindObjectsOfType<Coin>().Length; }
    public static int MonsterCount() { return Object.FindObjectsOfType<Monster>().Length; }

    // totalCoins and totalMonsters are counted by the door at the start of the level.
    // Returns true when everything is done.
    // Otherwise shows a message like "Coins collected: 5 / 8" and returns false.
    public static bool CheckDone(int totalCoins, int totalMonsters)
    {
        int coins = totalCoins - CoinCount();            // collected
        int monsters = totalMonsters - MonsterCount();   // killed
        if (coins >= totalCoins && monsters >= totalMonsters) return true;

        MessageUI.Show("You must collect all the coins: " + Progress(coins, totalCoins) + "\n"
                     + "You must kill all the monsters: " + Progress(monsters, totalMonsters));
        return false;
    }

    // "5 / 8", in green when finished
    static string Progress(int done, int total)
    {
        string text = "<b>" + done + " / " + total + "</b>";
        return done >= total ? "<color=#7FBF6A>" + text + "</color>" : text;
    }
}
