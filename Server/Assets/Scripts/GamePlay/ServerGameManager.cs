using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;

public class ServerGameManager
{
    public ClientAndCardDeckInfo ClientA;
    public ClientAndCardDeckInfo ClientB;
    public ServerPlayer CurrentPlayer;
    public ServerPlayer PlayerA;
    public ServerPlayer PlayerB;

    public ServerGameManager(ClientAndCardDeckInfo clientA, ClientAndCardDeckInfo clientB)
    {
        ClientA = clientA;
        ClientB = clientB;
    }

    private bool isInitialized = false;
    private bool isPlayerAReady_Initialized = false;
    private bool isPlayerBReady_Initialized = false;

    public void TryInitialized(int ReadyClientId)
    {
        if (isInitialized) return;
        if (ReadyClientId == ClientA.ClientId) isPlayerAReady_Initialized = true;
        if (ReadyClientId == ClientB.ClientId) isPlayerBReady_Initialized = true;
        if (isPlayerAReady_Initialized && isPlayerBReady_Initialized)
        {
            Thread sgmThread = new Thread(Initialized);
            sgmThread.IsBackground = true;
            sgmThread.Start();
        }
    }

    private void Initialized()
    {
        isInitialized = true;
        ServerLog.Print("StartGameSuccess! Between: " + ClientA.ClientId + " and " + ClientB.ClientId);
        PlayerA = new ServerPlayer(ClientA.ClientId, ClientB.ClientId, 0, 1, this);
        PlayerA.MyCardDeckManager.M_CurrentCardDeck = new CardDeck(ClientA.CardDeckInfo);
        PlayerB = new ServerPlayer(ClientB.ClientId, ClientA.ClientId, 0, 1, this);
        PlayerB.MyCardDeckManager.M_CurrentCardDeck = new CardDeck(ClientB.CardDeckInfo);

        PlayerRequest request1 = new PlayerRequest(ClientA.ClientId, 0, 1);
        BroadcastBothPlayers(request1);

        PlayerRequest request2 = new PlayerRequest(ClientB.ClientId, 0, 1);
        BroadcastBothPlayers(request2);
    }


    private bool isGameBegin = false;
    private bool isPlayerAReady_GameBegin = false;
    private bool isPlayerBReady_GameBegin = false;

    public void TryGameBegin(int clientId)
    {
        if (isGameBegin) return;
        if (clientId == ClientA.ClientId) isPlayerAReady_GameBegin = true;
        if (clientId == ClientB.ClientId) isPlayerBReady_GameBegin = true;
        if (isPlayerAReady_GameBegin && isPlayerBReady_GameBegin) GameBegin();
    }

    void GameBegin()
    {
        isGameBegin = true;
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

    private void BroadcastBothPlayers(Request r)
    {
        Server.SV.SendMessageToClientId(r, PlayerA.ClientId);
        Server.SV.SendMessageToClientId(r, PlayerB.ClientId);
    }

    #endregion
}