using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

internal class ServerGameManager
{
    public ClientProxy ClientA;
    public ClientProxy ClientB;
    public ServerPlayer CurrentPlayer;
    public ServerPlayer IdlePlayer;
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

    #region 游戏初始化

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

        ClientA.CurrentClientRequestResponse = new GameStart_Response();
        ClientB.CurrentClientRequestResponse = new GameStart_Response();

        SetPlayerRequest request1 = new SetPlayerRequest(ClientA.ClientId, 0, GamePlaySettings.BeginCost);
        Broadcast_AddRequestToOperationResponse(request1);
        SetPlayerRequest request2 = new SetPlayerRequest(ClientB.ClientId, 0, GamePlaySettings.BeginCost);
        Broadcast_AddRequestToOperationResponse(request2);

        GameBegin();

        Broadcast_SendOperationResponse(); //初始化的大包请求
    }

    void GameBegin()
    {
        CurrentPlayer = new Random().Next(0, 2) == 0 ? PlayerA : PlayerB;
        IdlePlayer = CurrentPlayer == PlayerA ? PlayerB : PlayerA;
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
        Broadcast_AddRequestToOperationResponse(request);

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

    #endregion

    #region 回合中的基础步骤

    void OnSwitchPlayer()
    {
        CurrentPlayer = CurrentPlayer == PlayerA ? PlayerB : PlayerA;
        IdlePlayer = IdlePlayer == PlayerA ? PlayerB : PlayerA;
        PlayerTurnRequest request = new PlayerTurnRequest(CurrentPlayer.ClientId);
        Broadcast_AddRequestToOperationResponse(request);
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
        CurrentPlayer.MyHandManager.EndRound();
        CurrentPlayer.MyBattleGroundManager.EndRound();
        OnSwitchPlayer();
        OnBeginRound();
        OnDrawCardPhase();
    }

    #endregion

    #region ClientOperationResponses

    public void OnClientSummonRetinueRequest(SummonRetinueRequest r)
    {
        ClientA.CurrentClientRequestResponse = new SummonRetinueRequest_Response();
        ClientB.CurrentClientRequestResponse = new SummonRetinueRequest_Response();

        ServerPlayer sp = GetPlayerByClientId(r.clientId);
        sp.MyBattleGroundManager.AddRetinue(r.cardInfo, r.battleGroundIndex);
        sp.MyHandManager.UseCardAt(r.handCardIndex);
        sp.UseCostAboveZero(r.cardInfo.BaseInfo.Cost);

        Broadcast_SendOperationResponse();
    }


    public void OnClientEquipWeaponRequest(EquipWeaponRequest r)
    {
        ClientA.CurrentClientRequestResponse = new EquipWeaponRequest_Response();
        ClientB.CurrentClientRequestResponse = new EquipWeaponRequest_Response();

        ServerPlayer sp = GetPlayerByClientId(r.clientId);
        sp.MyBattleGroundManager.EquipWeapon(r);
        sp.MyHandManager.UseCardAt(r.handCardIndex);
        sp.UseCostAboveZero(r.cardInfo.BaseInfo.Cost);

        Broadcast_SendOperationResponse();
    }

    public void OnClientEquipShieldRequest(EquipShieldRequest r)
    {
        ClientA.CurrentClientRequestResponse = new EquipShieldRequest_Response();
        ClientB.CurrentClientRequestResponse = new EquipShieldRequest_Response();

        ServerPlayer sp = GetPlayerByClientId(r.clientId);
        sp.MyBattleGroundManager.EquipShield(r);
        sp.MyHandManager.UseCardAt(r.handCardIndex);
        sp.UseCostAboveZero(r.cardInfo.BaseInfo.Cost);

        Broadcast_SendOperationResponse();
    }

    public void OnClientRetinueAttackRetinueRequest(RetinueAttackRetinueRequest r)
    {
        ClientA.CurrentClientRequestResponse = new RetinueAttackRetinueRequest_Response();
        ClientB.CurrentClientRequestResponse = new RetinueAttackRetinueRequest_Response();

        RetinueAttackRetinueServerRequest request = new RetinueAttackRetinueServerRequest(r.AttackRetinueClientId, r.AttackRetinuePlaceIndex, r.BeAttackedRetinueClientId, r.BeAttackedRetinuePlaceIndex);
        Broadcast_AddRequestToOperationResponse(request);

        ServerModuleRetinue attackRetinue = GetPlayerByClientId(r.AttackRetinueClientId).MyBattleGroundManager.GetRetinue(r.AttackRetinuePlaceIndex);
        ServerModuleRetinue beAttackedRetinue = GetPlayerByClientId(r.BeAttackedRetinueClientId).MyBattleGroundManager.GetRetinue(r.BeAttackedRetinuePlaceIndex);

        List<int> attackSeries = attackRetinue.AllModulesAttack();
        foreach (int attack in attackSeries)
        {
            beAttackedRetinue.BeAttacked(attack);
        }

        Broadcast_SendOperationResponse();
    }

    public void OnEndRoundRequest(EndRoundRequest r)
    {
        ClientA.CurrentClientRequestResponse = new EndRoundRequest_Response();
        ClientB.CurrentClientRequestResponse = new EndRoundRequest_Response();

        if (CurrentPlayer.ClientId == r.clientId)
        {
            EndRound();
        }

        Broadcast_SendOperationResponse();
    }

    private bool isStopped = false;

    public void OnLeaveGameRequest(LeaveGameRequest r)
    {
        OnStopGame(GetClientProxyByClientId(r.clientId));
    }

    public void OnStopGame(ClientProxy clientProxy)
    {
        if (isStopped) return;
        ClientA.ClientState = ProxyBase.ClientStates.SubmitCardDeck;
        ClientB.ClientState = ProxyBase.ClientStates.SubmitCardDeck;

        GameStopByLeaveRequest request = new GameStopByLeaveRequest(clientProxy.ClientId);
        BroadcastRequest(request);

        Server.SV.SGMM.RemoveGame(this, ClientA, ClientB);

        PlayerA?.OnDestroyed();
        PlayerB?.OnDestroyed();

        isStopped = true;
    }

    #endregion


    #region SideEffects

    public void SE_SummonRetinue(ServerPlayer invokePlayer, string player, int cardId, int retinuePlaceIndex)
    {
        CardInfo_Retinue retinueCardInfo = (CardInfo_Retinue) AllCards.GetCard(cardId);
        switch (player)
        {
            case "self":
            {
                invokePlayer.MyBattleGroundManager.AddRetinue(retinueCardInfo, retinuePlaceIndex);
                break;
            }
            case "enemy":
            {
                invokePlayer.MyEnemyPlayer.MyBattleGroundManager.AddRetinue(retinueCardInfo, retinuePlaceIndex);
                break;
            }
            case "all":
            {
                invokePlayer.MyBattleGroundManager.AddRetinue(retinueCardInfo, retinuePlaceIndex);
                invokePlayer.MyEnemyPlayer.MyBattleGroundManager.AddRetinue(retinueCardInfo, retinuePlaceIndex);
                break;
            }
        }
    }

    public void SE_EquipWeapon(EquipWeaponRequest r)
    {
        //Todo
    }

    public void SE_EquipShield(EquipShieldRequest r)
    {
        //Todo
    }

    public void SE_KillAllInBattleGround(ServerPlayer invokePlayer, string player) //杀死清场
    {
        switch (player)
        {
            case "self":
            {
                invokePlayer.MyBattleGroundManager.KillAllInBattleGround();
                break;
            }
            case "enemy":
            {
                invokePlayer.MyEnemyPlayer.MyBattleGroundManager.KillAllInBattleGround();
                break;
            }
            case "all":
            {
                invokePlayer.MyBattleGroundManager.KillAllInBattleGround();
                invokePlayer.MyEnemyPlayer.MyBattleGroundManager.KillAllInBattleGround();
                break;
            }
        }
    }

    public void SE_AddLifeForSomeRetinue(ServerPlayer invokePlayer, string player, int retinuePlaceIndex, int value) //增加某随从生命
    {
        switch (player)
        {
            case "self":
            {
                invokePlayer.MyBattleGroundManager.AddLifeForSomeRetinue(retinuePlaceIndex, value);
                break;
            }
            case "enemy":
            {
                invokePlayer.MyEnemyPlayer.MyBattleGroundManager.AddLifeForSomeRetinue(retinuePlaceIndex, value);
                break;
            }
            case "all":
            {
                invokePlayer.MyBattleGroundManager.AddLifeForSomeRetinue(retinuePlaceIndex, value);
                invokePlayer.MyEnemyPlayer.MyBattleGroundManager.AddLifeForSomeRetinue(retinuePlaceIndex, value);
                break;
            }
        }
    }

    #endregion


    #region Utils

    public void Broadcast_AddRequestToOperationResponse(ServerRequestBase request)
    {
        ClientA.CurrentClientRequestResponse.SideEffects.Add(request);
        ClientB.CurrentClientRequestResponse.SideEffects.Add(request);
    }

    private void Broadcast_SendOperationResponse()
    {
        ClientA.SendMessage(ClientA.CurrentClientRequestResponse);
        ClientB.SendMessage(ClientB.CurrentClientRequestResponse);
        ClientA.CurrentClientRequestResponse = null;
        ClientB.CurrentClientRequestResponse = null;
    }

    private void BroadcastRequest(ServerRequestBase request)
    {
        ClientA.SendMessage(request);
        ClientB.SendMessage(request);
    }

    public ClientProxy GetClientProxyByClientId(int clientId)
    {
        if (ClientA.ClientId == clientId)
        {
            return ClientA;
        }
        else if (ClientB.ClientId == clientId)
        {
            return ClientB;
        }

        return null;
    }

    public ClientProxy GetEnemyClientProxyByClientId(int clientId)
    {
        if (ClientA.ClientId == clientId)
        {
            return ClientB;
        }
        else if (ClientB.ClientId == clientId)
        {
            return ClientA;
        }

        return null;
    }

    public ServerPlayer GetPlayerByClientId(int clientId)
    {
        if (PlayerA.ClientId == clientId)
        {
            return PlayerA;
        }
        else if (PlayerB.ClientId == clientId)
        {
            return PlayerB;
        }

        return null;
    }

    #endregion
}