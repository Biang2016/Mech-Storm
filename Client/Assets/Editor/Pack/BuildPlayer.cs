using System.IO;
using UnityEditor;
using UnityEngine;

public class BuildPlayer
{
    [MenuItem("Build/Windows")]
    public static void Build_Windows()
    {
        Build(BuildTarget.StandaloneWindows64);
    }

    [MenuItem("Build/MacOS")]
    public static void Build_MacOS()
    {
        Build(BuildTarget.StandaloneOSX);
    }

    public static void Build(BuildTarget build_target)
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

        string[] levels = new string[] {"Assets/Scenes/FirstScene.unity", "Assets/Scenes/MainScene.unity"};
        BuildOptions option_build = BuildOptions.CompressWithLz4;

        string res = Application.dataPath + "/Resources";
        string res_back = Application.dataPath + "/Resources_back";

        string ab_Windows = Application.streamingAssetsPath + "/AssetBundle/windows";
        string ab_MacOS = Application.streamingAssetsPath + "/AssetBundle/osx";

        string ab_Windows_back = Application.dataPath + "/StreamingAsset_back/AssetBundle/windows";
        string ab_MacOS_back = Application.dataPath + "/StreamingAsset_back/AssetBundle/osx";

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
            Directory.Move(res, res_back);
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
            if (Directory.Exists(ab_Windows_back))
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
        }
        finally
        {
            if (Directory.Exists(res_back))
            {
                Directory.Move(res_back, res);
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