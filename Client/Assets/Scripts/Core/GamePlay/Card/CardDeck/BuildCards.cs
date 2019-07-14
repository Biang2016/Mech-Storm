using System.Collections.Generic;

public class BuildCards : IClone<BuildCards>
{
    public SortedDictionary<int, CardSelectInfo> CardSelectInfos; // Key: CardID

    public BuildCards(DefaultCardLimitNumTypes defaultCardLimitNumType, SortedDictionary<int, CardSelectInfo> cardSelectInfos = null, SortedDictionary<int, bool> cardUnlockInfos = null)
    {
        CardSelectInfos = new SortedDictionary<int, CardSelectInfo>();
        foreach (KeyValuePair<int, CardInfo_Base> kv in AllCards.CardDict)
        {
            CardSelectInfos.Add(kv.Key, new CardSelectInfo(kv.Key, 0, (defaultCardLimitNumType == DefaultCardLimitNumTypes.BasedOnZero) ? 0 : kv.Value.BaseInfo.LimitNum));
        }

        if (cardSelectInfos != null)
        {
            foreach (KeyValuePair<int, CardSelectInfo> kv in cardSelectInfos)
            {
                if (AllCards.CardDict.ContainsKey(kv.Key))
                {
                    CardSelectInfos[kv.Key] = kv.Value.Clone();
                }
            }
        }
    }

    public enum DefaultCardLimitNumTypes
    {
        BasedOnCardBaseInfoLimitNum,
        BasedOnZero,
    }

    public void ClearAllCardCounts()
    {
        foreach (KeyValuePair<int, CardSelectInfo> kv in CardSelectInfos)
        {
            kv.Value.CardSelectCount = 0;
        }
    }

