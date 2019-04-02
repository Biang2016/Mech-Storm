﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

/// <summary>
/// 卡片字典，其中记录的值不轻易更改
/// </summary>
public static class AllCards
{
    public static Dictionary<int, CardInfo_Base> CardDict = new Dictionary<int, CardInfo_Base>();
    public static Dictionary<int, int> CardPicIDDict = new Dictionary<int, int>();
    public static Dictionary<int, List<CardInfo_Base>> CardLevelDict = new Dictionary<int, List<CardInfo_Base>>();
    public static Dictionary<int, List<CardInfo_Base>> CardLevelDict_Remain = new Dictionary<int, List<CardInfo_Base>>(); //某等级的卡片还剩哪些还没解锁

    private static void addCard(CardInfo_Base cardInfo)
    {
        if (!CardDict.ContainsKey(cardInfo.CardID)) CardDict.Add(cardInfo.CardID, cardInfo);
        if (!CardPicIDDict.ContainsKey(cardInfo.CardID)) CardPicIDDict.Add(cardInfo.CardID, cardInfo.BaseInfo.PictureID);
        if (cardInfo.UpgradeInfo.CardLevel <= 1) //按照不同星级的同一张卡片不存储两次
        {
            if (!cardInfo.BaseInfo.Hide && !cardInfo.BaseInfo.IsTemp)
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
        if (AllCards.CardLevelDict_Remain.ContainsKey(levelNum))
        {
            List<CardInfo_Base> levelCards = AllCards.CardLevelDict_Remain[levelNum];
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

    public static void AddAllCards(string cardsXMLPath)
    {
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
            SideEffectBundle sideEffectBundle_OnBattleGround = new SideEffectBundle();

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

                        baseInfo = new BaseInfo(int.Parse(cardInfo.Attributes["pictureID"].Value),
                            cardNameDict,
                            cardInfo.Attributes["isTemp"].Value == "True",
                            cardInfo.Attributes["hide"].Value == "True",
                            int.Parse(cardInfo.Attributes["metal"].Value),
                            int.Parse(cardInfo.Attributes["energy"].Value),
                            int.Parse(cardInfo.Attributes["coin"].Value),
                            int.Parse(cardInfo.Attributes["effectFactor"].Value),
                            int.Parse(cardInfo.Attributes["limitNum"].Value),
                            int.Parse(cardInfo.Attributes["cardRareLevel"].Value),
                            (CardTypes) Enum.Parse(typeof(CardTypes), cardInfo.Attributes["cardType"].Value));
                        break;
                    case "upgradeInfo":
                        upgradeInfo = new UpgradeInfo(int.Parse(cardInfo.Attributes["upgradeCardID"].Value), int.Parse(cardInfo.Attributes["degradeCardID"].Value),
                            int.Parse(cardInfo.Attributes["cardLevel"].Value));
                        break;
                    case "lifeInfo":
                        lifeInfo = new LifeInfo(int.Parse(cardInfo.Attributes["life"].Value),
                            int.Parse(cardInfo.Attributes["totalLife"].Value));
                        break;
                    case "battleInfo":
                        battleInfo = new BattleInfo(int.Parse(cardInfo.Attributes["basicAttack"].Value),
                            int.Parse(cardInfo.Attributes["basicShield"].Value),
                            int.Parse(cardInfo.Attributes["basicArmor"].Value));
                        break;
                    case "retinueInfo":
                        retinueInfo = new RetinueInfo(
                            cardInfo.Attributes["isSoldier"].Value == "True",
                            cardInfo.Attributes["isDefence"].Value == "True",
                            cardInfo.Attributes["isSniper"].Value == "True",
                            cardInfo.Attributes["isCharger"].Value == "True",
                            cardInfo.Attributes["isFrenzy"].Value == "True",
                            (SlotTypes) Enum.Parse(typeof(SlotTypes), cardInfo.Attributes["slot1"].Value),
                            (SlotTypes) Enum.Parse(typeof(SlotTypes), cardInfo.Attributes["slot2"].Value),
                            (SlotTypes) Enum.Parse(typeof(SlotTypes), cardInfo.Attributes["slot3"].Value),
                            (SlotTypes) Enum.Parse(typeof(SlotTypes), cardInfo.Attributes["slot4"].Value));
                        break;
                    case "weaponInfo":
                        weaponInfo = new WeaponInfo(int.Parse(cardInfo.Attributes["energy"].Value),
                            int.Parse(cardInfo.Attributes["energyMax"].Value),
                            int.Parse(cardInfo.Attributes["attack"].Value),
                            (WeaponTypes) Enum.Parse(typeof(WeaponTypes), cardInfo.Attributes["weaponType"].Value),
                            cardInfo.Attributes["isSentry"].Value == "True",
                            cardInfo.Attributes["isFrenzy"].Value == "True");
                        equipInfo = new EquipInfo(SlotTypes.Weapon);
                        break;
                    case "shieldInfo":
                        shieldInfo = new ShieldInfo(int.Parse(cardInfo.Attributes["armor"].Value),
                            int.Parse(cardInfo.Attributes["shield"].Value),
                            (ShieldTypes) Enum.Parse(typeof(ShieldTypes), cardInfo.Attributes["shieldType"].Value),
                            cardInfo.Attributes["isDefence"].Value == "True");
                        equipInfo = new EquipInfo(SlotTypes.Shield);
                        break;
                    case "packInfo":
                        packInfo = new PackInfo(
                            cardInfo.Attributes["isFrenzy"].Value == "True",
                            cardInfo.Attributes["isDefence"].Value == "True",
                            cardInfo.Attributes["isSniper"].Value == "True",
                            int.Parse(cardInfo.Attributes["dodgeProp"].Value)
                        );
                        equipInfo = new EquipInfo(SlotTypes.Pack);
                        break;
                    case "maInfo":
                        maInfo = new MAInfo(
                            cardInfo.Attributes["isFrenzy"].Value == "True",
                            cardInfo.Attributes["isDefence"].Value == "True",
                            cardInfo.Attributes["isSniper"].Value == "True"
                        );
                        equipInfo = new EquipInfo(SlotTypes.MA);
                        break;
                    case "sideEffectsBundle":
                    {
                        ExtractSideEffectBundle(baseInfo.CardType, cardInfo, sideEffectBundle);
                        break;
                    }

                    case "sideEffectsBundle_OnBattleGround":
                    {
                        ExtractSideEffectBundle(baseInfo.CardType, cardInfo, sideEffectBundle_OnBattleGround);
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
                        sideEffectBundle: sideEffectBundle,
                        sideEffectBundle_OnBattleGround: sideEffectBundle_OnBattleGround));
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
                        sideEffectBundle: sideEffectBundle,
                        sideEffectBundle_OnBattleGround: sideEffectBundle_OnBattleGround));
                    break;
                case CardTypes.Spell:
                    addCard(new CardInfo_Spell(
                        cardID: int.Parse(card.Attributes["id"].Value),
                        baseInfo: baseInfo,
                        upgradeInfo: upgradeInfo,
                        sideEffectBundle: sideEffectBundle,
                        sideEffectBundle_OnBattleGround: sideEffectBundle_OnBattleGround));
                    break;
                case CardTypes.Energy:
                    addCard(new CardInfo_Spell(
                        cardID: int.Parse(card.Attributes["id"].Value),
                        baseInfo: baseInfo,
                        upgradeInfo: upgradeInfo,
                        sideEffectBundle: sideEffectBundle,
                        sideEffectBundle_OnBattleGround: sideEffectBundle_OnBattleGround));
                    break;
            }
        }
    }

    private static void ExtractSideEffectBundle(CardTypes cardType, XmlNode cardInfo, SideEffectBundle cur_seb)
    {
        SideEffectBundle.TriggerTime triggerTime = (SideEffectBundle.TriggerTime) Enum.Parse(typeof(SideEffectBundle.TriggerTime), cardInfo.Attributes["triggerTime"].Value);
        SideEffectBundle.TriggerRange triggerRange = (SideEffectBundle.TriggerRange) Enum.Parse(typeof(SideEffectBundle.TriggerRange), cardInfo.Attributes["triggerRange"].Value);
        int triggerDelayTimes = int.Parse(cardInfo.Attributes["triggerDelayTimes"].Value);
        int triggerTimes = int.Parse(cardInfo.Attributes["triggerTimes"].Value);
        SideEffectBundle.TriggerTime removeTriggerTime = (SideEffectBundle.TriggerTime) Enum.Parse(typeof(SideEffectBundle.TriggerTime), cardInfo.Attributes["removeTriggerTime"].Value);
        SideEffectBundle.TriggerRange removeTriggerRange = (SideEffectBundle.TriggerRange) Enum.Parse(typeof(SideEffectBundle.TriggerRange), cardInfo.Attributes["removeTriggerRange"].Value);
        int removeTriggerTimes = int.Parse(cardInfo.Attributes["removeTriggerTimes"].Value);

        for (int k = 0; k < cardInfo.ChildNodes.Count; k++)
        {
            XmlNode sideEffectInfo = cardInfo.ChildNodes[k];
            SideEffectBase sideEffect = AllSideEffects.SideEffectsNameDict[sideEffectInfo.Attributes["name"].Value].Clone();
            GetInfoForSideEffect(sideEffectInfo, sideEffect);

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

            SideEffectExecute see = new SideEffectExecute(sideEffectFrom, sideEffect, triggerTime, triggerRange, triggerDelayTimes, triggerTimes, removeTriggerTime, removeTriggerRange, removeTriggerTimes);
            cur_seb.AddSideEffectExecute(see);
        }
    }

    public static void GetInfoForSideEffect(XmlNode sideEffectInfo, SideEffectBase sideEffect)
    {
        for (int l = 0; l < sideEffectInfo.Attributes.Count; l++)
        {
            if (sideEffectInfo.Attributes[l].Name == "name") continue;
            XmlAttribute attr = sideEffectInfo.Attributes[l];
            FieldInfo fi = sideEffect.GetType().GetField(attr.Name);
            if (fi == null) //如果fi为空，表明这是override携带的buff的参数
            {
                if (sideEffect is AddPlayerBuff_Base se)
                {
                    fi = se.AttachedBuffSEE.GetType().GetField(attr.Name); //override其携带的buff的参数
                    SetAttr(se.AttachedBuffSEE, attr, fi);

                    fi = se.AttachedBuffSEE.SideEffectBase.GetType().GetField(attr.Name); //override其buff中的se的参数
                    SetAttr(se.AttachedBuffSEE.SideEffectBase, attr, fi);

                    foreach (SideEffectBase sub_se in se.AttachedBuffSEE.SideEffectBase.Sub_SideEffect) //override其buff中的se中的sub_se的参数
                    {
                        fi = sub_se.GetType().GetField(attr.Name);
                        SetAttr(sub_se, attr, fi);
                    }
                }
            }
            else
            {
                SetAttr(sideEffect, attr, fi);
            }
        }
    }

    public delegate void DebugLog(string log);

    public static DebugLog DebugLogHandler;

    private static void SetAttr(object obj, XmlAttribute attr, FieldInfo fi)
    {
        if (fi == null)
        {
            return; //!!!
        }

        switch (fi.FieldType.Name)
        {
            case "Int32":
                fi.SetValue(obj, int.Parse(attr.Value));
                break;
            case "String":
                fi.SetValue(obj, attr.Value);
                break;
            case "Boolean":
                fi.SetValue(obj, attr.Value == "True");
                break;
            case "TargetRange":
                fi.SetValue(obj, (TargetSideEffect.TargetRange) Enum.Parse(typeof(TargetSideEffect.TargetRange), attr.Value));
                break;
            case "TriggerTime":
                fi.SetValue(obj, (SideEffectBundle.TriggerTime) Enum.Parse(typeof(SideEffectBundle.TriggerTime), attr.Value));
                break;
            case "TriggerRange":
                fi.SetValue(obj, (SideEffectBundle.TriggerRange) Enum.Parse(typeof(SideEffectBundle.TriggerRange), attr.Value));
                break;
            case "CardTypes":
                fi.SetValue(obj, (CardTypes) Enum.Parse(typeof(CardTypes), attr.Value));
                break;
            case "SideEffectValue":
                fi.SetValue(obj, new SideEffectValue(int.Parse(attr.Value)));
                break;
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
        if (card1.CardID == card2.CardID) return true;
        int level1 = card1.UpgradeInfo.CardLevel;
        int level2 = card2.UpgradeInfo.CardLevel;
        if (level1 > level2)
        {
            return IsASeries(card2, card1);
        }

        int tmpUpgradeID = card1.UpgradeInfo.UpgradeCardID;
        while (tmpUpgradeID != -1)
        {
            if (tmpUpgradeID == card2.CardID)
            {
                return true;
            }

            tmpUpgradeID = GetCard((int) tmpUpgradeID).UpgradeInfo.UpgradeCardID;
        }

        return false;
    }

    public static int GetCardBaseCardID(int cardID)
    {
        CardInfo_Base de = CardDict[cardID];
        while ((de = GetDegradeCardInfo(de)) != null)
        {
        }

        return de.CardID;
    }

    public static List<CardInfo_Base> GetCardSeries(CardInfo_Base cardInfo)
    {
        List<CardInfo_Base> res = new List<CardInfo_Base>();
        res.Add(cardInfo);
        CardInfo_Base de = cardInfo;
        while ((de = GetDegradeCardInfo(de)) != null)
        {
            res.Add(de);
        }

        CardInfo_Base up = cardInfo;
        while ((up = GetUpgradeCardInfo(up)) != null)
        {
            res.Add(up);
        }

        res.Sort((a, b) => a.UpgradeInfo.CardLevel.CompareTo(b.UpgradeInfo.CardLevel));
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