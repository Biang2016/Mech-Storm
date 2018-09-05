﻿using System;
using System.Collections.Generic;
using System.Threading;

internal class ServerGameManager
{
    public ClientProxy ClientA;
    public ClientProxy ClientB;
    public ServerPlayer CurrentPlayer;
    public ServerPlayer IdlePlayer;
    public ServerPlayer PlayerA;
    public ServerPlayer PlayerB;

    public RandomNumberGenerator RandomNumberGenerator;

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

        SyncRandomNumber();

        int PA_LIFE = ClientA.CurrentBuildInfo.Life;
        int PA_MAGIC = ClientA.CurrentBuildInfo.Energy;
        int PB_LIFE = ClientB.CurrentBuildInfo.Life;
        int PB_MAGIC = ClientB.CurrentBuildInfo.Energy;

        PlayerA = new ServerPlayer(ClientA.UserName, ClientA.ClientId, ClientB.ClientId, 0, GamePlaySettings.BeginMetal, PA_LIFE, PA_LIFE, PA_MAGIC, PA_MAGIC, this);
        PlayerA.MyCardDeckManager.M_CurrentCardDeck = new CardDeck(ClientA.CurrentBuildInfo, PlayerA.OnCardDeckLeftChange);
        PlayerA.MyClientProxy = ClientA;

        PlayerB = new ServerPlayer(ClientB.UserName, ClientB.ClientId, ClientA.ClientId, 0, GamePlaySettings.BeginMetal, PB_LIFE, PB_LIFE, PB_MAGIC, PB_MAGIC, this);
        PlayerB.MyCardDeckManager.M_CurrentCardDeck = new CardDeck(ClientB.CurrentBuildInfo, PlayerB.OnCardDeckLeftChange);
        PlayerB.MyCardDeckManager.M_CurrentCardDeck.CardDeckCountChangeHandler += PlayerB.OnCardDeckLeftChange;
        PlayerB.MyClientProxy = ClientB;

        PlayerA.MyEnemyPlayer = PlayerB;
        PlayerB.MyEnemyPlayer = PlayerA;

        ClientA.CurrentClientRequestResponseBundle = new GameStart_ResponseBundle();
        ClientB.CurrentClientRequestResponseBundle = new GameStart_ResponseBundle();

        SetPlayerRequest request1 = new SetPlayerRequest(ClientA.UserName, ClientA.ClientId, 0, GamePlaySettings.BeginMetal, PA_LIFE, PA_LIFE, PA_MAGIC, PA_MAGIC);
        Broadcast_AddRequestToOperationResponse(request1);
        SetPlayerRequest request2 = new SetPlayerRequest(ClientA.UserName, ClientB.ClientId, 0, GamePlaySettings.BeginMetal, PB_LIFE, PB_LIFE, PB_MAGIC, PB_MAGIC);
        Broadcast_AddRequestToOperationResponse(request2);

        GameBegin();

