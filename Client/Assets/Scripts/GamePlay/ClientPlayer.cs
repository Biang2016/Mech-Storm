using System.Collections.Generic;
using UnityEngine;

internal class ClientPlayer : Player
{
    public BoardAreaTypes MyBattleGroundArea; //卡牌所属方的战场区
    public HandManager MyHandManager; //卡牌所属的手部区管理器
    internal BattleGroundManager MyBattleGroundManager; //卡牌所属方的战场区域管理器
    internal BoardAreaTypes MyHandArea; //卡牌所属的手部区
    internal Players WhichPlayer;
    public int ClientId;

    internal ClientPlayer(int costLeft, int costMax, Players whichPlayer) : base(costLeft, costMax)
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

    public void DoChangeCost(PlayerCostChangeRequest request)
    {
        if (request.change == PlayerCostChangeRequest.CostChangeFlag.Both)
        {
            AddCost(request.addCost_left);
            AddCostMax(request.addCost_max);
        }
        else if (request.change == PlayerCostChangeRequest.CostChangeFlag.Left)
        {
            AddCost(request.addCost_left);
        }
        else if (request.change == PlayerCostChangeRequest.CostChangeFlag.Max)
        {
            AddCostMax(request.addCost_max);
        }
    }

    #endregion
}

internal enum Players
{
    Self = 0,
    Enemy = 1
}