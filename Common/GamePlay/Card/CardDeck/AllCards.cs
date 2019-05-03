using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

/// <summary>
/// 卡片字典，其中记录的值不轻易更改
/// </summary>
public static class AllCards
{
    public static SortedDictionary<int, CardInfo_Base> CardDict = new SortedDictionary<int, CardInfo_Base>();
    public static SortedDictionary<int, List<CardInfo_Base>> CardLevelDict = new SortedDictionary<int, List<CardInfo_Base>>();
    public static SortedDictionary<int, List<CardInfo_Base>> CardLevelDict_Remain = new SortedDictionary<int, List<CardInfo_Base>>(); //某等级的卡片还剩哪些还没解锁

    public static void Reset()
    {
        CardDict.Clear();
        CardLevelDict.Clear();
        CardLevelDict_Remain.Clear();
    }

    private static void addCard(CardInfo_Base cardInfo)
    {
        if (!CardDict.ContainsKey(cardInfo.CardID))
        {
            CardDict.Add(cardInfo.CardID, cardInfo);
        }
        else
        {
            CardDict[cardInfo.CardID] = cardInfo;
        }

        if (cardInfo.UpgradeInfo.CardLevel <= 1) //按照不同星级的同一张卡片不存储两次
        {
            if (!cardInfo.BaseInfo.IsHide && !cardInfo.BaseInfo.IsTemp)
            {
                if (!CardLevelDict.ContainsKey(cardInfo.BaseInfo.CardRareLevel))
                {
                    CardLevelDict.Add(cardInfo.BaseInfo.CardRareLevel, new List<CardInfo_Base> {cardInfo});
                    CardLevelDict_Remain.Add(cardInfo.BaseInfo.CardRareLevel, new List<CardInfo_Base> {cardInfo});
                }
                else
                {
                    CardLevelDict[cardInfo.BaseInfo.CardRareLevel].Add(cardInfo);
                    CardLevelDict_Remain[cardInfo.BaseInfo.CardRareLevel].Add(cardInfo);
                }
            }
        }
    }

    public static CardInfo_Base GetRandomCardInfoByLevelNum(int levelNum, HashSet<int> exceptCardIDs = null)
    {
        if (exceptCardIDs == null)
        {
            exceptCardIDs = new HashSet<int>();
        }

        CardInfo_Base res = null;
        if (CardLevelDict_Remain.ContainsKey(levelNum))
        {
            List<CardInfo_Base> levelCards = CardLevelDict_Remain[levelNum];
            List<CardInfo_Base> removeLevelCards = new List<CardInfo_Base>();
            foreach (CardInfo_Base cb in levelCards)
            {
                if (exceptCardIDs.Contains(cb.CardID))
                {
                    removeLevelCards.Add(cb);
                }
            }

            foreach (CardInfo_Base cb in removeLevelCards)
            {
                levelCards.Remove(cb);
            }

            if (levelCards.Count >= 1)
            {
                res = Utils.GetRandomFromList(levelCards, 1)[0];
            }
        }

        return res == null ? res : res.Clone();
    }

    public static void ResetCardLevelDictRemain(List<int> unlockedCards)
    {
        CardLevelDict_Remain = new Dictionary<int, List<CardInfo_Base>>();
        foreach (KeyValuePair<int, List<CardInfo_Base>> kv in CardLevelDict)
        {
            foreach (CardInfo_Base cardInfo in kv.Value)
            {
                if (!CardLevelDict_Remain.ContainsKey(cardInfo.BaseInfo.CardRareLevel))
                {
                    CardLevelDict_Remain.Add(cardInfo.BaseInfo.CardRareLevel, new List<CardInfo_Base> {cardInfo});
                }
                else
                {
                    CardLevelDict_Remain[cardInfo.BaseInfo.CardRareLevel].Add(cardInfo);
                }
            }
        }

        foreach (int id in unlockedCards)
        {
            CardInfo_Base cb = CardDict[id];
            if (CardLevelDict_Remain.ContainsKey(cb.BaseInfo.CardRareLevel))
            {
                CardLevelDict_Remain[cb.BaseInfo.CardRareLevel].Remove(cb);
            }
        }
    }

    private static string CardsXMLPath = "";

    public static void ReloadCardXML()
    {
        AddAllCards(CardsXMLPath);
    }

