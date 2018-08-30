﻿using System.Collections.Generic;

/// <summary>
/// 本类中封装卡组操作的游戏逻辑高级功能
/// </summary>
internal class ServerCardDeckManager
{
    public ServerPlayer ServerPlayer;

    public ServerCardDeckManager(ServerPlayer serverPlayer)
    {
        ServerPlayer = serverPlayer;
    }

    private CardDeck m_CurrentCardDeck;

    public CardDeck M_CurrentCardDeck
    {
        get { return m_CurrentCardDeck; }
        set { m_CurrentCardDeck = value; }
    }

    public CardInfo_Base DrawSoldierCard()
    {
        if (M_CurrentCardDeck.IsEmpty)
        {
            return null;
        }

        bool success = M_CurrentCardDeck.GetASoldierCardToTheTop();
        if (success)
        {
            CardInfo_Base newCardInfoBase = DrawCardOnTop();
            return newCardInfoBase;
        }
        else
        {
            return null;
        }
    }

    public CardInfo_Base DrawCardOnTop()
    {
        if (M_CurrentCardDeck.IsEmpty)
        {
            return null;
        }

        CardInfo_Base newCardInfoBase = M_CurrentCardDeck.DrawCardOnTop();
        return newCardInfoBase;
    }

    public List<CardInfo_Base> DrawCardsOnTop(int cardNumber)
    {
        if (M_CurrentCardDeck.IsEmpty)
        {
            return new List<CardInfo_Base>();
        }

        List<CardInfo_Base> newCardInfoBases = M_CurrentCardDeck.DrawCardsOnTop(cardNumber);
        List<int> cardIds = new List<int>();
        foreach (CardInfo_Base newCardInfoBase in newCardInfoBases)
        {
            cardIds.Add(newCardInfoBase.CardID);
        }

        return newCardInfoBases;
    }

    public void BeginRound()
    {
        M_CurrentCardDeck.AbandonCardRecycle();
    }
}