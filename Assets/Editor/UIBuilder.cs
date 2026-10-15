using UnityEditor;
using UnityEngine;

// EDITOR ONLY (not part of the game): loads a sprite of a sprite sheet by its name.
// The other UI helpers are in Scripts/UIFactory.cs (they also work in the game).
public static class UIBuilder
{
    public static Sprite LoadSprite(string sheet, string spriteName)
    {
        foreach (Object o in AssetDatabase.LoadAllAssetsAtPath(sheet))
            if (o is Sprite && o.name == spriteName) return (Sprite)o;
        Debug.LogError("Sprite not found: " + spriteName);
        return null;
    }
}
