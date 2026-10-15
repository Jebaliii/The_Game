using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

// EDITOR ONLY (not part of the game).
// The project is in a OneDrive folder, and Unity sometimes doesn't notice files changed
// outside of Unity (for example scripts edited in another program). Then the game runs old code.
// This checks the scripts, scenes and prefabs every 2 seconds and imports the changed ones.
[InitializeOnLoad]
public static class AutoImport
{
    static readonly string[] folders = { "Assets/Scripts", "Assets/Editor", "Assets/Prefabs", "Assets" };
    static readonly string[] extensions = { ".cs", ".unity", ".prefab" };

    static Dictionary<string, System.DateTime> lastSeen = new Dictionary<string, System.DateTime>();
    static double nextCheck = 0;

    static AutoImport()
    {
        Scan(false);   // remember how the files are now
        EditorApplication.update += Update;
    }

    static void Update()
    {
        if (EditorApplication.timeSinceStartup < nextCheck) return;
        nextCheck = EditorApplication.timeSinceStartup + 2;
        if (EditorApplication.isCompiling || EditorApplication.isUpdating) return;
        Scan(true);
    }

    static void Scan(bool importChanged)
    {
        List<string> changed = new List<string>();
        foreach (string folder in folders)
        {
            if (!Directory.Exists(folder)) continue;
            // "Assets" itself: only the scenes at the top, the other folders are checked on their own
            SearchOption option = folder == "Assets" ? SearchOption.TopDirectoryOnly : SearchOption.AllDirectories;
            foreach (string file in Directory.GetFiles(folder, "*.*", option))
            {
                if (System.Array.IndexOf(extensions, Path.GetExtension(file)) < 0) continue;
                string path = file.Replace('\\', '/');
                System.DateTime time = File.GetLastWriteTimeUtc(path);

                System.DateTime before;
                bool known = lastSeen.TryGetValue(path, out before);
                if (importChanged && (!known || before != time)) changed.Add(path);
                lastSeen[path] = time;
            }
        }

        if (changed.Count == 0) return;
        foreach (string path in changed)
        {
            // A scene that is open in the editor is reloaded by Unity itself (it asks you)
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }
        Debug.Log("AutoImport: imported " + changed.Count + " changed file(s): " + string.Join(", ", changed));
    }
}
