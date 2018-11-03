using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

internal class AllServerBuilds
{
    public static string DefaultBuildDirectory = "../../Config/DefaultBuilds/";
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
                for (int j = 0; j < build.ChildNodes.Count; j++)
                {
                    XmlNode cardInfo = build.ChildNodes[j];
                    switch (cardInfo.Attributes["name"].Value)
                    {
                        case "baseInfo":
                            buildInfo.BuildID = BuildInfo.GenerateBuildID();
                            buildInfo.BuildName = cardInfo.Attributes["BuildName"].Value;
                            buildInfo.DrawCardNum = int.Parse(cardInfo.Attributes["DrawCardNum"].Value);
                            buildInfo.Life = int.Parse(cardInfo.Attributes["Life"].Value);
                            buildInfo.Energy = int.Parse(cardInfo.Attributes["Energy"].Value);
                            break;
                        case "cardIDs":
                            string[] cardID_str = cardInfo.Attributes["ids"].Value.Split(',');
                            foreach (string s in cardID_str)
                            {
                                if (string.IsNullOrEmpty(s)) continue;
                                buildInfo.CardIDs.Add(int.Parse(s));
                            }

                            break;
                    }
                }

                Database.Instance.AddOrModifyBuild(pureName, buildInfo);
                Database.Instance.SpecialBuildsDict[pureName].AddBuild(buildInfo.BuildName, buildInfo.BuildID);
            }
        }
    }

    public static void ExportBuilds(Database.SpecialBuilds builds)
    {
        XmlDocument doc = new XmlDocument();
        XmlDeclaration xmldecl = doc.CreateXmlDeclaration("1.0", "utf-8", null);
        XmlElement root = doc.DocumentElement;
        doc.InsertBefore(xmldecl, root);

        XmlElement ele = doc.CreateElement("AllBuilds");
        doc.AppendChild(ele);

        List<BuildInfo> buildInfos = builds.AllBuildInfo();
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
        }

        doc.Save(DefaultBuildDirectory + builds.ManagerName + ".xml");
    }
}