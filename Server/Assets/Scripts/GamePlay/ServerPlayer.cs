using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ServerPlayer : Player
{
    public ClientProxy MyClientProxy;
    public ServerPlayer MyEnemyPlayer;

    public int ClientId;
    public int EnemyClientId;
    public int CostMax;
    public int CostLeft;
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

    public void AddCostWithoutLimit(int addCostValue)
    {
        CostLeft += addCostValue;
        PlayerCostRequest request = new PlayerCostRequest(ClientId, CostChangeFlag.Left, 1, addCostValue, 0, 0);
        MyClientProxy.SendRequestsQueue.Enqueue(request);
        MyEnemyPlayer.MyClientProxy.SendRequestsQueue.Enqueue(request);
    }

    public void AddCostWithinMax(int addCostValue)
    {
        int costLeftBefore = CostLeft;
        if (CostMax - CostLeft > addCostValue)
            CostLeft += addCostValue;
        else
            CostLeft = CostMax;
        PlayerCostRequest request = new PlayerCostRequest(ClientId, CostChangeFlag.Left, 1, CostLeft - costLeftBefore, 0, 0);
        MyClientProxy.SendRequestsQueue.Enqueue(request);
        MyEnemyPlayer.MyClientProxy.SendRequestsQueue.Enqueue(request);
    }

    public void AddAllCost()
    {
        int costLeftBefore = CostLeft;
        CostLeft = CostMax;
        PlayerCostRequest request = new PlayerCostRequest(ClientId, CostChangeFlag.Left, 1, CostLeft - costLeftBefore, 0, 0);
        MyClientProxy.SendRequestsQueue.Enqueue(request);
        MyEnemyPlayer.MyClientProxy.SendRequestsQueue.Enqueue(request);
    }

    public void UseAllCost()
    {
        int costLeftBefore = CostLeft;
        CostLeft = 0;
        PlayerCostRequest request = new PlayerCostRequest(ClientId, CostChangeFlag.Left, -1, costLeftBefore - CostLeft, 0, 0);
        MyClientProxy.SendRequestsQueue.Enqueue(request);
        MyEnemyPlayer.MyClientProxy.SendRequestsQueue.Enqueue(request);
    }


    public void IncreaseCostMax(int increaseValue)
    {
        int costMaxBefore = CostMax;
        if (CostMax + increaseValue <= GamePlaySettings.MaxCost)
            CostMax += increaseValue;
        else
            CostMax = GamePlaySettings.MaxCost;
        PlayerCostRequest request = new PlayerCostRequest(ClientId, CostChangeFlag.Max, 0, 0, 1, CostMax - costMaxBefore);
        MyClientProxy.SendRequestsQueue.Enqueue(request);
        MyEnemyPlayer.MyClientProxy.SendRequestsQueue.Enqueue(request);
    }

    public void DecreaseCostMax(int decreaseValue)
    {
        int costMaxBefore = CostMax;
        if (CostMax <= decreaseValue)
        {
            CostMax = 0;
        }
        else
        {
            CostMax -= decreaseValue;
            if (CostLeft > CostMax) CostLeft = CostMax;
        }

        PlayerCostRequest request = new PlayerCostRequest(ClientId, CostChangeFlag.Max, 0, 0, -1, costMaxBefore - CostMax);
        MyClientProxy.SendRequestsQueue.Enqueue(request);
        MyEnemyPlayer.MyClientProxy.SendRequestsQueue.Enqueue(request);
    }
}