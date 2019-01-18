using System;
using System.Collections;
using System.IO;
using Boo.Lang;
using UnityEngine;
using UnityEngine.Networking;

public class ABManager
{
    public static string GetAssetBundleFolderPath()
    {
        return Application.streamingAssetsPath + "/AssetBundle/" + ClientUtils.GetPlatformAbbr() + "/";
    }

    private static AssetBundleManifest AssetBundleManifest;

    public static AssetBundle LoadAssetBundle(string abName)
    {
        abName = abName.ToLower();

        IEnumerable bundles = AssetBundle.GetAllLoadedAssetBundles();

        foreach (AssetBundle b in bundles)
        {
            if (b.name == abName)
            {
                return b;
            }
        }

        if (!AssetBundleManifest)
        {
            AssetBundle manifest_bundle = AssetBundle.LoadFromFile(GetAssetBundleFolderPath() + ClientUtils.GetPlatformAbbr());
            AssetBundleManifest = manifest_bundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            manifest_bundle.Unload(false);
        }

        AssetBundle bundle = AssetBundle.LoadFromFile(GetAssetBundleFolderPath() + abName);
        string[] dependencies = AssetBundleManifest.GetAllDependencies(bundle.name);
        foreach (string dependency in dependencies)
        {
            LoadAssetBundle(dependency);
        }

        return bundle;
    }

    public static List<AssetBundle> LoadAllAssetBundleNamedLike(string prefix)
    {
        List<AssetBundle> res = new List<AssetBundle>();
        foreach (string ab_name in AssetBundleManifest.GetAllAssetBundles())
        {
            if (ab_name.StartsWith(prefix))
            {
                res.Add(LoadAssetBundle(ab_name));
            }
        }

        return res;
    }
}