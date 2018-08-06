using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyCardGameCommon;

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
            newCard.M_CardInstanceId = ServerPlayer.MyGameManager.GeneratorNewCardInstanceId();
            cards.Add(newCard);
        }
    }


    internal void DrawCards(int cardNumber)
    {
        List<CardInfo_Base> newCardsInfo = ServerPlayer.MyCardDeckManager.DrawCardsOnTop(cardNumber);
        List<DrawCardRequest.CardIdAndInstanceId> infos = new List<DrawCardRequest.CardIdAndInstanceId>();
        foreach (CardInfo_Base cardInfoBase in newCardsInfo)
        {
            ServerCardBase newCard = ServerCardBase.InstantiateCardByCardInfo(cardInfoBase, ServerPlayer);
            newCard.M_CardInstanceId = ServerPlayer.MyGameManager.GeneratorNewCardInstanceId();
            infos.Add(new DrawCardRequest.CardIdAndInstanceId(cardInfoBase.CardID, newCard.M_CardInstanceId));
            cards.Add(newCard);
            cardNumber++;
            if (cardNumber >= GamePlaySettings.MaxHandCard)
            {
                break;
            }
        }

        OnPlayerGetCards(infos);
    }

    internal void GetACardByID(int cardID)
    {
        CardInfo_Base cardInfo = AllCards.GetCard(cardID);
        ServerCardBase newCard = ServerCardBase.InstantiateCardByCardInfo(cardInfo, ServerPlayer);
        newCard.M_CardInstanceId = ServerPlayer.MyGameManager.GeneratorNewCardInstanceId();
        OnPlayerGetCard(cardID, newCard.M_CardInstanceId);
        cards.Add(newCard);
    }

    internal void DrawSodierCard()
    {
        CardInfo_Base newCardInfo = ServerPlayer.MyCardDeckManager.DrawSodierCard();
        if (newCardInfo == null) return;
        ServerCardBase newCard = ServerCardBase.InstantiateCardByCardInfo(newCardInfo, ServerPlayer);
        newCard.M_CardInstanceId = ServerPlayer.MyGameManager.GeneratorNewCardInstanceId();
        OnPlayerGetCard(newCardInfo.CardID, newCard.M_CardInstanceId);
        cards.Add(newCard);
    }

    public void OnPlayerGetCard(int cardId, int cardInstanceId)
    {
        DrawCardRequest request1 = new DrawCardRequest(ServerPlayer.ClientId, new DrawCardRequest.CardIdAndInstanceId(cardId, cardInstanceId), true);
        DrawCardRequest request2 = new DrawCardRequest(ServerPlayer.ClientId, new DrawCardRequest.CardIdAndInstanceId(cardId, cardInstanceId), false);
        ServerPlayer?.MyClientProxy?.CurrentClientRequestResponse.SideEffects.Add(request1);
        ServerPlayer?.MyEnemyPlayer?.MyClientProxy?.CurrentClientRequestResponse.SideEffects.Add(request2);
    }

    public void OnPlayerGetCards(List<DrawCardRequest.CardIdAndInstanceId> cardInfos)
    {
        if (cardInfos.Count == 0) return;
        DrawCardRequest request1 = new DrawCardRequest(ServerPlayer.ClientId, cardInfos, true);
        DrawCardRequest request2 = new DrawCardRequest(ServerPlayer.ClientId, cardInfos, false);
        ServerPlayer?.MyClientProxy?.CurrentClientRequestResponse.SideEffects.Add(request1);
        ServerPlayer?.MyEnemyPlayer?.MyClientProxy?.CurrentClientRequestResponse.SideEffects.Add(request2);
    }

    internal void DropCard(int cardInstanceId)
    {
        DropCard(GetCardByCardInstanceId(cardInstanceId));
    }

    internal void DropCard(ServerCardBase dropCard)
    {
        DropCardRequest request = new DropCardRequest(ServerPlayer.ClientId, cards.IndexOf(dropCard));
        ServerPlayer.MyClientProxy.MyServerGameManager.Broadcast_AddRequestToOperationResponse(request);
        cards.Remove(dropCard);
    }

    internal void UseCard(int cardInstanceId, Vector3 lastDragPosition)
    {
        ServerCardBase card = GetCardByCardInstanceId(cardInstanceId);
        ServerPlayer.UseCostAboveZero(card.CardInfo.BaseInfo.Cost);
        UseCard(card, lastDragPosition);
    }

    internal void UseCard(ServerCardBase useCard, Vector3 lastDragPosition)
    {
        UseCardRequest request = new UseCardRequest(ServerPlayer.ClientId, useCard.M_CardInstanceId, useCard.CardInfo.Clone(), lastDragPosition);
        ServerPlayer.MyClientProxy.MyServerGameManager.Broadcast_AddRequestToOperationResponse(request);
        cards.Remove(useCard);
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

    #region Utils

    internal ServerCardBase GetCardByCardInstanceId(int cardInstanceId)
    {
        foreach (ServerCardBase serverCardBase in cards)
        {
            if (serverCardBase.M_CardInstanceId == cardInstanceId)
            {
                return serverCardBase;
            }
        }

        return null;
    }

    internal CardInfo_Base GetHandCardInfo(int cardInstanceId)
    {
        foreach (ServerCardBase serverCardBase in cards)
        {
            if (serverCardBase.M_CardInstanceId == cardInstanceId)
            {
                return serverCardBase.CardInfo;
            }
        }

        return null;
    }

    #endregion
}