    public static void AddAllCards(string cardsXMLPath)
    {
        CardsXMLPath = cardsXMLPath;
        Reset();
        SortedDictionary<string, string> cardNameKeyDict = new SortedDictionary<string, string>();
        foreach (int v in Enum.GetValues(typeof(LanguageShorts)))
        {
            string strName = Enum.GetName(typeof(LanguageShorts), v);
            cardNameKeyDict[strName] = "cardName_" + strName;
        }

        string text;
        using (StreamReader sr = new StreamReader(cardsXMLPath))
        {
            text = sr.ReadToEnd();
        }

        XmlDocument doc = new XmlDocument();
        doc.LoadXml(text);
        XmlElement allCards = doc.DocumentElement;
        for (int i = 0; i < allCards.ChildNodes.Count; i++)
        {
            XmlNode card = allCards.ChildNodes.Item(i);
            int cardID = int.Parse(card.Attributes["id"].Value);
            CardDict.Add(cardID, new CardInfo_Base());
        }

        for (int i = 0; i < allCards.ChildNodes.Count; i++)
        {
            XmlNode card = allCards.ChildNodes.Item(i);
            BaseInfo baseInfo = new BaseInfo();
            UpgradeInfo upgradeInfo = new UpgradeInfo();
            LifeInfo lifeInfo = new LifeInfo();
            BattleInfo battleInfo = new BattleInfo();
            RetinueInfo retinueInfo = new RetinueInfo();
            EquipInfo equipInfo = new EquipInfo();
            WeaponInfo weaponInfo = new WeaponInfo();
            ShieldInfo shieldInfo = new ShieldInfo();
            PackInfo packInfo = new PackInfo();
            MAInfo maInfo = new MAInfo();

            SideEffectBundle sideEffectBundle = new SideEffectBundle();

            for (int j = 0; j < card.ChildNodes.Count; j++)
            {
                XmlNode cardInfo = card.ChildNodes[j];
                switch (cardInfo.Attributes["name"].Value)
                {
                    case "baseInfo":
                        SortedDictionary<string, string> cardNameDict = new SortedDictionary<string, string>();
                        foreach (KeyValuePair<string, string> kv in cardNameKeyDict)
                        {
                            string cardName = cardInfo.Attributes[kv.Value].Value;
                            cardNameDict[kv.Key] = cardName;
                        }

                        baseInfo = new BaseInfo(
                            pictureID: int.Parse(cardInfo.Attributes["pictureID"].Value),
                            cardNames: cardNameDict,
                            isTemp: cardInfo.Attributes["isTemp"].Value == "True",
                            isHide: cardInfo.Attributes["isHide"].Value == "True",
                            metal: int.Parse(cardInfo.Attributes["metal"].Value),
                            energy: int.Parse(cardInfo.Attributes["energy"].Value),
                            coin: int.Parse(cardInfo.Attributes["coin"].Value),
                            effectFactor: 1,
                            limitNum: int.Parse(cardInfo.Attributes["limitNum"].Value),
                            cardRareLevel: int.Parse(cardInfo.Attributes["cardRareLevel"].Value),
                            cardType: (CardTypes) Enum.Parse(typeof(CardTypes), cardInfo.Attributes["cardType"].Value));
                        break;
                    case "upgradeInfo":
                        upgradeInfo = new UpgradeInfo(
                            upgradeCardID: int.Parse(cardInfo.Attributes["upgradeCardID"].Value),
                            degradeCardID: int.Parse(cardInfo.Attributes["degradeCardID"].Value),
                            cardLevel: 1,
                            cardLevelMax: 1);
                        break;
                    case "lifeInfo":
                        lifeInfo = new LifeInfo(
                            life: int.Parse(cardInfo.Attributes["life"].Value),
                            totalLife: int.Parse(cardInfo.Attributes["totalLife"].Value));
                        break;
                    case "battleInfo":
                        battleInfo = new BattleInfo(
                            basicAttack: int.Parse(cardInfo.Attributes["basicAttack"].Value),
                            basicArmor: int.Parse(cardInfo.Attributes["basicShield"].Value),
                            basicShield: int.Parse(cardInfo.Attributes["basicArmor"].Value));
                        break;
                    case "retinueInfo":
                        retinueInfo = new RetinueInfo(
                            isSoldier: cardInfo.Attributes["isSoldier"].Value == "True",
                            isDefense: cardInfo.Attributes["isDefense"].Value == "True",
                            isSniper: cardInfo.Attributes["isSniper"].Value == "True",
                            isCharger: cardInfo.Attributes["isCharger"].Value == "True",
                            isFrenzy: cardInfo.Attributes["isFrenzy"].Value == "True",
                            slot1: (SlotTypes) Enum.Parse(typeof(SlotTypes), cardInfo.Attributes["slot1"].Value),
                            slot2: (SlotTypes) Enum.Parse(typeof(SlotTypes), cardInfo.Attributes["slot2"].Value),
                            slot3: (SlotTypes) Enum.Parse(typeof(SlotTypes), cardInfo.Attributes["slot3"].Value),
                            slot4: (SlotTypes) Enum.Parse(typeof(SlotTypes), cardInfo.Attributes["slot4"].Value));
                        break;
                    case "weaponInfo":
                        weaponInfo = new WeaponInfo(
                            energy: int.Parse(cardInfo.Attributes["energy"].Value),
                            energyMax: int.Parse(cardInfo.Attributes["energyMax"].Value),
                            attack: int.Parse(cardInfo.Attributes["attack"].Value),
                            weaponType: (WeaponTypes) Enum.Parse(typeof(WeaponTypes), cardInfo.Attributes["weaponType"].Value),
                            isSentry: cardInfo.Attributes["isSentry"].Value == "True",
                            isFrenzy: cardInfo.Attributes["isFrenzy"].Value == "True");
                        equipInfo = new EquipInfo(SlotTypes.Weapon);
                        break;
                    case "shieldInfo":
                        shieldInfo = new ShieldInfo(
                            armor: int.Parse(cardInfo.Attributes["armor"].Value),
                            shield: int.Parse(cardInfo.Attributes["shield"].Value),
                            shieldType: (ShieldTypes) Enum.Parse(typeof(ShieldTypes), cardInfo.Attributes["shieldType"].Value),
                            isDefense: cardInfo.Attributes["isDefense"].Value == "True");
                        equipInfo = new EquipInfo(SlotTypes.Shield);
                        break;
                    case "packInfo":
                        packInfo = new PackInfo(
                            isFrenzy: cardInfo.Attributes["isFrenzy"].Value == "True",
                            isDefense: cardInfo.Attributes["isDefense"].Value == "True",
                            isSniper: cardInfo.Attributes["isSniper"].Value == "True",
                            dodgeProp: int.Parse(cardInfo.Attributes["dodgeProp"].Value)
                        );
                        equipInfo = new EquipInfo(SlotTypes.Pack);
                        break;
                    case "maInfo":
                        maInfo = new MAInfo(
                            isFrenzy: cardInfo.Attributes["isFrenzy"].Value == "True",
                            isDefense: cardInfo.Attributes["isDefense"].Value == "True",
                            isSniper: cardInfo.Attributes["isSniper"].Value == "True"
                        );
                        equipInfo = new EquipInfo(SlotTypes.MA);
                        break;
                    case "sideEffectsBundle":
                    {
                        ExtractSideEffectBundle(baseInfo.CardType, cardInfo, sideEffectBundle);
                        break;
                    }
                }
            }

            switch (baseInfo.CardType)
            {
                case CardTypes.Retinue:
                    addCard(new CardInfo_Retinue(
                        cardID: int.Parse(card.Attributes["id"].Value),
                        baseInfo: baseInfo,
                        upgradeInfo: upgradeInfo,
                        lifeInfo: lifeInfo,
                        battleInfo: battleInfo,
                        retinueInfo: retinueInfo,
                        sideEffectBundle: sideEffectBundle));
                    break;
                case CardTypes.Equip:
                    addCard(new CardInfo_Equip(
                        cardID: int.Parse(card.Attributes["id"].Value),
                        baseInfo: baseInfo,
                        upgradeInfo: upgradeInfo,
                        equipInfo: equipInfo,
                        weaponInfo: weaponInfo,
                        shieldInfo: shieldInfo,
                        packInfo: packInfo,
                        maInfo: maInfo,
                        sideEffectBundle: sideEffectBundle));
                    break;
                case CardTypes.Spell:
                    addCard(new CardInfo_Spell(
                        cardID: int.Parse(card.Attributes["id"].Value),
                        baseInfo: baseInfo,
                        upgradeInfo: upgradeInfo,
                        sideEffectBundle: sideEffectBundle));
                    break;
                case CardTypes.Energy:
                    addCard(new CardInfo_Spell(
                        cardID: int.Parse(card.Attributes["id"].Value),
                        baseInfo: baseInfo,
                        upgradeInfo: upgradeInfo,
                        sideEffectBundle: sideEffectBundle));
                    break;
            }
        }

        foreach (KeyValuePair<int, CardInfo_Base> kv in CardDict)
        {
            List<int> cardSeries = GetCardSeries(kv.Key);
            int cardLevelMax = cardSeries.Count;
            kv.Value.UpgradeInfo.CardLevelMax = cardLevelMax;
            int cardLevel = 0;
            foreach (int cardID in cardSeries)
            {
                cardLevel++;
                if (cardID == kv.Key)
                {
                    break;
                }
            }

            kv.Value.UpgradeInfo.CardLevel = cardLevel;
        }
    }

