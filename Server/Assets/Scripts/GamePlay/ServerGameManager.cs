using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;

public class ServerGameManager
{
    public ClientProxy ClientA;
    public ClientProxy ClientB;
    public ServerPlayer CurrentPlayer;
    public ServerPlayer PlayerA;
    public ServerPlayer PlayerB;

    public ServerGameManager(ClientProxy clientA, ClientProxy clientB)
    {
        ClientA = clientA;
        ClientB = clientB;
        ClientA.MyServerGameManager = this;
        ClientB.MyServerGameManager = this;

        Thread sgmThread = new Thread(Initialized);
        sgmThread.IsBackground = true;
        sgmThread.Start();
    }

    private void Initialized()
    {
        ServerLog.Print("StartGameSuccess! Between: " + ClientA.ClientId + " and " + ClientB.ClientId);
        PlayerA = new ServerPlayer(ClientA.ClientId, ClientB.ClientId, 0, 1, this);
        PlayerA.MyCardDeckManager.M_CurrentCardDeck = new CardDeck(ClientA.CardDeckInfo);
        PlayerA.MyClientProxy = ClientA;
        ClientA.MyServerPlayer = PlayerA;
        PlayerB = new ServerPlayer(ClientB.ClientId, ClientA.ClientId, 0, 1, this);
        PlayerB.MyCardDeckManager.M_CurrentCardDeck = new CardDeck(ClientB.CardDeckInfo);
        PlayerB.MyClientProxy = ClientB;
        ClientB.MyServerPlayer = PlayerB;

        PlayerA.MyEnemyPlayer = PlayerB;
        PlayerB.MyEnemyPlayer = PlayerA;

        PlayerRequest request1 = new PlayerRequest(ClientA.ClientId, 0, 1);
        BroadcastBothPlayers(request1);

        PlayerRequest request2 = new PlayerRequest(ClientB.ClientId, 0, 1);
        BroadcastBothPlayers(request2);

        GameBegin();
    }

    void GameBegin()
    {
        CurrentPlayer = new Random().Next(0, 2) == 0 ? PlayerA : PlayerB;
        PlayerTurnRequest request = new PlayerTurnRequest(CurrentPlayer.ClientId);
        BroadcastBothPlayers(request);

        //发英雄牌
        CurrentPlayer.MyHandManager.GetACardByID(99);
        //抽随从牌
        CurrentPlayer.MyHandManager.DrawRetinueCard();
        OnSwitchPlayer();

        //发英雄牌
        CurrentPlayer.MyHandManager.GetACardByID(99);
        //抽随从牌
        CurrentPlayer.MyHandManager.DrawRetinueCard();
        OnSwitchPlayer();

        CurrentPlayer.MyHandManager.DrawCards(GamePlaySettings.FirstDrawCard);
        EndRound();
        OnSwitchPlayer();
        CurrentPlayer.MyHandManager.DrawCards(GamePlaySettings.SecondDrawCard);
        EndRound();
        OnSwitchPlayer();
        OnBeginRound();
        OnDrawCardPhase();
    }

    void OnGameBigin()
    {
    }

    void OnSwitchPlayer()
    {
        CurrentPlayer = CurrentPlayer == PlayerA ? PlayerB : PlayerA;
        PlayerTurnRequest request = new PlayerTurnRequest(CurrentPlayer.ClientId);
        BroadcastBothPlayers(request);
    }

    void OnBeginRound()
    {
        CurrentPlayer.IncreaseCostMax(GamePlaySettings.CostIncrease);
        CurrentPlayer.AddAllCost();
        CurrentPlayer.MyHandManager.BeginRound();
        CurrentPlayer.MyBattleGroundManager.BeginRound();
    }

    void OnDrawCardPhase()
    {
        CurrentPlayer.MyHandManager.DrawCards(GamePlaySettings.DrawCardPerRound);
    }

    public void EndRound()
    {
        OnEndRound();
        OnSwitchPlayer();
        OnBeginRound();
        OnDrawCardPhase();
    }

    void OnEndRound()
    {
        CurrentPlayer.MyHandManager.EndRound();
        CurrentPlayer.MyBattleGroundManager.EndRound();
    }


    #region Utils

    private void BroadcastBothPlayers(ServerRequestBase r)
    {
        PlayerA.MyClientProxy.SendRequestsQueue.Enqueue(r);
        PlayerB.MyClientProxy.SendRequestsQueue.Enqueue(r);
    }

    #endregion
}