using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

/// <summary>
/// 本类中封装卡组操作的游戏逻辑高级功能
/// </summary>
public class ServerCardDeckManager
{
    public ServerPlayer ServerPlayer;

    public CardDeckInfo M_UnlockCards;
    public CardDeckInfo M_LockCards;
    public List<CardDeck> M_CardDecks;

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

    public CardInfo_Base DrawRetinueCard()
    {
        if (M_CurrentCardDeck.IsEmpty)
        {
            M_CurrentCardDeck.AbandonCardRecycle();
            M_CurrentCardDeck.SuffleSelf();
        }

        M_CurrentCardDeck.GetARetinueCardToTheTop();
        CardInfo_Base newCardInfoBase = DrawCardOnTop();
        return newCardInfoBase;
    }

    public CardInfo_Base DrawCardOnTop()
    {
        if (M_CurrentCardDeck.IsEmpty)
        {
            M_CurrentCardDeck.AbandonCardRecycle();
            M_CurrentCardDeck.SuffleSelf();
        }

        CardInfo_Base newCardInfoBase = M_CurrentCardDeck.DrawCardOnTop();
        OnPlayerGetCard(newCardInfoBase.CardID);
        return newCardInfoBase;
    }

    public List<CardInfo_Base> DrawCardsOnTop(int cardNumber)
    {
        if (M_CurrentCardDeck.IsEmpty)
        {
            M_CurrentCardDeck.AbandonCardRecycle();
            M_CurrentCardDeck.SuffleSelf();
        }

        List<CardInfo_Base> newCardInfoBases = M_CurrentCardDeck.DrawCardsOnTop(cardNumber);
        List<int> cardIds = new List<int>();
        foreach (CardInfo_Base newCardInfoBase in newCardInfoBases)
        {
            cardIds.Add(newCardInfoBase.CardID);
        }

        OnPlayerGetCards(cardIds);
        return newCardInfoBases;
    }

    public void OnPlayerGetCard(int cardId)
    {
        DrawCardRequest request1 = new DrawCardRequest(ServerPlayer.ClientId, cardId, true);
        DrawCardRequest request2 = new DrawCardRequest(ServerPlayer.ClientId, cardId, false);
        ServerPlayer.MyClientProxy.SendRequestsQueue.Enqueue(request1);
        ServerPlayer.MyEnemyPlayer.MyClientProxy.SendRequestsQueue.Enqueue(request2);
    }

    public void OnPlayerGetCards(List<int> cardIds)
    {
        DrawCardRequest request1 = new DrawCardRequest(ServerPlayer.ClientId, cardIds, true);
        DrawCardRequest request2 = new DrawCardRequest(ServerPlayer.ClientId, cardIds, false);
        ServerPlayer.MyClientProxy.SendRequestsQueue.Enqueue(request1);
        ServerPlayer.MyEnemyPlayer.MyClientProxy.SendRequestsQueue.Enqueue(request2);
    }
}