    private static void ExtractSideEffectBundle(CardTypes cardType, XmlNode cardInfo, SideEffectBundle seb)
    {
        SideEffectExecute.SideEffectFrom sideEffectFrom = SideEffectExecute.SideEffectFrom.Unknown;
        switch (cardType)
        {
            case CardTypes.Retinue:
                sideEffectFrom = SideEffectExecute.SideEffectFrom.RetinueSideEffect;
                break;
            case CardTypes.Equip:
                sideEffectFrom = SideEffectExecute.SideEffectFrom.EquipSideEffect;
                break;
            case CardTypes.Spell:
                sideEffectFrom = SideEffectExecute.SideEffectFrom.SpellCard;
                break;
            case CardTypes.Energy:
                sideEffectFrom = SideEffectExecute.SideEffectFrom.EnergyCard;
                break;
        }

        for (int k = 0; k < cardInfo.ChildNodes.Count; k++)
        {
            XmlNode sideEffectExecuteInfo = cardInfo.ChildNodes.Item(k);
            SideEffectExecute.ExecuteSetting es = SideEffectExecute.ExecuteSetting.GenerateFromXMLNode(sideEffectExecuteInfo);

            List<SideEffectBase> ses = new List<SideEffectBase>();

            for (int m = 0; m < sideEffectExecuteInfo.ChildNodes.Count; m++)
            {
                XmlNode sideEffectInfo = sideEffectExecuteInfo.ChildNodes.Item(m);
                SideEffectBase sideEffect = AllSideEffects.SideEffectsNameDict[sideEffectInfo.Attributes["name"].Value].Clone();
                GetInfoForSideEffect(sideEffectInfo, sideEffect);
                ses.Add(sideEffect);
            }

            SideEffectExecute see = new SideEffectExecute(sideEffectFrom, ses, es);
            seb.AddSideEffectExecute(see);
        }
    }