    public void ClearAllCardUpperLimit()
    {
        foreach (KeyValuePair<int, CardSelectInfo> kv in CardSelectInfos)
        {
            kv.Value.CardSelectCount = 0;
            kv.Value.CardSelectUpperLimit = 0;
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

    public List<int> GetHeroCardIDs(Editor_CardSelectModes mode)
    {
        List<int> heroCardIDs = new List<int>();
        foreach (KeyValuePair<int, CardSelectInfo> kv in CardSelectInfos)
        {
            if (AllCards.GetCard(kv.Key).CardStatType == CardStatTypes.HeroMech)
            {
                if (mode == Editor_CardSelectModes.SelectCount)
                {
                    for (int i = 0; i < kv.Value.CardSelectCount; i++)
                    {
                        heroCardIDs.Add(kv.Key);
                    }
                }
                else if (mode == Editor_CardSelectModes.UpperLimit)
                {
                    for (int i = 0; i < kv.Value.CardSelectUpperLimit; i++)
                    {
                        heroCardIDs.Add(kv.Key);
                    }
                }
            }
        }

        return heroCardIDs;
    }

    public SortedDictionary<CardStatTypes, int> GetTypeCardCountDict(Editor_CardSelectModes mode)
    {
        SortedDictionary<CardStatTypes, int> res = new SortedDictionary<CardStatTypes, int>();
        res.Add(CardStatTypes.Total, 0);
        res.Add(CardStatTypes.HeroMech, 0);
        res.Add(CardStatTypes.SoldierMech, 0);
        res.Add(CardStatTypes.Equip, 0);
        res.Add(CardStatTypes.Energy, 0);
        res.Add(CardStatTypes.Spell, 0);
        foreach (KeyValuePair<int, CardSelectInfo> kv in CardSelectInfos)
        {
            CardStatTypes type = AllCards.GetCard(kv.Key).CardStatType;
            if (mode == Editor_CardSelectModes.SelectCount)
            {
                res[type] += kv.Value.CardSelectCount;
                res[CardStatTypes.Total] += kv.Value.CardSelectCount;
            }
            else if (mode == Editor_CardSelectModes.UpperLimit)
            {
                res[type] += kv.Value.CardSelectUpperLimit;
                res[CardStatTypes.Total] += kv.Value.CardSelectUpperLimit;
            }
        }

        return res;
    }

    public SortedDictionary<int, int> GetCostDictByMetal(Editor_CardSelectModes mode, CardStatTypes cardStatType)
    {
        SortedDictionary<int, int> res = new SortedDictionary<int, int>();
        for (int i = 0; i <= 10; i++)
        {
            res.Add(i, 0);
        }

        foreach (KeyValuePair<int, CardSelectInfo> kv in CardSelectInfos)
        {
            CardInfo_Base ci = AllCards.GetCard(kv.Key);
            if (cardStatType == CardStatTypes.Total || ci.CardStatType == cardStatType)
            {
                if (mode == Editor_CardSelectModes.SelectCount)
                {
                    if (ci.BaseInfo.Metal < 10)
                    {
                        res[ci.BaseInfo.Metal] += kv.Value.CardSelectCount;
                    }
                    else
                    {
                        res[10] += kv.Value.CardSelectCount;
                    }
                }
                else if (mode == Editor_CardSelectModes.UpperLimit)
                {
                    if (ci.BaseInfo.Metal < 10)
                    {
                        res[ci.BaseInfo.Metal] += kv.Value.CardSelectUpperLimit;
                    }
                    else
                    {
                        res[10] += kv.Value.CardSelectUpperLimit;
                    }
                }
            }
        }

        return res;
    }

    public SortedDictionary<int, int> GetCostDictByEnergy(Editor_CardSelectModes mode, CardStatTypes cardStatType)
    {
        SortedDictionary<int, int> res = new SortedDictionary<int, int>();
        for (int i = 0; i <= 10; i++)
        {
            res.Add(i, 0);
        }

        foreach (KeyValuePair<int, CardSelectInfo> kv in CardSelectInfos)
        {
            CardInfo_Base ci = AllCards.GetCard(kv.Key);
            if (cardStatType == CardStatTypes.Total || ci.CardStatType == cardStatType)
            {
                if (mode == Editor_CardSelectModes.SelectCount)
                {
                    if (ci.BaseInfo.Energy < 10)
                    {
                        res[ci.BaseInfo.Energy] += kv.Value.CardSelectCount;
                    }
                    else
                    {
                        res[10] += kv.Value.CardSelectCount;
                    }
                }
                else if (mode == Editor_CardSelectModes.UpperLimit)
                {
                    if (ci.BaseInfo.Energy < 10)
                    {
                        res[ci.BaseInfo.Energy] += kv.Value.CardSelectUpperLimit;
                    }
                    else
                    {
                        res[10] += kv.Value.CardSelectUpperLimit;
                    }
                }
            }
        }

        return res;
    }

    public BuildCards Clone()
    {
        SortedDictionary<int, CardSelectInfo> cardSelectInfo = new SortedDictionary<int, CardSelectInfo>();
        foreach (KeyValuePair<int, CardSelectInfo> kv in CardSelectInfos)
        {
            cardSelectInfo.Add(kv.Key, kv.Value.Clone());
        }

        return new BuildCards(BuildCards.DefaultCardLimitNumTypes.BasedOnCardBaseInfoLimitNum, cardSelectInfo);
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
        writer.WriteSInt32(CardSelectInfos.Count);
        foreach (KeyValuePair<int, CardSelectInfo> kv in CardSelectInfos)
        {
            kv.Value.Serialize(writer);
        }
    }

    public static BuildCards Deserialize(DataStream reader)
    {
        int cardSelectInfoCount = reader.ReadSInt32();
        SortedDictionary<int, CardSelectInfo> cardSelectInfos = new SortedDictionary<int, CardSelectInfo>();
        for (int i = 0; i < cardSelectInfoCount; i++)
        {
            CardSelectInfo csi = CardSelectInfo.Deserialize(reader);
            cardSelectInfos.Add(csi.CardID, csi);
        }

        return new BuildCards(BuildCards.DefaultCardLimitNumTypes.BasedOnCardBaseInfoLimitNum, cardSelectInfos);
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

public enum Editor_CardSelectModes
{
    UpperLimit,
    SelectCount,
}