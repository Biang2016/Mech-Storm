using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using SideEffects;

internal class Battle_HandManager
{
    public BattlePlayer BattlePlayer;
    public List<CardBase> Cards = new List<CardBase>();
    public HashSet<int> UsableCards = new HashSet<int>();

    public Battle_HandManager(BattlePlayer battlePlayer)
    {
        BattlePlayer = battlePlayer;
    }

    #region DrawCards

    private void HandAddCard(CardBase newCard)
    {
        Cards.Add(newCard);
        BattlePlayer.BattleStatistics.Draws++;
    }

    internal void DrawCards(int cardNumber)
    {
        int maxDrawCardNumber = Math.Min(Math.Min(cardNumber, GamePlaySettings.MaxHandCard - Cards.Count), BattlePlayer.CardDeckManager.CardDeck.CardCount());
        if (maxDrawCardNumber != cardNumber)
        {
            DrawCards(maxDrawCardNumber);
            return;
        }

        List<DrawCardRequest.CardIdAndInstanceId> cardInfos = new List<DrawCardRequest.CardIdAndInstanceId>();

        foreach (CardInfo_Base cb in BattlePlayer.CardDeckManager.DrawCardsOnTop(maxDrawCardNumber))
        {
            CardBase newCard = CardBase.InstantiateCardByCardInfo(cb, BattlePlayer, BattlePlayer.GameManager.GenerateNewCardInstanceId());
            HandAddCard(newCard);
            BattlePlayer.CardDeckManager.CardDeck.AddCardInstanceId(newCard.CardInfo.CardID, newCard.M_CardInstanceId);
            cardInfos.Add(new DrawCardRequest.CardIdAndInstanceId(newCard.CardInfo.CardID, newCard.M_CardInstanceId));
        }

        OnPlayerGetCards(cardInfos);
    }

    internal void DrawCardsByType(CardTypes cardType, int number)
    {
        int newCardCount = BattlePlayer.CardDeckManager.PutCardsOnTopByType(cardType, number);
        DrawCards(newCardCount);
    }

    internal void DrawHeroCards(int number)
    {
        int newCardCount = BattlePlayer.CardDeckManager.PutHeroCardToTop(number);
        DrawCards(newCardCount);
    }

    internal void PutCardsOnTopByID(List<int> cardIDs)
    {
        BattlePlayer.CardDeckManager.CardDeck.PutCardsToTopByID(cardIDs);
    }

    #endregion

    internal List<int> GetRandomHandCardIds(int count, HashSet<int> exceptionInstanceIDList = null)
    {
        List<int> validCardIDs = new List<int>();
        foreach (CardBase cb in Cards)
        {
            if (!exceptionInstanceIDList.Contains(cb.M_CardInstanceId))
            {
                validCardIDs.Add(cb.CardInfo.CardID);
            }
        }

        List<int> res = Utils.GetRandomFromList(validCardIDs, count);
        return res;
    }

    internal List<int> GetRandomSpellCardInstanceIds(int count, int exceptCardInstanceID)
    {
        List<CardBase> spellCards = new List<CardBase>();
        Cards.ForEach(card =>
        {
            if (card is CardSpell spellCard)
            {
                if (spellCard.M_CardInstanceId != exceptCardInstanceID && spellCard.CardInfo.TargetInfo.HasNoTarget)
                {
                    spellCards.Add(spellCard);
                }
            }
        });
        if (count > spellCards.Count) count = spellCards.Count;
        List<CardBase> res = Utils.GetRandomFromList(spellCards, count);
        List<int> resIDs = new List<int>();
        res.ForEach(card => { resIDs.Add(card.M_CardInstanceId); });
        return resIDs;
    }

    internal void GetTempCardsByID(int cardID, int count)
    {
        for (int i = 0; i < count; i++)
        {
            CardInfo_Base cardInfo = AllCards.GetCard(cardID);
            CardBase newCard = CardBase.InstantiateCardByCardInfo(cardInfo, BattlePlayer, BattlePlayer.GameManager.GenerateNewTempCardInstanceId());
            OnPlayerGetCard(cardID, newCard.M_CardInstanceId);
            HandAddCard(newCard);
        }
    }

    internal void GetACardByID(int cardID, int overrideCardInstanceID = -1)
    {
        CardInfo_Base cardInfo = AllCards.GetCard(cardID);
        CardBase newCard;
        if (overrideCardInstanceID == -1)
        {
            newCard = CardBase.InstantiateCardByCardInfo(cardInfo, BattlePlayer, BattlePlayer.GameManager.GenerateNewTempCardInstanceId());
        }
        else
        {
            newCard = CardBase.InstantiateCardByCardInfo(cardInfo, BattlePlayer, overrideCardInstanceID);
        }

        OnPlayerGetCard(cardID, newCard.M_CardInstanceId);
        HandAddCard(newCard);
    }

