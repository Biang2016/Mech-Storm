using UnityEditor;
using System.IO;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;

public class BuildTool
{
    public static string[] GetBuildScenes()
    {
        List<string> names = new List<string>();

        foreach (EditorBuildSettingsScene e in EditorBuildSettings.scenes)
        {
            if (e == null)
                continue;

            if (e.enabled)
                names.Add(e.path);
        }

        return names.ToArray();
    }

    public static string GetVersion()
    {
        return Client.ClientVersion;
    }

    public static void DeleteDirectory(string target_dir)
    {
        string[] files = Directory.GetFiles(target_dir);
        string[] dirs = Directory.GetDirectories(target_dir);

        foreach (string file in files)
        {
            File.SetAttributes(file, FileAttributes.Normal);
            File.Delete(file);
        }

        foreach (string dir in dirs)
        {
            DeleteDirectory(dir);
        }

        Directory.Delete(target_dir, false);
    }


    //[MenuItem("LocalBuild/SetABMode")]
    public static void SetABMode()
    {
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, "ENABLE_AB_MODE");
    }

    //[MenuItem("LocalBuild/ClearABMode")]
    public static void ClearABMode()
    {
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, "");
    }

    //[MenuItem("LocalBuild/PC")]
    public static void BuildWindows()
    {
        SetABMode();
        string OutputPath = Application.dataPath + "/../pc";
        if (!Directory.Exists(OutputPath))
        {
            Directory.CreateDirectory(OutputPath);
        }

        PlayerSettings.bundleVersion = GetVersion();
        PlayerSettings.strippingLevel = StrippingLevel.StripAssemblies;
        BuildPipeline.BuildPlayer(GetBuildScenes(), OutputPath + "/GoldenClient.exe", BuildTarget.StandaloneWindows64, BuildOptions.None);
    }

    //[MenuItem("LocalBuild/PC_Dev")]
    public static void BuildWindowsDev()
    {
        SetABMode();
        string OutputPath = Application.dataPath + "/../pc";
        if (!Directory.Exists(OutputPath))
        {
            Directory.CreateDirectory(OutputPath);
        }

        PlayerSettings.bundleVersion = GetVersion();
        PlayerSettings.strippingLevel = StrippingLevel.StripAssemblies;
        BuildPipeline.BuildPlayer(GetBuildScenes(), OutputPath + "/GoldenClient.exe", BuildTarget.StandaloneWindows64, BuildOptions.Development | BuildOptions.AllowDebugging);
    }
}