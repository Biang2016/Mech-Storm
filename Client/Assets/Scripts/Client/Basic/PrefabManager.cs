using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoSingleton<PrefabManager>
{
    private PrefabManager()
    {
    }

    #region Prefab资源路径

    private const string ResourcePath = "Prefabs/";

    private static Dictionary<string, GameObject> PrefabNameDict = new Dictionary<string, GameObject>();

    public static void ClearPrefabDict()
    {
        PrefabNameDict.Clear();
    }

    public static void AddPrefabRes(string prefabName, GameObject prefab)
    {
        PrefabNameDict.Add(prefabName, prefab);
    }

    public GameObject GetPrefab(string prefabName)
    {
        if (PrefabNameDict.ContainsKey(prefabName))
        {
            return PrefabNameDict[prefabName];
        }

        return null;
    }

    #endregion
}