using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

    public ServerPlayer(int clientId, int enemyClientId, int costMax, int costLeft, ServerGameManager serverGameManager) : base(costMax, costLeft)
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
        PlayerCostChangeRequest request = new PlayerCostChangeRequest(ClientId, PlayerCostChangeRequest.CostChangeFlag.Left, addCost_left: addCostValue);
        BroadCastRequest(request);
    }

    public void AddCostWithinMax(int addCostValue)
    {
        int costLeftBefore = CostLeft;
        if (CostMax - CostLeft > addCostValue)
            AddCost(addCostValue);
        else
            AddCost(CostMax - CostLeft);
        PlayerCostChangeRequest request = new PlayerCostChangeRequest(ClientId, PlayerCostChangeRequest.CostChangeFlag.Left, addCost_left: CostLeft - costLeftBefore);
        BroadCastRequest(request);
    }

    public void UseCostAboveZero(int useCostValue)
    {
        int costLeftBefore = CostLeft;
        if (CostLeft > useCostValue)
            AddCost(-useCostValue);
        else
            AddCost(-CostLeft);
        PlayerCostChangeRequest request = new PlayerCostChangeRequest(ClientId, PlayerCostChangeRequest.CostChangeFlag.Left, addCost_left: CostLeft - costLeftBefore);
        BroadCastRequest(request);
    }



    public void AddAllCost()
    {
        int costLeftBefore = CostLeft;
        AddCost(CostMax - CostLeft);
        PlayerCostChangeRequest request = new PlayerCostChangeRequest(ClientId, PlayerCostChangeRequest.CostChangeFlag.Left, addCost_left: CostLeft - costLeftBefore);
        BroadCastRequest(request);
    }

    public void UseAllCost()
    {
        int costLeftBefore = CostLeft;
        AddCost(-CostLeft);
        PlayerCostChangeRequest request = new PlayerCostChangeRequest(ClientId, PlayerCostChangeRequest.CostChangeFlag.Left, addCost_left: CostLeft - costLeftBefore);
        BroadCastRequest(request);
    }


    public void IncreaseCostMax(int increaseValue)
    {
        int costMaxBefore = CostMax;
        if (CostMax + increaseValue <= GamePlaySettings.MaxCost)
            AddCostMax(increaseValue);
        else
            AddCostMax(GamePlaySettings.MaxCost - CostMax);
        PlayerCostChangeRequest request = new PlayerCostChangeRequest(ClientId, PlayerCostChangeRequest.CostChangeFlag.Max, 0, addCost_max: CostMax - costMaxBefore);
        BroadCastRequest(request);
    }

    public void DecreaseCostMax(int decreaseValue)
    {
        int costMaxBefore = CostMax;
        if (CostMax <= decreaseValue)
            AddCostMax(-CostMax);
        else
            AddCostMax(-decreaseValue);
        PlayerCostChangeRequest request = new PlayerCostChangeRequest(ClientId, PlayerCostChangeRequest.CostChangeFlag.Max, 0, addCost_max: CostMax - costMaxBefore);
        BroadCastRequest(request);
    }

    private void BroadCastRequest(PlayerCostChangeRequest request)
    {
        MyClientProxy?.CurrentClientRequestResponse.SideEffects.Add(request);
        MyEnemyPlayer?.MyClientProxy?.CurrentClientRequestResponse.SideEffects.Add(request);
    }
}