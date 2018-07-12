using System.Collections.Generic;
using UnityEngine;

internal class ClientPlayer : Player
{
    public BoardAreaTypes MyBattleGroundArea; //卡牌所属方的战场区
    public HandManager MyHandManager; //卡牌所属的手部区管理器
    internal BattleGroundManager MyBattleGroundManager; //卡牌所属方的战场区域管理器
    internal BoardAreaTypes MyHandArea; //卡牌所属的手部区
    internal Players WhichPlayer;

    internal ClientPlayer(int costMax, int costLeft, Players whichPlayer) : base(costMax, costLeft)
    {
        WhichPlayer = whichPlayer;
        MyHandArea = whichPlayer == Players.Self ? BoardAreaTypes.SelfHandArea : BoardAreaTypes.EnemyHandArea;
        MyBattleGroundArea = whichPlayer == Players.Self ? BoardAreaTypes.SelfBattleGroundArea : BoardAreaTypes.EnemyBattleGroundArea;
        MyHandManager = whichPlayer == Players.Self ? GameBoardManager.GBM.SelfHandManager : GameBoardManager.GBM.EnemyHandManager;
        MyBattleGroundManager = whichPlayer == Players.Self ? GameBoardManager.GBM.SelfBattleGroundManager : GameBoardManager.GBM.EnemyBattleGroundManager;
        MyHandManager.ClientPlayer = this;
        MyBattleGroundManager.ClientPlayer = this;
    }

    #region Cost

    protected override void OnCostChanged()
    {
        if (this == RoundManager.RM.SelfClientPlayer)
        {
            RoundManager.RM.SelfCostText.text = "Cost: " + CostLeft + "/" + CostMax;
        }
        else if (this == RoundManager.RM.EnemyClientPlayer)
        {
            RoundManager.RM.EnemyCostText.text = "Cost: " + CostLeft + "/" + CostMax;
        }
    }

    public void DoChangeCost(PlayerCostRequest request)
    {
        if (request.change == CostChangeFlag.Both)
        {
            switch (request.sign_left)
            {
                case 1:
                    AddCost(request.addCost_left);
                    break;
                case -1:
                    UseCost(request.addCost_left);
                    break;
                default:
                    break;
            }

            switch (request.sign_max)
            {
                case 1:
                    AddCostMax(request.addCost_max);
                    break;
                case -1:
                    ReduceCostMax(request.addCost_max);
                    break;
                default:
                    break;
            }
        }
        else if (request.change == CostChangeFlag.Left)
        {
            switch (request.sign_left)
            {
                case 1:
                    AddCost(request.addCost_left);
                    break;
                case -1:
                    UseCost(request.addCost_left);
                    break;
                default:
                    break;
            }
        }
        else if (request.change == CostChangeFlag.Max)
        {
            switch (request.sign_max)
            {
                case 1:
                    AddCostMax(request.addCost_max);
                    break;
                case -1:
                    ReduceCostMax(request.addCost_max);
                    break;
                default:
                    break;
            }
        }
    }

    #endregion
}

internal enum Players
{
    Self = 0,
    Enemy = 1
}