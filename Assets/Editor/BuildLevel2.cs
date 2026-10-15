using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using TMPro;

// EDITOR ONLY (not part of the game): builds Level2 from a copy of Level1.
// Click Tools > Build Level2. It replaces Assets/Level2.unity each time, so you can run it again
// after changing the map below.
// It keeps the camera, hero, HUD, messages and sounds of Level1, removes the river puzzle and the key,
// paints the map below and adds the door riddle, the princess and their two pop-ups.
public static class BuildLevel2
{
    // The map: one character = one tile (15 columns x 11 rows), same size as Level1.
    // '#' = tree, 'W' = stone wall of the princess room, '_' = stone floor of the room, '.' = grass.
    // Objects: S = hero start, c = coin, h = heart, m = slime, D = door (with the riddle), P = princess.
    static readonly string[] map =
    {
        "###############",   // row 0
        "#S..c..#m.W___#",   // row 1
        "#.##.#.#..W_P_#",   // row 2
        "#c#..#...cWWDW#",   // row 3
        "#..#.##.#..m..#",   // row 4
        "#..m.c....#.m.#",   // row 5
        "#.###.#.###.#.#",   // row 6
        "#cm.m.#.c...#c#",   // row 7
        "#.#.###.#.#.#m#",   // row 8
        "#...c..h..m...#",   // row 9
        "###############",   // row 10
    };

    // Same grid as Level1: column 0 is cell x = -7, row 0 is cell y = 5.
    const int left = -7;
    const int top = 5;

    const string coinPrefab = "Assets/Prefabs/Coin.prefab";
    const string slimePrefab = "Assets/Prefabs/Slime.prefab";
    const string heartPrefab = "Assets/Prefabs/Heart.prefab";

    [MenuItem("Tools/Build Level2")]
    static void BuildFromMenu()
    {
        if (!EditorUtility.DisplayDialog("Build Level2",
            "This replaces Assets/Level2.unity with a new level made from Level1.", "Build", "Cancel")) return;
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        Build();
    }

    // Public so it can also run from the command line (batch mode)
    public static void Build()
    {
        Scene level1 = EditorSceneManager.OpenScene("Assets/Level1.unity", OpenSceneMode.Single);
        EditorSceneManager.SaveScene(level1, "Assets/Level2.unity", true);   // copy, Level1 is not changed
        Scene scene = EditorSceneManager.OpenScene("Assets/Level2.unity", OpenSceneMode.Single);

        RemoveLevel1Objects(scene);
        if (!PaintMap()) return;

        // Hero at the start
        FindRoot(scene, "Player").transform.position = CellCenter('S')[0];

        // Slimes and coins
        GameObject monsters = new GameObject("Monsters");
        foreach (Vector3 pos in CellCenter('m')) Spawn(slimePrefab, pos, monsters.transform, "Slime");
        GameObject coins = new GameObject("Coins");
        foreach (Vector3 pos in CellCenter('c')) Spawn(coinPrefab, pos, coins.transform, "Coin");
        foreach (Vector3 pos in CellCenter('h'))
        {
            GameObject heart = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(heartPrefab), scene);
            heart.transform.position = pos;
        }

        // The Level1 gate becomes the door (same sprite and collider, new script)
        GameObject door = FindRoot(scene, "Gate");
        door.name = "Door";
        Object.DestroyImmediate(door.GetComponent<Gate>());
        door.transform.position = CellCenter('D')[0];
        DoorRiddle riddle = door.AddComponent<DoorRiddle>();

        // The princess behind the door
        GameObject princessObject = new GameObject("Princess");
        princessObject.transform.position = CellCenter('P')[0];
        SpriteRenderer sr = princessObject.AddComponent<SpriteRenderer>();
        sr.sprite = UIBuilder.LoadSprite("Assets/Art/TinyDungeon/TinyDungeon.png", "TinyDungeon_99");
        sr.sortingOrder = 1;
        BoxCollider2D trigger = princessObject.AddComponent<BoxCollider2D>();
        trigger.isTrigger = true;
        trigger.size = new Vector2(0.8f, 0.8f);
        Princess princess = princessObject.AddComponent<Princess>();