        Broadcast_SendOperationResponse(); //初始化的大包请求
    }

    private void SyncRandomNumber()
    {
        Random rd = new Random(DateTime.Now.Millisecond);
        int seed = rd.Next();
        RandomNumberGenerator = new RandomNumberGenerator(seed);

        RandomNumberSeedRequest request = new RandomNumberSeedRequest(seed);
        BroadcastRequest(request);
    }

    void GameBegin()
    {
        CurrentPlayer = new Random().Next(0, 2) == 0 ? PlayerA : PlayerB;
        IdlePlayer = CurrentPlayer == PlayerA ? PlayerB : PlayerA;
        bool isPlayerAFirst = CurrentPlayer == PlayerA;

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
        CurrentPlayer.IncreaseMetalMax(GamePlaySettings.MetalIncrease);
        CurrentPlayer.AddAllMetal();
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
        ClientA.CurrentClientRequestResponseBundle = new SummonRetinueRequest_ResponseBundle();
        ClientB.CurrentClientRequestResponseBundle = new SummonRetinueRequest_ResponseBundle();

        ServerPlayer sp = GetPlayerByClientId(r.clientId);
        CardInfo_Retinue info = (CardInfo_Retinue) sp.MyHandManager.GetHandCardInfo(r.handCardInstanceId);
        sp.MyHandManager.UseCard(r.handCardInstanceId, r.lastDragPosition);
        int targetRetinueId = r.targetRetinueId;
        if (r.isTargetRetinueIdTempId)
        {
            targetRetinueId = sp.MyBattleGroundManager.GetRetinueIdByClientRetinueTempId(r.clientRetinueTempId);
        }

        sp.MyBattleGroundManager.AddRetinue(info, r.battleGroundIndex, targetRetinueId, r.clientRetinueTempId);

        Broadcast_SendOperationResponse();
    }


    public void OnClientEquipWeaponRequest(EquipWeaponRequest r)
    {
        ClientA.CurrentClientRequestResponseBundle = new EquipWeaponRequest_ResponseBundle();
        ClientB.CurrentClientRequestResponseBundle = new EquipWeaponRequest_ResponseBundle();

        ServerPlayer sp = GetPlayerByClientId(r.clientId);
        CardInfo_Base cardInfo = sp.MyHandManager.GetHandCardInfo(r.handCardInstanceId);
        sp.MyHandManager.UseCard(r.handCardInstanceId, r.lastDragPosition);
        sp.MyBattleGroundManager.EquipWeapon(r, cardInfo);

        Broadcast_SendOperationResponse();
    }

    public void OnClientEquipShieldRequest(EquipShieldRequest r)
    {
        ClientA.CurrentClientRequestResponseBundle = new EquipShieldRequest_ResponseBundle();
        ClientB.CurrentClientRequestResponseBundle = new EquipShieldRequest_ResponseBundle();

        ServerPlayer sp = GetPlayerByClientId(r.clientId);
        CardInfo_Base cardInfo = sp.MyHandManager.GetHandCardInfo(r.handCardInstanceId);
        sp.MyHandManager.UseCard(r.handCardInstanceId, r.lastDragPosition);
        sp.MyBattleGroundManager.EquipShield(r, cardInfo);

        Broadcast_SendOperationResponse();
    }

    public void OnClientUseSpellCardRequest(UseSpellCardRequest r)
    {
        ClientA.CurrentClientRequestResponseBundle = new UseSpellCardRequset_ResponseBundle();
        ClientB.CurrentClientRequestResponseBundle = new UseSpellCardRequset_ResponseBundle();

        ServerPlayer sp = GetPlayerByClientId(r.clientId);
        CardInfo_Base cardInfo = sp.MyHandManager.GetHandCardInfo(r.handCardInstanceId);

        int targetRetinueId = r.targetRetinueId;
        if (r.isTargetRetinueIdTempId)
        {
            targetRetinueId = sp.MyBattleGroundManager.GetRetinueIdByClientRetinueTempId(r.targetRetinueId);
        }

        sp.MyHandManager.UseCard(r.handCardInstanceId, r.lastDragPosition, targetRetinueId);
        sp.MyBattleGroundManager.UseSpellCard(r, cardInfo);

        Broadcast_SendOperationResponse();
    }

    public void OnClientRetinueAttackRetinueRequest(RetinueAttackRetinueRequest r)
    {
        ClientA.CurrentClientRequestResponseBundle = new RetinueAttackRetinueRequest_ResponseBundle();
        ClientB.CurrentClientRequestResponseBundle = new RetinueAttackRetinueRequest_ResponseBundle();

        RetinueAttackRetinueServerRequest request = new RetinueAttackRetinueServerRequest(r.clientId, r.AttackRetinueId, r.BeAttackedRetinueClientId, r.BeAttackedRetinueId);
        Broadcast_AddRequestToOperationResponse(request);

        ServerPlayer cpat = GetPlayerByClientId(r.clientId);
        ServerPlayer cpba = GetPlayerByClientId(r.BeAttackedRetinueClientId);

        ServerModuleRetinue attackRetinue = cpat.MyBattleGroundManager.GetRetinue(r.AttackRetinueId);
        ServerModuleRetinue beAttackedRetinue = cpba.MyBattleGroundManager.GetRetinue(r.BeAttackedRetinueId);

        attackRetinue.Attack(beAttackedRetinue, false);

        Broadcast_SendOperationResponse();
    }

    public void OnClientRetinueAttackShipRequest(RetinueAttackShipRequest r)
    {
        ClientA.CurrentClientRequestResponseBundle = new RetinueAttackRetinueRequest_ResponseBundle();
        ClientB.CurrentClientRequestResponseBundle = new RetinueAttackRetinueRequest_ResponseBundle();

        RetinueAttackShipServerRequest request = new RetinueAttackShipServerRequest(r.clientId, r.AttackRetinueId);
        Broadcast_AddRequestToOperationResponse(request);

        ServerPlayer cpat = GetPlayerByClientId(r.clientId);
        ServerPlayer cpba = cpat.MyEnemyPlayer;

        ServerModuleRetinue attackRetinue = cpat.MyBattleGroundManager.GetRetinue(r.AttackRetinueId);

        attackRetinue.AttackShip(cpba);

        Broadcast_SendOperationResponse();
    }

    public void OnEndRoundRequest(EndRoundRequest r)
    {
        if (CurrentPlayer.ClientId == r.clientId)
        {
            ClientA.CurrentClientRequestResponseBundle = new EndRoundRequest_ResponseBundle();
            ClientB.CurrentClientRequestResponseBundle = new EndRoundRequest_ResponseBundle();
            EndRound();
            Broadcast_SendOperationResponse();
        }
    }

    public bool IsStopped = false;

    public void OnLeaveGameRequest(LeaveGameRequest r)
    {
        OnLeaveGame(r.clientId);
    }

    public void OnLeaveGame(int clientId)
    {
        if (IsStopped) return;
        GameStopByLeaveRequest request = new GameStopByLeaveRequest(clientId);
        BroadcastRequest(request);
        IsStopped = true;
    }

    public void OnEndGame(ServerPlayer winner)
    {
        if (IsStopped) return;
        GameStopByWinRequest request = new GameStopByWinRequest(winner.ClientId);
        BroadcastRequest(request);
        IsStopped = true;
    }

    public void StopGame()
    {
        if (IsStopped)
        {
            if (ClientA == null) ServerLog.Print(ClientA.ClientId + "   ClientA==null");
            ClientA.ClientState = ProxyBase.ClientStates.Login;
            if (ClientB == null) ServerLog.Print(ClientB.ClientId + "   ClientB==null");
            ClientB.ClientState = ProxyBase.ClientStates.Login;

            Server.SV.SGMM.RemoveGame(this, ClientA, ClientB);

            PlayerA?.OnDestroyed();
            PlayerB?.OnDestroyed();
        }

        IsStopped = false;
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

        PlayerA.MyBattleGroundManager.BattleGroundRemoveRetinues(tmp);
        PlayerB.MyBattleGroundManager.BattleGroundRemoveRetinues(tmp);

        RetinueDieRequest request1 = new RetinueDieRequest(tmp);
        Broadcast_AddRequestToOperationResponse(request1);
        BattleGroundRemoveRetinueRequest request2 = new BattleGroundRemoveRetinueRequest(tmp);
        Broadcast_AddRequestToOperationResponse(request2);
        DieRetinueList.Clear();
    }

    #endregion


    #region Utils

    public void Broadcast_AddRequestToOperationResponse(ServerRequestBase request)
    {
        ClientA.CurrentClientRequestResponseBundle.AttachedRequests.Add(request);
        ClientB.CurrentClientRequestResponseBundle.AttachedRequests.Add(request);
    }


    private void Broadcast_SendOperationResponse()
    {
        ClientA.SendMessage(ClientA.CurrentClientRequestResponseBundle);
        ClientB.SendMessage(ClientB.CurrentClientRequestResponseBundle);
        ClientA.CurrentClientRequestResponseBundle = null;
        ClientB.CurrentClientRequestResponseBundle = null;
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