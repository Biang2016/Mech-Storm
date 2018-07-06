using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ServerCardDeckManager
{
    /// <summary>
    /// 本类中封装卡组操作的游戏逻辑高级功能
    /// 暂时将双方牌库写在此类中
    /// </summary>
    /// 
    public ServerPlayer ServerPlayer;

    public CardDeckInfo M_UnlockCards;
    public CardDeckInfo M_LockCards;
    public List<CardDeck> M_CardDecks;
    private CardDeck m_CurrentCardDeck;

    public CardDeck M_CurrentCardDeck
    {
        get { return m_CurrentCardDeck; }
        set
        {
            m_CurrentCardDeck = value;
            CardDeckRequest cdRequest = new CardDeckRequest(ServerPlayer.ClientId, value.M_CardDeckInfo);
            Server.SV.SendMessage(cdRequest, ServerPlayer.ClientId);
        }
    }

    public CardInfo_Base DrawRetinueCard()
    {
        if (M_CurrentCardDeck.IsEmpty)
        {
            M_CurrentCardDeck.AbandonCardRecycle();
            M_CurrentCardDeck.SuffleSelf();
        }

        M_CurrentCardDeck.GetARetinueCardToTheTop();
        CardInfo_Base newCardInfoBase = DrawTop();
        return newCardInfoBase;
    }

    public CardInfo_Base DrawTop()
    {
        if (M_CurrentCardDeck.IsEmpty)
        {
            M_CurrentCardDeck.AbandonCardRecycle();
            M_CurrentCardDeck.SuffleSelf();
        }

        CardInfo_Base newCardInfoBase = M_CurrentCardDeck.DrawCardOnTop();
        DrawCardRequest request = new DrawCardRequest(ServerPlayer.ClientId, newCardInfoBase.CardID);
        Server.SV.SendMessage(request, ServerPlayer.ClientId);
        return newCardInfoBase;
    }
}