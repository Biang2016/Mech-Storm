using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

internal class AllServerBuilds
{
    public static Dictionary<int, BuildInfo> Build_Dict = new Dictionary<int, BuildInfo>();

    private static void addBuild(BuildInfo buildInfo)
    {
        if (!Build_Dict.ContainsKey(buildInfo.BuildID)) Build_Dict.Add(buildInfo.BuildID, buildInfo);
    }

    public static void AddAllBuilds(string buildsXMLPath)
    {
        Database.Instance.AddUser("ServerAdmin", "Xuedapao007");

        if (!File.Exists(buildsXMLPath)) return;

        string text;
        using (StreamReader sr = new StreamReader(buildsXMLPath))
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
                        buildInfo.BuildID = Database.Instance.GenerateBuildID();
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

            Database.Instance.AddOrModifyBuild("ServerAdmin", buildInfo);
            Database.Instance.AddServerBuild(buildInfo.BuildName, buildInfo.BuildID);
        }
    }

    public static void ExportAllBuilds()
    {
        string exportPath = "../../Config/ServerBuilds.xml";

        XmlDocument doc = new XmlDocument();
        XmlDeclaration xmldecl = doc.CreateXmlDeclaration("1.0", "utf-8", null);
        XmlElement root = doc.DocumentElement;
        doc.InsertBefore(xmldecl, root);

        XmlElement ele = doc.CreateElement("AllBuilds");
        doc.AppendChild(ele);

        List<BuildInfo> buildInfos = Database.Instance.AllServerBuildInfo();
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

        doc.Save(exportPath);
    }
}