using System.Collections.Generic;
using System.Linq;

public struct BuildInfo
{
    public int BuildID;

    public string BuildName;

    public List<int> CardIDs;

    public int CardConsumeMoney;

    public int LifeConsumeMoney;

    public int MagicConsumeMoney;

    public int Life;

    public int Magic;

    public BuildInfo(int buildID, string buildName, List<int> cardIDs, int cardConsumeMoney, int lifeConsumeMoney, int magicConsumeMoney, int life, int magic)
    {
        BuildID = buildID;
        BuildName = buildName;
        CardIDs = cardIDs;
        CardConsumeMoney = cardConsumeMoney;
        LifeConsumeMoney = lifeConsumeMoney;
        MagicConsumeMoney = magicConsumeMoney;
        Life = life;
        Magic = magic;
    }

    public int GetBuildConsumeMoney()
    {
        return CardConsumeMoney + LifeConsumeMoney + MagicConsumeMoney;
    }

    public int CardCount()
    {
        return CardIDs.Count;
    }

    public BuildInfo Clone()
    {
        return new BuildInfo(BuildID, BuildName, CardIDs.ToArray().ToList(),  CardConsumeMoney, LifeConsumeMoney, MagicConsumeMoney, Life, Magic);
    }

    public bool EqualsTo(BuildInfo targetBuildInfo)
    {
        if (BuildID != targetBuildInfo.BuildID) return false;
        if (!BuildName.Equals(targetBuildInfo.BuildName)) return false;
        if (CardConsumeMoney != targetBuildInfo.CardConsumeMoney) return false;
        if (LifeConsumeMoney != targetBuildInfo.LifeConsumeMoney) return false;
        if (MagicConsumeMoney != targetBuildInfo.MagicConsumeMoney) return false;
        if (Life != targetBuildInfo.Life) return false;
        if (Magic != targetBuildInfo.Magic) return false;
        if (CardIDs.Count != targetBuildInfo.CardIDs.Count) return false;

        CardIDs.Sort();
        targetBuildInfo.CardIDs.Sort();
        for (int i = 0; i < CardIDs.Count; i++)
        {
            if (CardIDs[i] != targetBuildInfo.CardIDs[i]) return false;
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

        writer.WriteSInt32(CardConsumeMoney);
        writer.WriteSInt32(LifeConsumeMoney);
        writer.WriteSInt32(MagicConsumeMoney);
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

        int CardConsumeMoney = reader.ReadSInt32();
        int LifeConsumeMoney = reader.ReadSInt32();
        int MagicConsumeMoney = reader.ReadSInt32();
        int Life = reader.ReadSInt32();
        int Magic = reader.ReadSInt32();

        BuildInfo buildInfo = new BuildInfo(BuildID, BuildName, CardIDs,  CardConsumeMoney, LifeConsumeMoney, MagicConsumeMoney, Life, Magic);
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

        log += " [CardConsumeMoney]=" + CardConsumeMoney;
        log += " [LifeConsumeMoney]=" + LifeConsumeMoney;
        log += " [MagicConsumeMoney]=" + MagicConsumeMoney;
        log += " [Life]=" + Life;
        log += " [Magic]=" + Magic;
        log += "</BuildInfo> ";
        return log;
    }
}