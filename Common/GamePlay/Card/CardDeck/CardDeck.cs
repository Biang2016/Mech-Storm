using System;
using System.Collections.Generic;

public class CardDeck
{
    /// <summary>
    /// 本类中封装卡组操作的基本功能
    /// </summary>
    public BuildInfo M_BuildInfo;

    private List<CardInfo_Base> Cards = new List<CardInfo_Base>();
    private List<CardInfo_Base> AbandonCards = new List<CardInfo_Base>();

    public bool IsEmpty
    {
        get { return Cards.Count == 0; }
    }

    public bool IsAbandonCardsEmpty
    {
        get { return AbandonCards.Count == 0; }
    }

    public delegate void OnCardDeckCountChange(int count);

    public OnCardDeckCountChange CardDeckCountChangeHandler;

    public int CardCount()
    {
        return Cards.Count;
    }

    public CardDeck(BuildInfo cdi, OnCardDeckCountChange handler)
    {
        M_BuildInfo = cdi;
        CardDeckCountChangeHandler = handler;
        AppendCards(AllCards.GetCards(M_BuildInfo.CardIDs.ToArray()));
        SuffleSelf();
    }

    private void AddCard(CardInfo_Base cardInfo, int index)
    {
        Cards.Insert(index, cardInfo);
        CardDeckCountChangeHandler(Cards.Count);
    }

    private void AppendCard(CardInfo_Base cardInfo)
    {
        Cards.Add(cardInfo);
        CardDeckCountChangeHandler(Cards.Count);
    }

    private void AppendCards(List<CardInfo_Base> cardInfos)
    {
        Cards.AddRange(cardInfos);
        CardDeckCountChangeHandler(Cards.Count);
    }

    private void RemoveCard(CardInfo_Base cardInfo)
    {
        Cards.Remove(cardInfo);
        CardDeckCountChangeHandler(Cards.Count);
    }

    public void RandomInsertTempCard(int cardId)
    {
        CardInfo_Base cb = AllCards.GetCard(cardId);
        int index = new Random().Next(0, Cards.Count);
        AddCard(cb, index);
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
            RemoveCard(res);
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
            RemoveCard(cb);
        }

        return resList;
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
        if (targetCardList.Count <= 1) return;
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

    public int PutHeroCardToTop(int number)
    {
        List<CardInfo_Base> res = new List<CardInfo_Base>();
        foreach (CardInfo_Base cb in Cards)
        {
            if (cb.BaseInfo.CardType == CardTypes.Retinue && !cb.RetinueInfo.IsSoldier)
            {
                number--;
                res.Add(cb);
                if (number == 0) break;
            }
        }

        foreach (CardInfo_Base cb in res)
        {
            RemoveCard(cb);
            AddCard(cb, 0);
        }

        return res.Count;
    }

    public int PutCardToTopByType(CardTypes cardType, int number)
    {
        List<CardInfo_Base> res = new List<CardInfo_Base>();
        foreach (CardInfo_Base cb in Cards)
        {
            if (cb.BaseInfo.CardType == cardType)
            {
                number--;
                res.Add(cb);
                if (number == 0) break;
            }
        }

        foreach (CardInfo_Base cb in res)
        {
            RemoveCard(cb);
            AddCard(cb, 0);
        }

        return res.Count;
    }

    private Dictionary<int, int> CardInstanceIdDict = new Dictionary<int, int>();

    public void AddCardInstanceId(int cardId, int cardInstanceId)
    {
        if (cardInstanceId == Const.CARD_INSTANCE_ID_NONE) return;
        if (CardInstanceIdDict.ContainsKey(cardInstanceId))
        {
            CardInstanceIdDict[cardInstanceId] = cardId;
        }
        else
        {
            CardInstanceIdDict.Add(cardInstanceId, cardId);
        }
    }

    public void RecycleCardInstanceID(int cardInstanceId)
    {
        if (CardInstanceIdDict.ContainsKey(cardInstanceId))
        {
            AbandonCards.Add(AllCards.GetCard(CardInstanceIdDict[cardInstanceId]));
            CardInstanceIdDict.Remove(cardInstanceId);
        }
    }

    public void AbandonCardRecycle()
    {
        foreach (CardInfo_Base ac in AbandonCards)
        {
            AppendCard(ac);
        }

        AbandonCards.Clear();
        Suffle(Cards);
    }
}