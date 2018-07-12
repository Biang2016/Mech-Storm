using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

internal class ServerHandManager
{
    public ServerPlayer ServerPlayer;
    List<ServerCardBase> cards = new List<ServerCardBase>();

    public ServerHandManager(ServerPlayer serverPlayer)
    {
        ServerPlayer = serverPlayer;
    }

    internal void DrawCard()
    {
        if (cards.Count >= GamePlaySettings.MaxHandCard)
        {
            //无法抽牌
        }
        else
        {
            CardInfo_Base newCardInfo = ServerPlayer.MyCardDeckManager.DrawCardOnTop();
            if (newCardInfo == null)
            {
                ServerLog.Print("No Card");
                return;
            }

            ServerCardBase newCard = ServerCardBase.InstantiateCardByCardInfo(newCardInfo, ServerPlayer);
            cards.Add(newCard);
        }
    }


    internal void DrawCards(int cardNumber)
    {
        List<CardInfo_Base> newCardsInfo = ServerPlayer.MyCardDeckManager.DrawCardsOnTop(cardNumber);
        foreach (CardInfo_Base cardInfoBase in newCardsInfo)
        {
            ServerCardBase newCard = ServerCardBase.InstantiateCardByCardInfo(cardInfoBase, ServerPlayer);
            cards.Add(newCard);
            cardNumber++;
            if (cardNumber >= GamePlaySettings.MaxHandCard)
            {
                break;
            }
        }
    }

    internal void GetACardByID(int cardID)
    {
        CardInfo_Base cardInfo = AllCards.GetCard(cardID);
        ServerCardBase newCard = ServerCardBase.InstantiateCardByCardInfo(cardInfo, ServerPlayer);
        ServerPlayer.MyCardDeckManager.OnPlayerGetCard(cardID);
        cards.Add(newCard);
    }

    internal void DrawRetinueCard()
    {
        CardInfo_Base newCardInfo = ServerPlayer.MyCardDeckManager.DrawRetinueCard();
        if (newCardInfo == null)
        {
            ServerLog.Print("No Card");
            return;
        }

        ServerCardBase newCard = ServerCardBase.InstantiateCardByCardInfo(newCardInfo, ServerPlayer);
        cards.Add(newCard);
    }

    internal void DropFirstCard()
    {
        DropCard(cards[0]);
    }

    internal void DropCard(ServerCardBase dropCard)
    {
        cards.Remove(dropCard);
    }

    internal void DropCardAt(int index)
    {
        cards.RemoveAt(index);
    }

    public void BeginRound()
    {
        foreach (ServerCardBase card in cards) card.OnBeginRound();
        RefreshAllCardUsable();
    }

    public void EndRound()
    {
        foreach (ServerCardBase card in cards) card.OnEndRound();
        SetAllCardUnusable();
    }

    public void RefreshAllCardUsable() //刷新所有卡牌是否可用
    {
        foreach (ServerCardBase card in cards) card.Usable = (ServerPlayer == ServerPlayer.MyGameManager.CurrentPlayer) && card.M_Cost <= ServerPlayer.CostLeft;
    }

    public void SetAllCardUnusable() //禁用所有手牌
    {
        foreach (ServerCardBase card in cards) card.Usable = false;
    }
}