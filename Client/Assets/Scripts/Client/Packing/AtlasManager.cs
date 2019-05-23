using System.Collections.Generic;
using UnityEngine.U2D;

public class AtlasManager
{
    public static Dictionary<string, SpriteAtlas> SpriteAtlasDict = new Dictionary<string, SpriteAtlas>();

    public static void Reset()
    {
        SpriteAtlasDict.Clear();
    }

    public static SpriteAtlas LoadAtlas(string atlasName)
    {
        if (SpriteAtlasDict.ContainsKey(atlasName))
        {
            return SpriteAtlasDict[atlasName];
        }

        return null;
    }
}