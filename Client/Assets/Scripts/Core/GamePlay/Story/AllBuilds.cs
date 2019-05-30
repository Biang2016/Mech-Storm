using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

public class AllBuilds
{
    private static string BuildDirectory => LoadAllBasicXMLFiles.ConfigFolderPath + "/Builds/";
    public static Dictionary<BuildGroups, BuildGroup> BuildGroupDict = new Dictionary<BuildGroups, BuildGroup>();

    public static void AddAllBuilds()
    {
        Reset();
        foreach (string path in Directory.GetFiles(BuildDirectory, "*.xml"))
        {
            FileInfo fi = new FileInfo(path);
            string pureName = fi.Name.Substring(0, fi.Name.LastIndexOf("."));
            BuildGroups bgType = (BuildGroups) Enum.Parse(typeof(BuildGroups), pureName);
            if (!BuildGroupDict.ContainsKey(bgType))
            {
                BuildGroup sb = new BuildGroup(pureName);
                BuildGroupDict.Add(bgType, sb);
            }

            string text;
            using (StreamReader sr = new StreamReader(path))
            {
                text = sr.ReadToEnd();
            }

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(text);
            XmlElement allBuilds = doc.DocumentElement;
            for (int i = 0; i < allBuilds.ChildNodes.Count; i++)
            {
                XmlNode buildInfoNode = allBuilds.ChildNodes.Item(i);
                BuildInfo buildInfo = GetBuildInfoFromXML(buildInfoNode);
                BuildStoryDatabase.Instance.AddOrModifyBuild(pureName, buildInfo);
                BuildGroupDict[bgType].AddBuild(buildInfo.BuildName, buildInfo);
            }
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

    public static BuildInfo GetBuildInfoFromXML(XmlNode buildInfoNode)
    {
        BuildInfo buildInfo = new BuildInfo();
        for (int i = 0; i < buildInfoNode.ChildNodes.Count; i++)
        {
            XmlNode cardInfo = buildInfoNode.ChildNodes.Item(i);
            switch (cardInfo.Attributes["name"].Value)
            {
                case "baseInfo":
                    buildInfo.BuildID = BuildInfo.GenerateBuildID();
                    buildInfo.BuildName = cardInfo.Attributes["BuildName"].Value;
                    buildInfo.DrawCardNum = int.Parse(cardInfo.Attributes["DrawCardNum"].Value);
                    buildInfo.DrawCardNum = int.Parse(cardInfo.Attributes["DrawCardNum"].Value);
                    buildInfo.Life = int.Parse(cardInfo.Attributes["Life"].Value);
                    buildInfo.Energy = int.Parse(cardInfo.Attributes["Energy"].Value);
                    buildInfo.BeginMetal = int.Parse(cardInfo.Attributes["BeginMetal"].Value);
                    buildInfo.IsHighLevelCardLocked = cardInfo.Attributes["IsHighLevelCardLocked"].Value.Equals("True");
                    break;
                case "cardIDs":
                    buildInfo.M_BuildCards = new BuildInfo.BuildCards();
                    string[] cardID_strs = cardInfo.Attributes["ids"].Value.Split(';');
                    foreach (string s in cardID_strs)
                    {
                        if (string.IsNullOrEmpty(s)) continue;
                        string[] cardSelectInfo_strs = s.Trim('(').Trim(')').Split(',');
                        int cardID = int.Parse(cardSelectInfo_strs[0]);
                        if (!AllCards.CardDict.ContainsKey(cardID)) continue;
                        int cardSelectCount = int.Parse(cardSelectInfo_strs[1]);
                        int cardSelectUpperLimit = int.Parse(cardSelectInfo_strs[2]);
                        BuildInfo.BuildCards.CardSelectInfo csi = new BuildInfo.BuildCards.CardSelectInfo(cardID, cardSelectCount, cardSelectUpperLimit);
                        buildInfo.M_BuildCards.CardSelectInfos[cardID] = csi;
                    }

                    break;
            }
        }

        return buildInfo;
    }

    public static void ExportBuilds(BuildGroup builds)
    {
        XmlDocument doc = new XmlDocument();
        XmlDeclaration xmldecl = doc.CreateXmlDeclaration("1.0", "utf-8", null);
        XmlElement root = doc.DocumentElement;
        doc.InsertBefore(xmldecl, root);

        XmlElement ele = doc.CreateElement("AllBuilds");
        doc.AppendChild(ele);

        SortedDictionary<int, List<BuildInfo>> sortedEnemyBuilds = new SortedDictionary<int, List<BuildInfo>>();

        if (builds.ManagerName == "EnemyBuilds")
        {
            foreach (BuildInfo buildInfo in builds.Builds.Values)
            {
                Match match = Regex_BuildName_StoryLevel.Match(buildInfo.BuildName);
                if (match.Success)
                {
                    int levelNum = -1;
                    foreach (Group matchGroup in match.Groups)
                    {
                        int.TryParse(matchGroup.Value, out levelNum);
                    }

                    if (levelNum != -1)
                    {
                        if (!sortedEnemyBuilds.ContainsKey(levelNum))
                        {
                            sortedEnemyBuilds.Add(levelNum, new List<BuildInfo>());
                        }

                        sortedEnemyBuilds[levelNum].Add(buildInfo);
                    }
                }
            }

            foreach (KeyValuePair<int, List<BuildInfo>> kv in sortedEnemyBuilds)
            {
                foreach (BuildInfo bi in kv.Value)
                {
                    builds.Builds.Remove(bi.BuildName);
                }
            }

            foreach (KeyValuePair<int, List<BuildInfo>> kv in sortedEnemyBuilds)
            {
                foreach (BuildInfo bi in kv.Value)
                {
                    builds.Builds.Add(bi.BuildName, bi);
                }
            }
        }

        foreach (BuildInfo buildInfo in builds.Builds.Values)
        {
            XmlElement buildInfo_Node = doc.CreateElement("BuildInfo");
            ele.AppendChild(buildInfo_Node);

            XmlElement baseInfo = doc.CreateElement("Info");
            buildInfo_Node.AppendChild(baseInfo);
            baseInfo.SetAttribute("name", "baseInfo");
            baseInfo.SetAttribute("BuildName", buildInfo.BuildName);
            baseInfo.SetAttribute("DrawCardNum", buildInfo.DrawCardNum.ToString());
            baseInfo.SetAttribute("Life", buildInfo.Life.ToString());
            baseInfo.SetAttribute("Energy", buildInfo.Energy.ToString());
            baseInfo.SetAttribute("BeginMetal", buildInfo.BeginMetal.ToString());
            baseInfo.SetAttribute("IsHighLevelCardLocked", buildInfo.IsHighLevelCardLocked.ToString());

            XmlElement cardIDs = doc.CreateElement("Info");
            buildInfo_Node.AppendChild(cardIDs);
            cardIDs.SetAttribute("name", "cardIDs");
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<int, BuildInfo.BuildCards.CardSelectInfo> kv in buildInfo.M_BuildCards.CardSelectInfos)
            {
                sb.Append(string.Format("({0},{1},{2});", kv.Value.CardID, kv.Value.CardSelectCount, kv.Value.CardSelectUpperLimit));
            }

            string cardID_str = sb.ToString().Trim(';');
            cardIDs.SetAttribute("ids", cardID_str);
        }

        doc.Save(BuildDirectory + builds.ManagerName + ".xml");
    }

    public static void Reset()
    {
        BuildGroupDict.Clear();
    }
}

public enum BuildGroups
{
    EnemyBuilds = 1,
    OnlineBuilds = 2,
    StoryBuilds = 3,
    CustomBuilds = 4
}