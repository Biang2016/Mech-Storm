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

    public ServerPlayer(int clientId, int enemyClientId, int costLeft, int costMax, ServerGameManager serverGameManager) : base(costLeft, costMax)
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