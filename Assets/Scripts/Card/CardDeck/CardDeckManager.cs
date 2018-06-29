using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CardDeckManager : MonoBehaviour
{
    /// <summary>
    /// 本类中封装卡组操作的游戏逻辑高级功能
    /// 暂时将双方牌库写在此类中
    /// </summary>
    internal Player Player;

    public CardDeckInfo M_UnlockCards;
    public CardDeckInfo M_LockCards;

    public List<CardDeck> M_CardDecks;

    private CardDeck m_CurrentCardDeck;

    public CardDeck M_CurrentCardDeck
    {
        get
        {
            if (m_CurrentCardDeck == null)
            {
                if (Player == GameManager.GM.SelfPlayer)
                {
                    m_CurrentCardDeck = new CardDeck(new CardDeckInfo(new int[] {0, 1, 100, 100, 100, 101, 200, 200, 201, 300, 301, 350, 351}));
                }
                else
                {
                    m_CurrentCardDeck = new CardDeck(new CardDeckInfo(new int[] {0, 1, 100, 100, 100, 101, 200, 200, 201, 300, 301, 350, 351}));
                }
            }

            return m_CurrentCardDeck;
        }
    }

    internal CardInfo_Base DrawRetinueCard()
    {
        if (M_CurrentCardDeck.IsEmpty)
        {
            M_CurrentCardDeck.AbandonCardRecycle();
            M_CurrentCardDeck.SuffleSelf();
        }

        return M_CurrentCardDeck.FindATypeOfCard<CardInfo_Retinue>();
    }

    internal CardInfo_Base DrawTop()
    {
        if (M_CurrentCardDeck.IsEmpty)
        {
            M_CurrentCardDeck.AbandonCardRecycle();
            M_CurrentCardDeck.SuffleSelf();
        }

        return M_CurrentCardDeck.DrawCardOnTop();
    }
}