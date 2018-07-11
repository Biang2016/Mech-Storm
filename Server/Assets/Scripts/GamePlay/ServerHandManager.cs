using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ServerHandManager
{
    public ServerPlayer ServerPlayer;
    private int cardNumber = 0; //手牌数
    List<ServerCardBase> cards = new List<ServerCardBase>();

    public ServerHandManager(ServerPlayer serverPlayer)
    {
        ServerPlayer = serverPlayer;
    }

    internal void DrawCards(int number)
    {
        for (int i = 0; i < number; i++)
        {
            DrawCard();
        }
    }

    internal void DrawCard()
    {
        if (cardNumber >= GamePlaySettings.MaxHandCard)
        {
            //无法抽牌
        }
        else
        {
            CardInfo_Base newCardInfo = ServerPlayer.MyCardDeckManager.DrawTop();
            if (newCardInfo == null)
            {
                ServerLog.Print("No Card");
                return;
            }

            ServerCardBase newCard = ServerCardBase.InstantiateCardByCardInfo(newCardInfo, ServerPlayer);
            cards.Add(newCard);
            cardNumber++;
        }
    }

    internal void GetACardByID(int cardID)
    {
        CardInfo_Base cardInfo = AllCards.GetCard(cardID);
        ServerCardBase newCard = ServerCardBase.InstantiateCardByCardInfo(cardInfo, ServerPlayer);
        ServerPlayer.MyCardDeckManager.OnPlayerGetCard(cardID);
        cards.Add(newCard);
        cardNumber++;
    }

    internal void DrawRetinueCard()
    {
        CardInfo_Base newCardInfo = ServerPlayer.MyCardDeckManager.DrawRetinueCard();
        if (newCardInfo == null)
        {
            ServerLog.Print("No Card");
            return;
        }

        ServerCardBase newCard = ServerCardBase.InstantiateCardByCardInfo(newCardInfo, ServerPlayer);
        cards.Add(newCard);
        cardNumber++;
    }

    internal void DropFirstCard()
    {
        DropCard(cards[0]);
    }

    internal void DropCard(ServerCardBase dropCard)
    {
        cards.Remove(dropCard);
        cardNumber--;
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
}