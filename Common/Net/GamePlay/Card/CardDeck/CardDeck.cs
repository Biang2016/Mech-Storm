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
                return (CardInfo_Type)cb;
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
                resList.Add((CardInfo_Type)cb);
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
            if (cb.BaseInfo.CardType == CardTypes.Retinue)
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

    public static string RetinueColor = "#5BAEF4FF";
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
            BaseInfo baseInfo = new BaseInfo();
            UpgradeInfo upgradeInfo = new UpgradeInfo();
            LifeInfo lifeInfo = new LifeInfo();
            BattleInfo battleInfo = new BattleInfo();
            SlotInfo slotInfo = new SlotInfo();
            WeaponInfo weaponInfo = new WeaponInfo();
            ShieldInfo shieldInfo = new ShieldInfo();

            for (int j = 0; j < card.ChildNodes.Count; j++)
            {
                XmlNode cardInfo = card.ChildNodes[j];
                switch (cardInfo.Attributes["name"].Value)
                {
                    case "baseInfo":
                        baseInfo = new BaseInfo(cardInfo.Attributes["cardName"].Value,
                                                        cardInfo.Attributes["cardDesc"].Value.Replace("\\n","\n"),
                                                        int.Parse(cardInfo.Attributes["cost"].Value),
                                                        (DragPurpose)Enum.Parse(typeof(DragPurpose), cardInfo.Attributes["dragPurpose"].Value),
                                                        (CardTypes)Enum.Parse(typeof(CardTypes), cardInfo.Attributes["cardType"].Value),
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
                        slotInfo = new SlotInfo((SlotTypes)Enum.Parse(typeof(SlotTypes), cardInfo.Attributes["slot1"].Value),
                                                      (SlotTypes)Enum.Parse(typeof(SlotTypes), cardInfo.Attributes["slot2"].Value),
                                                      (SlotTypes)Enum.Parse(typeof(SlotTypes), cardInfo.Attributes["slot3"].Value),
                                                      (SlotTypes)Enum.Parse(typeof(SlotTypes), cardInfo.Attributes["slot4"].Value));
                        break;
                    case "weaponInfo":
                        weaponInfo = new WeaponInfo(int.Parse(cardInfo.Attributes["energy"].Value),
                                                int.Parse(cardInfo.Attributes["energyMax"].Value),
                                                int.Parse(cardInfo.Attributes["attack"].Value),
                                                (WeaponTypes)Enum.Parse(typeof(WeaponTypes), cardInfo.Attributes["weaponType"].Value));
                        break;
                    case "shieldInfo":
                        shieldInfo = new ShieldInfo(int.Parse(cardInfo.Attributes["armor"].Value),
                                                    int.Parse(cardInfo.Attributes["armorMax"].Value),
                                                    int.Parse(cardInfo.Attributes["shield"].Value),
                                                    int.Parse(cardInfo.Attributes["shieldMax"].Value),
                                                    (ShieldTypes)Enum.Parse(typeof(ShieldTypes), cardInfo.Attributes["shieldType"].Value));
                        break;
                    default: break;
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
                            slotInfo: slotInfo));
                    break;
                case CardTypes.Weapon:
                    addCard(new CardInfo_Weapon(
                            cardID: int.Parse(card.Attributes["id"].Value),
                            baseInfo: baseInfo,
                            upgradeInfo: upgradeInfo,
                            weaponInfo: weaponInfo));
                    break;
                case CardTypes.Shield:
                    addCard(new CardInfo_Shield(
                            cardID: int.Parse(card.Attributes["id"].Value),
                            baseInfo: baseInfo,
                            upgradeInfo: upgradeInfo,
                            shieldInfo: shieldInfo));
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

            tmpUpgradeID = GetCard((int)tmpUpgradeID).UpgradeInfo.UpgradeCardID;
        }

        return false;
    }
}