using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using MyCardGameCommon.Net.GamePlay.Card;

public static class AllCards
{
    public static Dictionary<int, CardInfo_Base> CardDict = new Dictionary<int, CardInfo_Base>();

    private static void addCard(CardInfo_Base cardInfo)
    {
        CardDict.Add(cardInfo.CardID, cardInfo);
    }

    public static void AddAllCards()
    {
        XmlDocument doc = new XmlDocument();
        string text = CardResource.Cards;
        doc.LoadXml(text);
        XmlElement allCards = doc.DocumentElement;
        for (int i = 0; i < allCards.ChildNodes.Count; i++)
        {
            XmlNode card = allCards.ChildNodes.Item(i);
            BaseInfo baseInfo = new BaseInfo();
            UpgradeInfo upgradeInfo = new UpgradeInfo();
            LifeInfo lifeInfo = new LifeInfo();
            BattleInfo battleInfo = new BattleInfo();
            SlotInfo slotInfo = new SlotInfo();
            WeaponInfo weaponInfo = new WeaponInfo();
            ShieldInfo shieldInfo = new ShieldInfo();

            List<SideEffectBase> SideEffects_OnDie = new List<SideEffectBase>();
            List<SideEffectBase> SideEffects_OnSummoned = new List<SideEffectBase>();

            for (int j = 0; j < card.ChildNodes.Count; j++)
            {
                XmlNode cardInfo = card.ChildNodes[j];
                switch (cardInfo.Attributes["name"].Value)
                {
                    case "baseInfo":
                        baseInfo = new BaseInfo(cardInfo.Attributes["cardName"].Value,
                            cardInfo.Attributes["cardDesc"].Value.Replace("\\n", "\n"),
                            int.Parse(cardInfo.Attributes["cost"].Value),
                            (DragPurpose) Enum.Parse(typeof(DragPurpose), cardInfo.Attributes["dragPurpose"].Value),
                            (CardTypes) Enum.Parse(typeof(CardTypes), cardInfo.Attributes["cardType"].Value),
                            cardInfo.Attributes["cardColor"].Value);
                        break;
                    case "upgradeInfo":
                        upgradeInfo = new UpgradeInfo(int.Parse(cardInfo.Attributes["upgradeCardID"].Value),
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
                    case "slotInfo":
                        slotInfo = new SlotInfo((SlotTypes) Enum.Parse(typeof(SlotTypes), cardInfo.Attributes["slot1"].Value),
                            (SlotTypes) Enum.Parse(typeof(SlotTypes), cardInfo.Attributes["slot2"].Value),
                            (SlotTypes) Enum.Parse(typeof(SlotTypes), cardInfo.Attributes["slot3"].Value),
                            (SlotTypes) Enum.Parse(typeof(SlotTypes), cardInfo.Attributes["slot4"].Value));
                        break;
                    case "weaponInfo":
                        weaponInfo = new WeaponInfo(int.Parse(cardInfo.Attributes["energy"].Value),
                            int.Parse(cardInfo.Attributes["energyMax"].Value),
                            int.Parse(cardInfo.Attributes["attack"].Value),
                            (WeaponTypes) Enum.Parse(typeof(WeaponTypes), cardInfo.Attributes["weaponType"].Value));
                        break;
                    case "shieldInfo":
                        shieldInfo = new ShieldInfo(int.Parse(cardInfo.Attributes["armor"].Value),
                            int.Parse(cardInfo.Attributes["armorMax"].Value),
                            int.Parse(cardInfo.Attributes["shield"].Value),
                            int.Parse(cardInfo.Attributes["shieldMax"].Value),
                            (ShieldTypes) Enum.Parse(typeof(ShieldTypes), cardInfo.Attributes["shieldType"].Value));
                        break;
                    case "sideEffectsInfo":
                        List<SideEffectBase> sideEffects = new List<SideEffectBase>();
                        switch (cardInfo.Attributes["happenTime"].Value)
                        {
                            case "OnDie":
                                sideEffects = SideEffects_OnDie;
                                break;
                            case "OnSummoned":
                                sideEffects = SideEffects_OnSummoned;
                                break;
                        }

                        for (int k = 0; k < cardInfo.ChildNodes.Count; k++)
                        {
                            XmlNode sideEffectInfo = cardInfo.ChildNodes[k];
                            SideEffectBase sideEffect = AllSideEffects.SideEffectsNameDict[sideEffectInfo.Attributes["name"].Value].Clone();
                            for (int l = 0; l < sideEffectInfo.Attributes.Count; l++)
                            {
                                if (sideEffectInfo.Attributes[l].Name == "name") continue;
                                XmlAttribute attr = sideEffectInfo.Attributes[l];
                                FieldInfo fi = sideEffect.GetType().GetField(attr.Name);
                                switch (fi.FieldType.Name.ToString())
                                {
                                    case "Int32":
                                        fi.SetValue(sideEffect, int.Parse(attr.Value));
                                        break;
                                    case "String":
                                        fi.SetValue(sideEffect, attr.Value);
                                        break;
                                }
                            }

                            sideEffects.Add(sideEffect);
                        }

                        break;
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
                        slotInfo: slotInfo,
                        sideEffects_OnDie: SideEffects_OnDie,
                        sideEffects_OnSummoned: SideEffects_OnSummoned));
                    break;
                case CardTypes.Weapon:
                    addCard(new CardInfo_Weapon(
                        cardID: int.Parse(card.Attributes["id"].Value),
                        baseInfo: baseInfo,
                        upgradeInfo: upgradeInfo,
                        weaponInfo: weaponInfo,
                        sideEffects_OnDie: SideEffects_OnDie));
                    break;
                case CardTypes.Shield:
                    addCard(new CardInfo_Shield(
                        cardID: int.Parse(card.Attributes["id"].Value),
                        baseInfo: baseInfo,
                        upgradeInfo: upgradeInfo,
                        shieldInfo: shieldInfo,
                        sideEffects_OnDie: SideEffects_OnDie));
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
}