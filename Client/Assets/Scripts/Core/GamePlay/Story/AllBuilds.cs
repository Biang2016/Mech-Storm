using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public class AllBuilds
{
    private static string BuildDirectory => LoadAllBasicXMLFiles.ConfigFolderPath + "/Builds/";

    private static SortedDictionary<BuildGroups, string> BuildGroupXMLDict = new SortedDictionary<BuildGroups, string>
    {
        {BuildGroups.CustomBuilds, BuildDirectory + "CustomBuilds.xml"},
        {BuildGroups.EnemyBuilds, BuildDirectory + "EnemyBuilds.xml"},
        {BuildGroups.OnlineBuilds, BuildDirectory + "OnlineBuilds.xml"},
    };

    public static Dictionary<BuildGroups, BuildGroup> BuildGroupDict = new Dictionary<BuildGroups, BuildGroup>();

    private static bool NeedReload = false;

    public static void AddAllBuilds()
    {
        Reset();

        foreach (KeyValuePair<BuildGroups, string> kv in BuildGroupXMLDict)
        {
            if (!BuildGroupDict.ContainsKey(kv.Key))
            {
                BuildGroup sb = new BuildGroup(kv.Key.ToString());
                BuildGroupDict.Add(kv.Key, sb);
            }

            string text;
            using (StreamReader sr = new StreamReader(kv.Value))
            {
                text = sr.ReadToEnd();
            }

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(text);
            XmlElement allBuilds = doc.DocumentElement;
            for (int i = 0; i < allBuilds.ChildNodes.Count; i++)
            {
                XmlNode buildInfoNode = allBuilds.ChildNodes.Item(i);
                BuildInfo buildInfo = BuildInfo.GetBuildInfoFromXML(buildInfoNode, out bool needRefresh, BuildCards.DefaultCardLimitNumTypes.BasedOnCardBaseInfoLimitNum);
                NeedReload |= needRefresh;
                BuildStoryDatabase.Instance.AddOrModifyBuild(kv.Key.ToString(), buildInfo);
                BuildGroupDict[kv.Key].AddBuild(buildInfo.BuildName, buildInfo);
            }
        }

        //If any problem, refresh XML and reload
        if (NeedReload)
        {
            NeedReload = false;
            RefreshAllBuildXML();
            ReloadBuildXML();
        }
    }

    static Regex Regex_BuildName_StoryLevel = new Regex("[a-zA-Z0-9]_Lv([0-9]+)");

    public static BuildInfo GetBuildInfo(BuildGroups buildGroups, string buildName)
    {
        if (BuildGroupDict.ContainsKey(buildGroups))
        {
            if (BuildGroupDict[buildGroups].Builds.ContainsKey(buildName))
            {
                return BuildGroupDict[buildGroups].Builds[buildName].Clone();
            }
        }

        return null;
    }

    public static void ReloadBuildXML()
    {
        AddAllBuilds();
    }

    public static void RefreshBuildXML(BuildGroups buildGroup, BuildInfo buildInfo)
    {
        buildInfo = buildInfo.Clone();
        Dictionary<string, BuildInfo> dict = BuildGroupDict[buildGroup].Builds;
        if (dict.ContainsKey(buildInfo.BuildName))
        {
            dict[buildInfo.BuildName] = buildInfo;
        }
        else
        {
            dict.Add(buildInfo.BuildName, buildInfo);
        }

        string text;
        using (StreamReader sr = new StreamReader(BuildGroupXMLDict[buildGroup]))
        {
            text = sr.ReadToEnd();
        }

        XmlDocument doc = new XmlDocument();
        doc.LoadXml(text);
        XmlElement allBuilds = doc.DocumentElement;
        buildInfo.ExportToXML(allBuilds);

        using (StreamWriter sw = new StreamWriter(BuildGroupXMLDict[buildGroup]))
        {
            doc.Save(sw);
        }
    }

    public static void RefreshAllBuildXML()
    {
        foreach (KeyValuePair<BuildGroups, BuildGroup> kv in BuildGroupDict)
        {
            string text;
            using (StreamReader sr = new StreamReader(BuildGroupXMLDict[kv.Key]))
            {
                text = sr.ReadToEnd();
            }

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(text);
            XmlElement allBuilds = doc.DocumentElement;
            foreach (KeyValuePair<string, BuildInfo> _kv in kv.Value.Builds)
            {
                _kv.Value.ExportToXML(allBuilds);
            }

            SortedDictionary<string, XmlElement> buildNodesDict = new SortedDictionary<string, XmlElement>();
            foreach (XmlElement node in allBuilds.ChildNodes)
            {
                buildNodesDict.Add(node.FirstChild.Attributes["BuildName"].Value, node);
            }

            allBuilds.RemoveAll();
            foreach (KeyValuePair<string, XmlElement> _kv in buildNodesDict)
            {
                allBuilds.AppendChild(_kv.Value);
            }

            using (StreamWriter sw = new StreamWriter(BuildGroupXMLDict[kv.Key]))
            {
                doc.Save(sw);
            }
        }
    }

    /// <summary>
    /// Can only be executed in StoryEditor/CardEditor/LevelEditor
    /// </summary>
    public void DeleteBuild(BuildGroups buildGroup, string buildName)
    {
        string text;
        using (StreamReader sr = new StreamReader(BuildGroupXMLDict[buildGroup]))
        {
            text = sr.ReadToEnd();
        }

        XmlDocument doc = new XmlDocument();
        doc.LoadXml(text);
        XmlElement allBuilds = doc.DocumentElement;
        SortedDictionary<string, XmlElement> buildNodesDict = new SortedDictionary<string, XmlElement>();
        foreach (XmlElement node in allBuilds.ChildNodes)
        {
            string name = node.Attributes["BuildName"].Value;
            if (name != buildName)
            {
                buildNodesDict.Add(name, node);
            }
        }

        allBuilds.RemoveAll();
        foreach (KeyValuePair<string, XmlElement> kv in buildNodesDict)
        {
            allBuilds.AppendChild(kv.Value);
        }

        using (StreamWriter sw = new StreamWriter(BuildGroupXMLDict[buildGroup]))
        {
            doc.Save(sw);
        }

        ReloadBuildXML();

        //删除Story中同名的Level
        SortedDictionary<LevelTypes, List<string>> removeLevelPath = new SortedDictionary<LevelTypes, List<string>>();
        foreach (KeyValuePair<LevelTypes, SortedDictionary<string, Level>> kv in AllLevels.LevelDict)
        {
            removeLevelPath.Add(kv.Key, new List<string>());
            foreach (KeyValuePair<string, Level> _kv in kv.Value)
            {
                if (_kv.Value is Enemy enemy)
                {
                    if (enemy.BuildInfo.BuildName.Equals(buildName))
                    {
                        removeLevelPath[kv.Key].Add(_kv.Key);
                    }
                }
            }
        }

        foreach (KeyValuePair<LevelTypes, List<string>> kv in removeLevelPath)
        {
            SortedDictionary<string, Level> dict = AllLevels.LevelDict[kv.Key];
            foreach (string s in kv.Value)
            {
                AllLevels.DeleteLevel(dict[s].LevelType, dict[s].LevelNames["en"]);
            }
        }
    }

    public static void Reset()
    {
        BuildGroupDict.Clear();
    }
}

[JsonConverter(typeof(StringEnumConverter))]
public enum BuildGroups
{
    EnemyBuilds = 1,
    OnlineBuilds = 2,
    CustomBuilds = 3
}