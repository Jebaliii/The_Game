using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Small helpers to create UI by code, in the style of the Level1 HUD
// (gold border, dark inside, cream text).
// Used in the game by PauseMenu, and in the editor by BuildLevel2 and BuildMainMenu.
// You don't put this script on any object.
public static class UIFactory
{
    // Colors of the Level1 HUD
    public static readonly Color gold = new Color32(0xE5, 0xA9, 0x2F, 0xFF);
    public static readonly Color dark = new Color32(0x1B, 0x31, 0x3B, 0xFF);
    public static readonly Color buttonColor = new Color32(0x2C, 0x49, 0x55, 0xFF);
    public static readonly Color cream = new Color32(0xEC, 0xDC, 0xAE, 0xFF);
    public static readonly Color green = new Color32(0x7F, 0xBF, 0x6A, 0xFF);
    public static readonly Color red = new Color32(0xE0, 0x5A, 0x47, 0xFF);

    // Same font as the Level1 messages (the default font of TextMesh Pro)
    public static TMP_FontAsset font;

    // Dark see-through layer over the whole screen (also blocks clicks on the game)
    public static RectTransform Overlay(string name, Transform canvas)
    {
        RectTransform rt = Panel(name, canvas);
        rt.gameObject.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.6f);
        return rt;
    }

    // Empty object that fills its parent (to group things, like a menu page)
    public static RectTransform Panel(string name, Transform parent)
    {
        RectTransform rt = NewRect(name, parent, Vector2.zero, Vector2.zero);
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        return rt;
    }

    // Gold border + colored inside, like the Level1 message box. Returns the inside image.
    public static Image Frame(string name, Transform parent, Vector2 pos, Vector2 size, Color inside)
    {
        RectTransform outer = NewRect(name, parent, pos, size);
        outer.gameObject.AddComponent<Image>().color = gold;

        RectTransform inner = NewRect("Inner", outer, Vector2.zero, new Vector2(-6, -6));
        inner.anchorMin = Vector2.zero;
        inner.anchorMax = Vector2.one;
        Image image = inner.gameObject.AddComponent<Image>();
        image.color = inside;
        return image;
    }

    public static Button MakeButton(string name, Transform parent, string label, Vector2 pos, Vector2 size, out TMP_Text labelText)
    {
        Image inside = Frame(name, parent, pos, size, buttonColor);
        Button button = inside.transform.parent.gameObject.AddComponent<Button>();
        button.targetGraphic = inside;

        // A bit darker normally, full color under the mouse
        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.8f, 0.8f, 0.8f);
        colors.highlightedColor = Color.white;
        colors.pressedColor = new Color(0.6f, 0.6f, 0.6f);
        colors.selectedColor = colors.normalColor;
        colors.disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.6f);
        button.colors = colors;

        labelText = Text("Label", inside.transform, label, 40, cream, Vector2.zero, size);
        labelText.fontStyle = FontStyles.Bold;
        return button;
    }

    public static TMP_Text Text(string name, Transform parent, string text, float size, Color color, Vector2 pos, Vector2 box)
    {
        if (font == null) font = TMP_Settings.defaultFontAsset;

        RectTransform rt = NewRect(name, parent, pos, box);
        TextMeshProUGUI t = rt.gameObject.AddComponent<TextMeshProUGUI>();
        t.font = font;
        t.text = text;
        t.fontSize = size;
        t.color = color;
        t.alignment = TextAlignmentOptions.Center;
        t.raycastTarget = false;
        return t;
    }

    // A pixel-art picture (a sprite of the sheets) in the UI
    public static Image Picture(string name, Transform parent, Sprite sprite, Vector2 pos, Vector2 size)
    {
        RectTransform rt = NewRect(name, parent, pos, size);
        Image image = rt.gameObject.AddComponent<Image>();
        image.sprite = sprite;
        image.preserveAspect = true;
        image.raycastTarget = false;
        return image;
    }

    public static RectTransform NewRect(string name, Transform parent, Vector2 pos, Vector2 size)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.layer = LayerMask.NameToLayer("UI");
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.SetParent(parent, false);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        return rt;
    }
}
