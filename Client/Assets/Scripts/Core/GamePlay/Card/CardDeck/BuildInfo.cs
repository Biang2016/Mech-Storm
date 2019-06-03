using System.Collections.Generic;
using System.Text;
using System.Xml;

public class BuildInfo : IClone<BuildInfo>
{
    private static int BuildIdIndex = 1;

    public static int GenerateBuildID()
    {
        return BuildIdIndex++;
    }

    public int BuildID;
    public string BuildName;
    public BuildCards M_BuildCards;

    public int DrawCardNum;
    public int Life;
    public int Energy;
    public int BeginMetal;
    public bool IsHighLevelCardLocked = false;
    public GamePlaySettings GamePlaySettings; //只对客户端选卡起到限制作用，服务端这个字段没有作用

    public int CardConsumeCoin
    {
        get
        {
            int coin = 0;
            foreach (KeyValuePair<int, BuildCards.CardSelectInfo> kv in M_BuildCards.CardSelectInfos)
            {
                CardInfo_Base cb = AllCards.GetCard(kv.Key);
                if (cb != null)
                {
                    coin += cb.BaseInfo.Coin * kv.Value.CardSelectCount;
                }
            }

            return coin;
        }
    }

    public int LifeConsumeCoin
    {
        get
        {
            if (GamePlaySettings == null)
            {
                return 0;
            }
            else
            {
                return (Life - GamePlaySettings.DefaultLifeMin) * GamePlaySettings.LifeToCoin;
            }
        }
    }

    public int EnergyConsumeCoin
    {
        get { return Energy * GamePlaySettings.EnergyToCoin; }
    }

    public int DrawCardNumConsumeCoin
    {
        get
        {
            if (GamePlaySettings == null)
            {
                return 0;
            }
            else
            {
                return GamePlaySettings.DrawCardNumToCoin[DrawCardNum] - GamePlaySettings.DrawCardNumToCoin[GamePlaySettings.MinDrawCardNum];
            }
        }
    }

    public BuildInfo()
    {
    }

    public BuildInfo(int buildID, string buildName, BuildCards buildCards, int drawCardNum, int life, int energy, int beginMetal, bool isHighLevelCardLocked, GamePlaySettings gamePlaySettings)
    {
        BuildID = buildID;
        BuildName = buildName;
        M_BuildCards = buildCards.Clone();
        DrawCardNum = drawCardNum;
        Life = life;
        Energy = energy;
        BeginMetal = beginMetal;
        IsHighLevelCardLocked = isHighLevelCardLocked;
        GamePlaySettings = gamePlaySettings;
    }

    public int BuildConsumeCoin
    {
        get { return CardConsumeCoin + LifeConsumeCoin + EnergyConsumeCoin + DrawCardNumConsumeCoin; }
    }

    public int CardCount
    {
        get
        {
            int count = 0;
            foreach (KeyValuePair<int, BuildCards.CardSelectInfo> kv in M_BuildCards.CardSelectInfos)
            {
                count += kv.Value.CardSelectCount;
            }

            return count;
        }
    }

    public bool IsEnergyEnough()
    {
        foreach (KeyValuePair<int, BuildCards.CardSelectInfo> kv in M_BuildCards.CardSelectInfos)
        {
            if (AllCards.GetCard(kv.Key).BaseInfo.Energy > Energy)
            {
                return false;
            }
        }

        return true;
    }

    public BuildInfo Clone()
    {
        return new BuildInfo(GenerateBuildID(), BuildName, M_BuildCards, DrawCardNum, Life, Energy, BeginMetal, IsHighLevelCardLocked, GamePlaySettings);
    }

    public bool EqualsTo(BuildInfo targetBuildInfo)
    {
        if (BuildID != targetBuildInfo.BuildID) return false;
        if (BuildName != targetBuildInfo.BuildName) return false;
        if (!M_BuildCards.Equals(targetBuildInfo.M_BuildCards)) return false;
        if (DrawCardNum != targetBuildInfo.DrawCardNum) return false;
        if (Life != targetBuildInfo.Life) return false;
        if (Energy != targetBuildInfo.Energy) return false;
        if (BeginMetal != targetBuildInfo.BeginMetal) return false;

        return true;
    }

