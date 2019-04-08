using System.Collections.Generic;
using System.IO;
using UnityEditor;
//using UnityEditor.Build.Pipeline;
using UnityEngine;

public class AssetBundlePacker
{
    [MenuItem("AssetBundle/Packer/Windows")]
    static void AssetBundlePacker_StandaloneWindows()
    {
        Pack(BuildTarget.StandaloneWindows64);
    }

    [MenuItem("AssetBundle/Packer/MacOS")]
    static void AssetBundlePacker_MacOS()
    {
        Pack(BuildTarget.StandaloneOSX);
    }

    public static string GetPlatformForPackRes(BuildTarget target)
    {
        string res = "";
        switch (target)
        {
            case BuildTarget.StandaloneOSX:
            {
                res = "osx";
                break;
            }
            case BuildTarget.StandaloneWindows:
            {
                res = "windows";
                break;
            }
            case BuildTarget.StandaloneWindows64:
            {
                res = "windows";
                break;
            }
        }

        return res;
    }

    public static void Pack(BuildTarget build_target)
    {
        asset_ab_build_mapper.Clear();
        asset_ab_mapper.Clear();
        dynamic_ab_blacklist.Clear();
        EditorUtility.ClearProgressBar();

        // 准备打包 初始化Packer
        string platform = GetPlatformForPackRes(build_target);
        string out_path = Application.dataPath + "/StreamingAssets/AssetBundle/" + platform;

        if (!Directory.Exists(out_path))
        {
            Directory.CreateDirectory(out_path);
        }

        BuildAssetBundleOptions option = BuildAssetBundleOptions.ChunkBasedCompression
                                         | BuildAssetBundleOptions.DeterministicAssetBundle
            //| BuildAssetBundleOptions.DisableWriteTypeTree
            ;

        SetPack("Resources/Prefabs", "*.prefab", PackFolderToABOption.UseABName,"prefabs");
        SetPack("Animations", "*", PackFolderToABOption.UseABName, "animations");
        SetPack("Materials", "*", PackFolderToABOption.UseABName, "materials");
        SetPack("Materials", "*.shader", PackFolderToABOption.UseABName, "shaders");
        SetPack("Materials", "*.png", PackFolderToABOption.UseABName, "material_txs");
        SetPack("Models", "*.fbx", PackFolderToABOption.UseABName, "models");
        SetPack("Models", "*.mesh", PackFolderToABOption.UseABName, "models");
        SetPack("Fonts", "*", PackFolderToABOption.UseABName, "fonts");
        SetPack("Resources/SpriteAtlas", "*.spriteatlas", PackFolderToABOption.UseFileName, "atlas");
        SetPack("Resources/Audios/sfx", "*", PackFolderToABOption.UseABName, "audio_sfx");
        SetPack("Resources/Audios/bgm", "*", PackFolderToABOption.UseFileNamePrefix, "audio_bgm");
        SetPack("Textures", "*.png", PackFolderToABOption.UseSubFolderName, "textures");
        SetPack("Textures/Card", "*.png", PackFolderToABOption.UseSubFolderName, "textures_card");
        SetPack("Textures/Card/CardComponents", "*.png", PackFolderToABOption.UseSubFolderName, "textures_card_cardcomponents");
        SetPack("Textures/UI", "*.png", PackFolderToABOption.UseSubFolderName, "textures_ui");
//        CompatibilityBuildPipeline.BuildAssetBundles(out_path, GetAssetBundleBuild(), option, build_target);
    }

    enum PackFolderToABOption
    {
        UseFileName,
        UseFileNamePrefix,
        UseSubFolderName,
        UseABName,
    }

    static void SetPack(string relativeFolderPath, string fileExtension, PackFolderToABOption option, string abName = "")
    {
        abName = abName.ToLower();
        DirectoryInfo di = new DirectoryInfo(Application.dataPath + "/" + relativeFolderPath);
        if (option == PackFolderToABOption.UseABName)
        {
            foreach (FileInfo fi in di.GetFiles(fileExtension, SearchOption.AllDirectories))
            {
                SetAssetBundlePackGroup(fi.FullName, abName);
            }
        }
        else if (option == PackFolderToABOption.UseFileName)
        {
            foreach (FileInfo fi in di.GetFiles(fileExtension, SearchOption.AllDirectories))
            {
                string prefix = string.IsNullOrEmpty(abName) ? "" : (abName + "_");
                SetAssetBundlePackGroup(fi.FullName, prefix + fi.Name.Replace(fileExtension.Replace("*", ""), "").ToLower());
            }
        }
        else if (option == PackFolderToABOption.UseFileNamePrefix)
        {
            foreach (FileInfo fi in di.GetFiles(fileExtension, SearchOption.AllDirectories))
            {
                string prefix = (string.IsNullOrEmpty(abName) ? "" : (abName + "_")).ToLower();
                SetAssetBundlePackGroup(fi.FullName, prefix + fi.Name.Split('_')[0].ToLower());
            }
        }
        else if (option == PackFolderToABOption.UseSubFolderName)
        {
            foreach (DirectoryInfo sub_di in di.GetDirectories())
            {
                foreach (FileInfo fi in sub_di.GetFiles(fileExtension, SearchOption.AllDirectories))
                {
                    SetAssetBundlePackGroup(fi.FullName, abName + "_" + sub_di.Name.ToLower());
                }
            }
        }
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
            if (ab_build.assetNames.Length > 0)
            {
                ab_builds.Add(ab_build);
            }
        }

        return ab_builds.ToArray();
    }
}