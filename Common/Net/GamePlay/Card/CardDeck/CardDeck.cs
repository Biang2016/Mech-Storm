using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Xml;
using MyCardGameCommon;

public class CardDeck
{
    /// <summary>
    /// 本类中封装卡组操作的基本功能
    /// </summary>
    private CardDeckInfo M_CardDeckInfo;

    private List<CardInfo_Base> Cards;
    private List<CardInfo_Base> AbandonCards = new List<CardInfo_Base>();

    public bool IsEmpty = false;
    public bool IsAbandonCardsEmpty = false;

    private void checkEmpty()
    {
        IsEmpty = Cards.Count == 0;
        IsAbandonCardsEmpty = AbandonCards.Count == 0;
    }

    public CardDeck(CardDeckInfo cdi)
    {
        M_CardDeckInfo = cdi;
        Cards = AllCards.GetCards(M_CardDeckInfo.CardIDs);
        checkEmpty();
        //SuffleSelf();
    }

    public CardInfo_Type FindATypeOfCard<CardInfo_Type>() where CardInfo_Type : CardInfo_Base
    {
        foreach (CardInfo_Base cb in Cards)
        {
            if (cb is CardInfo_Type)
            {
                return (CardInfo_Type) cb;
            }
        }

        return null;
    }

    public List<CardInfo_Type> FindATypeOfCards<CardInfo_Type>(int cardNumber) where CardInfo_Type : CardInfo_Base
    {
        List<CardInfo_Type> resList = new List<CardInfo_Type>();
        int count = 0;
        foreach (CardInfo_Base cb in Cards)
        {
            if (cb is CardInfo_Type)
            {
                count++;
                resList.Add((CardInfo_Type) cb);
                if (count >= cardNumber)
                {
                    break;
                }
            }
        }

        return resList;
    }

    public CardInfo_Base DrawCardOnTop()
    {
        if (Cards.Count > 0)
        {
            CardInfo_Base res = Cards[0];
            Cards.Remove(res);
            AbandonCards.Add(res);
            checkEmpty();
            return res;
        }
        else
        {
            return null;
        }
    }

    public List<CardInfo_Base> DrawCardsOnTop(int cardNumber)
    {
        List<CardInfo_Base> resList = new List<CardInfo_Base>();
        for (int i = 0; i < Math.Min(Cards.Count, cardNumber); i++)
        {
            resList.Add(Cards[i]);
        }

        foreach (CardInfo_Base cb in resList)
        {
            Cards.Remove(cb);
            AbandonCards.Add(cb);
            checkEmpty();
        }

        return resList;
    }

    public void AddCardToButtom(CardInfo_Base newCard)
    {
        Cards.Add(newCard);
        checkEmpty();
    }

    public CardInfo_Base GetFirstCardInfo()
    {
        if (Cards.Count > 0)
        {
            return Cards[0];
        }
        else
        {
            return null;
        }
    }

    public List<CardInfo_Base> GetTopCardsInfo(int cardNumber)
    {
        List<CardInfo_Base> resList = new List<CardInfo_Base>();
        for (int i = 0; i < Math.Min(Cards.Count, cardNumber); i++)
        {
            resList.Add(Cards[i]);
        }

        return resList;
    }

    public void SuffleSelf()
    {
        Suffle(Cards);
    }

    public static void Suffle(List<CardInfo_Base> targetCardList)
    {
        for (int i = 0; i < targetCardList.Count * 1; i++)
        {
            int cardNum1 = new Random().Next(0, targetCardList.Count);
            int cardNum2 = new Random().Next(0, targetCardList.Count);
            if (cardNum1 != cardNum2)
            {
                CardInfo_Base tmp = targetCardList[cardNum1];
                targetCardList[cardNum1] = targetCardList[cardNum2];
                targetCardList[cardNum2] = tmp;
            }
            else
            {
                i--;
            }
        }
    }

    public void GetARetinueCardToTheTop()
    {
        CardInfo_Base target_cb = null;
        foreach (CardInfo_Base cb in Cards)
        {
            if (cb.CardType == CardTypes.Retinue)
            {
                target_cb = cb;
                break;
            }
        }

        if (target_cb != null)
        {
            Cards.Remove(target_cb);
            Cards.Add(target_cb);
        }
    }