    public static void GetInfoForSideEffect(XmlNode sideEffectInfo, SideEffectBase sideEffect)
    {
        List<XmlAttribute> noMatchAttrs = sideEffect.M_SideEffectParam.GetParamsFromXMLNode(sideEffectInfo);
        foreach (XmlAttribute attr in noMatchAttrs)
        {
            if (sideEffect is AddPlayerBuff_Base se)
            {
                if (attr.Name.Contains("Buff."))
                {
                    if (!attr.Name.Contains("Buff.SideEffect."))
                    {
                        string attrName = attr.Name.Replace("Buff.", "");
                        SideEffectValue sev = se.AttachedBuffSEE.SideEffectBases[0].M_SideEffectParam.GetParam(attrName); //override其携带的buff的参数
                        if (sev != null)
                        {
                            sev.SetValue(attr.Value);
                        }
                        else
                        {
                            Utils.DebugLog("OverrideParamForBuff -> NoMatchAttr in " + sideEffect.GetType() + " attrName: " + attr.Name);
                        }
                    }
                    else
                    {
                        foreach (SideEffectBase buff_subSE in se.AttachedBuffSEE.SideEffectBases[0].Sub_SideEffect)
                        {
                            string attrName = attr.Name.Replace("Buff.SideEffect.", "");
                            SideEffectValue sev = buff_subSE.M_SideEffectParam.GetParam(attrName); //override其携带的buff的sideEffect的参数
                            if (sev != null)
                            {
                                sev.SetValue(attr.Value);
                            }
                            else
                            {
                                Utils.DebugLog("OverrideParamForBuff -> NoMatchAttr in " + sideEffect.GetType() + " attrName: " + attr.Name);
                            }
                        }
                    }
                }
            }
            else
            {
                Utils.DebugLog("OverrideParamForBuff -> NoMatchAttr in " + sideEffect.GetType());
            }
        }
    }

    public static void RefreshCardXML(CardInfo_Base ci)
    {
        string text;
        using (StreamReader sr = new StreamReader(CardsXMLPath))
        {
            text = sr.ReadToEnd();
        }

        XmlDocument doc = new XmlDocument();
        doc.LoadXml(text);
        XmlElement allCards = doc.DocumentElement;
        ci.BaseExportToXML(allCards);
        SortedDictionary<int, XmlElement> cardNodesDict = new SortedDictionary<int, XmlElement>();
        foreach (XmlElement node in allCards.ChildNodes)
        {
            cardNodesDict.Add(int.Parse(node.Attributes["id"].Value), node);
        }

        allCards.RemoveAll();
        foreach (KeyValuePair<int, XmlElement> kv in cardNodesDict)
        {
            allCards.AppendChild(kv.Value);
        }

        using (StreamWriter sw = new StreamWriter(CardsXMLPath))
        {
            doc.Save(sw);
        }
    }