    public static BuildInfo GetBuildInfoFromXML(XmlNode buildInfoNode, out bool needRefresh)
    {
        needRefresh = false;
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
                    buildInfo.M_BuildCards = new BuildCards();
                    string[] cardID_strs = cardInfo.Attributes["ids"].Value.Split(';');
                    foreach (string s in cardID_strs)
                    {
                        if (string.IsNullOrEmpty(s)) continue;
                        string[] cardSelectInfo_strs = s.Trim('(').Trim(')').Split(',');
                        int cardID = int.Parse(cardSelectInfo_strs[0]);
                        if (!AllCards.CardDict.ContainsKey(cardID))
                        {
                            needRefresh = true;
                            continue;
                        }

                        int cardSelectCount = int.Parse(cardSelectInfo_strs[1]);
                        int cardSelectUpperLimit = int.Parse(cardSelectInfo_strs[2]);
                        BuildCards.CardSelectInfo csi = new BuildCards.CardSelectInfo(cardID, cardSelectCount, cardSelectUpperLimit);
                        buildInfo.M_BuildCards.CardSelectInfos[cardID] = csi;
                    }

                    break;
            }
        }

        return buildInfo;
    }

    public void ExportToXML(XmlElement parent_ele)
    {
        XmlDocument doc = parent_ele.OwnerDocument;
        XmlElement old_node = null;
        foreach (XmlElement build_node in parent_ele.ChildNodes)
        {
            if (build_node.FirstChild.Attributes["BuildName"].Value.Equals(BuildName))
            {
                old_node = build_node;
            }
        }

        if (old_node != null)
        {
            parent_ele.RemoveChild(old_node);
        }

        XmlElement buildInfo_Node = doc.CreateElement("BuildInfo");
        parent_ele.AppendChild(buildInfo_Node);

        XmlElement baseInfo = doc.CreateElement("Info");
        buildInfo_Node.AppendChild(baseInfo);
        baseInfo.SetAttribute("name", "baseInfo");
        baseInfo.SetAttribute("BuildName", BuildName);
        baseInfo.SetAttribute("DrawCardNum", DrawCardNum.ToString());
        baseInfo.SetAttribute("Life", Life.ToString());
        baseInfo.SetAttribute("Energy", Energy.ToString());
        baseInfo.SetAttribute("BeginMetal", BeginMetal.ToString());
        baseInfo.SetAttribute("IsHighLevelCardLocked", IsHighLevelCardLocked.ToString());

        XmlElement cardIDs = doc.CreateElement("Info");
        buildInfo_Node.AppendChild(cardIDs);
        cardIDs.SetAttribute("name", "cardIDs");
        StringBuilder sb = new StringBuilder();
        foreach (KeyValuePair<int, BuildCards.CardSelectInfo> kv in M_BuildCards.CardSelectInfos)
        {
            sb.Append(string.Format("({0},{1},{2});", kv.Value.CardID, kv.Value.CardSelectCount, kv.Value.CardSelectUpperLimit));
        }

        string cardID_str = sb.ToString().Trim(';');
        cardIDs.SetAttribute("ids", cardID_str);
    }

    public void Serialize(DataStream writer)
    {
        writer.WriteSInt32(BuildID);
        writer.WriteString8(BuildName);
        M_BuildCards.Serialize(writer);
        writer.WriteSInt32(DrawCardNum);
        writer.WriteSInt32(Life);
        writer.WriteSInt32(Energy);
        writer.WriteSInt32(BeginMetal);
        writer.WriteByte((byte) (IsHighLevelCardLocked ? 0x01 : 0x00));
    }

    public static BuildInfo Deserialize(DataStream reader)
    {
        int BuildID = reader.ReadSInt32();
        string BuildName = reader.ReadString8();
        BuildCards m_BuildCards = BuildCards.Deserialize(reader);
        int DrawCardNum = reader.ReadSInt32();
        int Life = reader.ReadSInt32();
        int Energy = reader.ReadSInt32();
        int BeginMetal = reader.ReadSInt32();
        bool IsHighLevelCardLocked = reader.ReadByte() == 0x01;
        BuildInfo buildInfo = new BuildInfo(BuildID, BuildName, m_BuildCards, DrawCardNum, Life, Energy, BeginMetal, IsHighLevelCardLocked, null);
        return buildInfo;
    }
}