    internal void GetTempCardByCardTypes(CardFilterTypes cardFilterTypes, int count)
    {
        List<int> cardIds = AllCards.GetRandomCardInfoByCardFilterType(cardFilterTypes, count);
        foreach (int cardId in cardIds)
        {
            GetTempCardsByID(cardId, 1);
        }
    }

    public void OnPlayerGetCard(int cardId, int cardInstanceId)
    {
        DrawCardRequest request1 = new DrawCardRequest(BattlePlayer.ClientId, new DrawCardRequest.CardIdAndInstanceId(cardId, cardInstanceId), true);
        DrawCardRequest request2 = new DrawCardRequest(BattlePlayer.ClientId, new DrawCardRequest.CardIdAndInstanceId(cardId, cardInstanceId), false);
        BattlePlayer?.MyClientProxy?.CurrentClientRequestResponseBundle.AttachedRequests.Add(request1);
        BattlePlayer?.MyEnemyPlayer?.MyClientProxy?.CurrentClientRequestResponseBundle.AttachedRequests.Add(request2);
    }

    public void OnPlayerGetCards(List<DrawCardRequest.CardIdAndInstanceId> cardInfos)
    {
        if (cardInfos.Count == 0) return;
        DrawCardRequest request1 = new DrawCardRequest(BattlePlayer.ClientId, cardInfos, true);
        DrawCardRequest request2 = new DrawCardRequest(BattlePlayer.ClientId, cardInfos, false);
        BattlePlayer?.MyClientProxy?.CurrentClientRequestResponseBundle.AttachedRequests.Add(request1);
        BattlePlayer?.MyEnemyPlayer?.MyClientProxy?.CurrentClientRequestResponseBundle.AttachedRequests.Add(request2);
    }

    internal int DropCardType(CardFilterTypes cardFilterType, HashSet<int> exceptCardInstanceId)
    {
        List<int> dropCardInstanceIds = new List<int>();
        switch (cardFilterType)
        {
            case CardFilterTypes.All:
            {
                foreach (CardBase cb in Cards)
                {
                    if (!exceptCardInstanceId.Contains(cb.M_CardInstanceId))
                    {
                        dropCardInstanceIds.Add(cb.M_CardInstanceId);
                    }
                }

                break;
            }
            case CardFilterTypes.SoldierMech:
            {
                foreach (CardBase cb in Cards)
                {
                    if (!exceptCardInstanceId.Contains(cb.M_CardInstanceId))
                    {
                        if (cb.CardInfo.BaseInfo.CardType == CardTypes.Mech && cb.CardInfo.MechInfo.IsSoldier)
                        {
                            dropCardInstanceIds.Add(cb.M_CardInstanceId);
                        }
                    }
                }

                break;
            }
            case CardFilterTypes.HeroMech:
            {
                foreach (CardBase cb in Cards)
                {
                    if (!exceptCardInstanceId.Contains(cb.M_CardInstanceId))
                    {
                        if (cb.CardInfo.BaseInfo.CardType == CardTypes.Mech && !cb.CardInfo.MechInfo.IsSoldier)
                        {
                            dropCardInstanceIds.Add(cb.M_CardInstanceId);
                        }
                    }
                }

                break;
            }
            case CardFilterTypes.Equip:
            {
                foreach (CardBase cb in Cards)
                {
                    if (!exceptCardInstanceId.Contains(cb.M_CardInstanceId))
                    {
                        if (cb.CardInfo.BaseInfo.CardType == CardTypes.Equip)
                        {
                            dropCardInstanceIds.Add(cb.M_CardInstanceId);
                        }
                    }
                }

                break;
            }
            case CardFilterTypes.Spell:
            {
                foreach (CardBase cb in Cards)
                {
                    if (!exceptCardInstanceId.Contains(cb.M_CardInstanceId))
                    {
                        if (cb.CardInfo.BaseInfo.CardType == CardTypes.Spell)
                        {
                            dropCardInstanceIds.Add(cb.M_CardInstanceId);
                        }
                    }
                }

                break;
            }
            case CardFilterTypes.Energy:
            {
                foreach (CardBase cb in Cards)
                {
                    if (!exceptCardInstanceId.Contains(cb.M_CardInstanceId))
                    {
                        if (cb.CardInfo.BaseInfo.CardType == CardTypes.Energy)
                        {
                            dropCardInstanceIds.Add(cb.M_CardInstanceId);
                        }
                    }
                }

                break;
            }
        }

        int count = 0;
        foreach (int dropCardInstanceId in dropCardInstanceIds)
        {
            bool suc = DropCard(dropCardInstanceId);
            if (suc) count++;
        }

        return count;
    }

