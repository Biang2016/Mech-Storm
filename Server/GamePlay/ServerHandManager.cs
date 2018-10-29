using System;
using System.Collections.Generic;

internal class ServerHandManager
{
    public ServerPlayer ServerPlayer;
    public List<ServerCardBase> Cards = new List<ServerCardBase>();
    public HashSet<int> CardInstanceIDSet = new HashSet<int>();

    public ServerHandManager(ServerPlayer serverPlayer)
    {
        ServerPlayer = serverPlayer;
    }

    #region DrawCards

    internal void DrawCards(int cardNumber)
    {
        int maxDrawCardNumber = Math.Min(Math.Min(cardNumber, GamePlaySettings.MaxHandCard - Cards.Count), ServerPlayer.MyCardDeckManager.CardDeck.CardCount());
        if (maxDrawCardNumber != cardNumber)
        {
            DrawCards(maxDrawCardNumber);
            return;
        }

        List<DrawCardRequest.CardIdAndInstanceId> cardInfos = new List<DrawCardRequest.CardIdAndInstanceId>();

        foreach (CardInfo_Base cb in ServerPlayer.MyCardDeckManager.DrawCardsOnTop(maxDrawCardNumber))
        {
            ServerCardBase newCard = ServerCardBase.InstantiateCardByCardInfo(cb, ServerPlayer, ServerPlayer.MyGameManager.GenerateNewCardInstanceId());
            Cards.Add(newCard);
            CardInstanceIDSet.Add(newCard.M_CardInstanceId);
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

    internal void DrawHeroCards(int number)
    {
        int newCardCount = ServerPlayer.MyCardDeckManager.PutHeroCardToTop(number);
        DrawCards(newCardCount);
    }

    #endregion

    internal int GetRandomHandCardId()
    {
        Random rd = new Random();
        int randomIndex = rd.Next(0, Cards.Count);
        return Cards[randomIndex].CardInfo.CardID;
    }

    internal void GetATempCardByID(int cardID)
    {
        CardInfo_Base cardInfo = AllCards.GetCard(cardID);
        ServerCardBase newCard = ServerCardBase.InstantiateCardByCardInfo(cardInfo, ServerPlayer, -1);
        OnPlayerGetCard(cardID, newCard.M_CardInstanceId);
        Cards.Add(newCard);
        CardInstanceIDSet.Add(newCard.M_CardInstanceId);
    }

    internal void GetACardByID(int cardID)
    {
        CardInfo_Base cardInfo = AllCards.GetCard(cardID);
        ServerCardBase newCard = ServerCardBase.InstantiateCardByCardInfo(cardInfo, ServerPlayer, ServerPlayer.MyGameManager.GenerateNewCardInstanceId());
        OnPlayerGetCard(cardID, newCard.M_CardInstanceId);
        Cards.Add(newCard);
        CardInstanceIDSet.Add(newCard.M_CardInstanceId);
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
        DropCardRequest request = new DropCardRequest(ServerPlayer.ClientId, Cards.IndexOf(dropCard));
        ServerPlayer.MyClientProxy.MyServerGameManager.Broadcast_AddRequestToOperationResponse(request);
        Cards.Remove(dropCard);
        CardInstanceIDSet.Remove(dropCard.M_CardInstanceId);
        if (!dropCard.CardInfo.BaseInfo.IsTemp) ServerPlayer.MyCardDeckManager.CardDeck.RecycleCardInstanceID(dropCard.M_CardInstanceId);
    }

    internal void UseCard(int cardInstanceId, int targetRetinueId = SideEffectBase.ExecuterInfo.EXECUTE_INFO_NONE, int targetEquipId = SideEffectBase.ExecuterInfo.EXECUTE_INFO_NONE, int targetClientId = -1)
    {
        ServerCardBase useCard = GetCardByCardInstanceId(cardInstanceId);
        UseCardRequest request = new UseCardRequest(ServerPlayer.ClientId, useCard.M_CardInstanceId, useCard.CardInfo.Clone());
        ServerPlayer.MyClientProxy.MyServerGameManager.Broadcast_AddRequestToOperationResponse(request);
        ServerPlayer.UseMetalAboveZero(useCard.CardInfo.BaseInfo.Metal);
        ServerPlayer.UseEnergyAboveZero(useCard.CardInfo.BaseInfo.Energy);

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
        Cards.Remove(useCard);
        CardInstanceIDSet.Remove(useCard.M_CardInstanceId);
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
        foreach (ServerCardBase card in Cards) card.Usable = (ServerPlayer == ServerPlayer.MyGameManager.CurrentPlayer) && card.M_Metal <= ServerPlayer.MetalLeft && card.M_Energy <= ServerPlayer.EnergyLeft;
    }

    public void SetAllCardUnusable() //禁用所有手牌
    {
        foreach (ServerCardBase card in Cards) card.Usable = false;
    }

    #region Utils

    internal ServerCardBase GetCardByCardInstanceId(int cardInstanceId)
    {
        foreach (ServerCardBase serverCardBase in Cards)
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
        foreach (ServerCardBase serverCardBase in Cards)
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