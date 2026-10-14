using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Tilemaps;

// EDITOR ONLY (not part of the game): paints the Level1 tilemaps from the map below.
// Open Level1, then click Tools > Paint Level1. Ctrl+Z undoes it.
// You can delete this file when the level is painted.
public static class PaintLevel1
{
    // The prototype map: one character = one tile (15 columns x 11 rows).
    // Row 0 is the top, column 0 is the left.
    // '#' = tree on Walls. Every cell gets grass on Ground.
    // The other letters (S, c, R, P, ~, B, m, K, G) are objects placed in the scene, not tiles.
    static readonly string[] map =
    {
        "###############",   // row 0
        "#S...c...c...c#",   // row 1
        "#.###.###.###.#",   // row 2
        "#c#.R..P..#..c#",   // row 3
        "#...#.#.#...#.#",   // row 4
        "#.m.#...#.m...#",   // row 5
        "#~~~~~~B~~~~~~#",   // row 6
        "#c...........c#",   // row 7
        "#.###.#.K.#.#.#",   // row 8
        "#...m.#...#..G#",   // row 9
        "###############",   // row 10
    };

    // Column 0 is cell x = -7, row 0 is cell y = 5.
    // So the center of (column, row) in the world is (column - 6.5, 5.5 - row).
    const int left = -7;
    const int top = 5;

    [MenuItem("Tools/Paint Level1")]
    static void Paint()
    {
        Tilemap ground = FindTilemap("Ground");
        Tilemap decorations = FindTilemap("Decorations");
        Tilemap walls = FindTilemap("Walls");
        if (ground == null || decorations == null || walls == null) return;

        TileBase grass = LoadTile(0);
        TileBase tree = LoadTile(28);
        TileBase flowers = LoadTile(2);
        TileBase mushrooms = LoadTile(29);
        if (grass == null || tree == null || flowers == null || mushrooms == null) return;

        Undo.RegisterCompleteObjectUndo(new Object[] { ground, decorations, walls }, "Paint Level1");
        ground.ClearAllTiles();
        decorations.ClearAllTiles();
        walls.ClearAllTiles();

        for (int row = 0; row < map.Length; row++)
        {
            for (int col = 0; col < map[row].Length; col++)
            {
                Vector3Int cell = new Vector3Int(left + col, top - row, 0);

                ground.SetTile(cell, grass);                       // grass under every cell
                if (map[row][col] == '#') walls.SetTile(cell, tree);
            }
        }

        // A few decorations on '.' cells that are off the player's path (column, row)
        decorations.SetTile(new Vector3Int(left + 3, top - 5, 0), flowers);
        decorations.SetTile(new Vector3Int(left + 12, top - 5, 0), mushrooms);
        decorations.SetTile(new Vector3Int(left + 2, top - 9, 0), flowers);
        decorations.SetTile(new Vector3Int(left + 11, top - 9, 0), mushrooms);

        EditorSceneManager.MarkSceneDirty(ground.gameObject.scene);
        Debug.Log("Level1 painted. Press Ctrl+S to save the scene.");
    }

    static Tilemap FindTilemap(string name)
    {
        GameObject go = GameObject.Find(name);
        Tilemap tilemap = go != null ? go.GetComponent<Tilemap>() : null;
        if (tilemap == null) Debug.LogError("No Tilemap called '" + name + "' in the open scene.");
        return tilemap;
    }

    static TileBase LoadTile(int number)
    {
        string file = "Assets/Art/Tiles/TinyTown/TinyTown_" + number + ".asset";
        TileBase tile = AssetDatabase.LoadAssetAtPath<TileBase>(file);
        if (tile == null) Debug.LogError("Tile not found: " + file);
        return tile;
    }
}
