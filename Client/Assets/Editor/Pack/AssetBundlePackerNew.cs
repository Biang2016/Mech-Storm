using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public class AssetBundlePackerNew
{
    [MenuItem("AssetBundle/Packer/PC")]
    static void AssetBundlePacker_StandaloneWindows()
    {
        Pack(BuildTarget.StandaloneWindows64);
    }

    public static void Pack(BuildTarget build_target)
    {
        EditorUtility.ClearProgressBar();

        // 准备打包 初始化Packer
        string out_path = Application.dataPath + "/../AssetBundle";
        string platform = "";
        if (build_target == BuildTarget.StandaloneWindows64)
        {
            platform = "/pc";
        }

        out_path += platform;

        if (!Directory.Exists(out_path))
        {
            Directory.CreateDirectory(out_path);
        }

        BuildAssetBundleOptions option = BuildAssetBundleOptions.ChunkBasedCompression
                                         | BuildAssetBundleOptions.DeterministicAssetBundle
                                         | BuildAssetBundleOptions.DisableWriteTypeTree;

        Set_PackPrefabs();
        BuildPipeline.BuildAssetBundles(out_path, GetAssetBundleBuild(), option, build_target);

        CreateSceneALL();
        BuildPipeline.BuildAssetBundles(out_path, option, build_target);
    }

    static void Set_PackPrefabs()
    {
        DirectoryInfo di = new DirectoryInfo(Application.dataPath + "/Prefabs");
        foreach (FileInfo fi in di.GetFiles("*.prefab", SearchOption.AllDirectories))
        {
            SetAssetBundlePackGroup(fi.FullName, fi.Name.Replace(".prefab", ""));
        }
    }

    static void CreateSceneALL()
    {
        //清空一下缓存  
        Caching.ClearCache();

        string Path = Application.dataPath + "/MyScene.unity3d";

        //选择的要保存的对象 
        string[] levels =
        {
            "Assets/Scenes/FirstScene.unity",
            "Assets/Scenes/MainScene.unity"
        };

        //打包场景  
        BuildPipeline.BuildPlayer(levels, Path, BuildTarget.StandaloneWindows64, BuildOptions.BuildAdditionalStreamedScenes);

        // 刷新，可以直接在Unity工程中看见打包后的文件
        AssetDatabase.Refresh();
    }

    static Dictionary<string, List<string>> asset_ab_build_mapper = new Dictionary<string, List<string>>();
    static Dictionary<string, string> asset_ab_mapper = new Dictionary<string, string>();

    public static void SetAssetBundlePackGroup(string file_name, string bundle_name)
    {
        string asset_name = FileUtils.GetAssetsRelativePath(file_name);
        AssetImporter importer = AssetImporter.GetAtPath(asset_name);
        if (null != importer)
        {
            // AssetBundleName永远为空
            importer.SetAssetBundleNameAndVariant("", "");
            if (!string.IsNullOrEmpty(bundle_name))
            {
                UpdateAssetBundleBuild(bundle_name, asset_name);
            }
        }
    }

    public static void UpdateAssetBundleBuild(string ab_name, string asset_path)
    {
        if (!IsDynamicBlacklist(asset_path))
        {
            // 不在动态黑名单，已在的将会被忽略掉
            if (!asset_ab_mapper.ContainsKey(asset_path))
            {
                asset_ab_mapper.Add(asset_path, ab_name);
            }
            else
            {
                if (asset_ab_build_mapper.ContainsKey(asset_ab_mapper[asset_path]))
                {
                    asset_ab_build_mapper[asset_ab_mapper[asset_path]].Remove(asset_path);
                }

                asset_ab_mapper[asset_path] = ab_name;
            }

            if (asset_ab_build_mapper.ContainsKey(ab_name))
            {
                asset_ab_build_mapper[ab_name].Add(asset_path);
            }
            else
            {
                List<string> assets = new List<string>();
                assets.Add(asset_path);
                asset_ab_build_mapper.Add(ab_name, assets);
            }
        }
    }

    static Dictionary<string, bool> dynamic_ab_blacklist = new Dictionary<string, bool>();

    public static bool IsDynamicBlacklist(string asset_path)
    {
        return dynamic_ab_blacklist.ContainsKey(asset_path);
    }

    static AssetBundleBuild[] GetAssetBundleBuild()
    {
        List<AssetBundleBuild> ab_builds = new List<AssetBundleBuild>();
        foreach (KeyValuePair<string, List<string>> build_item in asset_ab_build_mapper)
        {
            AssetBundleBuild ab_build = new AssetBundleBuild();
            ab_build.assetBundleName = build_item.Key;
            ab_build.assetBundleVariant = "ab";
            ab_build.assetNames = build_item.Value.ToArray();
            ab_builds.Add(ab_build);
        }

        return ab_builds.ToArray();
    }
}