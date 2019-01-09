using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

internal class AllServerBuilds
{
    public static string DefaultBuildDirectory = "./Config/DefaultBuilds/";
    public static string ExportBuildDirectory = "../../Config/DefaultBuilds/";
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
                buildInfo.CardIDs = new List<int>();
                buildInfo.CriticalCardIDs = new List<int>();
                buildInfo.CardCountDict = new SortedDictionary<int, int>();
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
                            break;
                        case "cardIDs":
                            string[] cardID_str = cardInfo.Attributes["ids"].Value.Split(',');
                            foreach (string s in cardID_str)
                            {
                                if (string.IsNullOrEmpty(s)) continue;
                                buildInfo.CardIDs.Add(int.Parse(s));
                            }

                            break;
                        case "criticalCardIDs":
                            string[] criticalCardID_str = cardInfo.Attributes["ids"].Value.Split(',');
                            foreach (string s in criticalCardID_str)
                            {
                                if (string.IsNullOrEmpty(s)) continue;
                                buildInfo.CriticalCardIDs.Add(int.Parse(s));
                            }

                            break;
                        case "cardLimitNum":
                            string[] cardLimitNum = cardInfo.Attributes["ids"].Value.Split(',');
                            foreach (string s in cardLimitNum)
                            {
                                if (string.IsNullOrEmpty(s)) continue;
                                string[] values = s.Split('(');
                                if (values.Length == 1)
                                {
                                    buildInfo.CriticalCardIDs.Add(int.Parse(values[0]));
                                }
                                else if (values.Length == 2)
                                {
                                    int cardID = int.Parse(values[0]);
                                    buildInfo.CriticalCardIDs.Add(cardID);
                                    int cardLimitCount = int.Parse(values[1].TrimEnd(')'));
                                    buildInfo.CardCountDict.Add(cardID, cardLimitCount);
                                }
                            }

                            break;
                    }
                }

                bool isStory = pureName == "StoryAdmin";
                foreach (KeyValuePair<int, CardInfo_Base> kv in AllCards.CardDict)
                {
                    if (!buildInfo.CardCountDict.ContainsKey(kv.Key))
                    {
                        int limit = kv.Value.BaseInfo.LimitNum;
                        if (kv.Value.UpgradeInfo.CardLevel > 1)
                        {
                            limit = 0;
                        }

                        if (isStory)
                        {
                            limit = 0;
                        }

                        buildInfo.CardCountDict.Add(kv.Key, limit);
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

            XmlElement cardIDs = doc.CreateElement("Info");
            buildInfo_Node.AppendChild(cardIDs);
            cardIDs.SetAttribute("name", "cardIDs");

            int[] ids = buildInfo.CardIDs.ToArray();
            List<string> strs = new List<string>();
            foreach (int id in ids)
            {
                strs.Add(id.ToString());
            }

            cardIDs.SetAttribute("ids", string.Join(",", strs.ToArray()));

            if (builds.ManagerName == "StoryAdmin")
            {
                XmlElement cardLimitCount = doc.CreateElement("Info");
                buildInfo_Node.AppendChild(cardLimitCount);
                cardLimitCount.SetAttribute("name", "cardLimitNum");

                HashSet<int> unlockedSeriesCardIDs = new HashSet<int>();

                List<string> strs_ccd = new List<string>();
                foreach (KeyValuePair<int, int> kv in buildInfo.CardCountDict)
                {
                    List<int> temp = AllCards.GetCardSeries(kv.Key);
                    foreach (int cardID in temp)
                    {
                        unlockedSeriesCardIDs.Add(cardID);
                    }

                    strs_ccd.Add(kv.Key + "(" + kv.Value + ")");
                }

                foreach (KeyValuePair<int, CardInfo_Base> kv in AllCards.CardDict)
                {
                    if (!unlockedSeriesCardIDs.Contains(kv.Key))
                    {
                        strs_ccd.Add(kv.Key + "(0)");
                    }
                }

                cardLimitCount.SetAttribute("ids", string.Join(",", strs_ccd.ToArray()));
            }
        }

        doc.Save(ExportBuildDirectory + builds.ManagerName + ".xml");
    }
}