    internal bool DropCard(int cardInstanceId)
    {
        return DropCard(GetCardByCardInstanceId(cardInstanceId));
    }

    internal bool DropCard(CardBase dropCard)
    {
        if (Cards.Contains(dropCard))
        {
            DropCardRequest request = new DropCardRequest(BattlePlayer.ClientId, dropCard.M_CardInstanceId);
            BattlePlayer.MyClientProxy.BattleGameManager.Broadcast_AddRequestToOperationResponse(request);
            Cards.Remove(dropCard);
            UsableCards.Remove(dropCard.M_CardInstanceId);
            if (!dropCard.CardInfo.BaseInfo.IsTemp) BattlePlayer.CardDeckManager.CardDeck.RecycleCardInstanceID(dropCard.M_CardInstanceId);
            return true;
        }
        else
        {
            return false;
        }
    }

    internal void UseCard(int cardInstanceId, List<int> targetMechIds = null, List<int> targetEquipIds = null, List<int> targetClientIds = null, bool onlyTriggerNotUse = false)
    {
        CardBase useCard = GetCardByCardInstanceId(cardInstanceId);
        Utils.DebugLog("AI UseCard:" + useCard.CardInfo.BaseInfo.CardNames["zh"]);

        if (onlyTriggerNotUse)
        {
            CardBase copyCard = CardBase.InstantiateCardByCardInfo(useCard.CardInfo.Clone(), useCard.BattlePlayer, BattlePlayer.GameManager.GenerateNewTempCardInstanceId());
            BattlePlayer.GameManager.EventManager.Invoke(SideEffectExecute.GetTriggerTimeByCardType(copyCard.CardInfo.BaseInfo.CardType),
                new ExecutorInfo(
                    clientId: BattlePlayer.ClientId,
                    targetClientIds: targetClientIds,
                    targetMechIds: targetMechIds,
                    cardId: copyCard.CardInfo.CardID,
                    cardInstanceId: copyCard.M_CardInstanceId,
                    targetEquipIds: targetEquipIds));
            copyCard.UnRegisterSideEffect();
        }
        else
        {
            BattlePlayer.BattleStatistics.UseCard(useCard.CardInfo);
            UseCardRequest request = new UseCardRequest(BattlePlayer.ClientId, useCard.M_CardInstanceId, useCard.CardInfo.Clone());
            BattlePlayer.MyClientProxy.BattleGameManager.Broadcast_AddRequestToOperationResponse(request);
            BattlePlayer.UseMetal(useCard.CardInfo.BaseInfo.Metal);
            BattlePlayer.UseEnergy(useCard.CardInfo.BaseInfo.Energy);
            BattlePlayer.GameManager.EventManager.Invoke(SideEffectExecute.GetTriggerTimeByCardType(useCard.CardInfo.BaseInfo.CardType),
                new ExecutorInfo(
                    clientId: BattlePlayer.ClientId,
                    targetClientIds: targetClientIds,
                    targetMechIds: targetMechIds,
                    cardId: useCard.CardInfo.CardID,
                    cardInstanceId: cardInstanceId,
                    targetEquipIds: targetEquipIds));
            if (!useCard.CardInfo.BaseInfo.IsTemp)
            {
                if (useCard.CardInfo.BaseInfo.CardType == CardTypes.Spell || useCard.CardInfo.BaseInfo.CardType == CardTypes.Energy)
                {
                    BattlePlayer.CardDeckManager.CardDeck.RecycleCardInstanceID(cardInstanceId);
                }
            }

            Cards.Remove(useCard);
            UsableCards.Remove(useCard.M_CardInstanceId);
            useCard.UnRegisterSideEffect();
        }
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
        foreach (CardBase card in Cards) card.Usable = (BattlePlayer == BattlePlayer.GameManager.CurrentPlayer) && card.M_Metal <= BattlePlayer.MetalLeft && card.M_Energy <= BattlePlayer.EnergyLeft;
    }

    public void SetAllCardUnusable() //禁用所有手牌
    {
        foreach (CardBase card in Cards) card.Usable = false;
    }

    #region Utils

    internal CardBase GetCardByCardInstanceId(int cardInstanceId)
    {
        foreach (CardBase serverCardBase in Cards)
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
        foreach (CardBase serverCardBase in Cards)
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