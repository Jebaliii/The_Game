using UnityEngine;
using TMPro;

// Shows a short message at the top of the screen for a few seconds.
// Put it on the HUD Canvas. Other scripts call: MessageUI.Show("text");
// If there is no MessageUI in the scene (like the menu scenes), Show does nothing.
public class MessageUI : MonoBehaviour
{
    public GameObject box;        // the message box (hidden at the start)
    public TMP_Text messageText;  // the text inside the box
    public float showTime = 2f;   // seconds the message stays on screen

    private static MessageUI instance;   // the MessageUI of the current scene
    private float hideTime = 0f;
    private float startHeight;           // height of the box for one line

    void Awake()
    {
        instance = this;
        if (box != null) startHeight = ((RectTransform)box.transform).sizeDelta.y;
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

        // Make the box taller when the text has several lines
        RectTransform boxRect = (RectTransform)instance.box.transform;
        float textWidth = instance.messageText.rectTransform.rect.width;
        float textHeight = instance.messageText.GetPreferredValues(text, textWidth, 0f).y;
        boxRect.sizeDelta = new Vector2(boxRect.sizeDelta.x, Mathf.Max(instance.startHeight, textHeight + 20f));

        instance.box.SetActive(true);
        instance.hideTime = Time.time + instance.showTime;
    }
}