    public void AbandonCardRecycle()
    {
        Suffle(AbandonCards);
        foreach (CardInfo_Base ac in AbandonCards)
        {
            Cards.Add(ac);
        }

        checkEmpty();
    }
}

public struct CardDeckInfo
{
    public int CardNumber;

    public int[] CardIDs;

    public CardDeckInfo(int[] cardIDs)
    {
        CardIDs = cardIDs;
        CardNumber = cardIDs.Length;
    }
}

public static class AllCards
{
    public static Dictionary<int, CardInfo_Base> CardDict = new Dictionary<int, CardInfo_Base>();

    private static void addCard(CardInfo_Base cardInfo)
    {
        CardDict.Add(cardInfo.CardID, cardInfo);
    }

    public static string HeroColor = "#787878FF";
    public static string RetinueColor = "#5BAEF4FF";
    public static string WeaponSwordColor = "#FF229DFF";
    public static string WeaponGunColor = "#FF0000FF";
    public static string ShieldShieldColor = "#E6FF00FF";
    public static string ShieldArmorColor = "#FF8E00FF";

    public static void AddAllCards()
    {
        XmlDocument doc = new XmlDocument();
        string text = CardResource.Cards;
        doc.LoadXml(text);
        XmlElement allCards = doc.DocumentElement;
        for (int i = 0; i < allCards.ChildNodes.Count; i++)
        {
            XmlNode card = allCards.ChildNodes.Item(i);
            for (int j = 0; j < card.ChildNodes.Count; j++)
            {
                XmlNode cardInfo = card.ChildNodes[j];
                if (cardInfo.Name == "baseInfo")
                {

                }
            }

            addCard(
                new CardInfo_Retinue(
                    cardID: int.Parse(card.Attributes["id"].Value),
                    cardName:,
                    cardDesc: "",
                    cost: 0,
                    dragPurpose: DragPurpose.Summon,
                    cardType: CardTypes.Retinue,
                    cardColor: HeroColor,
                    upgradeCardID: -1,
                    cardLevel: 0,
                    life: 0,
                    totalLife: 0,
                    basicAttack: 0,
                    basicShield: 0,
                    basicArmor: 0,
                    slot1: SlotTypes.None,
                    slot2: SlotTypes.None,
                    slot3: SlotTypes.None,
                    slot4: SlotTypes.None
                ));
        }

        addCard(
            new CardInfo_Retinue(
                cardID: 999,
                cardName: "空牌",
                cardDesc: "",
                cost: 0,
                dragPurpose: DragPurpose.Summon,
                cardType: CardTypes.Retinue,
                cardColor: HeroColor,
                upgradeCardID: -1,
                cardLevel: 0,
                life: 0,
                totalLife: 0,
                basicAttack: 0,
                basicShield: 0,
                basicArmor: 0,
                slot1: SlotTypes.None,
                slot2: SlotTypes.None,
                slot3: SlotTypes.None,
                slot4: SlotTypes.None
            ));
        addCard(
            new CardInfo_Retinue(
                cardID: 99,
                cardName: "假装这是英雄",
                cardDesc: "它死了就输了",
                cost: 0,
                dragPurpose: DragPurpose.Summon,
                cardType: CardTypes.Retinue,
                cardColor: HeroColor,
                upgradeCardID: -1,
                cardLevel: 0,
                life: 50,
                totalLife: 50,
                basicAttack: 0,
                basicShield: 0,
                basicArmor: 0,
                slot1: SlotTypes.None,
                slot2: SlotTypes.None,
                slot3: SlotTypes.None,
                slot4: SlotTypes.None
            ));
        addCard(
            new CardInfo_Retinue(
                cardID: 0,
                cardName: "突击敢达",
                cardDesc: "具有初始攻击加成\n装备武器后如虎添翼",
                cost: 2,
                dragPurpose: DragPurpose.Summon,
                cardType: CardTypes.Retinue,
                cardColor: RetinueColor,
                upgradeCardID: -1,
                cardLevel: 0,
                life: 6,
                totalLife: 6,
                basicAttack: 1,
                basicShield: 0,
                basicArmor: 0,
                slot1: SlotTypes.Weapon,
                slot2: SlotTypes.Shield,
                slot3: SlotTypes.Pack,
                slot4: SlotTypes.MA
            ));
        addCard(
            new CardInfo_Retinue(
                cardID: 1,
                cardName: "守卫者",
                cardDesc: "身披护盾的守卫者\n可抵御大量伤害",
                cost: 2,
                dragPurpose: DragPurpose.Summon,
                cardType: CardTypes.Retinue,
                cardColor: RetinueColor,
                upgradeCardID: -1,
                cardLevel: 0,
                life: 5,
                totalLife: 5,
                basicAttack: 0,
                basicShield: 3,
                basicArmor: 3,
                slot1: SlotTypes.Weapon,
                slot2: SlotTypes.Shield,
                slot3: SlotTypes.Pack,
                slot4: SlotTypes.MA));
        addCard(
            new CardInfo_Weapon(
                cardID: 100,
                cardName: "热能斧Ⅰ",
                cardDesc: "伤害<color=#FFFF00>1</color>\n能量<color=#FFFF00>1/3</color>",
                cost: 1,
                dragPurpose: DragPurpose.Equip,
                cardType: CardTypes.Weapon,
                cardColor: WeaponSwordColor,
                upgradeCardID: 101,
                cardLevel: 1,
                energy: 1,
                energyMax: 3,
                attack: 1,
                weaponType: WeaponType.Sword));
        addCard(
            new CardInfo_Weapon(
                cardID: 101,
                cardName: "热能斧Ⅱ",
                cardDesc: "伤害<color=#FFFF00>1</color>\n能量<color=#FFFF00>2/5</color>",
                cost: 3,
                dragPurpose: DragPurpose.Equip,
                cardType: CardTypes.Weapon,
                cardColor: WeaponSwordColor,
                upgradeCardID: 102,
                cardLevel: 2,
                energy: 2,
                energyMax: 5,
                attack: 1,
                weaponType: WeaponType.Sword));
        addCard(
            new CardInfo_Weapon(
                cardID: 102,
                cardName: "热能斧Ⅲ",
                cardDesc: "伤害<color=#FFFF00>2</color>\n能量<color=#FFFF00>4/7</color>",
                cost: 5,
                dragPurpose: DragPurpose.Equip,
                cardType: CardTypes.Weapon,
                cardColor: WeaponSwordColor,
                upgradeCardID: -1,
                cardLevel: 3,
                energy: 4,
                energyMax: 7,
                attack: 2,
                weaponType: WeaponType.Sword));
        addCard(
            new CardInfo_Weapon(
                cardID: 200,
                cardName: "新兵步枪Ⅰ",
                cardDesc: "伤害<color=#FFFF00>1</color>\n弹药数<color=#FFFF00>3/3</color>",
                cost: 2,
                dragPurpose: DragPurpose.Equip,
                cardType: CardTypes.Weapon,
                cardColor: WeaponGunColor,
                upgradeCardID: 201,
                cardLevel: 1,
                energy: 3,
                energyMax: 3,
                attack: 1,
                weaponType: WeaponType.Gun));
        addCard(
            new CardInfo_Weapon(
                cardID: 201,
                cardName: "新兵步枪Ⅱ",
                cardDesc: "伤害<color=#FFFF00>1</color>\n弹药数<color=#FFFF00>5/5</color>",
                cost: 3,
                dragPurpose: DragPurpose.Equip,
                cardType: CardTypes.Weapon,
                cardColor: WeaponGunColor,
                upgradeCardID: 202,
                cardLevel: 2,
                energy: 5,
                energyMax: 5,
                attack: 1,
                weaponType: WeaponType.Gun));
        addCard(
            new CardInfo_Weapon(
                cardID: 202,
                cardName: "新兵步枪Ⅲ",
                cardDesc: "伤害<color=#FFFF00>1</color>\n弹药数<color=#FFFF00>8/8</color>",
                cost: 5,
                dragPurpose: DragPurpose.Equip,
                cardType: CardTypes.Weapon,
                cardColor: WeaponGunColor,
                upgradeCardID: -1,
                cardLevel: 3,
                energy: 8,
                energyMax: 8,
                attack: 1,
                weaponType: WeaponType.Gun));
        addCard(
            new CardInfo_Shield(
                cardID: 300,
                cardName: "新兵护盾Ⅰ",
                cardDesc: "提供<color=#67c8ff>4</color>点护盾",
                cost: 2,
                dragPurpose: DragPurpose.Equip,
                cardType: CardTypes.Shield,
                cardColor: ShieldShieldColor,
                upgradeCardID: 302,
                cardLevel: 1,
                shielType: ShieldType.Shield,
                armor: 0,
                armorMax: 0,
                shield: 4,
                shieldMax: 4));
        addCard(
            new CardInfo_Shield(
                cardID: 301,
                cardName: "新兵护盾Ⅱ",
                cardDesc: "提供<color=#67c8ff>6</color>点护盾",
                cost: 3,
                dragPurpose: DragPurpose.Equip,
                cardType: CardTypes.Shield,
                cardColor: ShieldShieldColor,
                upgradeCardID: 302,
                cardLevel: 2,
                shielType: ShieldType.Shield,
                armor: 0,
                armorMax: 0,
                shield: 6,
                shieldMax: 6));
        addCard(
            new CardInfo_Shield(
                cardID: 302,
                cardName: "新兵护盾Ⅲ",
                cardDesc: "提供<color=#67c8ff>8</color>点护盾",
                cost: 4,
                dragPurpose: DragPurpose.Equip,
                cardType: CardTypes.Shield,
                cardColor: ShieldShieldColor,
                upgradeCardID: -1,
                cardLevel: 3,
                shielType: ShieldType.Shield,
                armor: 0,
                armorMax: 0,
                shield: 8,
                shieldMax: 8));
        addCard(
            new CardInfo_Shield(
                cardID: 350,
                cardName: "新兵盾牌Ⅰ",
                cardDesc: "提供<color=#67c8ff>4</color>点护甲",
                cost: 1,
                dragPurpose: DragPurpose.Equip,
                cardType: CardTypes.Shield,
                cardColor: ShieldArmorColor,
                upgradeCardID: 303,
                cardLevel: 1,
                shielType: ShieldType.Armor,
                armor: 4,
                armorMax: 4,
                shield: 0,
                shieldMax: 0));
        addCard(
            new CardInfo_Shield(
                cardID: 351,
                cardName: "新兵盾牌Ⅱ",
                cardDesc: "提供<color=#67c8ff>6</color>点护甲",
                cost: 2,
                dragPurpose: DragPurpose.Equip,
                cardType: CardTypes.Shield,
                cardColor: ShieldArmorColor,
                upgradeCardID: 352,
                cardLevel: 2,
                shielType: ShieldType.Armor,
                armor: 6,
                armorMax: 6,
                shield: 0,
                shieldMax: 0));
        addCard(
            new CardInfo_Shield(
                cardID: 352,
                cardName: "新兵盾牌Ⅲ",
                cardDesc: "提供<color=#67c8ff>8</color>点护甲",
                cost: 3,
                dragPurpose: DragPurpose.Equip,
                cardType: CardTypes.Shield,
                cardColor: ShieldArmorColor,
                upgradeCardID: -1,
                cardLevel: 3,
                shielType: ShieldType.Armor,
                armor: 8,
                armorMax: 8,
                shield: 0,
                shieldMax: 0));
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
        int level1 = card1.CardLevel;
        int level2 = card2.CardLevel;
        if (level1 > level2)
        {
            return IsASeries(card2, card1);
        }

        int tmpUpgradeID = card1.UpgradeID;
        while (tmpUpgradeID != -1)
        {
            if (tmpUpgradeID == card2.CardID)
            {
                return true;
            }

            tmpUpgradeID = GetCard((int) tmpUpgradeID).UpgradeID;
        }

        return false;
    }
}