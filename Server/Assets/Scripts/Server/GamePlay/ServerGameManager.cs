using UnityEngine;
using System.Collections;

public class ServerGameManager
{
    private int PlayerAClientId;
    private int PlayerBClientId;
    public ServerPlayer PlayerA;
    public ServerPlayer PlayerB;

    public ServerGameManager(int clientId1, int clientId2)
    {
        PlayerAClientId = clientId1;
        PlayerBClientId = clientId2;
    }

    public void StartGame()
    {
        Debug.Log("StartGameSuccess! Between: " + PlayerAClientId + " and " + PlayerBClientId);
        InitializePlayers();
    }

    private ServerPlayer CurrentPlayer;

    private void InitializePlayers()
    {
        PlayerRequest request1 = new PlayerRequest(PlayerAClientId, 0, 20);
        BroadcastBothPlayers(request1);
        PlayerA = new ServerPlayer(PlayerAClientId, 0, 20);
        PlayerRequest request2 = new PlayerRequest(PlayerBClientId, 0, 20);
        BroadcastBothPlayers(request2);
        PlayerB = new ServerPlayer(PlayerAClientId, 0, 20);
        CurrentPlayer = Random.Range(0, 2) == 0 ? PlayerA : PlayerB;
    }

    void switchPlayer()
    {
        CurrentPlayer = CurrentPlayer == PlayerA ? PlayerB : PlayerA;
    }

    public void GameBegin()
    {
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

        //分配卡组
        CardDeckInfo cardDeckInfo = new CardDeckInfo(new int[] {0, 1, 100, 101, 200, 201, 300, 301}); //Todo
        CurrentPlayer.MyCardDeckManager.M_CurrentCardDeck = new CardDeck(cardDeckInfo);

        //抽随从牌
        CurrentPlayer.MyCardDeckManager.DrawRetinueCard();

        switchPlayer();
    }

    public void BeginRound()
    {
        CurrentPlayer.IncreaseCostMax(GamePlaySettings.CostIncrease);
        CurrentPlayer.AddAllCost();
        PlayerTurnRequest request = new PlayerTurnRequest(CurrentPlayer.ClientId);
        BroadcastBothPlayers(request);
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

    #region Utils

    private void BroadcastBothPlayers(Request r)
    {
        Server.SV.SendMessage(r, PlayerAClientId);
        Server.SV.SendMessage(r, PlayerBClientId);
    }

    #endregion
}