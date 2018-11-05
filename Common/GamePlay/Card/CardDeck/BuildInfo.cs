using System.Collections.Generic;
using System.Linq;

public class BuildInfo
{
    private static int BuildIdIndex = 1;

    public static int GenerateBuildID()
    {
        return BuildIdIndex++;
    }

    public int BuildID;

    public string BuildName;

    public List<int> CardIDs;

    public int CardConsumeCoin;
    public GamePlaySettings GamePlaySettings;

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

    public int DrawCardNum;

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

    public int Life;

    public int Energy;

    public BuildInfo()
    {
    }

    public BuildInfo(int buildID, string buildName, List<int> cardIDs, int drawCardNum, int life, int energy, GamePlaySettings gamePlaySettings)
    {
        BuildID = buildID;
        BuildName = buildName;
        CardIDs = cardIDs;
        DrawCardNum = drawCardNum;
        Life = life;
        Energy = energy;

        CardConsumeCoin = 0;
        foreach (int cardID in cardIDs)
        {
            CardInfo_Base cb = AllCards.GetCard(cardID);
            if (cb != null) CardConsumeCoin += AllCards.GetCard(cardID).BaseInfo.Coin;
        }

        GamePlaySettings = gamePlaySettings;
    }

    public int GetBuildConsumeCoin
    {
        get { return CardConsumeCoin + LifeConsumeCoin + EnergyConsumeCoin + DrawCardNumConsumeCoin; }
    }

    public int CardCount
    {
        get { return CardIDs.Count; }
    }

    public bool IsEnergyEnough()
    {
        foreach (int cardID in CardIDs)
        {
            if (AllCards.GetCard(cardID).BaseInfo.Energy > Energy) return false;
        }

        return true;
    }

    public BuildInfo Clone()
    {
        return new BuildInfo(GenerateBuildID(), BuildName, CardIDs.ToArray().ToList(), DrawCardNum, Life, Energy, GamePlaySettings);
    }

    public bool EqualsTo(BuildInfo targetBuildInfo)
    {
        if (BuildID != targetBuildInfo.BuildID) return false;
        if (BuildName != targetBuildInfo.BuildName) return false;
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
        writer.WriteString8(BuildName);
        writer.WriteSInt32(CardIDs.Count);
        foreach (int cardID in CardIDs)
        {
            writer.WriteSInt32(cardID);
        }

        writer.WriteSInt32(CardConsumeCoin);
        writer.WriteSInt32(DrawCardNum);
        writer.WriteSInt32(Life);
        writer.WriteSInt32(Energy);
    }

    public static BuildInfo Deserialize(DataStream reader)
    {
        int BuildID = reader.ReadSInt32();
        string BuildName = reader.ReadString8();

        int cardIdCount = reader.ReadSInt32();
        List<int> CardIDs = new List<int>();
        for (int i = 0; i < cardIdCount; i++)
        {
            CardIDs.Add(reader.ReadSInt32());
        }

        int CardConsumeCoin = reader.ReadSInt32();
        int DrawCardNum = reader.ReadSInt32();
        int Life = reader.ReadSInt32();
        int Energy = reader.ReadSInt32();

        BuildInfo buildInfo = new BuildInfo(BuildID, BuildName, CardIDs, DrawCardNum, Life, Energy, null);
        return buildInfo;
    }
}