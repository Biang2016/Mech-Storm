using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

internal class AllServerBuilds
{
    public static string DefaultBuildDirectory
    {
        get { return ServerConsole.ServerRoot + "Config/DefaultBuilds/"; }
    }

    public static string ExportBuildDirectory
    {
        get
        {
            if (ServerConsole.Platform == ServerConsole.DEVELOP.FORMAL)
            {
                return ServerConsole.ServerRoot + "Config/DefaultBuilds/";
            }
            else
            {
                return "../../Config/DefaultBuilds/";
            }
        }
    }

    public static string SuperAccountPassword = "Xuedapao007";

    public static void AddAllBuilds()
    {
        foreach (string path in Directory.GetFiles(DefaultBuildDirectory))
        {
            FileInfo fi = new FileInfo(path);
            string pureName = fi.Name.Substring(0, fi.Name.LastIndexOf("."));
            Database.Instance.AddUser(pureName, SuperAccountPassword, true);
            if (!Database.Instance.SpecialBuildsDict.ContainsKey(pureName))
            {
                Database.SpecialBuilds sb = new Database.SpecialBuilds(pureName);
                Database.Instance.SpecialBuildsDict.Add(pureName, sb);
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
                XmlNode build = allBuilds.ChildNodes.Item(i);
                BuildInfo buildInfo = new BuildInfo();
                for (int j = 0; j < build.ChildNodes.Count; j++)
                {
                    XmlNode cardInfo = build.ChildNodes[j];
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
                            buildInfo.M_BuildCards = new BuildInfo.BuildCards(buildInfo.BuildName, new SortedDictionary<int, BuildInfo.BuildCards.CardSelectInfo>());
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
                                buildInfo.M_BuildCards.CardSelectInfos.Add(csi.CardID, csi);
                            }

                            foreach (int cardID in AllCards.CardDict.Keys)
                            {
                                int limit = AllCards.CardDict[cardID].BaseInfo.LimitNum;
                                if (!buildInfo.M_BuildCards.CardSelectInfos.ContainsKey(cardID))
                                {
                                    buildInfo.M_BuildCards.CardSelectInfos.Add(cardID, new BuildInfo.BuildCards.CardSelectInfo(cardID, 0, limit));
                                }
                            }

                            break;
                    }
                }

                Database.Instance.AddOrModifyBuild(pureName, buildInfo);
                Database.Instance.SpecialBuildsDict[pureName].AddBuild(buildInfo.BuildName, buildInfo.BuildID);
            }
        }
    }

    static Regex Regex_BuildName_StoryLevel = new Regex("[a-zA-Z0-9]_Lv([0-9]+)");

    public static void ExportBuilds(Database.SpecialBuilds builds)
    {
        XmlDocument doc = new XmlDocument();
        XmlDeclaration xmldecl = doc.CreateXmlDeclaration("1.0", "utf-8", null);
        XmlElement root = doc.DocumentElement;
        doc.InsertBefore(xmldecl, root);

        XmlElement ele = doc.CreateElement("AllBuilds");
        doc.AppendChild(ele);

        List<BuildInfo> buildInfos = builds.AllBuildInfo();
        SortedDictionary<int, List<BuildInfo>> sortedServerAdminBuilds = new SortedDictionary<int, List<BuildInfo>>();

        if (builds.ManagerName == "ServerAdmin")
        {
            foreach (BuildInfo buildInfo in buildInfos)
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
                        if (!sortedServerAdminBuilds.ContainsKey(levelNum))
                        {
                            sortedServerAdminBuilds.Add(levelNum, new List<BuildInfo>());
                        }

                        sortedServerAdminBuilds[levelNum].Add(buildInfo);
                    }
                }
            }

            foreach (KeyValuePair<int, List<BuildInfo>> kv in sortedServerAdminBuilds)
            {
                foreach (BuildInfo bi in kv.Value)
                {
                    buildInfos.Remove(bi);
                }
            }

            foreach (KeyValuePair<int, List<BuildInfo>> kv in sortedServerAdminBuilds)
            {
                buildInfos.AddRange(kv.Value);
            }
        }

        foreach (BuildInfo buildInfo in buildInfos)
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

            foreach (int cardID in AllCards.CardDict.Keys)
            {
                if (!buildInfo.M_BuildCards.CardSelectInfos.ContainsKey(cardID))
                {
                    sb.Append(string.Format("({0},{1},{2});", cardID, 0, AllCards.CardDict[cardID].BaseInfo.LimitNum));
                }
            }

            string cardID_str = sb.ToString().Trim(';');
            cardIDs.SetAttribute("ids", cardID_str);
        }

        doc.Save(ExportBuildDirectory + builds.ManagerName + ".xml");
    }
}