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
        PlayerCostRequest request = new PlayerCostRequest(ClientId, CostChangeFlag.Left, 1, addCostValue, 0, 0);
        MyClientProxy?.SendMessage(request);
        MyEnemyPlayer?.MyClientProxy?.SendMessage(request);
    }

    public void AddCostWithinMax(int addCostValue)
    {
        int costLeftBefore = CostLeft;
        if (CostMax - CostLeft > addCostValue)
            AddCost(addCostValue);
        else
            AddCost(CostMax - CostLeft);
        PlayerCostRequest request = new PlayerCostRequest(ClientId, CostChangeFlag.Left, 1, CostLeft - costLeftBefore, 0, 0);
        MyClientProxy?.SendMessage(request);
        MyEnemyPlayer?.MyClientProxy?.SendMessage(request);
    }

    public void UseCostAboveZero(int useCostValue)
    {
        int costLeftBefore = CostLeft;
        if (CostLeft > useCostValue)
            UseCost(useCostValue);
        else
            UseCost(CostLeft);
        PlayerCostRequest request = new PlayerCostRequest(ClientId, CostChangeFlag.Left, -1, costLeftBefore - CostLeft, 0, 0);
        MyClientProxy?.SendMessage(request);
        MyEnemyPlayer?.MyClientProxy?.SendMessage(request);
    }

    public void AddAllCost()
    {
        int costLeftBefore = CostLeft;
        AddCost(CostMax - CostLeft);
        PlayerCostRequest request = new PlayerCostRequest(ClientId, CostChangeFlag.Left, 1, CostLeft - costLeftBefore, 0, 0);
        MyClientProxy?.SendMessage(request);
        MyEnemyPlayer?.MyClientProxy?.SendMessage(request);
    }

    public void UseAllCost()
    {
        int costLeftBefore = CostLeft;
        UseCost(CostLeft);
        PlayerCostRequest request = new PlayerCostRequest(ClientId, CostChangeFlag.Left, -1, costLeftBefore - CostLeft, 0, 0);
        MyClientProxy?.SendMessage(request);
        MyEnemyPlayer?.MyClientProxy?.SendMessage(request);
    }


    public void IncreaseCostMax(int increaseValue)
    {
        int costMaxBefore = CostMax;
        if (CostMax + increaseValue <= GamePlaySettings.MaxCost)
            AddCostMax(increaseValue);
        else
            AddCostMax(GamePlaySettings.MaxCost - CostMax);
        PlayerCostRequest request = new PlayerCostRequest(ClientId, CostChangeFlag.Max, 0, 0, 1, CostMax - costMaxBefore);
        MyClientProxy?.SendMessage(request);
        MyEnemyPlayer?.MyClientProxy?.SendMessage(request);
    }

    public void DecreaseCostMax(int decreaseValue)
    {
        int costMaxBefore = CostMax;
        if (CostMax <= decreaseValue)
            ReduceCostMax(CostMax);
        else
            ReduceCostMax(decreaseValue);
        PlayerCostRequest request = new PlayerCostRequest(ClientId, CostChangeFlag.Max, 0, 0, -1, costMaxBefore - CostMax);
        MyClientProxy?.SendMessage(request);
        MyEnemyPlayer?.MyClientProxy?.SendMessage(request);
    }
}