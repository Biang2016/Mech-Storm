using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CardDeckManager : MonoBehaviour {
    /// <summary>
    /// 本类中封装卡组操作的游戏逻辑高级功能
    /// 暂时将双方牌库写在此类中
    /// </summary>

    internal Player Player;

    public CardDeckInfo M_UnlockCards;
    public CardDeckInfo M_LockCards;

    public List<CardDeck> M_CardDecks;

    public CardDeck M_CurrentCardDeck;

    void Start()
    {
        if (Player == GameManager.GM.SelfPlayer) {
            M_CurrentCardDeck = new CardDeck(new CardDeckInfo(new int[] { 1, 301, 300, 100, 100, 300, 0, 200, 301, 300, 100, 100, 300, 200, 301, 300, 100, 100, 300, 200 }));
        } else {
            M_CurrentCardDeck = new CardDeck(new CardDeckInfo(new int[] { 1, 301, 300, 100, 100, 300, 0, 200, 301, 300, 100, 100, 300, 200, 301, 300, 100, 100, 300, 200 }));
        }
    }

    void Update()
    {

    }

    internal CardInfo_Base DrawRetinueCard()
    {
        if (M_CurrentCardDeck.IsEmpty) {
            M_CurrentCardDeck.AbandonCardRecycle();
            M_CurrentCardDeck.SuffleSelf();
        }
        return M_CurrentCardDeck.FindATypeOfCard<CardInfo_Retinue>();
    }

    internal CardInfo_Base DrawTop()
    {
        if (M_CurrentCardDeck.IsEmpty) {
            M_CurrentCardDeck.AbandonCardRecycle();
            M_CurrentCardDeck.SuffleSelf();
        }
        return M_CurrentCardDeck.DrawCardOnTop();
    }
}


