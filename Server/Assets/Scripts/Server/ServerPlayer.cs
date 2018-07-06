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

    public ServerPlayer(int clientId, int costMax, int costLeft,int enemyClientId) : base(costMax, costLeft)
    {
        ClientId = clientId;
        EnemyClientId = enemyClientId;
        MyHandManager = new ServerHandManager();
        MyBattleGroundManager = new ServerBattleGroundManager();
    }

    public void IncreaseCostMax(int increaseValue)
    {
        if (CostMax + increaseValue <= GamePlaySettings.MaxCost)
            CostMax += increaseValue;
        else
            CostMax = GamePlaySettings.MaxCost;
    }

    public void DecreaseCostMax(int decreaseValue)
    {
        if (CostMax <= decreaseValue)
        {
            CostMax = 0;
        }
        else
        {
            CostMax -= decreaseValue;
            if (CostLeft > CostMax) CostLeft = CostMax;
        }
    }
}