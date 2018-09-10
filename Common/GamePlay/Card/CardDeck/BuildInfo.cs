using System.Collections.Generic;
using System.Linq;

public struct BuildInfo
{
    public int BuildID;

    public string BuildName;

    public List<int> CardIDs;

    public int CardConsumeCoin;

    public int LifeConsumeCoin;

    public int EnergyConsumeCoin;

    public int DrawCardNum;

    public int Life;

    public int Energy;

    public BuildInfo(int buildID, string buildName, List<int> cardIDs, int cardConsumeCoin, int lifeConsumeCoin, int energyConsumeCoin, int drawCardNum, int life, int energy)
    {
        BuildID = buildID;
        BuildName = buildName;
        CardIDs = cardIDs;
        CardConsumeCoin = cardConsumeCoin;
        LifeConsumeCoin = lifeConsumeCoin;
        EnergyConsumeCoin = energyConsumeCoin;
        DrawCardNum = drawCardNum;
        Life = life;
        Energy = energy;
    }

    public int GetBuildConsumeCoin()
    {
        return CardConsumeCoin + LifeConsumeCoin + EnergyConsumeCoin + GamePlaySettings.GetDrawCardConsume(DrawCardNum);
    }

    public int CardCount()
    {
        return CardIDs.Count;
    }

    public BuildInfo Clone()
    {
        return new BuildInfo(BuildID, BuildName, CardIDs.ToArray().ToList(), CardConsumeCoin, LifeConsumeCoin, EnergyConsumeCoin, DrawCardNum, Life, Energy);
    }

    public bool EqualsTo(BuildInfo targetBuildInfo)
    {
        if (BuildID != targetBuildInfo.BuildID) return false;
        if (!BuildName.Equals(targetBuildInfo.BuildName)) return false;
        if (CardConsumeCoin != targetBuildInfo.CardConsumeCoin) return false;
        if (LifeConsumeCoin != targetBuildInfo.LifeConsumeCoin) return false;
        if (EnergyConsumeCoin != targetBuildInfo.EnergyConsumeCoin) return false;
        if (DrawCardNum != targetBuildInfo.DrawCardNum) return false;
        if (Life != targetBuildInfo.Life) return false;
        if (Energy != targetBuildInfo.Energy) return false;
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

        writer.WriteSInt32(CardConsumeCoin);
        writer.WriteSInt32(LifeConsumeCoin);
        writer.WriteSInt32(EnergyConsumeCoin);
        writer.WriteSInt32(DrawCardNum);
        writer.WriteSInt32(Life);
        writer.WriteSInt32(Energy);
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

        int CardConsumeCoin = reader.ReadSInt32();
        int LifeConsumeCoin = reader.ReadSInt32();
        int EnergyConsumeCoin = reader.ReadSInt32();
        int DrawCardNum = reader.ReadSInt32();
        int Life = reader.ReadSInt32();
        int Energy = reader.ReadSInt32();

        BuildInfo buildInfo = new BuildInfo(BuildID, BuildName, CardIDs, CardConsumeCoin, LifeConsumeCoin, EnergyConsumeCoin, DrawCardNum, Life, Energy);
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

        log += " [CardConsumeCoin]=" + CardConsumeCoin;
        log += " [LifeConsumeCoin]=" + LifeConsumeCoin;
        log += " [EnergyConsumeCoin]=" + EnergyConsumeCoin;
        log += " [DrawCardNum]=" + DrawCardNum;
        log += " [Life]=" + Life;
        log += " [Energy]=" + Energy;
        log += "</BuildInfo> ";
        return log;
    }
}