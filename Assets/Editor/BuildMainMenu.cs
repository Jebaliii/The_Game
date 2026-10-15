using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

// EDITOR ONLY (not part of the game): builds the MainMenu scene.
// Click Tools > Build Main Menu. It replaces Assets/MainMenu.unity each time.
// Pages: main (Play, How to play, Settings, Quit), difficulty (Easy, Medium, Hard), how to play, settings.
public static class BuildMainMenu
{
    const string dungeon = "Assets/Art/TinyDungeon/TinyDungeon.png";
    const string town = "Assets/Art/TinyTown/TinyTown.png";

    [MenuItem("Tools/Build Main Menu")]
    static void BuildFromMenu()
    {
        if (!EditorUtility.DisplayDialog("Build Main Menu",
            "This replaces Assets/MainMenu.unity with a new menu.", "Build", "Cancel")) return;
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        Build();
    }

    // Public so it can also run from the command line (batch mode)
    public static void Build()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // Camera with the dark color of the HUD as background
        GameObject cameraObject = new GameObject("Main Camera");
        cameraObject.tag = "MainCamera";
        Camera cam = cameraObject.AddComponent<Camera>();
        cam.orthographic = true;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = UIFactory.dark;
        cameraObject.AddComponent<AudioListener>();
        cameraObject.transform.position = new Vector3(0, 0, -10);

        // Needed to click the buttons
        GameObject eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<EventSystem>();
        eventSystem.AddComponent<StandaloneInputModule>();

        // Canvas (same size rules as the HUD of the levels)
        GameObject canvasObject = new GameObject("Menu Canvas");
        canvasObject.layer = LayerMask.NameToLayer("UI");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        canvasObject.AddComponent<GraphicRaycaster>();
        Transform root = canvasObject.transform;

        MenuManager menu = new GameObject("MenuManager").AddComponent<MenuManager>();

        BuildDecor(root);
        BuildTitle(root);
        menu.mainPanel = BuildMainPage(root, menu);
        menu.difficultyPanel = BuildDifficultyPage(root, menu);
        menu.instructionsPanel = BuildHowToPlayPage(root, menu);
        menu.settingsPanel = BuildSettingsPage(root, menu);

        // Only the main page is visible at the start
        menu.difficultyPanel.SetActive(false);
        menu.instructionsPanel.SetActive(false);
        menu.settingsPanel.SetActive(false);

