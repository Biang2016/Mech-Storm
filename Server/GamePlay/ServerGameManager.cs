using System;
using System.Collections.Generic;
using System.Linq;

internal class ServerGameManager
{
    public ClientProxy ClientA;
    public ClientProxy ClientB;
    public ServerPlayer CurrentPlayer;
    public ServerPlayer IdlePlayer;
    public ServerPlayer PlayerA;
    public ServerPlayer PlayerB;

    public EventManager EventManager;

    public RandomNumberGenerator RandomNumberGenerator;

    public ServerGameManager(ClientProxy clientA, ClientProxy clientB)
    {
        ClientA = clientA;
        ClientB = clientB;

        ClientA.MyServerGameManager = this;
        ClientB.MyServerGameManager = this;

        EventManager = new EventManager();

        EventManager.OnEventPlayerBuffUpdateHandler += OnPlayerBuffReduce;
        EventManager.OnEventPlayerBuffRemoveHandler += OnPlayerBuffRemove;
        EventManager.OnEventInvokeEndHandler += SendAllDieInfos;
        EventManager.OnEventInvokeHandler += OnSETriggered;

        Initialized();
    }


    #region 游戏初始化

    private int gameRetinueIdGenerator = 1000;

    public int GenerateNewRetinueId()
    {
        return gameRetinueIdGenerator++;
    }

    private int gameCardInstanceIdGenerator = 2000;

    public int GenerateNewCardInstanceId()
    {
        return gameCardInstanceIdGenerator++;
    }

    private int gameEquipIdGenerator = 100;

    public int GenerateNewEquipId()
    {
        return gameEquipIdGenerator++;
    }

    private void Initialized()
    {
        ClientA.ClientState = ProxyBase.ClientStates.Playing;
        ClientB.ClientState = ProxyBase.ClientStates.Playing;

        if (ServerConsole.Platform == ServerConsole.DEVELOP.DEVELOP || ServerConsole.Platform == ServerConsole.DEVELOP.TEST)
            ServerLog.Print("StartGameSuccess! Between: " + ClientA.ClientId + " and " + ClientB.ClientId);

        SyncRandomNumber();

        int PA_LIFE = ClientA.CurrentBuildInfo.Life;
        int PA_MAGIC = ClientA.CurrentBuildInfo.Energy;
        int PA_BEGINMETAL = ClientA.CurrentBuildInfo.BeginMetal;
        int PB_LIFE = ClientB.CurrentBuildInfo.Life;
        int PB_MAGIC = ClientB.CurrentBuildInfo.Energy;
        int PB_BEGINMETAL = ClientB.CurrentBuildInfo.BeginMetal;

        PlayerA = new ServerPlayer(ClientA.UserName, ClientA.ClientId, ClientB.ClientId, 0, PA_BEGINMETAL, PA_LIFE, PA_LIFE, 0, PA_MAGIC, this);
        PlayerA.MyCardDeckManager.CardDeck = new CardDeck(ClientA.CurrentBuildInfo, PlayerA.OnCardDeckLeftChange,PlayerA.MyCardDeckManager.OnUpdatePlayerCoolDownCard, PlayerA.MyCardDeckManager.OnRemovePlayerCoolDownCard);
        PlayerA.MyClientProxy = ClientA;

        PlayerB = new ServerPlayer(ClientB.UserName, ClientB.ClientId, ClientA.ClientId, 0, PB_BEGINMETAL, PB_LIFE, PB_LIFE, 0, PB_MAGIC, this);
        PlayerB.MyCardDeckManager.CardDeck = new CardDeck(ClientB.CurrentBuildInfo, PlayerB.OnCardDeckLeftChange, PlayerB.MyCardDeckManager.OnUpdatePlayerCoolDownCard, PlayerB.MyCardDeckManager.OnRemovePlayerCoolDownCard);
        PlayerB.MyClientProxy = ClientB;

        PlayerA.MyEnemyPlayer = PlayerB;
        PlayerB.MyEnemyPlayer = PlayerA;

        ClientA.InitGameInfo();
        ClientB.InitGameInfo();

        ClientA.CurrentClientRequestResponseBundle = new GameStart_ResponseBundle();
        ClientB.CurrentClientRequestResponseBundle = new GameStart_ResponseBundle();

        SetPlayerRequest request1 = new SetPlayerRequest(ClientA.UserName, ClientA.ClientId, 0, PA_BEGINMETAL, PA_LIFE, PA_LIFE, 0, PA_MAGIC);
        Broadcast_AddRequestToOperationResponse(request1);
        SetPlayerRequest request2 = new SetPlayerRequest(ClientA.UserName, ClientB.ClientId, 0, PB_BEGINMETAL, PB_LIFE, PB_LIFE, 0, PB_MAGIC);
        Broadcast_AddRequestToOperationResponse(request2);

        GameBegin();

        Broadcast_SendOperationResponse(); //初始化的大包请求
    }

