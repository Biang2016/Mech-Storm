using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public class ABManager
{
    public static string GetAssetBundleFolderPath()
    {
        return Application.streamingAssetsPath + "/AssetBundle/" + ClientUtils.GetPlatformAbbr() + "/";
    }

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

        if (Manifest == null)
        {
            Manifest = new Dictionary<string, List<string>>();
            LoadManifestFile();
        }

        AssetBundle bundle = AssetBundle.LoadFromFile(GetAssetBundleFolderPath() + abName);
        if (!Manifest.ContainsKey(bundle.name))
        {
            Debug.Log(bundle.name);
        }

        List<string> dependencies = Manifest[bundle.name];
        foreach (string dependency in dependencies)
        {
            LoadAssetBundle(dependency);
        }

        return bundle;
    }

    public static Boo.Lang.List<AssetBundle> LoadAllAssetBundleNamedLike(string prefix)
    {
        Boo.Lang.List<AssetBundle> res = new Boo.Lang.List<AssetBundle>();
        foreach (string ab_name in Manifest.Keys)
        {
            if (ab_name.StartsWith(prefix))
            {
                res.Add(LoadAssetBundle(ab_name));
            }
        }

        DirectoryInfo di = new DirectoryInfo(GetAssetBundleFolderPath());
        foreach (FileInfo fi in di.GetFiles("*", SearchOption.AllDirectories))
        {
            if (fi.Name.StartsWith(prefix) && !fi.Name.EndsWith(".meta"))
            {
                res.Add(LoadAssetBundle(fi.Name));
            }
        }

        return res;
    }

    private static Dictionary<string, List<string>> Manifest;
    static Regex rg_name = new Regex(@"^\s+Name: (?<abName>[^\s]+)$");
    static Regex rg_depend = new Regex(@"^\s+(Dependency_[0-9]+:\s(?<dpName>[^\s]+)\s+)+.*$");
    static Regex rg_dependName = new Regex(@"Dependency_[0-9]+:\s(?<dpName>[^\s]+)");

    public static void LoadManifestFile()
    {
        string path = GetAssetBundleFolderPath() + "/" + ClientUtils.GetPlatformAbbr() + ".manifest";
        StreamReader sr = new StreamReader(path);
        string line = sr.ReadLine();
        string abName = null;
        while (!string.IsNullOrEmpty(line))
        {
            if (rg_name.IsMatch(line))
            {
                Match m = rg_name.Match(line);
                if (abName != null)
                {
                    Manifest.Add(abName, new List<string>());
                    abName = null;
                }

                abName = m.Groups["abName"].Value;
            }

            if (rg_depend.IsMatch(line))
            {
                List<string> dependencyList = new List<string>();
                MatchCollection mc = rg_dependName.Matches(line);
                foreach (Match match in mc)
                {
                    string dependencyName = match.Groups["dpName"].Value;
                    dependencyList.Add(dependencyName);
                }

                if (abName != null)
                {
                    Manifest.Add(abName, dependencyList);
                    abName = null;
                }
                else
                {
                    Debug.LogError("!!!" + line);
                }
            }

            line = sr.ReadLine();
        }

        if (abName != null)
        {
            Manifest.Add(abName, new List<string>());
        }

        sr.Close();
    }
}