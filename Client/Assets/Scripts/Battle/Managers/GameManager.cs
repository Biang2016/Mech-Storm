using System;
using System.Collections.Generic;

internal partial class GameManager
{
    public BattleClientProxy ClientA;
    public BattleClientProxy ClientB;
    public BattlePlayer CurrentPlayer;
    public BattlePlayer IdlePlayer;
    public BattlePlayer PlayerA;
    public BattlePlayer PlayerB;

    public EventManager EventManager;

    public RandomNumberGenerator RandomNumberGenerator;

    public GameManager(BattleClientProxy clientA, BattleClientProxy clientB)
    {
        ClientA = clientA;
        ClientB = clientB;

        ClientA.BattleGameManager = this;
        ClientB.BattleGameManager = this;

        EventManager = new EventManager();

        EventManager.OnEventPlayerBuffUpdateHandler += OnPlayerBuffReduce;
        EventManager.OnEventPlayerBuffRemoveHandler += OnPlayerBuffRemove;
        EventManager.OnEventInvokeEndHandler += SendAllDieInfos;
        EventManager.OnEventInvokeHandler += OnSETriggered;

        Initialized();
    }

    #region 游戏初始化

    private int gameMechIdGenerator = 1000;

    public int GenerateNewMechId()
    {
        return gameMechIdGenerator++;
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

        BattleLog.Instance.Log.Print("StartGameSuccess! Between: " + ClientA.ClientId + " and " + ClientB.ClientId);

        SyncRandomNumber();

        int PA_LIFE = ClientA.CurrentBuildInfo.Life;
        int PA_MAGIC = ClientA.CurrentBuildInfo.Energy;
        int PA_BEGINMETAL = ClientA.CurrentBuildInfo.BeginMetal;
        int PB_LIFE = ClientB.CurrentBuildInfo.Life;
        int PB_MAGIC = ClientB.CurrentBuildInfo.Energy;
        int PB_BEGINMETAL = ClientB.CurrentBuildInfo.BeginMetal;

        PlayerA = new BattlePlayer(ClientA.UserName, ClientA.ClientId, 0, PA_BEGINMETAL, PA_LIFE, PA_LIFE, 0, PA_MAGIC, this);
        PlayerA.CardDeckManager.CardDeck = new CardDeck(ClientA.CurrentBuildInfo, PlayerA.OnCardDeckLeftChange, PlayerA.CardDeckManager.OnUpdatePlayerCoolDownCard, PlayerA.CardDeckManager.OnRemovePlayerCoolDownCard);
        PlayerA.MyClientProxy = ClientA;

        PlayerB = new BattlePlayer(ClientB.UserName, ClientB.ClientId, 0, PB_BEGINMETAL, PB_LIFE, PB_LIFE, 0, PB_MAGIC, this);
        PlayerB.CardDeckManager.CardDeck = new CardDeck(ClientB.CurrentBuildInfo, PlayerB.OnCardDeckLeftChange, PlayerB.CardDeckManager.OnUpdatePlayerCoolDownCard, PlayerB.CardDeckManager.OnRemovePlayerCoolDownCard);
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

        PlayerA.HandManager.DrawHeroCards(100);
        PlayerB.HandManager.DrawHeroCards(100);

        if (isPlayerAFirst)
        {
            PlayerA.HandManager.DrawCards(GamePlaySettings.FirstDrawCard);
            PlayerB.HandManager.DrawCards(GamePlaySettings.SecondDrawCard);
        }
        else
        {
            PlayerB.HandManager.DrawCards(GamePlaySettings.FirstDrawCard);
            PlayerA.HandManager.DrawCards(GamePlaySettings.SecondDrawCard);
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
        CurrentPlayer.MetalMaxChange(GamePlaySettings.MetalIncrease);
        CurrentPlayer.AddAllMetal();
        CurrentPlayer.HandManager.BeginRound();
        CurrentPlayer.BattleGroundManager.BeginRound();
        PlayerTurnRequest request = new PlayerTurnRequest(CurrentPlayer.ClientId);
        Broadcast_AddRequestToOperationResponse(request);
    }

    void OnDrawCardPhase()
    {
        CurrentPlayer.CardDeckManager.OnDrawCardPhase();
        CurrentPlayer.HandManager.DrawHeroCards(100);
        CurrentPlayer.HandManager.DrawCards(CurrentPlayer.CardDeckManager.CardDeck.M_BuildInfo.DrawCardNum);
    }

    public void EndRound()
    {
        CurrentPlayer.HandManager.EndRound();
        CurrentPlayer.BattleGroundManager.EndRound();
        OnSwitchPlayer();
        CurrentPlayer.CardDeckManager.EndRound();
        OnDrawCardPhase();
        OnBeginRound();
    }

    #endregion

    #region ClientOperationResponses

    public void OnClientSummonMechRequest(SummonMechRequest r)
    {
        ClientA.CurrentClientRequestResponseBundle = new SummonMechRequest_ResponseBundle();
        ClientB.CurrentClientRequestResponseBundle = new SummonMechRequest_ResponseBundle();

        BattlePlayer sp = GetPlayerByClientId(r.clientId);
        CardInfo_Mech info = (CardInfo_Mech) sp.HandManager.GetHandCardInfo(r.handCardInstanceId);
        int targetMechId = r.targetMechId;
        if (r.isTargetMechIdTempId)
        {
            targetMechId = sp.BattleGroundManager.GetMechIdByClientMechTempId(r.clientMechTempId);
        }

        sp.HandManager.UseCard(r.handCardInstanceId, targetMechId);
        sp.BattleGroundManager.AddMech(info, r.battleGroundIndex, targetMechId, r.clientMechTempId, r.handCardInstanceId);

        Broadcast_SendOperationResponse();
    }

    public void OnClientEquipWeaponRequest(EquipWeaponRequest r)
    {
        ClientA.CurrentClientRequestResponseBundle = new EquipWeaponRequest_ResponseBundle();
        ClientB.CurrentClientRequestResponseBundle = new EquipWeaponRequest_ResponseBundle();

        BattlePlayer sp = GetPlayerByClientId(r.clientId);
        CardInfo_Base cardInfo = sp.HandManager.GetHandCardInfo(r.handCardInstanceId);
        sp.HandManager.UseCard(r.handCardInstanceId);
        sp.BattleGroundManager.EquipWeapon(r, cardInfo);

        Broadcast_SendOperationResponse();
    }

    public void OnClientEquipShieldRequest(EquipShieldRequest r)
    {
        ClientA.CurrentClientRequestResponseBundle = new EquipShieldRequest_ResponseBundle();
        ClientB.CurrentClientRequestResponseBundle = new EquipShieldRequest_ResponseBundle();

        BattlePlayer sp = GetPlayerByClientId(r.clientId);
        CardInfo_Base cardInfo = sp.HandManager.GetHandCardInfo(r.handCardInstanceId);
        sp.HandManager.UseCard(r.handCardInstanceId);
        sp.BattleGroundManager.EquipShield(r, cardInfo);

        Broadcast_SendOperationResponse();
    }

    public void OnClientEquipPackRequest(EquipPackRequest r)
    {
        ClientA.CurrentClientRequestResponseBundle = new EquipPackRequest_ResponseBundle();
        ClientB.CurrentClientRequestResponseBundle = new EquipPackRequest_ResponseBundle();

        BattlePlayer sp = GetPlayerByClientId(r.clientId);
        CardInfo_Base cardInfo = sp.HandManager.GetHandCardInfo(r.handCardInstanceId);
        sp.HandManager.UseCard(r.handCardInstanceId);
        sp.BattleGroundManager.EquipPack(r, cardInfo);

        Broadcast_SendOperationResponse();
    }

    public void OnClientEquipMARequest(EquipMARequest r)
    {
        ClientA.CurrentClientRequestResponseBundle = new EquipMARequest_ResponseBundle();
        ClientB.CurrentClientRequestResponseBundle = new EquipMARequest_ResponseBundle();

        BattlePlayer sp = GetPlayerByClientId(r.clientId);
        CardInfo_Base cardInfo = sp.HandManager.GetHandCardInfo(r.handCardInstanceId);
        sp.HandManager.UseCard(r.handCardInstanceId);
        sp.BattleGroundManager.EquipMA(r, cardInfo);

        Broadcast_SendOperationResponse();
    }

    public void OnClientUseSpellCardRequest(UseSpellCardRequest r)
    {
        ClientA.CurrentClientRequestResponseBundle = new UseSpellCardRequset_ResponseBundle();
        ClientB.CurrentClientRequestResponseBundle = new UseSpellCardRequset_ResponseBundle();
        BattlePlayer sp = GetPlayerByClientId(r.clientId);
        sp.HandManager.UseCard(r.handCardInstanceId);
        Broadcast_SendOperationResponse();
    }

    public void OnClientUseSpellCardToMechRequest(UseSpellCardToMechRequest r)
    {
        ClientA.CurrentClientRequestResponseBundle = new UseSpellCardRequset_ResponseBundle();
        ClientB.CurrentClientRequestResponseBundle = new UseSpellCardRequset_ResponseBundle();

        BattlePlayer sp = GetPlayerByClientId(r.clientId);

        int targetMechId = r.targetMechId;
        if (r.isTargetMechIdTempId)
        {
            targetMechId = sp.BattleGroundManager.GetMechIdByClientMechTempId(r.clientMechTempId);
        }

        sp.HandManager.UseCard(r.handCardInstanceId, targetMechId: targetMechId);
        Broadcast_SendOperationResponse();
    }

    public void OnClientUseSpellCardToEquipRequest(UseSpellCardToEquipRequest r)
    {
        ClientA.CurrentClientRequestResponseBundle = new UseSpellCardRequset_ResponseBundle();
        ClientB.CurrentClientRequestResponseBundle = new UseSpellCardRequset_ResponseBundle();
        BattlePlayer sp = GetPlayerByClientId(r.clientId);
        sp.HandManager.UseCard(r.handCardInstanceId, targetEquipId: r.targetEquipId);
        Broadcast_SendOperationResponse();
    }

    public void OnClientUseSpellCardToShipRequest(UseSpellCardToShipRequest r)
    {
        ClientA.CurrentClientRequestResponseBundle = new UseSpellCardRequset_ResponseBundle();
        ClientB.CurrentClientRequestResponseBundle = new UseSpellCardRequset_ResponseBundle();
        BattlePlayer sp = GetPlayerByClientId(r.clientId);
        sp.HandManager.UseCard(r.handCardInstanceId, targetClientId: r.targetClientId);
        Broadcast_SendOperationResponse();
    }

    public void OnClientMechAttackMechRequest(MechAttackMechRequest r)
    {
        ClientA.CurrentClientRequestResponseBundle = new MechAttackMechRequest_ResponseBundle();
        ClientB.CurrentClientRequestResponseBundle = new MechAttackMechRequest_ResponseBundle();

        BattlePlayer cpat = GetPlayerByClientId(r.clientId);
        BattlePlayer cpba = GetPlayerByClientId(r.BeAttackedMechClientId);

        ModuleMech attackMech = cpat.BattleGroundManager.GetMech(r.AttackMechId);
        ModuleMech beAttackedMech = cpba.BattleGroundManager.GetMech(r.BeAttackedMechId);

        bool isAttackValid = attackMech.BeforeAttack(beAttackedMech, false);
        if (isAttackValid)
        {
            MechAttackMechServerRequest request = new MechAttackMechServerRequest(r.clientId, r.AttackMechId, r.BeAttackedMechClientId, r.BeAttackedMechId);
            Broadcast_AddRequestToOperationResponse(request);

            attackMech.Attack(beAttackedMech, false);
        }

        Broadcast_SendOperationResponse();
    }

    public void OnClientMechAttackShipRequest(MechAttackShipRequest r)
    {
        ClientA.CurrentClientRequestResponseBundle = new MechAttackMechRequest_ResponseBundle();
        ClientB.CurrentClientRequestResponseBundle = new MechAttackMechRequest_ResponseBundle();

        MechAttackShipServerRequest request = new MechAttackShipServerRequest(r.clientId, r.AttackMechId);
        Broadcast_AddRequestToOperationResponse(request);

        BattlePlayer cpat = GetPlayerByClientId(r.clientId);
        BattlePlayer cpba = cpat.MyEnemyPlayer;

        ModuleMech attackMech = cpat.BattleGroundManager.GetMech(r.AttackMechId);

        attackMech.AttackShip(cpba);

        Broadcast_SendOperationResponse();
    }

    public void OnWinDirectlyRequest(WinDirectlyRequest r, BattlePlayer player)
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

    public void OnEndGame(BattlePlayer winner)
    {
        if (IsStopped) return;
        GameStopByWinRequest request = new GameStopByWinRequest(winner.ClientId);
        Broadcast_AddRequestToOperationResponse(request);
        OnEndGameHandler(winner);
        //if (ClientB is ClientProxyAI AI && AI.IsStoryMode)
        //{
        //    if (winner == PlayerA)
        //    {
        //        BeatEnemyRequest request2 = new BeatEnemyRequest();
        //        ClientA.SendMessage(request2);

        //        Story story = Database.Instance.PlayerStoryStates[ClientA.UserName];
        //        story.BeatEnemy(AI.LevelID, AI.EnemyPicID);
        //        if (AI.LevelID < story.Chapters.Count - 1)
        //        {
        //            story.UnlockChapterEnemies(AI.LevelID + 1);
        //            List<int> nextLevelBossPicIDs = new List<int>();
        //            nextLevelBossPicIDs = story.LevelUnlockBossInfo[AI.LevelID + 1];
        //            NextChapterEnemiesRequest request3 = new NextChapterEnemiesRequest(AI.LevelID + 1, nextLevelBossPicIDs);
        //            ClientA.SendMessage(request3);
        //        }
        //    }
        //}

        IsStopped = true;
    }

    public delegate void OnEndGameDelegate(BattlePlayer winner);

    public OnEndGameDelegate OnEndGameHandler;

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
            if (ClientA == null)
                BattleLog.Instance.Log.Print(ClientA.ClientId + "   ClientA==null");

            ClientA.ClientState = ProxyBase.ClientStates.Login;

            if (ClientB == null)
                BattleLog.Instance.Log.Print(ClientB.ClientId + "   ClientB==null");

            ClientB.ClientState = ProxyBase.ClientStates.Login;

            OnEndGameHandler(PlayerA);
//            Server.SV.SGMM.RemoveGame(ClientA);

            PlayerA?.OnDestroyed();
            PlayerB?.OnDestroyed();

            EventManager.ClearAllEvents();

            BattleLog.Instance.Log.PrintClientStates("GameStopSucBetween: " + PlayerA.ClientId + ", " + PlayerB.ClientId);
        }

