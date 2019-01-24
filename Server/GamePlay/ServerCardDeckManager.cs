using System.Collections.Generic;

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

    public CardDeck CardDeck;

    public int PutCardsOnTopByType(CardTypes cardType, int number)
    {
        List<CardInfo_Base> res = new List<CardInfo_Base>();
        if (CardDeck.IsEmpty)
        {
            return 0;
        }

        int count = CardDeck.PutCardToTopByType(cardType, number);
        return count;
    }

    public int PutHeroCardToTop(int number)
    {
        List<CardInfo_Base> res = new List<CardInfo_Base>();
        if (CardDeck.IsEmpty)
        {
            return 0;
        }

        int count = CardDeck.PutHeroCardToTop(number);
        return count;
    }

    public CardInfo_Base DrawCardOnTop()
    {
        if (CardDeck.IsEmpty)
        {
            return null;
        }

        CardInfo_Base newCardInfoBase = CardDeck.DrawCardOnTop();
        return newCardInfoBase;
    }

    public List<CardInfo_Base> DrawCardsOnTop(int number)
    {
        if (CardDeck.IsEmpty)
        {
            return new List<CardInfo_Base>();
        }

        return CardDeck.DrawCardsOnTop(number);
    }

    public void RandomInsertTempCard(int cardId)
    {
        CardDeck.RandomInsertTempCard(cardId);
    }

    public void OnDrawCardPhase()
    {
        CardDeck.UpdateCoolDownCards();
    }

    public void EndRound()
    {
        CardDeck.AbandonCardRecycle();
    }

    public void OnUpdatePlayerCoolDownCard(CardDeck.CoolingDownCard cdc)
    {
        ServerPlayer.MyGameManager.Broadcast_AddRequestToOperationResponse(new PlayerCoolDownCardUpdateRequest(ServerPlayer.ClientId, cdc));
    }

    public void OnRemovePlayerCoolDownCard(CardDeck.CoolingDownCard cdc)
    {
        ServerPlayer.MyGameManager.Broadcast_AddRequestToOperationResponse(new PlayerCoolDownCardRemoveRequest(ServerPlayer.ClientId, cdc));
    }
}