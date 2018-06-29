using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class CardDeck
{
    /// <summary>
    /// 本类中封装卡组操作的基本功能
    /// </summary>
    public CardDeckInfo M_CardDeckInfo;

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
        Cards = GameManager.GM.AllCard.GetCards(M_CardDeckInfo.CardIDs);
        checkEmpty();
        SuffleSelf();
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
        for (int i = 0; i < targetCardList.Count * 3; i++)
        {
            int cardNum1 = UnityEngine.Random.Range(0, targetCardList.Count);
            int cardNum2 = UnityEngine.Random.Range(0, targetCardList.Count);
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

public class AllCards
{
    public Dictionary<int, CardInfo_Base> CardDict = new Dictionary<int, CardInfo_Base>();

    private void addCard(CardInfo_Base cardInfo)
    {
        CardDict.Add(cardInfo.CardID,cardInfo);
    }

    public AllCards()
    {
        addCard(
            new CardInfo_Retinue(
                cardID: 99,
                cardName: "英雄",
                cardDesc: "死了游戏结束",
                cost: 0,
                hasTarget: false,
                cardType: CardTypes.Retinue,
                cardColor: Color.blue,
                upgreaCardID: -1,
                life: 50,
                totalLife: 50,
                basicAttack: 0,
                basicShield: 0,
                basicArmor: 0));
        addCard(
            new CardInfo_Retinue(
                cardID: 0,
                cardName: "海牛高达",
                cardDesc: "暂无",
                cost: 2,
                hasTarget: false,
                cardType: CardTypes.Retinue,
                cardColor: Color.blue,
                upgreaCardID: -1,
                life: 2,
                totalLife: 2,
                basicAttack: 2,
                basicShield: 2,
                basicArmor: 2));
        addCard(
            new CardInfo_Retinue(
                cardID: 1,
                cardName: "GAT-X105E",
                cardDesc: "暂无",
                cost: 2,
                hasTarget: false,
                cardType: CardTypes.Retinue,
                cardColor: Color.blue,
                upgreaCardID: -1,
                life: 2,
                totalLife: 2,
                basicAttack: 1,
                basicShield: 3,
                basicArmor: 3));
        addCard(
            new CardInfo_Weapon(
                cardID: 100,
                cardName: "热能斧Ⅰ",
                cardDesc: "每次攻击积攒<color=#FFFF00>1</color>点能量\n攻击时每点能量可造成<color=#FFFF00>1</color>倍伤害\n最高充能<color=#FFFF00>3</color>",
                cost: 1,
                hasTarget: true,
                cardType: CardTypes.Weapon,
                cardColor: Color.red,
                upgreaCardID: 101,
                energy: 1,
                energyMax: 3,
                attack: 1,
                weaponType: WeaponType.Sword));
        addCard(
            new CardInfo_Weapon(
                cardID: 101,
                cardName: "热能斧Ⅱ",
                cardDesc: "每次攻击积攒<color=#FFFF00>1</color>点能量\n攻击时每点能量可造成<color=#FFFF00>1</color>倍伤害\n最高充能<color=#FFFF00>5</color>",
                cost: 3,
                hasTarget: true,
                cardType: CardTypes.Weapon,
                cardColor: Color.red,
                upgreaCardID: -1,
                energy: 1,
                energyMax: 5,
                attack: 1,
                weaponType: WeaponType.Sword));
        addCard(
            new CardInfo_Weapon(
                cardID: 200,
                cardName: "新兵步枪Ⅰ",
                cardDesc: "攻击时打出<color=#FFFF00>所有</color>弹药\n弹药数<color=#FFFF00>3</color>",
                cost: 2,
                hasTarget: true,
                cardType: CardTypes.Weapon,
                cardColor: Color.red,
                upgreaCardID: 201,
                energy: 3,
                energyMax: 3,
                attack: 2,
                weaponType: WeaponType.Gun));
        addCard(
            new CardInfo_Weapon(
                cardID: 201,
                cardName: "新兵步枪Ⅱ",
                cardDesc: "攻击时打出<color=#FFFF00>所有</color>弹药\n弹药数<color=#FFFF00>5</color>",
                cost: 3,
                hasTarget: true,
                cardType: CardTypes.Weapon,
                cardColor: Color.red,
                upgreaCardID: -1,
                energy: 5,
                energyMax: 5,
                attack: 2,
                weaponType: WeaponType.Gun));
        addCard(
            new CardInfo_Shield(
                cardID: 300,
                cardName: "新兵护盾",
                cardDesc: "提供<color=#5000bd>4</color>点护盾\n受到大于护盾值攻击时耐久<color=#5000bd>减半</color>\n受到小于护盾值攻击时耐久<color=#5000bd>-1</color>",
                cost: 1,
                hasTarget: true,
                cardType: CardTypes.Shield,
                cardColor: Color.yellow,
                upgreaCardID: -1,
                shielType: ShieldType.Shield,
                armor: 0,
                armorMax: 0,
                shield: 2,
                shieldMax: 2));
        addCard(
            new CardInfo_Shield(
                cardID: 301,
                cardName: "新兵盾牌",
                cardDesc: "提供<color=#5000bd>8</color>点护甲",
                cost: 1,
                hasTarget: true,
                cardType: CardTypes.Shield,
                cardColor: Color.yellow,
                upgreaCardID: -1,
                shielType: ShieldType.Armor,
                armor: 4,
                armorMax: 4,
                shield: 0,
                shieldMax: 0));
    }

    public CardInfo_Base GetCard(int cardID)
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

    public List<CardInfo_Base> GetCards(int[] cardIDs)
    {
        List<CardInfo_Base> res = new List<CardInfo_Base>();
        foreach (int cardID in cardIDs)
        {
            res.Add(GetCard(cardID));
        }

        return res;
    }
}