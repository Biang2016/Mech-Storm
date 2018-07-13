using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;

internal class ServerGameManager
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

    private bool isStopped = false;

    public void OnStopGame(ClientProxy clientProxy)
    {
        if (isStopped) return;
        ClientA.ClientState = ProxyBase.ClientStates.SubmitCardDeck;
        ClientB.ClientState = ProxyBase.ClientStates.SubmitCardDeck;

        GameStopByLeaveRequest request = new GameStopByLeaveRequest(clientProxy.ClientId);
        BroadcastBothPlayers(request);

        Server.SV.SGMM.RemoveGame(this, ClientA, ClientB);

        PlayerA?.OnDestroyed();
        PlayerB?.OnDestroyed();

        isStopped = true;
    }

    private void Initialized()
    {
        ClientA.ClientState = ProxyBase.ClientStates.Playing;
        ClientB.ClientState = ProxyBase.ClientStates.Playing;

        ServerLog.Print("StartGameSuccess! Between: " + ClientA.ClientId + " and " + ClientB.ClientId);
        PlayerA = new ServerPlayer(ClientA.ClientId, ClientB.ClientId, 0, 0, this);
        PlayerA.MyCardDeckManager.M_CurrentCardDeck = new CardDeck(ClientA.CardDeckInfo);
        PlayerA.MyClientProxy = ClientA;
        PlayerB = new ServerPlayer(ClientB.ClientId, ClientA.ClientId, 0, 0, this);
        PlayerB.MyCardDeckManager.M_CurrentCardDeck = new CardDeck(ClientB.CardDeckInfo);
        PlayerB.MyClientProxy = ClientB;

        PlayerA.MyEnemyPlayer = PlayerB;
        PlayerB.MyEnemyPlayer = PlayerA;

        PlayerRequest request1 = new PlayerRequest(ClientA.ClientId, 0, GamePlaySettings.BeginCost);
        BroadcastBothPlayers(request1);

        PlayerRequest request2 = new PlayerRequest(ClientB.ClientId, 0, GamePlaySettings.BeginCost);
        BroadcastBothPlayers(request2);

        GameBegin();
    }

    void GameBegin()
    {
        CurrentPlayer = new Random().Next(0, 2) == 0 ? PlayerA : PlayerB;
        bool isPlayerAFirst = CurrentPlayer == PlayerA;
        //发英雄牌
        PlayerA.MyHandManager.GetACardByID(99);
        //抽随从牌
        PlayerA.MyHandManager.DrawRetinueCard();

        //发英雄牌
        PlayerB.MyHandManager.GetACardByID(99);
        //抽随从牌
        PlayerB.MyHandManager.DrawRetinueCard();

        PlayerTurnRequest request = new PlayerTurnRequest(CurrentPlayer.ClientId);
        BroadcastBothPlayers(request);

        if (isPlayerAFirst)
        {
            PlayerA.MyHandManager.DrawCards(GamePlaySettings.FirstDrawCard);
            PlayerB.MyHandManager.DrawCards(GamePlaySettings.SecondDrawCard);
        }
        else
        {
            PlayerB.MyHandManager.DrawCards(GamePlaySettings.FirstDrawCard);
            PlayerA.MyHandManager.DrawCards(GamePlaySettings.SecondDrawCard);
        }

        OnBeginRound();
        OnDrawCardPhase();
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

    public void OnClientSummonRetinueRequest(SummonRetinueRequest summonRetinueRequest)
    {
        ServerPlayer sp = summonRetinueRequest.clientId == PlayerA.ClientId ? PlayerA : PlayerB;
        if (sp.MyBattleGroundManager.BattleGroundIsFull)
        {
            return;
        }
        else
        {
            if (sp.MyBattleGroundManager.SummonRetinue(summonRetinueRequest))
            {
                sp.MyHandManager.DropCardAt(summonRetinueRequest.handCardIndex);
            }

            sp.UseCostAboveZero(summonRetinueRequest.cardInfo.Cost);
            SummonRetinueRequest_Response request = new SummonRetinueRequest_Response(summonRetinueRequest.clientId, summonRetinueRequest.cardInfo, summonRetinueRequest.handCardIndex, summonRetinueRequest.battleGroundIndex);
            BroadcastBothPlayers(request);
        }
    }

    public void OnEndRoundRequest(EndRoundRequest r)
    {
        if (CurrentPlayer.ClientId == r.clientId)
        {
            EndRound();
        }
    }

    public void EndRound()
    {
        CurrentPlayer.MyHandManager.EndRound();
        CurrentPlayer.MyBattleGroundManager.EndRound();
        OnSwitchPlayer();
        OnBeginRound();
        OnDrawCardPhase();
    }


    #region Utils

    private void BroadcastBothPlayers(ServerRequestBase r)
    {
        PlayerA.MyClientProxy.SendMessage(r);
        PlayerB.MyClientProxy.SendMessage(r);
    }

    #endregion
}