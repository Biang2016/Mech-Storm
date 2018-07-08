﻿using System;
using System.Collections;

public class ServerGameManager
{
    public ClientAndCardDeckInfo ClientA;
    public ClientAndCardDeckInfo ClientB;
    public ServerPlayer PlayerA;
    public ServerPlayer PlayerB;

    public ServerGameManager(ClientAndCardDeckInfo clientA, ClientAndCardDeckInfo clientB)
    {
        ClientA = clientA;
        ClientB = clientB;
    }

    public void StartGame()
    {
        ServerLog.Print("StartGameSuccess! Between: " + ClientA.ClientId + " and " + ClientB.ClientId);
        InitializePlayers();
    }

    public ServerPlayer CurrentPlayer;

    private void InitializePlayers()
    {
        PlayerA = new ServerPlayer(ClientA.ClientId, ClientB.ClientId, 0, 1, this);
        PlayerA.MyCardDeckManager.M_CurrentCardDeck = new CardDeck(ClientA.CardDeckInfo);
        PlayerB = new ServerPlayer(ClientB.ClientId, ClientA.ClientId, 0, 1, this);
        PlayerB.MyCardDeckManager.M_CurrentCardDeck = new CardDeck(ClientB.CardDeckInfo);

        PlayerRequest request1 = new PlayerRequest(ClientA.ClientId, 0, 1);
        BroadcastBothPlayers(request1);

        PlayerRequest request2 = new PlayerRequest(ClientB.ClientId, 0, 1);
        BroadcastBothPlayers(request2);

        GameBegin();
    }


    void switchPlayer()
    {
        CurrentPlayer = CurrentPlayer == PlayerA ? PlayerB : PlayerA;
        PlayerTurnRequest request = new PlayerTurnRequest(CurrentPlayer.ClientId);
        BroadcastBothPlayers(request);
    }

    public void GameBegin()
    {
        CurrentPlayer = new Random().Next(0, 2) == 0 ? PlayerA : PlayerB;
        PlayerTurnRequest request = new PlayerTurnRequest(CurrentPlayer.ClientId);
        BroadcastBothPlayers(request);

        OnGameBigin();
        OnGameBigin();

        CurrentPlayer.MyHandManager.DrawCards(GamePlaySettings.FirstDrawCard);
        EndRound();
        switchPlayer();
        CurrentPlayer.MyHandManager.DrawCards(GamePlaySettings.SecondDrawCard);
        EndRound();
        switchPlayer();
        BeginRound();
        DrawCardPhase();
    }

    private void OnGameBigin()
    {
        //发英雄牌
        CurrentPlayer.MyHandManager.GetACardByID(99);

        //抽随从牌
        CurrentPlayer.MyHandManager.DrawRetinueCard();

        switchPlayer();
    }

    public void BeginRound()
    {
        CurrentPlayer.IncreaseCostMax(GamePlaySettings.CostIncrease);
        CurrentPlayer.AddAllCost();
        CurrentPlayer.MyHandManager.BeginRound();
        CurrentPlayer.MyBattleGroundManager.BeginRound();
    }

    public void DrawCardPhase()
    {
        CurrentPlayer.MyHandManager.DrawCards(GamePlaySettings.DrawCardPerRound);
    }

    public void EndRound()
    {
        CurrentPlayer.MyHandManager.EndRound();
        CurrentPlayer.MyBattleGroundManager.EndRound();
    }

    public void OnEndRound()
    {
        EndRound();
        switchPlayer();
        BeginRound();
        DrawCardPhase();
    }

    #region Utils

    private void BroadcastBothPlayers(Request r)
    {
        Server.SV.SendMessageToClientId(r, PlayerA.ClientId);
        Server.SV.SendMessageToClientId(r, PlayerB.ClientId);
    }

    #endregion
}