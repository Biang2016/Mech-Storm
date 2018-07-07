using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ServerPlayer : Player
{
    public int ClientId;
    public int EnemyClientId;
    public int CostMax;
    public int CostLeft;
    public ServerGameManager MyGameManager;
    public ServerHandManager MyHandManager;
    public ServerCardDeckManager MyCardDeckManager;
    public ServerBattleGroundManager MyBattleGroundManager;

    public ServerPlayer(int clientId, int costMax, int costLeft, int enemyClientId) : base(costMax, costLeft)
    {
        ClientId = clientId;
        EnemyClientId = enemyClientId;
        MyHandManager = new ServerHandManager();
        MyBattleGroundManager = new ServerBattleGroundManager();
    }

    public void AddCostWithoutLimit(int addCostValue)
    {
        CostLeft += addCostValue;
        PlayerCostRequest request = new PlayerCostRequest(ClientId, CostChangeFlag.Left, 0, 0, 1, addCostValue);
        Server.SV.SendMessage(request, ClientId);
        Server.SV.SendMessage(request, EnemyClientId);
    }

    public void AddCostWithinMax(int addCostValue)
    {
        int costLeftBefore = CostLeft;
        if (CostMax - CostLeft > addCostValue)
            CostLeft += addCostValue;
        else
            CostLeft = CostMax;
        PlayerCostRequest request = new PlayerCostRequest(ClientId, CostChangeFlag.Left, 0, 0, 1, CostLeft - costLeftBefore);
        Server.SV.SendMessage(request, ClientId);
        Server.SV.SendMessage(request, EnemyClientId);
    }

    public void AddAllCost()
    {
        int costLeftBefore = CostLeft;
        CostLeft = CostMax;
        PlayerCostRequest request = new PlayerCostRequest(ClientId, CostChangeFlag.Left, 0, 0, 1, CostLeft - costLeftBefore);
        Server.SV.SendMessage(request, ClientId);
        Server.SV.SendMessage(request, EnemyClientId);
    }

    public void UseAllCost()
    {
        int costLeftBefore = CostLeft;
        CostLeft = 0;
        PlayerCostRequest request = new PlayerCostRequest(ClientId, CostChangeFlag.Left, 0, 0, -1, costLeftBefore - CostLeft);
        Server.SV.SendMessage(request, ClientId);
        Server.SV.SendMessage(request, EnemyClientId);
    }


    public void IncreaseCostMax(int increaseValue)
    {
        int costMaxBefore = CostMax;
        if (CostMax + increaseValue <= GamePlaySettings.MaxCost)
            CostMax += increaseValue;
        else
            CostMax = GamePlaySettings.MaxCost;
        PlayerCostRequest request = new PlayerCostRequest(ClientId, CostChangeFlag.Max, 0, 0, 1, CostMax - costMaxBefore);
        Server.SV.SendMessage(request, ClientId);
        Server.SV.SendMessage(request, EnemyClientId);
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
        Server.SV.SendMessage(request, ClientId);
        Server.SV.SendMessage(request, EnemyClientId);
    }
}