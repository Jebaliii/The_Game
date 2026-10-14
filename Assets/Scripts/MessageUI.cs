using UnityEngine;
using TMPro;

// Shows a short message at the top of the screen for a few seconds.
// Put it on the HUD Canvas. Other scripts call: MessageUI.Show("text");
// If there is no MessageUI in the scene (like Level 2), Show does nothing.
public class MessageUI : MonoBehaviour
{
    public GameObject box;        // the message box (hidden at the start)
    public TMP_Text messageText;  // the text inside the box
    public float showTime = 2f;   // seconds the message stays on screen

    private static MessageUI instance;   // the MessageUI of the current scene
    private float hideTime = 0f;

    void Awake()
    {
        instance = this;
        if (box != null) box.SetActive(false);
    }

    void Update()
    {
        // Hide the box when its time is over
        if (box != null && box.activeSelf && Time.time > hideTime)
            box.SetActive(false);
    }

    public static void Show(string text)
    {
        if (instance == null || instance.box == null || instance.messageText == null) return;

        instance.messageText.text = text;
        instance.box.SetActive(true);
        instance.hideTime = Time.time + instance.showTime;
    }
}
