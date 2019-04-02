using System;
using System.Collections.Generic;
using System.Security.Policy;

public class CardDeck
{
    /// <summary>
    /// 本类中封装卡组操作的基本功能
    /// </summary>
    public BuildInfo M_BuildInfo;

    private List<CardInfo_Base> Cards = new List<CardInfo_Base>();
    private List<CardInfo_Base> AbandonCards = new List<CardInfo_Base>();
    public List<CoolingDownCard> CoolingDownCards = new List<CoolingDownCard>();

    public class CoolingDownCard
    {
        public int CardID;
        public int CardInstanceID;
        public int LeftRounds;
        public bool ShowInBattleShip;

        public CoolingDownCard(int cardId, int cardInstanceId, int leftRounds, bool showInBattleShip)
        {
            CardID = cardId;
            CardInstanceID = cardInstanceId;
            LeftRounds = leftRounds;
            ShowInBattleShip = showInBattleShip;
        }

        public void Serialize(DataStream writer)
        {
            writer.WriteSInt32(CardID);
            writer.WriteSInt32(CardInstanceID);
            writer.WriteSInt32(LeftRounds);
            writer.WriteByte((byte) (ShowInBattleShip ? 0x01 : 0x00));
        }

        public static CoolingDownCard Deserialize(DataStream reader)
        {
            int CardID = reader.ReadSInt32();
            int CardInstanceID = reader.ReadSInt32();
            int LeftRounds = reader.ReadSInt32();
            bool ShowInBattleShip = reader.ReadByte() == 0x01;
            return new CoolingDownCard(CardID, CardInstanceID, LeftRounds, ShowInBattleShip);
        }
    }

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

    public CardDeck(BuildInfo cdi, OnCardDeckCountChange onCardDeckCountChangeHandler, Action<CoolingDownCard> onUpdateCoolDownCardHandler, Action<CoolingDownCard> onRemoveCoolDownCardHandler)
    {
        M_BuildInfo = cdi;
        CardDeckCountChangeHandler = onCardDeckCountChangeHandler;
        UpdateCoolDownCardHandler = onUpdateCoolDownCardHandler;
        RemoveCoolDownCardHandler = onRemoveCoolDownCardHandler;
        AppendCards(AllCards.GetCards(M_BuildInfo.GetCardIDs().ToArray()));
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
        for (int i = 0; i < 3; i++)
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
            if (cb == null) continue;
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

    public void PutCardToTopByID(int cardID)
    {
        CardInfo_Base targetCard = null;
        foreach (CardInfo_Base cb in Cards)
        {
            if (cb.CardID == cardID)
            {
                targetCard = cb;
            }
        }

        if (targetCard != null)
        {
            RemoveCard(targetCard);
            AddCard(targetCard, 0);
        }
    }

    public void PutCardsToTopByID(List<int> cardIDs)
    {
        for (int i = cardIDs.Count - 1; i >= 0; i--)
        {
            int cardID = cardIDs[i];
            PutCardToTopByID(cardID);
        }
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
            CardInfo_Base cib = AllCards.GetCard(CardInstanceIdDict[cardInstanceId]);
            if (cib.BaseInfo.CardType == CardTypes.Retinue && !cib.RetinueInfo.IsSoldier)
            {
                CoolingDownCard cdc = new CoolingDownCard(cib.CardID, cardInstanceId, 2, true);
                CoolingDownCards.Add(cdc);
                UpdateCoolDownCardHandler(cdc);
            }
            else
            {
                AbandonCards.Add(cib);
            }

            CardInstanceIdDict.Remove(cardInstanceId);
        }
    }

    public void UpdateCoolDownCards()
    {
        List<CoolingDownCard> removeList = new List<CoolingDownCard>();
        foreach (CoolingDownCard coolingDownCard in CoolingDownCards)
        {
            if (coolingDownCard.LeftRounds == 1)
            {
                CardInfo_Base cib = AllCards.GetCard(coolingDownCard.CardID);
                removeList.Add(coolingDownCard);
                AbandonCards.Add(cib);
            }
            else
            {
                coolingDownCard.LeftRounds--;
                UpdateCoolDownCardHandler(coolingDownCard);
            }
        }

        foreach (CoolingDownCard coolingDownCard in removeList)
        {
            CoolingDownCards.Remove(coolingDownCard);
            RemoveCoolDownCardHandler(coolingDownCard);
        }

        AbandonCardRecycle();
    }

    public void AbandonCardRecycle()
    {
        bool needRecycle = AbandonCards.Count > 0;

        if (needRecycle)
        {
            foreach (CardInfo_Base ac in AbandonCards)
            {
                AppendCard(ac);
            }

            AbandonCards.Clear();
            Suffle(Cards);
        }
    }

    public Action<CoolingDownCard> RemoveCoolDownCardHandler;
    public Action<CoolingDownCard> UpdateCoolDownCardHandler;
}