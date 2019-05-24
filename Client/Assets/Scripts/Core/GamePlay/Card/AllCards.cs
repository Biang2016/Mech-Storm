using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

/// <summary>
/// 卡片字典，其中记录的值不轻易更改
/// </summary>
public static class AllCards
{
    public static string CardsXMLFile => LoadAllBasicXMLFiles.ConfigFolderPath + "/Basic/Cards.xml";

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
        CardLevelDict_Remain = new SortedDictionary<int, List<CardInfo_Base>>();
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

    public static void ReloadCardXML()
    {
        AddAllCards();
    }

    public static void AddAllCards()
    {
        Reset();
        SortedDictionary<string, string> cardNameKeyDict = new SortedDictionary<string, string>();
        foreach (int v in Enum.GetValues(typeof(LanguageShorts)))
        {
            string strName = Enum.GetName(typeof(LanguageShorts), v);
            cardNameKeyDict[strName] = "cardName_" + strName;
        }

        string text;
        using (StreamReader sr = new StreamReader(CardsXMLFile))
        {
            text = sr.ReadToEnd();
        }

        XmlDocument doc = new XmlDocument();
        doc.LoadXml(text);
        XmlElement node_AllCards = doc.DocumentElement;
        for (int i = 0; i < node_AllCards.ChildNodes.Count; i++)
        {
            XmlNode node_Card = node_AllCards.ChildNodes.Item(i);
            int cardID = int.Parse(node_Card.Attributes["id"].Value);
            CardDict.Add(cardID, new CardInfo_Base());
        }

        for (int i = 0; i < node_AllCards.ChildNodes.Count; i++)
        {
            XmlNode node_Card = node_AllCards.ChildNodes.Item(i);
            BaseInfo baseInfo = new BaseInfo();
            UpgradeInfo upgradeInfo = new UpgradeInfo();
            LifeInfo lifeInfo = new LifeInfo();
            BattleInfo battleInfo = new BattleInfo();
            MechInfo mechInfo = new MechInfo();
            EquipInfo equipInfo = new EquipInfo();
            WeaponInfo weaponInfo = new WeaponInfo();
            ShieldInfo shieldInfo = new ShieldInfo();
            PackInfo packInfo = new PackInfo();
            MAInfo maInfo = new MAInfo();

            SideEffectBundle sideEffectBundle = new SideEffectBundle();

            for (int j = 0; j < node_Card.ChildNodes.Count; j++)
            {
                XmlNode node_CardInfo = node_Card.ChildNodes[j];
                switch (node_CardInfo.Attributes["name"].Value)
                {
                    case "baseInfo":
                        SortedDictionary<string, string> cardNameDict = new SortedDictionary<string, string>();
                        foreach (KeyValuePair<string, string> kv in cardNameKeyDict)
                        {
                            string cardName = node_CardInfo.Attributes[kv.Value].Value;
                            cardNameDict[kv.Key] = cardName;
                        }

                        baseInfo = new BaseInfo(
                            pictureID: int.Parse(node_CardInfo.Attributes["pictureID"].Value),
                            cardNames: cardNameDict,
                            isTemp: node_CardInfo.Attributes["isTemp"].Value == "True",
                            isHide: node_CardInfo.Attributes["isHide"].Value == "True",
                            metal: int.Parse(node_CardInfo.Attributes["metal"].Value),
                            energy: int.Parse(node_CardInfo.Attributes["energy"].Value),
                            coin: int.Parse(node_CardInfo.Attributes["coin"].Value),
                            effectFactor: 1,
                            limitNum: int.Parse(node_CardInfo.Attributes["limitNum"].Value),
                            cardRareLevel: int.Parse(node_CardInfo.Attributes["cardRareLevel"].Value),
                            cardType: (CardTypes) Enum.Parse(typeof(CardTypes), node_CardInfo.Attributes["cardType"].Value));
                        break;
                    case "upgradeInfo":
                        int u_id = int.Parse(node_CardInfo.Attributes["upgradeCardID"].Value);
                        int d_id = int.Parse(node_CardInfo.Attributes["degradeCardID"].Value);
                        if (!CardDict.ContainsKey(u_id))
                        {
                            u_id = -1;
                        }

                        if (!CardDict.ContainsKey(d_id))
                        {
                            d_id = -1;
                        }

                        upgradeInfo = new UpgradeInfo(
                            upgradeCardID: u_id,
                            degradeCardID: d_id,
                            cardLevel: 1,
                            cardLevelMax: 1);
                        break;
                    case "lifeInfo":
                        lifeInfo = new LifeInfo(
                            life: int.Parse(node_CardInfo.Attributes["life"].Value),
                            totalLife: int.Parse(node_CardInfo.Attributes["totalLife"].Value));
                        break;
                    case "battleInfo":
                        battleInfo = new BattleInfo(
                            basicAttack: int.Parse(node_CardInfo.Attributes["basicAttack"].Value),
                            basicArmor: int.Parse(node_CardInfo.Attributes["basicShield"].Value),
                            basicShield: int.Parse(node_CardInfo.Attributes["basicArmor"].Value));
                        break;
                    case "mechInfo":
                        mechInfo = new MechInfo(
                            isSoldier: node_CardInfo.Attributes["isSoldier"].Value == "True",
                            isDefense: node_CardInfo.Attributes["isDefense"].Value == "True",
                            isSniper: node_CardInfo.Attributes["isSniper"].Value == "True",
                            isCharger: node_CardInfo.Attributes["isCharger"].Value == "True",
                            isFrenzy: node_CardInfo.Attributes["isFrenzy"].Value == "True",
                            slot1: (SlotTypes) Enum.Parse(typeof(SlotTypes), node_CardInfo.Attributes["slot1"].Value),
                            slot2: (SlotTypes) Enum.Parse(typeof(SlotTypes), node_CardInfo.Attributes["slot2"].Value),
                            slot3: (SlotTypes) Enum.Parse(typeof(SlotTypes), node_CardInfo.Attributes["slot3"].Value),
                            slot4: (SlotTypes) Enum.Parse(typeof(SlotTypes), node_CardInfo.Attributes["slot4"].Value));
                        break;
                    case "weaponInfo":
                        weaponInfo = new WeaponInfo(
                            energy: int.Parse(node_CardInfo.Attributes["energy"].Value),
                            energyMax: int.Parse(node_CardInfo.Attributes["energyMax"].Value),
                            attack: int.Parse(node_CardInfo.Attributes["attack"].Value),
                            weaponType: (WeaponTypes) Enum.Parse(typeof(WeaponTypes), node_CardInfo.Attributes["weaponType"].Value),
                            isSentry: node_CardInfo.Attributes["isSentry"].Value == "True",
                            isFrenzy: node_CardInfo.Attributes["isFrenzy"].Value == "True");
                        equipInfo = new EquipInfo(SlotTypes.Weapon);
                        break;
                    case "shieldInfo":
                        shieldInfo = new ShieldInfo(
                            armor: int.Parse(node_CardInfo.Attributes["armor"].Value),
                            shield: int.Parse(node_CardInfo.Attributes["shield"].Value),
                            shieldType: (ShieldTypes) Enum.Parse(typeof(ShieldTypes), node_CardInfo.Attributes["shieldType"].Value),
                            isDefense: node_CardInfo.Attributes["isDefense"].Value == "True");
                        equipInfo = new EquipInfo(SlotTypes.Shield);
                        break;
                    case "packInfo":
                        packInfo = new PackInfo(
                            isFrenzy: node_CardInfo.Attributes["isFrenzy"].Value == "True",
                            isDefense: node_CardInfo.Attributes["isDefense"].Value == "True",
                            isSniper: node_CardInfo.Attributes["isSniper"].Value == "True",
                            dodgeProp: int.Parse(node_CardInfo.Attributes["dodgeProp"].Value)
                        );
                        equipInfo = new EquipInfo(SlotTypes.Pack);
                        break;
                    case "maInfo":
                        maInfo = new MAInfo(
                            isFrenzy: node_CardInfo.Attributes["isFrenzy"].Value == "True",
                            isDefense: node_CardInfo.Attributes["isDefense"].Value == "True",
                            isSniper: node_CardInfo.Attributes["isSniper"].Value == "True"
                        );
                        equipInfo = new EquipInfo(SlotTypes.MA);
                        break;
                    case "sideEffectsBundle":
                    {
                        ExtractSideEffectBundle(baseInfo.CardType, node_CardInfo, sideEffectBundle);
                        break;
                    }
                }
            }

            switch (baseInfo.CardType)
            {
                case CardTypes.Mech:
                    addCard(new CardInfo_Mech(
                        cardID: int.Parse(node_Card.Attributes["id"].Value),
                        baseInfo: baseInfo,
                        upgradeInfo: upgradeInfo,
                        lifeInfo: lifeInfo,
                        battleInfo: battleInfo,
                        mechInfo: mechInfo,
                        sideEffectBundle: sideEffectBundle));
                    break;
                case CardTypes.Equip:
                    addCard(new CardInfo_Equip(
                        cardID: int.Parse(node_Card.Attributes["id"].Value),
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
                        cardID: int.Parse(node_Card.Attributes["id"].Value),
                        baseInfo: baseInfo,
                        upgradeInfo: upgradeInfo,
                        sideEffectBundle: sideEffectBundle));
                    break;
                case CardTypes.Energy:
                    addCard(new CardInfo_Spell(
                        cardID: int.Parse(node_Card.Attributes["id"].Value),
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

    private static void ExtractSideEffectBundle(CardTypes cardType, XmlNode node_CardInfo, SideEffectBundle seb)
    {
        SideEffectExecute.SideEffectFrom sideEffectFrom = SideEffectExecute.SideEffectFrom.Unknown;
        switch (cardType)
        {
            case CardTypes.Mech:
                sideEffectFrom = SideEffectExecute.SideEffectFrom.MechSideEffect;
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

        for (int k = 0; k < node_CardInfo.ChildNodes.Count; k++)
        {
            XmlNode node_SideEffectExecute = node_CardInfo.ChildNodes.Item(k);
            SideEffectExecute.ExecuteSetting es = SideEffectExecute.ExecuteSetting.GenerateFromXMLNode(node_SideEffectExecute);

            List<SideEffectBase> ses = new List<SideEffectBase>();

            for (int m = 0; m < node_SideEffectExecute.ChildNodes.Count; m++)
            {
                XmlNode node_SideEffect = node_SideEffectExecute.ChildNodes.Item(m);
                SideEffectBase sideEffect = AllSideEffects.SideEffectsNameDict[node_SideEffect.Attributes["name"].Value].Clone();
                GetInfoForSideEffect(node_SideEffect, sideEffect);
                ses.Add(sideEffect);
            }

            SideEffectExecute see = new SideEffectExecute(sideEffectFrom, ses, es);
            seb.AddSideEffectExecute(see);
        }
    }

    public static void GetInfoForSideEffect(XmlNode node_SideEffect, SideEffectBase sideEffect)
    {
        sideEffect.M_SideEffectParam.GetParamsFromXMLNode(node_SideEffect);
        if (sideEffect is AddPlayerBuff_Base se)
        {
            se.BuffName = node_SideEffect.Attributes["BuffName"].Value;
            XmlNode node_Buff = node_SideEffect.FirstChild;
            GetInfoForBuff(node_Buff, se);
        }
    }

    public static void GetInfoForBuff(XmlNode node_Buff, AddPlayerBuff_Base parentSE)
    {
        parentSE.AttachedBuffSEE.M_ExecuteSetting = SideEffectExecute.ExecuteSetting.GenerateFromXMLNode(node_Buff);
        GetInfoForSideEffect(node_Buff, parentSE.AttachedBuffSEE.SideEffectBases[0]);
        for (int i = 0; i < node_Buff.ChildNodes.Count; i++)
        {
            XmlNode node_Buff_Sub_SideEffect = node_Buff.ChildNodes.Item(i);
            SideEffectBase sub_SideEffect = AllSideEffects.SideEffectsNameDict[node_Buff_Sub_SideEffect.Attributes["name"].Value].Clone();
            GetInfoForSideEffect(node_Buff_Sub_SideEffect, sub_SideEffect);
            parentSE.AttachedBuffSEE.SideEffectBases[0].Sub_SideEffect.Add(sub_SideEffect);
        }
    }

    public static void RefreshCardXML(CardInfo_Base ci)
    {
        if (CardDict.ContainsKey(ci.CardID))
        {
            CardDict[ci.CardID] = ci;
        }
        else
        {
            CardDict.Add(ci.CardID, ci);
        }

        string text;
        using (StreamReader sr = new StreamReader(CardsXMLFile))
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

        using (StreamWriter sw = new StreamWriter(CardsXMLFile))
        {
            doc.Save(sw);
        }
    }

    public static void RefreshAllCardXML()
    {
        string text;
        using (StreamReader sr = new StreamReader(CardsXMLFile))
        {
            text = sr.ReadToEnd();
        }

        XmlDocument doc = new XmlDocument();
        doc.LoadXml(text);
        XmlElement allCards = doc.DocumentElement;
        foreach (KeyValuePair<int, CardInfo_Base> kv in CardDict)
        {
            kv.Value.BaseExportToXML(allCards);
        }

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

        using (StreamWriter sw = new StreamWriter(CardsXMLFile))
        {
            doc.Save(sw);
        }
    }

    public static void DeleteCard(int cardID)
    {
        string text;
        using (StreamReader sr = new StreamReader(CardsXMLFile))
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

        using (StreamWriter sw = new StreamWriter(CardsXMLFile))
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