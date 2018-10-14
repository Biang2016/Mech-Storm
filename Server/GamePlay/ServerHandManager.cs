using System;
using System.Collections.Generic;
using MyCardGameCommon;

internal class ServerHandManager
{
    public ServerPlayer ServerPlayer;
    List<ServerCardBase> cards = new List<ServerCardBase>();

    public ServerHandManager(ServerPlayer serverPlayer)
    {
        ServerPlayer = serverPlayer;
    }

    #region DrawCards

    internal void DrawCards(int cardNumber)
    {
        int maxDrawCardNumber = Math.Min(Math.Min(cardNumber, GamePlaySettings.MaxHandCard - cards.Count), ServerPlayer.MyCardDeckManager.CardDeck.CardCount());
        if (maxDrawCardNumber != cardNumber)
        {
            DrawCards(maxDrawCardNumber);
            return;
        }

        List<DrawCardRequest.CardIdAndInstanceId> cardInfos = new List<DrawCardRequest.CardIdAndInstanceId>();

        foreach (CardInfo_Base cb in ServerPlayer.MyCardDeckManager.DrawCardsOnTop(maxDrawCardNumber))
        {
            ServerCardBase newCard = ServerCardBase.InstantiateCardByCardInfo(cb, ServerPlayer, ServerPlayer.MyGameManager.GenerateNewCardInstanceId());
            cards.Add(newCard);
            ServerPlayer.MyCardDeckManager.CardDeck.AddCardInstanceId(newCard.CardInfo.CardID, newCard.M_CardInstanceId);
            cardInfos.Add(new DrawCardRequest.CardIdAndInstanceId(newCard.CardInfo.CardID, newCard.M_CardInstanceId));
        }

        OnPlayerGetCards(cardInfos);
    }

    internal void DrawCardsByType(CardTypes cardType, int number)
    {
        int newCardCount = ServerPlayer.MyCardDeckManager.PutCardsOnTopByType(cardType, number);
        DrawCards(newCardCount);
    }

    #endregion


    internal void GetACardByID(int cardID)
    {
        CardInfo_Base cardInfo = AllCards.GetCard(cardID);
        ServerCardBase newCard = ServerCardBase.InstantiateCardByCardInfo(cardInfo, ServerPlayer, ServerPlayer.MyGameManager.GenerateNewCardInstanceId());
        OnPlayerGetCard(cardID, newCard.M_CardInstanceId);
        cards.Add(newCard);
    }


    public void OnPlayerGetCard(int cardId, int cardInstanceId)
    {
        DrawCardRequest request1 = new DrawCardRequest(ServerPlayer.ClientId, new DrawCardRequest.CardIdAndInstanceId(cardId, cardInstanceId), true);
        DrawCardRequest request2 = new DrawCardRequest(ServerPlayer.ClientId, new DrawCardRequest.CardIdAndInstanceId(cardId, cardInstanceId), false);
        ServerPlayer?.MyClientProxy?.CurrentClientRequestResponseBundle.AttachedRequests.Add(request1);
        ServerPlayer?.MyEnemyPlayer?.MyClientProxy?.CurrentClientRequestResponseBundle.AttachedRequests.Add(request2);
    }

    public void OnPlayerGetCards(List<DrawCardRequest.CardIdAndInstanceId> cardInfos)
    {
        if (cardInfos.Count == 0) return;
        DrawCardRequest request1 = new DrawCardRequest(ServerPlayer.ClientId, cardInfos, true);
        DrawCardRequest request2 = new DrawCardRequest(ServerPlayer.ClientId, cardInfos, false);
        ServerPlayer?.MyClientProxy?.CurrentClientRequestResponseBundle.AttachedRequests.Add(request1);
        ServerPlayer?.MyEnemyPlayer?.MyClientProxy?.CurrentClientRequestResponseBundle.AttachedRequests.Add(request2);
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
        if (!dropCard.CardInfo.BaseInfo.IsTemp) ServerPlayer.MyCardDeckManager.CardDeck.RecycleCardInstanceID(dropCard.M_CardInstanceId);
    }

    internal void UseCard(int cardInstanceId, Vector3 lastDragPosition)
    {
        UseCard(cardInstanceId, lastDragPosition, -999, -999, -1);
    }

    internal void UseCard(int cardInstanceId, Vector3 lastDragPosition, int targetRetinueId = -999, int targetEquipId = -999, int targetClientId = -1)
    {
        ServerCardBase useCard = GetCardByCardInstanceId(cardInstanceId);
        ServerPlayer.UseMetalAboveZero(useCard.CardInfo.BaseInfo.Metal);
        ServerPlayer.UseEnergyAboveZero(useCard.CardInfo.BaseInfo.Energy);

        UseCardRequest request = new UseCardRequest(ServerPlayer.ClientId, useCard.M_CardInstanceId, useCard.CardInfo.Clone(), lastDragPosition);
        ServerPlayer.MyClientProxy.MyServerGameManager.Broadcast_AddRequestToOperationResponse(request);

        ServerPlayer.MyGameManager.EventManager.Invoke(SideEffectBundle.TriggerTime.OnPlayCard,
            new SideEffectBase.ExecuterInfo(
                clientId: ServerPlayer.ClientId,
                targetClientId: targetClientId,
                targetRetinueId: targetRetinueId,
                cardId: useCard.CardInfo.CardID,
                cardInstanceId: cardInstanceId,
                targetEquipId: targetEquipId));

        if (!useCard.CardInfo.BaseInfo.IsTemp)
        {
            if (useCard.CardInfo.BaseInfo.CardType == CardTypes.Spell || useCard.CardInfo.BaseInfo.CardType == CardTypes.Energy)
            {
                ServerPlayer.MyCardDeckManager.CardDeck.RecycleCardInstanceID(cardInstanceId);
            }
        }

        useCard.UnRegisterSideEffect();
        cards.Remove(useCard);
    }

    public void BeginRound()
    {
        RefreshAllCardUsable();
    }

    public void EndRound()
    {
        SetAllCardUnusable();
    }

    public void RefreshAllCardUsable() //刷新所有卡牌是否可用
    {
        foreach (ServerCardBase card in cards) card.Usable = (ServerPlayer == ServerPlayer.MyGameManager.CurrentPlayer) && card.M_Metal <= ServerPlayer.MetalLeft;
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