    private void SyncRandomNumber()
    {
        Random rd = new Random(DateTime.Now.Millisecond);
        int seed = rd.Next() % 255;
        RandomNumberGenerator = new RandomNumberGenerator(seed);

        RandomNumberSeedRequest request = new RandomNumberSeedRequest(seed);
        BroadcastRequest(request);
    }

    void GameBegin()
    {
        if (ClientB is ClientProxyAI)
        {
            CurrentPlayer = PlayerA;
        }
        else
        {
            CurrentPlayer = new Random().Next(0, 2) == 0 ? PlayerA : PlayerB;
        }

        IdlePlayer = CurrentPlayer == PlayerA ? PlayerB : PlayerA;
        bool isPlayerAFirst = CurrentPlayer == PlayerA;

        PlayerA.MyHandManager.DrawHeroCards(100);
        PlayerB.MyHandManager.DrawHeroCards(100);

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

        OnDrawCardPhase();
        OnBeginRound();
    }

    #endregion

    #region 回合中的基础步骤

    void OnSwitchPlayer()
    {
        CurrentPlayer = CurrentPlayer == PlayerA ? PlayerB : PlayerA;
        IdlePlayer = IdlePlayer == PlayerA ? PlayerB : PlayerA;
    }

    void OnBeginRound()
    {
        CurrentPlayer.IncreaseMetalMax(GamePlaySettings.MetalIncrease);
        CurrentPlayer.AddAllMetal();
        CurrentPlayer.MyHandManager.BeginRound();
        CurrentPlayer.MyBattleGroundManager.BeginRound();
        PlayerTurnRequest request = new PlayerTurnRequest(CurrentPlayer.ClientId);
        Broadcast_AddRequestToOperationResponse(request);
    }

    void OnDrawCardPhase()
    {
        CurrentPlayer.MyCardDeckManager.OnDrawCardPhase();
        CurrentPlayer.MyHandManager.DrawHeroCards(100);
        CurrentPlayer.MyHandManager.DrawCards(CurrentPlayer.MyCardDeckManager.CardDeck.M_BuildInfo.DrawCardNum);
    }

    public void EndRound()
    {
        CurrentPlayer.MyHandManager.EndRound();
        CurrentPlayer.MyBattleGroundManager.EndRound();
        OnSwitchPlayer();
        CurrentPlayer.MyCardDeckManager.EndRound();
        OnDrawCardPhase();
        OnBeginRound();
    }

    #endregion

    #region ClientOperationResponses

    public void OnClientSummonRetinueRequest(SummonRetinueRequest r)
    {
        ClientA.CurrentClientRequestResponseBundle = new SummonRetinueRequest_ResponseBundle();
        ClientB.CurrentClientRequestResponseBundle = new SummonRetinueRequest_ResponseBundle();

        ServerPlayer sp = GetPlayerByClientId(r.clientId);
        CardInfo_Retinue info = (CardInfo_Retinue) sp.MyHandManager.GetHandCardInfo(r.handCardInstanceId);
        int targetRetinueId = r.targetRetinueId;
        if (r.isTargetRetinueIdTempId)
        {
            targetRetinueId = sp.MyBattleGroundManager.GetRetinueIdByClientRetinueTempId(r.clientRetinueTempId);
        }

        sp.MyHandManager.UseCard(r.handCardInstanceId, targetRetinueId);
        sp.MyBattleGroundManager.AddRetinue(info, r.battleGroundIndex, targetRetinueId, r.clientRetinueTempId, r.handCardInstanceId);

        Broadcast_SendOperationResponse();
    }


