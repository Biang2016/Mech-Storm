using System;
using System.Collections.Generic;
using System.IO;
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

    public SortedDictionary<int, CardSelectInfo> CardSelectInfos; // Key: CardID
    public int DrawCardNum;
    public int Life;
    public int Energy;
    public int BeginMetal;
    public GamePlaySettings GamePlaySettings; //只对客户端选卡起到限制作用，服务端这个字段没有作用

    public class CardSelectInfo
    {
        public int CardID;
        public int CardSelectUpperLimit;
        public int CardSelectCount;

        private CardSelectInfo(int cardID, int cardSelectUpperLimit, int cardSelectCount)
        {
            CardID = cardID;
            CardSelectUpperLimit = cardSelectUpperLimit;
            CardSelectCount = cardSelectCount;
        }

        public CardSelectInfo Clone()
        {
            return new CardSelectInfo(CardID, CardSelectUpperLimit, CardSelectCount);
        }

        public bool EqualsTo(CardSelectInfo o)
        {
            return o.CardID == CardID && o.CardSelectUpperLimit == CardSelectUpperLimit && o.CardSelectCount == CardSelectCount;
        }

        public void Serialize(DataStream writer)
        {
            writer.WriteSInt32(CardID);
            writer.WriteSInt32(CardSelectUpperLimit);
            writer.WriteSInt32(CardSelectCount);
        }

        public static CardSelectInfo Deserialize(DataStream reader)
        {
            int cardID = reader.ReadSInt32();
            int cardSelectUpperLimit = reader.ReadSInt32();
            int cardSelectCount = reader.ReadSInt32();
            return new CardSelectInfo(cardID, cardSelectUpperLimit, cardSelectCount);
        }
    }

    public List<int> GetCardIDs()
    {
        List<int> res = new List<int>();
        foreach (KeyValuePair<int, CardSelectInfo> kv in CardSelectInfos)
        {
            for (int i = 0; i < kv.Value.CardSelectCount; i++)
            {
                res.Add(kv.Value.CardID);
            }
        }

        return res;
    }

    public int CardConsumeCoin
    {
        get
        {
            int coin = 0;
            foreach (KeyValuePair<int, CardSelectInfo> kv in CardSelectInfos)
            {
                CardInfo_Base cb = AllCards.GetCard(kv.Key);
                if (cb != null)
                {
                    coin += cb.BaseInfo.Coin;
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

    public BuildInfo(int buildID, string buildName, SortedDictionary<int, CardSelectInfo> cardSelectInfos, int drawCardNum, int life, int energy, int beginMetal, GamePlaySettings gamePlaySettings)
    {
        BuildID = buildID;
        BuildName = buildName;
        CardSelectInfos = CloneCardSelectInfos(cardSelectInfos);
        DrawCardNum = drawCardNum;
        Life = life;
        Energy = energy;
        BeginMetal = beginMetal;
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
            foreach (KeyValuePair<int, CardSelectInfo> kv in CardSelectInfos)
            {
                count += kv.Value.CardSelectCount;
            }

            return count;
        }
    }

    public bool IsEnergyEnough()
    {
        foreach (KeyValuePair<int, CardSelectInfo> kv in CardSelectInfos)
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
        return new BuildInfo(GenerateBuildID(), BuildName, CardSelectInfos, DrawCardNum, Life, Energy, BeginMetal, GamePlaySettings);
    }

    public static SortedDictionary<int, CardSelectInfo> CloneCardSelectInfos(SortedDictionary<int, CardSelectInfo> cardSelectInfos)
    {
        if (cardSelectInfos == null) return null;
        SortedDictionary<int, CardSelectInfo> res = new SortedDictionary<int, CardSelectInfo>();
        foreach (KeyValuePair<int, CardSelectInfo> kv in cardSelectInfos)
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

        foreach (KeyValuePair<int, CardSelectInfo> kv in CardSelectInfos)
        {
            targetBuildInfo.CardSelectInfos.TryGetValue(kv.Key, out CardSelectInfo csi);
            if (csi != null)
            {
                if (!kv.Value.EqualsTo(csi))
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
        writer.WriteSInt32(CardSelectInfos.Count);
        foreach (KeyValuePair<int, CardSelectInfo> kv in CardSelectInfos)
        {
            kv.Value.Serialize(writer);
        }

        writer.WriteSInt32(DrawCardNum);
        writer.WriteSInt32(Life);
        writer.WriteSInt32(Energy);
        writer.WriteSInt32(BeginMetal);
    }

    public static BuildInfo Deserialize(DataStream reader)
    {
        int BuildID = reader.ReadSInt32();
        string BuildName = reader.ReadString8();

        int cardSelectInfoCount = reader.ReadSInt32();
        SortedDictionary<int, CardSelectInfo> cardSelectInfos = new SortedDictionary<int, CardSelectInfo>();
        CardSelectInfo csi = CardSelectInfo.Deserialize(reader);
        cardSelectInfos.Add(csi.CardID, csi);

        int DrawCardNum = reader.ReadSInt32();
        int Life = reader.ReadSInt32();
        int Energy = reader.ReadSInt32();
        int BeginMetal = reader.ReadSInt32();
        BuildInfo buildInfo = new BuildInfo(BuildID, BuildName, cardSelectInfos, DrawCardNum, Life, Energy, BeginMetal, null);
        return buildInfo;
    }
}