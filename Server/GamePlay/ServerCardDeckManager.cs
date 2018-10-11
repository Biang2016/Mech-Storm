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

    public List<CardInfo_Base> PutCardsOnTopByType(CardTypes cardType, int number)
    {
        List<CardInfo_Base> res = new List<CardInfo_Base>();
        if (CardDeck.IsEmpty)
        {
            return null;
        }

        int count = CardDeck.PutCardToTopByType(cardType, number);
        for (int i = 0; i < count; i++)
        {
            res.Add(DrawCardOnTop());
        }

        return res;
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

    public void BeginRound()
    {
        CardDeck.AbandonCardRecycle();
    }
}