        // HUD: new level name, no key in this level
        GameObject canvas = FindRoot(scene, "HUD Canvas");
        UIFactory.font = FindText(canvas, "MessageText").font;
        FindText(canvas, "LevelText").text = "LEVEL 2 · THE PRINCESS";
        HUD hud = canvas.GetComponent<HUD>();
        hud.barKeyText.gameObject.SetActive(false);
        hud.barKeyText = null;

        BuildRiddlePopup(canvas.transform, riddle);
        BuildWinPopup(canvas.transform, princess);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("Level2 built and saved.");
    }

    // ---------- Level ----------

    static void RemoveLevel1Objects(Scene scene)
    {
        // The river puzzle, the key and an old hidden test square
        string[] names = { "RiverLeft", "RiverRight", "RiverWaves", "WaterBlock", "Bridge", "Plate", "Rock", "Square", "Key" };
        foreach (string name in names)
        {
            GameObject go = FindRoot(scene, name);
            if (go != null) Object.DestroyImmediate(go);
        }

        // All the coins, slimes and the heart of Level1 (they get new places)
        List<GameObject> old = new List<GameObject>();
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            foreach (Transform t in root.GetComponentsInChildren<Transform>(true))
            {
                if (!PrefabUtility.IsOutermostPrefabInstanceRoot(t.gameObject)) continue;
                string path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(t.gameObject);
                if (path == coinPrefab || path == slimePrefab || path == heartPrefab) old.Add(t.gameObject);
            }
        }
        foreach (GameObject go in old)
            if (go != null) Object.DestroyImmediate(go);
    }

    static bool PaintMap()
    {
        Tilemap ground = GameObject.Find("Ground").GetComponent<Tilemap>();
        Tilemap decorations = GameObject.Find("Decorations").GetComponent<Tilemap>();
        Tilemap walls = GameObject.Find("Walls").GetComponent<Tilemap>();

        TileBase grass = LoadTile(0);
        TileBase flowers = LoadTile(2);
        TileBase tree = LoadTile(28);
        TileBase mushrooms = LoadTile(29);
        TileBase stoneFloor = LoadTile(109);
        TileBase stoneWall = LoadTile(126);
        if (grass == null || flowers == null || tree == null || mushrooms == null || stoneFloor == null || stoneWall == null)
            return false;

        ground.ClearAllTiles();
        decorations.ClearAllTiles();
        walls.ClearAllTiles();

        for (int row = 0; row < map.Length; row++)
        {
            for (int col = 0; col < map[row].Length; col++)
            {
                Vector3Int cell = new Vector3Int(left + col, top - row, 0);
                char c = map[row][col];

                // Stone floor inside the princess room (and under the door), grass everywhere else
                bool inRoom = c == '_' || c == 'P' || c == 'D' || c == 'W';
                ground.SetTile(cell, inRoom ? stoneFloor : grass);

                if (c == '#') walls.SetTile(cell, tree);
                if (c == 'W') walls.SetTile(cell, stoneWall);
            }
        }

        // A few decorations on '.' cells (column, row)
        decorations.SetTile(new Vector3Int(left + 9, top - 1, 0), flowers);
        decorations.SetTile(new Vector3Int(left + 13, top - 4, 0), mushrooms);
        decorations.SetTile(new Vector3Int(left + 1, top - 9, 0), flowers);
        decorations.SetTile(new Vector3Int(left + 10, top - 7, 0), mushrooms);
        return true;
    }

    // World position of the center of every cell with this letter
    static List<Vector3> CellCenter(char letter)
    {
        List<Vector3> list = new List<Vector3>();
        for (int row = 0; row < map.Length; row++)
            for (int col = 0; col < map[row].Length; col++)
                if (map[row][col] == letter) list.Add(new Vector3(col - 6.5f, 5.5f - row, 0));
        return list;
    }

    static void Spawn(string prefabPath, Vector3 pos, Transform parent, string baseName)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        GameObject go = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
        go.transform.position = pos;
        int n = parent.childCount - 1;
        go.name = n == 0 ? baseName : baseName + " (" + n + ")";
    }

    // ---------- Pop-ups ----------

    static void BuildRiddlePopup(Transform canvas, DoorRiddle riddle)
    {
        RectTransform popup = UIFactory.Overlay("RiddlePopup", canvas);
        RectTransform box = UIFactory.Frame("Box", popup, Vector2.zero, new Vector2(1000, 540), UIFactory.dark).rectTransform;

        UIFactory.Text("Title", box, "THE DOOR IS LOCKED", 48, UIFactory.gold, new Vector2(0, 195), new Vector2(920, 70)).fontStyle = FontStyles.Bold;
        TMP_Text question = UIFactory.Text("Question", box, riddle.question, 36, UIFactory.cream, new Vector2(0, 95), new Vector2(920, 110));

        Button[] buttons = new Button[3];
        TMP_Text[] labels = new TMP_Text[3];
        for (int i = 0; i < 3; i++)
        {
            buttons[i] = UIFactory.MakeButton("Answer " + (i + 1), box, riddle.answers[i], new Vector2(-300 + 300 * i, -40), new Vector2(240, 100), out labels[i]);
            UnityEventTools.AddIntPersistentListener(buttons[i].onClick, riddle.Answer, i);
        }

        TMP_Text feedback = UIFactory.Text("Feedback", box, "", 32, UIFactory.red, new Vector2(0, -175), new Vector2(920, 60));

        riddle.riddlePanel = popup.gameObject;
        riddle.questionText = question;
        riddle.answerButtons = buttons;
        riddle.answerTexts = labels;
        riddle.feedbackText = feedback;
        popup.gameObject.SetActive(false);
    }

    static void BuildWinPopup(Transform canvas, Princess princess)
    {
        RectTransform popup = UIFactory.Overlay("WinPopup", canvas);
        RectTransform box = UIFactory.Frame("Box", popup, Vector2.zero, new Vector2(1000, 500), UIFactory.dark).rectTransform;

        UIFactory.Text("Title", box, "YOU WIN!", 72, UIFactory.gold, new Vector2(0, 160), new Vector2(920, 100)).fontStyle = FontStyles.Bold;
        UIFactory.Text("Text", box, "You saved the princess!", 40, UIFactory.cream, new Vector2(0, 65), new Vector2(920, 70));
        TMP_Text score = UIFactory.Text("Score", box, "SCORE 0", 36, UIFactory.green, new Vector2(0, -10), new Vector2(920, 60));

        Button playAgain = UIFactory.MakeButton("PlayAgainButton", box, "PLAY AGAIN", new Vector2(0, -135), new Vector2(360, 100), out _);
        UnityEventTools.AddPersistentListener(playAgain.onClick, princess.PlayAgain);

        princess.winPanel = popup.gameObject;
        princess.winScoreText = score;
        popup.gameObject.SetActive(false);
    }

    // ---------- Helpers ----------

    static GameObject FindRoot(Scene scene, string name)
    {
        foreach (GameObject go in scene.GetRootGameObjects())
            if (go.name == name) return go;
        return null;
    }

    static TMP_Text FindText(GameObject canvas, string name)
    {
        foreach (TMP_Text t in canvas.GetComponentsInChildren<TMP_Text>(true))
            if (t.name == name) return t;
        return null;
    }

    static TileBase LoadTile(int number)
    {
        string file = "Assets/Art/Tiles/TinyTown/TinyTown_" + number + ".asset";
        TileBase tile = AssetDatabase.LoadAssetAtPath<TileBase>(file);
        if (tile == null) Debug.LogError("Tile not found: " + file);
        return tile;
    }
}