    public static void DeleteCard(int cardID)
    {
        string text;
        using (StreamReader sr = new StreamReader(CardsXMLPath))
        {
            text = sr.ReadToEnd();
        }

        XmlDocument doc = new XmlDocument();
        doc.LoadXml(text);
        XmlElement allCards = doc.DocumentElement;
        SortedDictionary<int, XmlElement> cardNodesDict = new SortedDictionary<int, XmlElement>();
        foreach (XmlElement node in allCards.ChildNodes)
        {
            int id = int.Parse(node.Attributes["id"].Value);
            if (cardID != id)
            {
                cardNodesDict.Add(id, node);
            }
        }

        allCards.RemoveAll();
        foreach (KeyValuePair<int, XmlElement> kv in cardNodesDict)
        {
            allCards.AppendChild(kv.Value);
        }

        using (StreamWriter sw = new StreamWriter(CardsXMLPath))
        {
            doc.Save(sw);
        }
    }

    public static CardInfo_Base GetCard(int cardID)
    {
        if (CardDict.ContainsKey(cardID))
        {
            return CardDict[cardID].Clone();
        }
        else
        {
            return null;
        }
    }

    public static List<CardInfo_Base> GetCards(int[] cardIDs)
    {
        List<CardInfo_Base> res = new List<CardInfo_Base>();
        foreach (int cardID in cardIDs)
        {
            res.Add(GetCard(cardID));
        }

        return res;
    }

    public static bool IsASeries(CardInfo_Base card1, CardInfo_Base card2)
    {
        foreach (CardInfo_Base ci in GetCardSeries(card1))
        {
            if (card2.CardID == ci.CardID)
            {
                return true;
            }
        }

        return false;
    }

    public static int GetCardBaseCardID(int cardID)
    {
        CardInfo_Base de = CardDict[cardID];
        int baseID = de.CardID;
        while ((de = GetDegradeCardInfo(de)) != null)
        {
            baseID = de.CardID;
        }

        return baseID;
    }

    public static List<CardInfo_Base> GetCardSeries(CardInfo_Base cardInfo)
    {
        List<CardInfo_Base> res = new List<CardInfo_Base>();
        CardInfo_Base basic_card = cardInfo;
        CardInfo_Base de = cardInfo;
        while ((de = GetDegradeCardInfo(de)) != null)
        {
            basic_card = de;
        }

        res.Add(basic_card);

        CardInfo_Base up = basic_card;
        while ((up = GetUpgradeCardInfo(up)) != null)
        {
            res.Add(up);
        }

        return res;
    }

    public static List<int> GetCardSeries(int cardId)
    {
        List<int> res = new List<int>();
        CardInfo_Base cb = GetCard(cardId);
        if (cb == null) return res;
        List<CardInfo_Base> cis = GetCardSeries(cb);
        foreach (CardInfo_Base ci in cis)
        {
            res.Add(ci.CardID);
        }

        return res;
    }

    public static CardInfo_Base GetUpgradeCardInfo(CardInfo_Base cardInfo)
    {
        if (cardInfo.UpgradeInfo.UpgradeCardID == -1) return null;
        if (CardDict.ContainsKey(cardInfo.UpgradeInfo.UpgradeCardID))
        {
            return CardDict[cardInfo.UpgradeInfo.UpgradeCardID];
        }
        else
        {
            return null;
        }
    }

    public static CardInfo_Base GetUpgradeCardInfo(int cardId)
    {
        return GetUpgradeCardInfo(GetCard(cardId));
    }

    public static CardInfo_Base GetDegradeCardInfo(CardInfo_Base cardInfo)
    {
        if (cardInfo.UpgradeInfo.DegradeCardID == -1) return null;
        if (CardDict.ContainsKey(cardInfo.UpgradeInfo.DegradeCardID))
        {
            return CardDict[cardInfo.UpgradeInfo.DegradeCardID];
        }
        else
        {
            return null;
        }
    }

    public static CardInfo_Base GetDegradeCardInfo(int cardId)
    {
        return GetDegradeCardInfo(GetCard(cardId));
    }
}