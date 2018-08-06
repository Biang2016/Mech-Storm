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

    private int gameRetinueIdGenerator = 0;

    public int GeneratorNewRetinueId()
    {
        return gameRetinueIdGenerator++;
    }

    private int gameCardInstanceIdGenerator = 0;

    public int GeneratorNewCardInstanceId()
    {
        return gameCardInstanceIdGenerator++;
    }

    private void Initialized()
    {
        ClientA.ClientState = ProxyBase.ClientStates.Playing;
        ClientB.ClientState = ProxyBase.ClientStates.Playing;

        ServerLog.Print("StartGameSuccess! Between: " + ClientA.ClientId + " and " + ClientB.ClientId);

        PlayerA = new ServerPlayer(ClientA.ClientId, ClientB.ClientId, 0, GamePlaySettings.BeginCost, this);
        PlayerA.MyCardDeckManager.M_CurrentCardDeck = new CardDeck(ClientA.CardDeckInfo, PlayerA.OnCardDeckLeftChange);
        PlayerA.MyClientProxy = ClientA;

        PlayerB = new ServerPlayer(ClientB.ClientId, ClientA.ClientId, 0, GamePlaySettings.BeginCost, this);
        PlayerB.MyCardDeckManager.M_CurrentCardDeck = new CardDeck(ClientB.CardDeckInfo, PlayerB.OnCardDeckLeftChange);
        PlayerB.MyCardDeckManager.M_CurrentCardDeck.CardDeckCountChangeHandler += PlayerB.OnCardDeckLeftChange;
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

        PlayerTurnRequest request = new PlayerTurnRequest(CurrentPlayer.ClientId);
        Broadcast_AddRequestToOperationResponse(request);

        foreach (CardInfo_Base cardInfo in PlayerA.MyCardDeckManager.M_CurrentCardDeck.BeginRetinueCards)
        {
            PlayerA.MyBattleGroundManager.AddRetinue((CardInfo_Retinue)cardInfo);
        }
        foreach (CardInfo_Base cardInfo in PlayerB.MyCardDeckManager.M_CurrentCardDeck.BeginRetinueCards)
        {
            PlayerB.MyBattleGroundManager.AddRetinue((CardInfo_Retinue)cardInfo);
        }

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
        CurrentPlayer.MyCardDeckManager.BeginRound();
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
        CardInfo_Retinue info = (CardInfo_Retinue) sp.MyHandManager.GetHandCardInfo(r.handCardInstanceId);
        sp.MyHandManager.UseCard(r.handCardInstanceId, r.lastDragPosition);
        sp.MyBattleGroundManager.AddRetinue(info, r.battleGroundIndex);

        Broadcast_SendOperationResponse();
    }


    public void OnClientEquipWeaponRequest(EquipWeaponRequest r)
    {
        ClientA.CurrentClientRequestResponse = new EquipWeaponRequest_Response();
        ClientB.CurrentClientRequestResponse = new EquipWeaponRequest_Response();

        ServerPlayer sp = GetPlayerByClientId(r.clientId);
        CardInfo_Base cardInfo = sp.MyHandManager.GetHandCardInfo(r.handCardInstanceId);
        sp.MyHandManager.UseCard(r.handCardInstanceId, r.lastDragPosition);
        sp.MyBattleGroundManager.EquipWeapon(r, cardInfo);

        Broadcast_SendOperationResponse();
    }

    public void OnClientEquipShieldRequest(EquipShieldRequest r)
    {
        ClientA.CurrentClientRequestResponse = new EquipShieldRequest_Response();
        ClientB.CurrentClientRequestResponse = new EquipShieldRequest_Response();

        ServerPlayer sp = GetPlayerByClientId(r.clientId);
        CardInfo_Base cardInfo = sp.MyHandManager.GetHandCardInfo(r.handCardInstanceId);
        sp.MyHandManager.UseCard(r.handCardInstanceId, r.lastDragPosition);
        sp.MyBattleGroundManager.EquipShield(r, cardInfo);

        Broadcast_SendOperationResponse();
    }

    public void OnClientRetinueAttackRetinueRequest(RetinueAttackRetinueRequest r)
    {
        ClientA.CurrentClientRequestResponse = new RetinueAttackRetinueRequest_Response();
        ClientB.CurrentClientRequestResponse = new RetinueAttackRetinueRequest_Response();

        RetinueAttackRetinueServerRequest request = new RetinueAttackRetinueServerRequest(r.AttackRetinueClientId, r.AttackRetinueId, r.BeAttackedRetinueClientId, r.BeAttackedRetinueId);
        Broadcast_AddRequestToOperationResponse(request);

        ServerPlayer cpat = GetPlayerByClientId(r.AttackRetinueClientId);
        ServerPlayer cpba = GetPlayerByClientId(r.BeAttackedRetinueClientId);

        ServerModuleRetinue attackRetinue = cpat.MyBattleGroundManager.GetRetinue(r.AttackRetinueId);
        ServerModuleRetinue beAttackedRetinue = cpba.MyBattleGroundManager.GetRetinue(r.BeAttackedRetinueId);

        attackRetinue.Attack(beAttackedRetinue, false);
 
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
        if (ClientA == null) ServerLog.Print(ClientA.ClientId + "   ClientA==null");
        ClientA.ClientState = ProxyBase.ClientStates.SubmitCardDeck;
        if (ClientB == null) ServerLog.Print(ClientB.ClientId + "   ClientB==null");
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

    Queue<SideEffectBase> SideEffectQueue = new Queue<SideEffectBase>();

    public void EnqueueSideEffect(SideEffectBase se)
    {
        SideEffectQueue.Enqueue(se);
    }

    public void ExecuteAllSideEffects()
    {
        while (SideEffectQueue.Count > 0)
        {
            SideEffectBase se = SideEffectQueue.Dequeue();
            se.Excute(se.Player);
        }

        SendAllDieInfos();
    }

    List<int> DieRetinueList = new List<int>();

    public void AddDieTogatherRetinuesInfo(int dieRetinueId)
    {
        DieRetinueList.Add(dieRetinueId);
    }

    public void SendAllDieInfos()
    {
        if (DieRetinueList.Count == 0) return;
        List<int> tmp = new List<int>();
        foreach (int id in DieRetinueList)
        {
            tmp.Add(id);
        }

        RetinueDieRequest request1 = new RetinueDieRequest(tmp);
        Broadcast_AddRequestToOperationResponse(request1);
        BattleGroundRemoveRetinueRequest request2 = new BattleGroundRemoveRetinueRequest(tmp);
        Broadcast_AddRequestToOperationResponse(request2);
        DieRetinueList.Clear();
    }

    public int DamageTogatherRequestId;

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