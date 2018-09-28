using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

public static class AllCards
{
    public static Dictionary<int, CardInfo_Base> CardDict = new Dictionary<int, CardInfo_Base>();

    private static void addCard(CardInfo_Base cardInfo)
    {
        CardDict.Add(cardInfo.CardID, cardInfo);
    }

    public static void AddAllCards(string cardsXMLPath)
    {
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

            SideEffectBundle sideEffects = new SideEffectBundle();
            SideEffectBundle sideEffects_OnBattleGround = new SideEffectBundle();

            for (int j = 0; j < card.ChildNodes.Count; j++)
            {
                XmlNode cardInfo = card.ChildNodes[j];
                switch (cardInfo.Attributes["name"].Value)
                {
                    case "baseInfo":
                        baseInfo = new BaseInfo(int.Parse(cardInfo.Attributes["pictureID"].Value),
                            cardInfo.Attributes["cardName"].Value,
                            cardInfo.Attributes["cardName_en"].Value,
                            cardInfo.Attributes["cardDesc"].Value.Replace("\\n", "\n"),
                            cardInfo.Attributes["hide"].Value=="True",
                            int.Parse(cardInfo.Attributes["metal"].Value),
                            int.Parse(cardInfo.Attributes["energy"].Value),
                            int.Parse(cardInfo.Attributes["coin"].Value),
                            int.Parse(cardInfo.Attributes["effectFactor"].Value),
                            (DragPurpose) Enum.Parse(typeof(DragPurpose), cardInfo.Attributes["dragPurpose"].Value),
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
                            (SlotTypes) Enum.Parse(typeof(SlotTypes), cardInfo.Attributes["slot1"].Value),
                            (SlotTypes) Enum.Parse(typeof(SlotTypes), cardInfo.Attributes["slot2"].Value),
                            (SlotTypes) Enum.Parse(typeof(SlotTypes), cardInfo.Attributes["slot3"].Value),
                            (SlotTypes) Enum.Parse(typeof(SlotTypes), cardInfo.Attributes["slot4"].Value));
                        break;
                    case "weaponInfo":
                        weaponInfo = new WeaponInfo(int.Parse(cardInfo.Attributes["energy"].Value),
                            int.Parse(cardInfo.Attributes["energyMax"].Value),
                            int.Parse(cardInfo.Attributes["attack"].Value),
                            (WeaponTypes) Enum.Parse(typeof(WeaponTypes), cardInfo.Attributes["weaponType"].Value));
                        equipInfo = new EquipInfo(SlotTypes.Weapon);
                        break;
                    case "shieldInfo":
                        shieldInfo = new ShieldInfo(int.Parse(cardInfo.Attributes["armor"].Value),
                            int.Parse(cardInfo.Attributes["shield"].Value),
                            (ShieldTypes) Enum.Parse(typeof(ShieldTypes), cardInfo.Attributes["shieldType"].Value));
                        equipInfo = new EquipInfo(SlotTypes.Shield);
                        break;
                    case "packInfo":
                        packInfo = new PackInfo();
                        equipInfo = new EquipInfo(SlotTypes.Pack);
                        break;
                    case "maInfo":
                        maInfo = new MAInfo();
                        equipInfo = new EquipInfo(SlotTypes.MA);
                        break;
                    case "sideEffectsInfo":
                    {
                        SideEffectBundle.TriggerTime triggerTime = (SideEffectBundle.TriggerTime) Enum.Parse(typeof(SideEffectBundle.TriggerTime), cardInfo.Attributes["triggerTime"].Value);
                        SideEffectBundle.TriggerRange triggerRange = (SideEffectBundle.TriggerRange) Enum.Parse(typeof(SideEffectBundle.TriggerRange), cardInfo.Attributes["triggerRange"].Value);

                        for (int k = 0; k < cardInfo.ChildNodes.Count; k++)
                        {
                            XmlNode sideEffectInfo = cardInfo.ChildNodes[k];
                            SideEffectBase sideEffect = AllSideEffects.SideEffectsNameDict[sideEffectInfo.Attributes["name"].Value].Clone();
                            for (int l = 0; l < sideEffectInfo.Attributes.Count; l++)
                            {
                                if (sideEffectInfo.Attributes[l].Name == "name") continue;
                                XmlAttribute attr = sideEffectInfo.Attributes[l];
                                FieldInfo fi = sideEffect.GetType().GetField(attr.Name);
                                switch (fi.FieldType.Name)
                                {
                                    case "Int32":
                                        fi.SetValue(sideEffect, int.Parse(attr.Value));
                                        break;
                                    case "String":
                                        fi.SetValue(sideEffect, attr.Value);
                                        break;
                                    case "Boolean":
                                        fi.SetValue(sideEffect, attr.Value == "True");
                                        break;
                                    case "TargetRange":
                                        fi.SetValue(sideEffect, (TargetSideEffect.TargetRange) Enum.Parse(typeof(TargetSideEffect.TargetRange), attr.Value));
                                        break;
                                }
                            }

                            sideEffects.AddSideEffect(sideEffect, triggerTime, triggerRange);
                        }

                        break;
                    }

                    case "sideEffectsInfo_OnBattleGround":
                    {
                        SideEffectBundle.TriggerTime triggerTime = (SideEffectBundle.TriggerTime) Enum.Parse(typeof(SideEffectBundle.TriggerTime), cardInfo.Attributes["triggerTime"].Value);
                        SideEffectBundle.TriggerRange triggerRange = (SideEffectBundle.TriggerRange) Enum.Parse(typeof(SideEffectBundle.TriggerRange), cardInfo.Attributes["triggerRange"].Value);

                        for (int k = 0; k < cardInfo.ChildNodes.Count; k++)
                        {
                            XmlNode sideEffectInfo = cardInfo.ChildNodes[k];
                            SideEffectBase sideEffect = AllSideEffects.SideEffectsNameDict[sideEffectInfo.Attributes["name"].Value].Clone();
                            for (int l = 0; l < sideEffectInfo.Attributes.Count; l++)
                            {
                                if (sideEffectInfo.Attributes[l].Name == "name") continue;
                                XmlAttribute attr = sideEffectInfo.Attributes[l];
                                FieldInfo fi = sideEffect.GetType().GetField(attr.Name);
                                switch (fi.FieldType.Name)
                                {
                                    case "Int32":
                                        fi.SetValue(sideEffect, int.Parse(attr.Value));
                                        break;
                                    case "String":
                                        fi.SetValue(sideEffect, attr.Value);
                                        break;
                                    case "Boolean":
                                        fi.SetValue(sideEffect, attr.Value == "True");
                                        break;
                                    case "TargetRange":
                                        fi.SetValue(sideEffect, (TargetSideEffect.TargetRange) Enum.Parse(typeof(TargetSideEffect.TargetRange), attr.Value));
                                        break;
                                }
                            }

                            sideEffects_OnBattleGround.AddSideEffect(sideEffect, triggerTime, triggerRange);
                        }

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
                        sideEffects: sideEffects,
                        sideEffects_OnBattleGround: sideEffects_OnBattleGround));
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
                        sideEffects: sideEffects,
                        sideEffects_OnBattleGround: sideEffects_OnBattleGround));
                    break;
                case CardTypes.Spell:
                    addCard(new CardInfo_Spell(
                        cardID: int.Parse(card.Attributes["id"].Value),
                        baseInfo: baseInfo,
                        upgradeInfo: upgradeInfo,
                        sideEffects: sideEffects,
                        sideEffects_OnBattleGround: sideEffects_OnBattleGround));
                    break;
                case CardTypes.Energy:
                    addCard(new CardInfo_Spell(
                        cardID: int.Parse(card.Attributes["id"].Value),
                        baseInfo: baseInfo,
                        upgradeInfo: upgradeInfo,
                        sideEffects: sideEffects,
                        sideEffects_OnBattleGround: sideEffects_OnBattleGround));
                    break;
            }
        }
    }

    public static CardInfo_Base GetCard(int cardID)
    {
        if (CardDict.ContainsKey(cardID))
        {
            return CardDict[cardID];
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
        List<CardInfo_Base> cis = GetCardSeries(GetCard(cardId));
        List<int> res = new List<int>();
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