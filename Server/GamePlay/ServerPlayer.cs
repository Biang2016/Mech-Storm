internal class ServerPlayer : Player
{
    public ClientProxy MyClientProxy;
    public ServerPlayer MyEnemyPlayer;

    public int ClientId;
    public int EnemyClientId;
    public ServerGameManager MyGameManager;
    public ServerHandManager MyHandManager;
    public ServerCardDeckManager MyCardDeckManager;
    public ServerBattleGroundManager MyBattleGroundManager;

    public ServerPlayer(int clientId, int enemyClientId, int costLeft, int costMax, int lifeLeft, int lifeMax, int magicLeft, int magicMax, ServerGameManager serverGameManager) : base(costLeft, costMax, lifeLeft, lifeMax, magicLeft, magicMax)
    {
        ClientId = clientId;
        EnemyClientId = enemyClientId;
        MyGameManager = serverGameManager;
        MyHandManager = new ServerHandManager(this);
        MyCardDeckManager = new ServerCardDeckManager(this);
        MyBattleGroundManager = new ServerBattleGroundManager(this);
    }

    public void OnDestroyed()
    {
        MyEnemyPlayer = null;
        MyGameManager = null;
        MyHandManager = null;
        MyCardDeckManager = null;
        MyBattleGroundManager = null;
    }

    #region CostChange

    public void AddCostWithoutLimit(int addCostValue)
    {
        AddCost(addCostValue);
        if (addCostValue != 0)
        {
            PlayerCostChangeRequest request = new PlayerCostChangeRequest(ClientId, PlayerCostChangeRequest.CostChangeFlag.Left, addCost_left: addCostValue);
            BroadCastRequest(request);
        }
    }

    public void AddCostWithinMax(int addCostValue)
    {
        int costLeftBefore = CostLeft;
        if (CostMax - CostLeft > addCostValue)
            AddCost(addCostValue);
        else
            AddCost(CostMax - CostLeft);
        if (addCostValue != 0)
        {
            PlayerCostChangeRequest request = new PlayerCostChangeRequest(ClientId, PlayerCostChangeRequest.CostChangeFlag.Left, addCost_left: CostLeft - costLeftBefore);
            BroadCastRequest(request);
        }
    }

    public void UseCostAboveZero(int useCostValue)
    {
        int costLeftBefore = CostLeft;
        if (CostLeft > useCostValue)
            AddCost(-useCostValue);
        else
            AddCost(-CostLeft);
        if (useCostValue != 0)
        {
            PlayerCostChangeRequest request = new PlayerCostChangeRequest(ClientId, PlayerCostChangeRequest.CostChangeFlag.Left, addCost_left: CostLeft - costLeftBefore);
            BroadCastRequest(request);
        }
    }

    public void AddAllCost()
    {
        int costLeftBefore = CostLeft;
        AddCost(CostMax - CostLeft);
        if (CostLeft - costLeftBefore != 0)
        {
            PlayerCostChangeRequest request = new PlayerCostChangeRequest(ClientId, PlayerCostChangeRequest.CostChangeFlag.Left, addCost_left: CostLeft - costLeftBefore);
            BroadCastRequest(request);
        }
    }

    public void UseAllCost()
    {
        int costLeftBefore = CostLeft;
        AddCost(-CostLeft);
        if (CostLeft - costLeftBefore != 0)
        {
            PlayerCostChangeRequest request = new PlayerCostChangeRequest(ClientId, PlayerCostChangeRequest.CostChangeFlag.Left, addCost_left: CostLeft - costLeftBefore);
            BroadCastRequest(request);
        }
    }


    public void IncreaseCostMax(int increaseValue)
    {
        int costMaxBefore = CostMax;
        if (CostMax + increaseValue <= GamePlaySettings.MaxCost)
            AddCostMax(increaseValue);
        else
            AddCostMax(GamePlaySettings.MaxCost - CostMax);
        if (increaseValue != 0)
        {
            PlayerCostChangeRequest request = new PlayerCostChangeRequest(ClientId, PlayerCostChangeRequest.CostChangeFlag.Max, 0, addCost_max: CostMax - costMaxBefore);
            BroadCastRequest(request);
        }
    }

    public void DecreaseCostMax(int decreaseValue)
    {
        int costMaxBefore = CostMax;
        if (CostMax <= decreaseValue)
            AddCostMax(-CostMax);
        else
            AddCostMax(-decreaseValue);
        if (decreaseValue != 0)
        {
            PlayerCostChangeRequest request = new PlayerCostChangeRequest(ClientId, PlayerCostChangeRequest.CostChangeFlag.Max, 0, addCost_max: CostMax - costMaxBefore);
            BroadCastRequest(request);
        }
    }

    #endregion

    #region LifeChange

    public void AddLifeWithinMax(int addLifeValue)
    {
        int LifeLeftBefore = LifeLeft;
        if (LifeMax - LifeLeft > addLifeValue)
            AddLife(addLifeValue);
        else
            AddLife(LifeMax - LifeLeft);
        if (addLifeValue != 0)
        {
            PlayerLifeChangeRequest request = new PlayerLifeChangeRequest(ClientId, PlayerLifeChangeRequest.LifeChangeFlag.Left, addLife_left: LifeLeft - LifeLeftBefore);
            BroadCastRequest(request);
        }
    }

    public void UseLifeAboveZero(int useLifeValue)
    {
        int LifeLeftBefore = LifeLeft;
        if (LifeLeft > useLifeValue)
            AddLife(-useLifeValue);
        else
            AddLife(-LifeLeft);
        if (useLifeValue != 0)
        {
            PlayerLifeChangeRequest request = new PlayerLifeChangeRequest(ClientId, PlayerLifeChangeRequest.LifeChangeFlag.Left, addLife_left: LifeLeft - LifeLeftBefore);
            BroadCastRequest(request);
        }
    }

    public void AddAllLife()
    {
        int LifeLeftBefore = LifeLeft;
        AddLife(LifeMax - LifeLeft);
        if (LifeLeft - LifeLeftBefore != 0)
        {
            PlayerLifeChangeRequest request = new PlayerLifeChangeRequest(ClientId, PlayerLifeChangeRequest.LifeChangeFlag.Left, addLife_left: LifeLeft - LifeLeftBefore);
            BroadCastRequest(request);
        }
    }

    #endregion

    #region MagicChange

    public void AddMagicWithinMax(int addMagicValue)
    {
        int MagicLeftBefore = MagicLeft;
        if (MagicMax - MagicLeft > addMagicValue)
            AddMagic(addMagicValue);
        else
            AddMagic(MagicMax - MagicLeft);
        if (addMagicValue != 0)
        {
            PlayerMagicChangeRequest request = new PlayerMagicChangeRequest(ClientId, PlayerMagicChangeRequest.MagicChangeFlag.Left, addMagic_left: MagicLeft - MagicLeftBefore);
            BroadCastRequest(request);
        }
    }

    public void UseMagicAboveZero(int useMagicValue)
    {
        int MagicLeftBefore = MagicLeft;
        if (MagicLeft > useMagicValue)
            AddMagic(-useMagicValue);
        else
            AddMagic(-MagicLeft);
        if (useMagicValue != 0)
        {
            PlayerMagicChangeRequest request = new PlayerMagicChangeRequest(ClientId, PlayerMagicChangeRequest.MagicChangeFlag.Left, addMagic_left: MagicLeft - MagicLeftBefore);
            BroadCastRequest(request);
        }
    }

    public void AddAllMagic()
    {
        int MagicLeftBefore = MagicLeft;
        AddMagic(MagicMax - MagicLeft);
        if (MagicLeft - MagicLeftBefore != 0)
        {
            PlayerMagicChangeRequest request = new PlayerMagicChangeRequest(ClientId, PlayerMagicChangeRequest.MagicChangeFlag.Left, addMagic_left: MagicLeft - MagicLeftBefore);
            BroadCastRequest(request);
        }
    }

    #endregion

    public void OnCardDeckLeftChange(int count)
    {
        CardDeckLeftChangeRequest request = new CardDeckLeftChangeRequest(ClientId, count);
        BroadCastRequest(request);
    }

    private void BroadCastRequest(ServerRequestBase request)
    {
        MyClientProxy?.CurrentClientRequestResponseBundle.AttachedRequests.Add(request);
        MyEnemyPlayer?.MyClientProxy?.CurrentClientRequestResponseBundle.AttachedRequests.Add(request);
    }
}