        IsStopped = false;
    }

    #endregion

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

    #region SideEffects

    public List<int> DieMechList = new List<int>();

    public void AddDieTogetherMechsInfo(int dieMechId)
    {
        DieMechList.Add(dieMechId);
    }

    public void SendAllDieInfos()
    {
        if (DieMechList.Count == 0) return;
        List<int> tmp = new List<int>();
        foreach (int id in DieMechList)
        {
            tmp.Add(id);
        }

        tmp.Sort();

        PlayerA.BattleGroundManager.RemoveMechs(tmp);
        PlayerB.BattleGroundManager.RemoveMechs(tmp);

        MechDieRequest request1 = new MechDieRequest(tmp);
        Broadcast_AddRequestToOperationResponse(request1);
        BattleGroundRemoveMechRequest request2 = new BattleGroundRemoveMechRequest(tmp);
        Broadcast_AddRequestToOperationResponse(request2);
        DieMechList.Clear();
    }

    public void OnSETriggered(ShowSideEffectTriggeredRequest request)
    {
        Broadcast_AddRequestToOperationResponse(request);
    }

    public void OnPlayerBuffReduce(SideEffectExecute see, bool isAdd) //buff剩余次数减少
    {
        foreach (SideEffectBase se in see.SideEffectBases)
        {
            ((BattlePlayer) se.Player).UpdatePlayerBuff(see, isAdd);
        }
    }

    public void OnPlayerBuffRemove(SideEffectExecute see, PlayerBuffSideEffects buff) //buff剩余次数减少
    {
        foreach (SideEffectBase se in see.SideEffectBases)
        {
            ((BattlePlayer) se.Player).RemovePlayerBuff(see, buff);
        }
    }

    #endregion
}