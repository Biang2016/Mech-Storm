using System.Collections.Generic;

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

    public class BuildCards : IClone<BuildCards>
    {
        public string BuildName;
        public SortedDictionary<int, CardSelectInfo> CardSelectInfos; // Key: CardID

        public BuildCards(string buildName, SortedDictionary<int, CardSelectInfo> cardSelectInfos)
        {
            BuildName = buildName;
            CardSelectInfos = new SortedDictionary<int, CardSelectInfo>();
            foreach (KeyValuePair<int, CardInfo_Base> kv in AllCards.CardDict)
            {
                CardSelectInfos.Add(kv.Key, new CardSelectInfo(kv.Key, 0, kv.Value.BaseInfo.LimitNum));
            }

            foreach (KeyValuePair<int, CardSelectInfo> kv in cardSelectInfos)
            {
                if (AllCards.CardDict.ContainsKey(kv.Key))
                {
                    CardSelectInfos[kv.Key] = kv.Value.Clone();
                }
            }
        }

        public void ClearAllCardCounts()
        {
            foreach (KeyValuePair<int, CardSelectInfo> kv in CardSelectInfos)
            {
                kv.Value.CardSelectCount = 0;
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

        public SortedDictionary<int, int> GetCardLimitDict()
        {
            SortedDictionary<int, int> res = new SortedDictionary<int, int>();
            foreach (KeyValuePair<int, CardSelectInfo> kv in CardSelectInfos)
            {
                res[kv.Key] = kv.Value.CardSelectUpperLimit;
            }

            return res;
        }

        public SortedDictionary<int, int> GetBaseCardLimitDict()
        {
            SortedDictionary<int, int> res = new SortedDictionary<int, int>();
            foreach (KeyValuePair<int, CardSelectInfo> kv in CardSelectInfos)
            {
                int baseCardID = AllCards.GetCardBaseCardID(kv.Key);
                if (!res.ContainsKey(baseCardID))
                {
                    res.Add(baseCardID, kv.Value.CardSelectUpperLimit);
                }
                else
                {
                    res[baseCardID] += kv.Value.CardSelectUpperLimit;
                }
            }

            return res;
        }

        public BuildCards Clone()
        {
            SortedDictionary<int, CardSelectInfo> res = new SortedDictionary<int, CardSelectInfo>();
            foreach (KeyValuePair<int, CardSelectInfo> kv in CardSelectInfos)
            {
                res.Add(kv.Key, kv.Value.Clone());
            }

            return new BuildCards(BuildName, res);
        }

        public bool Equals(BuildCards o)
        {
            foreach (KeyValuePair<int, CardSelectInfo> kv in CardSelectInfos)
            {
                o.CardSelectInfos.TryGetValue(kv.Key, out CardSelectInfo csi);
                if (csi == null)
                {
                    return false;
                }
                else
                {
                    if (!csi.EqualsTo(kv.Value))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public void Serialize(DataStream writer)
        {
            writer.WriteString8(BuildName);
            writer.WriteSInt32(CardSelectInfos.Count);
            foreach (KeyValuePair<int, CardSelectInfo> kv in CardSelectInfos)
            {
                kv.Value.Serialize(writer);
            }
        }

        public static BuildCards Deserialize(DataStream reader)
        {
            string buildName = reader.ReadString8();
            int cardSelectInfoCount = reader.ReadSInt32();
            SortedDictionary<int, CardSelectInfo> cardSelectInfos = new SortedDictionary<int, CardSelectInfo>();
            for (int i = 0; i < cardSelectInfoCount; i++)
            {
                CardSelectInfo csi = CardSelectInfo.Deserialize(reader);
                if (cardSelectInfos.ContainsKey(csi.CardID))
                {
                    Utils.DebugLog("key duplicated : " + csi.CardID + " my buildname = " + buildName);
                }
                else
                {
                    cardSelectInfos.Add(csi.CardID, csi);
                }
            }

            return new BuildCards(buildName, cardSelectInfos);
        }

        public class CardSelectInfo : IClone<CardSelectInfo>
        {
            public int CardID;
            public int CardSelectCount;
            public int CardSelectUpperLimit;

            public CardSelectInfo(int cardID, int cardSelectCount, int cardSelectUpperLimit)
            {
                CardID = cardID;
                CardSelectCount = cardSelectCount;
                CardSelectUpperLimit = cardSelectUpperLimit;
            }

            public CardSelectInfo Clone()
            {
                return new CardSelectInfo(CardID, CardSelectCount, CardSelectUpperLimit);
            }

            public bool EqualsTo(CardSelectInfo o)
            {
                return o.CardID == CardID && o.CardSelectCount == CardSelectCount && o.CardSelectUpperLimit == CardSelectUpperLimit;
            }

            public void Serialize(DataStream writer)
            {
                writer.WriteSInt32(CardID);
                writer.WriteSInt32(CardSelectCount);
                writer.WriteSInt32(CardSelectUpperLimit);
            }

            public static CardSelectInfo Deserialize(DataStream reader)
            {
                int cardID = reader.ReadSInt32();
                int cardSelectCount = reader.ReadSInt32();
                int cardSelectUpperLimit = reader.ReadSInt32();
                return new CardSelectInfo(cardID, cardSelectCount, cardSelectUpperLimit);
            }
        }
    }

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