    public void OnClientEquipWeaponRequest(EquipWeaponRequest r)
    {
        ClientA.CurrentClientRequestResponseBundle = new EquipWeaponRequest_ResponseBundle();
        ClientB.CurrentClientRequestResponseBundle = new EquipWeaponRequest_ResponseBundle();

        ServerPlayer sp = GetPlayerByClientId(r.clientId);
        CardInfo_Base cardInfo = sp.MyHandManager.GetHandCardInfo(r.handCardInstanceId);
        sp.MyHandManager.UseCard(r.handCardInstanceId);
        sp.MyBattleGroundManager.EquipWeapon(r, cardInfo);

        Broadcast_SendOperationResponse();
    }

    public void OnClientEquipShieldRequest(EquipShieldRequest r)
    {
        ClientA.CurrentClientRequestResponseBundle = new EquipShieldRequest_ResponseBundle();
        ClientB.CurrentClientRequestResponseBundle = new EquipShieldRequest_ResponseBundle();

        ServerPlayer sp = GetPlayerByClientId(r.clientId);
        CardInfo_Base cardInfo = sp.MyHandManager.GetHandCardInfo(r.handCardInstanceId);
        sp.MyHandManager.UseCard(r.handCardInstanceId);
        sp.MyBattleGroundManager.EquipShield(r, cardInfo);

        Broadcast_SendOperationResponse();
    }

    public void OnClientEquipPackRequest(EquipPackRequest r)
    {
        ClientA.CurrentClientRequestResponseBundle = new EquipPackRequest_ResponseBundle();
        ClientB.CurrentClientRequestResponseBundle = new EquipPackRequest_ResponseBundle();

        ServerPlayer sp = GetPlayerByClientId(r.clientId);
        CardInfo_Base cardInfo = sp.MyHandManager.GetHandCardInfo(r.handCardInstanceId);
        sp.MyHandManager.UseCard(r.handCardInstanceId);
        sp.MyBattleGroundManager.EquipPack(r, cardInfo);

        Broadcast_SendOperationResponse();
    }

    public void OnClientEquipMARequest(EquipMARequest r)
    {
        ClientA.CurrentClientRequestResponseBundle = new EquipMARequest_ResponseBundle();
        ClientB.CurrentClientRequestResponseBundle = new EquipMARequest_ResponseBundle();

        ServerPlayer sp = GetPlayerByClientId(r.clientId);
        CardInfo_Base cardInfo = sp.MyHandManager.GetHandCardInfo(r.handCardInstanceId);
        sp.MyHandManager.UseCard(r.handCardInstanceId);
        sp.MyBattleGroundManager.EquipMA(r, cardInfo);

        Broadcast_SendOperationResponse();
    }

    public void OnClientUseSpellCardRequest(UseSpellCardRequest r)
    {
        ClientA.CurrentClientRequestResponseBundle = new UseSpellCardRequset_ResponseBundle();
        ClientB.CurrentClientRequestResponseBundle = new UseSpellCardRequset_ResponseBundle();
        ServerPlayer sp = GetPlayerByClientId(r.clientId);
        sp.MyHandManager.UseCard(r.handCardInstanceId);
        Broadcast_SendOperationResponse();
    }

    public void OnClientUseSpellCardToRetinueRequest(UseSpellCardToRetinueRequest r)
    {
        ClientA.CurrentClientRequestResponseBundle = new UseSpellCardRequset_ResponseBundle();
        ClientB.CurrentClientRequestResponseBundle = new UseSpellCardRequset_ResponseBundle();

        ServerPlayer sp = GetPlayerByClientId(r.clientId);

        int targetRetinueId = r.targetRetinueId;
        if (r.isTargetRetinueIdTempId)
        {
            targetRetinueId = sp.MyBattleGroundManager.GetRetinueIdByClientRetinueTempId(r.clientRetinueTempId);
        }

        sp.MyHandManager.UseCard(r.handCardInstanceId, targetRetinueId: targetRetinueId);
        Broadcast_SendOperationResponse();
    }

