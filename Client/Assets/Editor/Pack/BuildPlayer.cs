using System.IO;
using UnityEditor;
using UnityEngine;

public class BuildPlayer
{
    [MenuItem("Build/Windows (Build)")]
    public static void Build_Windows_OnlyBuild()
    {
        Build(BuildTarget.StandaloneWindows64);
    }

    [MenuItem("Build/MacOS (Build)")]
    public static void Build_MacOS_OnlyBuild()
    {
        Build(BuildTarget.StandaloneOSX);
    }

    [MenuItem("Build/Windows (Pack & Build)")]
    public static void Build_Windows_PackBuild()
    {
        AssetBundlePacker.AssetBundlePacker_StandaloneWindows();
        Build(BuildTarget.StandaloneWindows64);
    }

    [MenuItem("Build/MacOS (Pack & Build)")]
    public static void Build_MacOS_PackBuild()
    {
        AssetBundlePacker.AssetBundlePacker_MacOS();
        Build(BuildTarget.StandaloneOSX);
    }

    private static void Build(BuildTarget build_target)
    {
        string platform = AssetBundlePacker.GetPlatformForPackRes(build_target);
        string build_path = "";
        string build_ExecutableFile = "";
        if (platform == "windows")
        {
            build_path = Application.dataPath + "/../Build/" + platform + "/";
            build_ExecutableFile = build_path + "MechStorm.exe";
        }
        else if (platform == "osx")
        {
            build_path = Application.dataPath + "/../Build/" + platform + "/";
            build_ExecutableFile = build_path + "MechStorm.app";
        }

        string[] levels =
        {
            "Assets/Scenes/FirstScene.unity",
            "Assets/Scenes/MainScene.unity",
            "Assets/Scenes/CardEditorScene.unity",
            "Assets/Scenes/StoryEditorScene.unity",
        };

        BuildOptions option_build = BuildOptions.CompressWithLz4;

        string res = Application.dataPath + "/Resources/";
        string res_back = Application.dataPath + "/Resources_back/";

        string res_asset = "Assets/Resources";
        string res_back_asset = "Assets/Resources_back";

        string ab_Windows = Application.streamingAssetsPath + "/AssetBundle/windows/";
        string ab_MacOS = Application.streamingAssetsPath + "/AssetBundle/osx/";

        string ab_back = Application.dataPath + "/StreamingAsset_back/AssetBundle/";
        string ab_Windows_back = Application.dataPath + "/StreamingAsset_back/AssetBundle/windows/";
        string ab_MacOS_back = Application.dataPath + "/StreamingAsset_back/AssetBundle/osx/";

        if (Directory.Exists(ab_back))
        {
            Directory.Delete(ab_back, true);
        }

        Directory.CreateDirectory(ab_back);

        if (Directory.Exists(ab_Windows_back))
        {
            Directory.Delete(ab_Windows_back, true);
        }

        if (Directory.Exists(ab_MacOS_back))
        {
            Directory.Delete(ab_MacOS_back, true);
        }

        if (Directory.Exists(build_path))
        {
            Directory.Delete(build_path, true);
        }

        Directory.CreateDirectory(build_path);

        if (Directory.Exists(res_back))
        {
            Directory.Delete(res_back, true);
        }

        if (Directory.Exists(res))
        {
            if (Directory.Exists(res_back))
            {
                AssetDatabase.MoveAssetToTrash(res_back_asset);
            }

            string msg = AssetDatabase.MoveAsset(res_asset, res_back_asset);
            if (!string.IsNullOrEmpty(msg))
            {
                Debug.LogError(msg);
            }
        }

        if (build_target == BuildTarget.StandaloneWindows64)
        {
            if (Directory.Exists(ab_MacOS))
            {
                Directory.Move(ab_MacOS, ab_MacOS_back);
            }
        }
        else if (build_target == BuildTarget.StandaloneOSX)
        {
            if (Directory.Exists(ab_Windows))
            {
                Directory.Move(ab_Windows, ab_Windows_back);
            }
        }

        try
        {
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
            BuildPipeline.BuildPlayer(levels, build_ExecutableFile, build_target, option_build);
        }
        catch
        {
            // ignored
        }
        finally
        {
            if (Directory.Exists(res_back))
            {
                if (Directory.Exists(res))
                {
                    AssetDatabase.MoveAssetToTrash(res_asset);
                }

                string msg = AssetDatabase.MoveAsset(res_back_asset, res_asset);
                if (!string.IsNullOrEmpty(msg))
                {
                    Debug.LogError(msg);
                }
            }

            if (build_target == BuildTarget.StandaloneWindows64)
            {
                if (Directory.Exists(ab_MacOS_back))
                {
                    Directory.Move(ab_MacOS_back, ab_MacOS);
                }
            }
            else if (build_target == BuildTarget.StandaloneOSX)
            {
                if (Directory.Exists(ab_Windows_back))
                {
                    Directory.Move(ab_Windows_back, ab_Windows);
                }
            }
        }
    }
}