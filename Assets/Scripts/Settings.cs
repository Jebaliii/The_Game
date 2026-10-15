using UnityEngine;

// The player's settings (volume, screen shake, fullscreen).
// They are saved on the computer (PlayerPrefs), so they are kept for the next time.
// You don't put this script on any object. The Settings page of the main menu changes them.
public static class Settings
{
    // Sound volume, from 0 (silent) to 1 (full), by steps of 10%
    public static float Volume
    {
        get { return PlayerPrefs.GetFloat("volume", 1f); }
        set
        {
            float v = Mathf.Clamp01(Mathf.Round(value * 10f) / 10f);
            PlayerPrefs.SetFloat("volume", v);
            AudioListener.volume = v;
        }
    }

    // Does the camera shake when hitting / getting hit?
    public static bool ScreenShake
    {
        get { return PlayerPrefs.GetInt("screenShake", 1) == 1; }
        set { PlayerPrefs.SetInt("screenShake", value ? 1 : 0); }
    }

    // Only works in the built game, not in the Unity editor
    public static bool Fullscreen
    {
        get { return Screen.fullScreen; }
        set { Screen.fullScreen = value; }
    }

    // Texts of the settings buttons (main menu and pause menu)
    public static string VolumeText() { return "VOLUME  " + Mathf.RoundToInt(Volume * 100) + "%"; }
    public static string ShakeText() { return "SCREEN SHAKE: " + (ScreenShake ? "ON" : "OFF"); }
    public static string FullscreenText(bool on) { return "FULLSCREEN: " + (on ? "ON" : "OFF"); }

    // Unity calls this when the game starts: use the saved volume
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void ApplyOnStart()
    {
        AudioListener.volume = Volume;
    }
}