    public void OnClientUseSpellCardToEquipRequest(UseSpellCardToEquipRequest r)
    {
        ClientA.CurrentClientRequestResponseBundle = new UseSpellCardRequset_ResponseBundle();
        ClientB.CurrentClientRequestResponseBundle = new UseSpellCardRequset_ResponseBundle();
        ServerPlayer sp = GetPlayerByClientId(r.clientId);
        sp.MyHandManager.UseCard(r.handCardInstanceId, targetEquipId: r.targetEquipId);
        Broadcast_SendOperationResponse();
    }

    public void OnClientUseSpellCardToShipRequest(UseSpellCardToShipRequest r)
    {
        ClientA.CurrentClientRequestResponseBundle = new UseSpellCardRequset_ResponseBundle();
        ClientB.CurrentClientRequestResponseBundle = new UseSpellCardRequset_ResponseBundle();
        ServerPlayer sp = GetPlayerByClientId(r.clientId);
        sp.MyHandManager.UseCard(r.handCardInstanceId, targetClientId: r.targetClientId);
        Broadcast_SendOperationResponse();
    }

    public void OnClientRetinueAttackRetinueRequest(RetinueAttackRetinueRequest r)
    {
        ClientA.CurrentClientRequestResponseBundle = new RetinueAttackRetinueRequest_ResponseBundle();
        ClientB.CurrentClientRequestResponseBundle = new RetinueAttackRetinueRequest_ResponseBundle();

        ServerPlayer cpat = GetPlayerByClientId(r.clientId);
        ServerPlayer cpba = GetPlayerByClientId(r.BeAttackedRetinueClientId);

        ServerModuleRetinue attackRetinue = cpat.MyBattleGroundManager.GetRetinue(r.AttackRetinueId);
        ServerModuleRetinue beAttackedRetinue = cpba.MyBattleGroundManager.GetRetinue(r.BeAttackedRetinueId);

        bool isAttackValid = attackRetinue.BeforeAttack(beAttackedRetinue, false);
        if (isAttackValid)
        {
            RetinueAttackRetinueServerRequest request = new RetinueAttackRetinueServerRequest(r.clientId, r.AttackRetinueId, r.BeAttackedRetinueClientId, r.BeAttackedRetinueId);
            Broadcast_AddRequestToOperationResponse(request);

            attackRetinue.Attack(beAttackedRetinue, false);
        }

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

    public void OnWinDirectlyRequest(WinDirectlyRequest r, ServerPlayer player)
    {
        if (CurrentPlayer.ClientId == r.clientId)
        {
            ClientA.CurrentClientRequestResponseBundle = new WinDirectlyRequest_ResponseBundle();
            ClientB.CurrentClientRequestResponseBundle = new WinDirectlyRequest_ResponseBundle();
            OnEndGame(player);
            Broadcast_SendOperationResponse();
        }
    }

    public void OnEndRoundRequest(EndRoundRequest r)
    {
        if (CurrentPlayer.ClientId == r.clientId)
        {
            ClientA.CurrentClientRequestResponseBundle = new EndRoundRequest_ResponseBundle(r.clientId);
            ClientB.CurrentClientRequestResponseBundle = new EndRoundRequest_ResponseBundle(r.clientId);
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
        Broadcast_AddRequestToOperationResponse(request);
        if (ClientB is ClientProxyAI AI && AI.IsStoryMode)
        {
            if (winner == PlayerA)
            {
                BeatBossRequest request2 = new BeatBossRequest(AI.LevelID, AI.BossPicID);
                ClientA.SendMessage(request2);

                Story story = Database.Instance.PlayerStoryStates[ClientA.UserName];
                story.BeatBoss(AI.LevelID, AI.BossPicID);
                if (AI.LevelID < story.Levels.Count - 1)
                {
                    story.UnlockLevelBosses(AI.LevelID + 1);
                    List<int> nextLevelBossPicIDs = new List<int>();
                    nextLevelBossPicIDs = story.LevelUnlockBossInfo[AI.LevelID + 1];
                    NextLevelBossesRequest request3 = new NextLevelBossesRequest(AI.LevelID + 1, nextLevelBossPicIDs);
                    ClientA.SendMessage(request3);
                }
            }
        }

        IsStopped = true;
    }

    public void OnEndGameByServerError()
    {
        if (IsStopped) return;
        GameStopByServerErrorRequest request = new GameStopByServerErrorRequest();
        BroadcastRequest(request);
        IsStopped = true;
    }

    public void StopGame()
    {
        if (IsStopped)
        {
            if (ServerConsole.Platform == ServerConsole.DEVELOP.DEVELOP || ServerConsole.Platform == ServerConsole.DEVELOP.TEST)
                if (ClientA == null)
                    ServerLog.Print(ClientA.ClientId + "   ClientA==null");

            ClientA.ClientState = ProxyBase.ClientStates.Login;
            if (ServerConsole.Platform == ServerConsole.DEVELOP.DEVELOP || ServerConsole.Platform == ServerConsole.DEVELOP.TEST)
                if (ClientB == null)
                    ServerLog.Print(ClientB.ClientId + "   ClientB==null");

            ClientB.ClientState = ProxyBase.ClientStates.Login;

            Server.SV.SGMM.RemoveGame(ClientA);

            PlayerA?.OnDestroyed();
            PlayerB?.OnDestroyed();

            EventManager.ClearAllEvents();

            if (ServerConsole.Platform == ServerConsole.DEVELOP.DEVELOP || ServerConsole.Platform == ServerConsole.DEVELOP.TEST)
                ServerLog.PrintClientStates("GameStopSucBetween: " + PlayerA.ClientId + ", " + PlayerB.ClientId);
        }

        IsStopped = false;
    }

    #endregion

    #region SideEffects

    public List<int> DieRetinueList = new List<int>();

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

        PlayerA.MyBattleGroundManager.RemoveRetinues(tmp);
        PlayerB.MyBattleGroundManager.RemoveRetinues(tmp);

        RetinueDieRequest request1 = new RetinueDieRequest(tmp);
        Broadcast_AddRequestToOperationResponse(request1);
        BattleGroundRemoveRetinueRequest request2 = new BattleGroundRemoveRetinueRequest(tmp);
        Broadcast_AddRequestToOperationResponse(request2);
        DieRetinueList.Clear();
    }

    public void OnSETriggered(ShowSideEffectTriggeredRequest request)
    {
        Broadcast_AddRequestToOperationResponse(request);
    }

    public void OnPlayerBuffReduce(SideEffectExecute see, bool isAdd) //buff剩余次数减少
    {
        ((ServerPlayer) see.SideEffectBase.Player).UpdatePlayerBuff(see, isAdd);
    }

    public void OnPlayerBuffRemove(SideEffectExecute see, PlayerBuffSideEffects buff) //buff剩余次数减少
    {
        ((ServerPlayer) see.SideEffectBase.Player).RemovePlayerBuff(see, buff);
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

    public ServerModuleRetinue GetRetinueOnBattleGround(int retinueId)
    {
        ServerModuleRetinue retinue = PlayerA.MyBattleGroundManager.GetRetinue(retinueId);
        if (retinue == null) retinue = PlayerB.MyBattleGroundManager.GetRetinue(retinueId);
        return retinue;
    }

    public ServerModuleRetinue GetRandomAliveRetinueExcept(ServerBattleGroundManager.RetinueType retinueType, int exceptRetinueId)
    {
        int countA = PlayerA.MyBattleGroundManager.CountAliveRetinueExcept(retinueType, exceptRetinueId);
        int countB = PlayerB.MyBattleGroundManager.CountAliveRetinueExcept(retinueType, exceptRetinueId);
        Random rd = new Random();
        int ranResult = rd.Next(0, countA + countB);
        if (ranResult < countA)
        {
            return PlayerA.MyBattleGroundManager.GetRandomRetinue(retinueType, exceptRetinueId);
        }
        else
        {
            return PlayerB.MyBattleGroundManager.GetRandomRetinue(retinueType, exceptRetinueId);
        }
    }

    public int CountAliveRetinueExcept(ServerBattleGroundManager.RetinueType retinueType, int exceptRetinueId)
    {
        int countA = PlayerA.MyBattleGroundManager.CountAliveRetinueExcept(retinueType, exceptRetinueId);
        int countB = PlayerB.MyBattleGroundManager.CountAliveRetinueExcept(retinueType, exceptRetinueId);
        return countA + countB;
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