        EditorSceneManager.SaveScene(scene, "Assets/MainMenu.unity");
        Debug.Log("MainMenu built and saved.");
    }

    // ---------- Background ----------

    static void BuildDecor(Transform root)
    {
        RectTransform decor = UIFactory.Panel("Decor", root);

        // Grass at the bottom of the screen
        RectTransform grass = UIFactory.NewRect("Grass", decor, Vector2.zero, new Vector2(0, 110));
        grass.anchorMin = new Vector2(0, 0);
        grass.anchorMax = new Vector2(1, 0);
        grass.pivot = new Vector2(0.5f, 0);
        Image grassImage = grass.gameObject.AddComponent<Image>();
        grassImage.sprite = UIBuilder.LoadSprite(town, "TinyTown_0");
        grassImage.type = Image.Type.Tiled;
        grassImage.raycastTarget = false;

        // A row of trees on the grass
        Sprite tree = UIBuilder.LoadSprite(town, "TinyTown_28");
        for (int i = 0; i < 17; i++)
        {
            Image t = UIFactory.Picture("Tree", decor, tree, new Vector2(-960 + 60 + 120 * i, 0), new Vector2(120, 120));
            t.rectTransform.anchorMin = t.rectTransform.anchorMax = new Vector2(0.5f, 0);
            t.rectTransform.anchoredPosition = new Vector2(-960 + 60 + 120 * i, 150);
        }

        // The hero, the princess and a slime
        UIFactory.Picture("Knight", decor, UIBuilder.LoadSprite(dungeon, "TinyDungeon_96"), new Vector2(-640, -60), new Vector2(220, 220));
        UIFactory.Picture("Princess", decor, UIBuilder.LoadSprite(dungeon, "TinyDungeon_99"), new Vector2(640, -60), new Vector2(220, 220));
        Image slime = UIFactory.Picture("Slime", decor, UIBuilder.LoadSprite(dungeon, "TinyDungeon_108"), Vector2.zero, new Vector2(110, 110));
        slime.rectTransform.anchorMin = slime.rectTransform.anchorMax = new Vector2(0.5f, 0);
        slime.rectTransform.anchoredPosition = new Vector2(430, 70);
    }

    static void BuildTitle(Transform root)
    {
        // Dark copy a bit lower-right = shadow
        TMP_Text shadow = UIFactory.Text("TitleShadow", root, "FOREST QUEST", 130, new Color(0, 0, 0, 0.5f), new Vector2(7, 353), new Vector2(1400, 160));
        shadow.fontStyle = FontStyles.Bold;
        TMP_Text title = UIFactory.Text("Title", root, "FOREST QUEST", 130, UIFactory.gold, new Vector2(0, 360), new Vector2(1400, 160));
        title.fontStyle = FontStyles.Bold;
        UIFactory.Text("Subtitle", root, "Save the princess!", 40, UIFactory.cream, new Vector2(0, 265), new Vector2(1000, 60));
    }

    // ---------- Pages ----------

    static GameObject BuildMainPage(Transform root, MenuManager menu)
    {
        RectTransform page = UIFactory.Panel("MainPage", root);
        Vector2 size = new Vector2(460, 96);
        Button play = UIFactory.MakeButton("PlayButton", page, "PLAY", new Vector2(0, 140), size, out _);
        Button howTo = UIFactory.MakeButton("HowToPlayButton", page, "HOW TO PLAY", new Vector2(0, 25), size, out _);
        Button settings = UIFactory.MakeButton("SettingsButton", page, "SETTINGS", new Vector2(0, -90), size, out _);
        Button quit = UIFactory.MakeButton("QuitButton", page, "QUIT", new Vector2(0, -205), size, out _);

        UnityEventTools.AddPersistentListener(play.onClick, menu.ShowDifficulty);
        UnityEventTools.AddPersistentListener(howTo.onClick, menu.ShowInstructions);
        UnityEventTools.AddPersistentListener(settings.onClick, menu.ShowSettings);
        UnityEventTools.AddPersistentListener(quit.onClick, menu.Quit);
        return page.gameObject;
    }

    static GameObject BuildDifficultyPage(Transform root, MenuManager menu)
    {
        RectTransform page = UIFactory.Panel("DifficultyPage", root);
        UIFactory.Text("Heading", page, "CHOOSE DIFFICULTY", 56, UIFactory.gold, new Vector2(0, 150), new Vector2(1000, 80)).fontStyle = FontStyles.Bold;

        Button easy = DifficultyCard(page, "EASY", "Slow monsters\n1 hit to kill them", UIFactory.green, -380);
        Button medium = DifficultyCard(page, "MEDIUM", "The normal\nadventure", UIFactory.gold, 0);
        Button hard = DifficultyCard(page, "HARD", "Fast monsters\n3 hits to kill them", UIFactory.red, 380);
        Button back = UIFactory.MakeButton("BackButton", page, "BACK", new Vector2(0, -230), new Vector2(300, 90), out _);

        UnityEventTools.AddPersistentListener(easy.onClick, menu.PlayEasy);
        UnityEventTools.AddPersistentListener(medium.onClick, menu.PlayMedium);
        UnityEventTools.AddPersistentListener(hard.onClick, menu.PlayHard);
        UnityEventTools.AddPersistentListener(back.onClick, menu.ShowMain);
        return page.gameObject;
    }

    // A big button: the name in color on top, a short description under it
    static Button DifficultyCard(Transform page, string name, string description, Color color, float x)
    {
        TMP_Text label;
        Button card = UIFactory.MakeButton(name + "Button", page, name, new Vector2(x, -20), new Vector2(330, 200), out label);
        label.color = color;
        label.fontSize = 50;
        label.rectTransform.anchoredPosition = new Vector2(0, 45);
        label.rectTransform.sizeDelta = new Vector2(330, 70);
        UIFactory.Text("Description", label.transform.parent, description, 28, UIFactory.cream, new Vector2(0, -35), new Vector2(300, 90));
        return card;
    }

    static GameObject BuildHowToPlayPage(Transform root, MenuManager menu)
    {
        RectTransform page = UIFactory.Panel("HowToPlayPage", root);
        UIFactory.Text("Heading", page, "HOW TO PLAY", 56, UIFactory.gold, new Vector2(0, 175), new Vector2(1000, 80)).fontStyle = FontStyles.Bold;

        Transform box = UIFactory.Frame("Box", page, new Vector2(0, -25), new Vector2(1000, 330), UIFactory.dark).transform;
        UIFactory.Text("Text", box,
            "<color=#E5A92F>Move:</color> Arrows / WASD / ZQSD      <color=#E5A92F>Attack:</color> Space\n\n"
            + "Kill all the monsters and collect all the coins to open the way.\n"
            + "Level 1: push the rock to make a bridge, then find the key.\n"
            + "Level 2: answer the question at the door and save the princess!\n"
            + "A heart on the ground gives you back your 3 hearts.",
            30, UIFactory.cream, Vector2.zero, new Vector2(940, 300));

        Button back = UIFactory.MakeButton("BackButton", page, "BACK", new Vector2(0, -255), new Vector2(300, 90), out _);
        UnityEventTools.AddPersistentListener(back.onClick, menu.ShowMain);
        return page.gameObject;
    }

    static GameObject BuildSettingsPage(Transform root, MenuManager menu)
    {
        RectTransform page = UIFactory.Panel("SettingsPage", root);
        UIFactory.Text("Heading", page, "SETTINGS", 56, UIFactory.gold, new Vector2(0, 175), new Vector2(1000, 80)).fontStyle = FontStyles.Bold;

        // Volume:  [-]  VOLUME 100%  [+]
        Button down = UIFactory.MakeButton("VolumeDownButton", page, "-", new Vector2(-300, 75), new Vector2(100, 96), out _);
        menu.volumeText = UIFactory.Text("VolumeText", page, "VOLUME  100%", 44, UIFactory.cream, new Vector2(0, 75), new Vector2(440, 96));
        menu.volumeText.fontStyle = FontStyles.Bold;
        Button up = UIFactory.MakeButton("VolumeUpButton", page, "+", new Vector2(300, 75), new Vector2(100, 96), out _);

        Button shake = UIFactory.MakeButton("ShakeButton", page, "SCREEN SHAKE: ON", new Vector2(0, -40), new Vector2(700, 96), out menu.shakeText);
        Button fullscreen = UIFactory.MakeButton("FullscreenButton", page, "FULLSCREEN: OFF", new Vector2(0, -155), new Vector2(700, 96), out menu.fullscreenText);
        Button back = UIFactory.MakeButton("BackButton", page, "BACK", new Vector2(0, -268), new Vector2(300, 90), out _);

        UnityEventTools.AddPersistentListener(down.onClick, menu.VolumeDown);
        UnityEventTools.AddPersistentListener(up.onClick, menu.VolumeUp);
        UnityEventTools.AddPersistentListener(shake.onClick, menu.ToggleShake);
        UnityEventTools.AddPersistentListener(fullscreen.onClick, menu.ToggleFullscreen);
        UnityEventTools.AddPersistentListener(back.onClick, menu.ShowMain);
        return page.gameObject;
    }
}
