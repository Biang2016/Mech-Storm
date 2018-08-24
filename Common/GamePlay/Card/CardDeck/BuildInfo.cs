using System.Collections.Generic;
using System.Linq;

public struct BuildInfo
{
    public int BuildID;

    public string BuildName;

    public List<int> CardIDs;

    public List<int> BeginRetinueIDs;

    public int BuildConsumeMoney;

    public int Life;

    public int Magic;

    public BuildInfo(int buildID, string buildName, List<int> cardIDs, List<int> beginRetinueIDs, int buildConsumeMoney, int life, int magic)
    {
        BuildID = buildID;
        BuildName = buildName;
        CardIDs = cardIDs;
        BeginRetinueIDs = beginRetinueIDs;
        BuildConsumeMoney = buildConsumeMoney;
        Life = life;
        Magic = magic;
    }

    public int CardCount()
    {
        return CardIDs.Count + BeginRetinueIDs.Count;
    }

    public BuildInfo Clone()
    {
        return new BuildInfo(BuildID, BuildName, CardIDs.ToArray().ToList(), BeginRetinueIDs.ToArray().ToList(), BuildConsumeMoney, Life, Magic);
    }

    public bool EqualsTo(BuildInfo targetBuildInfo)
    {
        if (BuildID != targetBuildInfo.BuildID) return false;
        if (!BuildName.Equals(targetBuildInfo.BuildName)) return false;
        if (BuildConsumeMoney != targetBuildInfo.BuildConsumeMoney) return false;
        if (Life != targetBuildInfo.Life) return false;
        if (Magic != targetBuildInfo.Magic) return false;
        if (CardIDs.Count != targetBuildInfo.CardIDs.Count) return false;
        if (BeginRetinueIDs.Count != targetBuildInfo.BeginRetinueIDs.Count) return false;

        CardIDs.Sort();
        targetBuildInfo.CardIDs.Sort();
        for (int i = 0; i < CardIDs.Count; i++)
        {
            if (CardIDs[i] != targetBuildInfo.CardIDs[i]) return false;
        }

        BeginRetinueIDs.Sort();
        targetBuildInfo.BeginRetinueIDs.Sort();
        for (int i = 0; i < BeginRetinueIDs.Count; i++)
        {
            if (BeginRetinueIDs[i] != targetBuildInfo.BeginRetinueIDs[i]) return false;
        }

        return true;
    }

    public void Serialize(DataStream writer)
    {
        writer.WriteSInt32(BuildID);
        writer.WriteString16(BuildName);
        writer.WriteSInt32(CardIDs.Count);
        foreach (int cardID in CardIDs)
        {
            writer.WriteSInt32(cardID);
        }

        writer.WriteSInt32(BeginRetinueIDs.Count);
        foreach (int beginRetinueID in BeginRetinueIDs)
        {
            writer.WriteSInt32(beginRetinueID);
        }

        writer.WriteSInt32(BuildConsumeMoney);
        writer.WriteSInt32(Life);
        writer.WriteSInt32(Magic);
    }

    public static BuildInfo Deserialize(DataStream reader)
    {
        int BuildID = reader.ReadSInt32();
        string BuildName = reader.ReadString16();

        int cardIdCount = reader.ReadSInt32();
        List<int> CardIDs = new List<int>();
        for (int i = 0; i < cardIdCount; i++)
        {
            CardIDs.Add(reader.ReadSInt32());
        }

        int ceginRetinueIDCount = reader.ReadSInt32();
        List<int> BeginRetinueIDs = new List<int>();
        for (int i = 0; i < ceginRetinueIDCount; i++)
        {
            BeginRetinueIDs.Add(reader.ReadSInt32());
        }

        int BuildConsumeMoney = reader.ReadSInt32();
        int Life = reader.ReadSInt32();
        int Magic = reader.ReadSInt32();

        BuildInfo buildInfo = new BuildInfo(BuildID, BuildName, CardIDs, BeginRetinueIDs, BuildConsumeMoney, Life, Magic);
        return buildInfo;
    }

    public string DeserializeLog()
    {
        string log = " <BuildInfo>";
        log += " [BuildID]=" + BuildID;
        log += " [BuildName]=" + BuildName;
        log += " [CardIDs]=";
        foreach (int cardID in CardIDs)
        {
            log += cardID + " ";
        }

        log += " [BeginRetinueIDs]=";
        foreach (int cardID in BeginRetinueIDs)
        {
            log += cardID + " ";
        }

        log += " [BuildConsumeMoney]=" + BuildConsumeMoney;
        log += " [Life]=" + Life;
        log += " [Magic]=" + Magic;
        log += "</BuildInfo> ";
        return log;
    }
}