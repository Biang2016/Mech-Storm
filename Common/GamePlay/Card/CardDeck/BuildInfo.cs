using System;
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
    public SortedDictionary<int, int> CardCountDict; // 卡组中每张牌的选牌上限

    public List<int> CriticalCardIDs; //AI专用，重要卡牌起手抽到

    public int CardConsumeCoin;
    public GamePlaySettings GamePlaySettings; //只对客户端选卡起到限制作用，服务端这个字段没有作用

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

    public int BeginMetal;

    public BuildInfo()
    {
        CriticalCardIDs = new List<int>();
    }

    public BuildInfo(int buildID, string buildName, List<int> cardIDs, List<int> criticalCardIDs, int drawCardNum, int life, int energy, int beginMetal, GamePlaySettings gamePlaySettings, SortedDictionary<int, int> cardCountDict = null)
    {
        BuildID = buildID;
        BuildName = buildName;
        CardIDs = cardIDs;
        CriticalCardIDs = criticalCardIDs;
        DrawCardNum = drawCardNum;
        Life = life;
        Energy = energy;
        BeginMetal = beginMetal;
        BeginMetal = beginMetal;

        if (cardCountDict == null) // 缺省用默认配表
        {
            CardCountDict = new SortedDictionary<int, int>();
        }
        else
        {
            CardCountDict = cardCountDict;
        }

        foreach (int cardID in AllCards.CardDict.Keys)
        {
            if (!CardCountDict.ContainsKey(cardID))
            {
                CardInfo_Base cb = AllCards.GetCard(cardID);
                if (cb != null)
                {
                    CardCountDict.Add(cardID, cb.BaseInfo.LimitNum);
                }
            }
        }

        CardConsumeCoin = 0;
        foreach (int cardID in cardIDs)
        {
            CardInfo_Base cb = AllCards.GetCard(cardID);
            if (cb != null)
            {
                CardConsumeCoin += AllCards.GetCard(cardID).BaseInfo.Coin;
            }
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
        return new BuildInfo(GenerateBuildID(), BuildName, CardIDs.ToArray().ToList(), CriticalCardIDs.ToArray().ToList(), DrawCardNum, Life, Energy, BeginMetal, GamePlaySettings, CloneCardCountDict(CardCountDict));
    }

    public static SortedDictionary<int, int> CloneCardCountDict(SortedDictionary<int, int> cardCountDict)
    {
        SortedDictionary<int, int> res = new SortedDictionary<int, int>();
        foreach (KeyValuePair<int, int> kv in cardCountDict)
        {
            res.Add(kv.Key, kv.Value);
        }

        return res;
    }

    public bool EqualsTo(BuildInfo targetBuildInfo)
    {
        if (BuildID != targetBuildInfo.BuildID) return false;
        if (BuildName != targetBuildInfo.BuildName) return false;
        if (DrawCardNum != targetBuildInfo.DrawCardNum) return false;
        if (Life != targetBuildInfo.Life) return false;
        if (Energy != targetBuildInfo.Energy) return false;
        if (CardIDs.Count != targetBuildInfo.CardIDs.Count) return false;
        if (CardCountDict.Count != targetBuildInfo.CardCountDict.Count) return false;

        CardIDs.Sort();
        targetBuildInfo.CardIDs.Sort();
        for (int i = 0; i < CardIDs.Count; i++)
        {
            if (CardIDs[i] != targetBuildInfo.CardIDs[i]) return false;
        }

        foreach (KeyValuePair<int, int> kv in CardCountDict)
        {
            if (targetBuildInfo.CardCountDict.ContainsKey(kv.Key))
            {
                if (targetBuildInfo.CardCountDict[kv.Key] != kv.Value)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
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

        writer.WriteSInt32(CriticalCardIDs.Count);
        foreach (int cardID in CriticalCardIDs)
        {
            writer.WriteSInt32(cardID);
        }

        writer.WriteSInt32(CardConsumeCoin);
        writer.WriteSInt32(DrawCardNum);
        writer.WriteSInt32(Life);
        writer.WriteSInt32(Energy);
        writer.WriteSInt32(BeginMetal);

        writer.WriteSInt32(CardCountDict.Count);
        foreach (KeyValuePair<int, int> kv in CardCountDict)
        {
            writer.WriteSInt32(kv.Key);
            writer.WriteSInt32(kv.Value);
        }
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

        int criticalCardIdCount = reader.ReadSInt32();
        List<int> CriticalCardIDs = new List<int>();
        for (int i = 0; i < criticalCardIdCount; i++)
        {
            CriticalCardIDs.Add(reader.ReadSInt32());
        }

        int CardConsumeCoin = reader.ReadSInt32();
        int DrawCardNum = reader.ReadSInt32();
        int Life = reader.ReadSInt32();
        int Energy = reader.ReadSInt32();
        int BeginMetal = reader.ReadSInt32();

        SortedDictionary<int, int> CardCountDict = new SortedDictionary<int, int>();
        int cardCountDictCount = reader.ReadSInt32();
        for (int i = 0; i < cardCountDictCount; i++)
        {
            int key = reader.ReadSInt32();
            int value = reader.ReadSInt32();
            if (!CardCountDict.ContainsKey(key))
            {
                CardCountDict.Add(key, value);
            }
        }

        BuildInfo buildInfo = new BuildInfo(BuildID, BuildName, CardIDs, CriticalCardIDs, DrawCardNum, Life, Energy, BeginMetal, null, CardCountDict);
        return buildInfo;
    }
}