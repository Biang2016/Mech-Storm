using System.Collections.Generic;

public class ClientPlayer : Player
{
    public BoardAreaTypes MyBattleGroundArea; //卡牌所属方的战场区
    public HandManager MyHandManager; //卡牌所属的手部区管理器
    internal BattleGroundManager MyBattleGroundManager; //卡牌所属方的战场区域管理器
    internal BoardAreaTypes MyHandArea; //卡牌所属的手部区
    internal Players WhichPlayer;

    //internal Queue<PlayerCostRequest> PlayerCostRequests = new Queue<PlayerCostRequest>();

    internal ClientPlayer(int costMax, int costLeft, Players whichPlayer) : base(costMax, costLeft)
    {
        WhichPlayer = whichPlayer;
        MyHandArea = whichPlayer == Players.Self ? BoardAreaTypes.SelfHandArea : BoardAreaTypes.EnemyHandArea;
        MyBattleGroundArea = whichPlayer == Players.Self ? BoardAreaTypes.SelfBattleGroundArea : BoardAreaTypes.EnemyBattleGroundArea;
        MyHandManager = whichPlayer == Players.Self ? GameBoardManager.GBM.SelfHandManager : GameBoardManager.GBM.EnemyHandManager;
        MyBattleGroundManager = whichPlayer == Players.Self ? GameBoardManager.GBM.SelfBattleGroundManager : GameBoardManager.GBM.EnemyBattleGroundManager;
        MyCardDeckManager = whichPlayer == Players.Self ? GameBoardManager.GBM.SelfCardDeckManager : GameBoardManager.GBM.EnemyCardDeckManager;
        MyHandManager.ClientPlayer = this;
        MyBattleGroundManager.ClientPlayer = this;
        MyCardDeckManager.Player = this;
    }

    #region Cost

    public override void OnCostChanged()
    {
        if (this == GameManager.GM.SelfClientPlayer)
        {
            RoundManager.RM.SelfCostText.text = "Cost: " + CostLeft + "/" + CostMax;
        }
        else if (this == GameManager.GM.EnemyClientPlayer)
        {
            RoundManager.RM.EnemyCostText.text = "Cost: " + CostLeft + "/" + CostMax;
        }
    }

    public void DoChangeCost(PlayerCostResponse resp)
    {
        switch (resp.Sign)
        {
            case 1:
                AddCostWithinMax(resp.AddCost);
                break;
            case -1:
                UseCost(resp.AddCost);
                break;
            case 2:
                AddAllCost();
                break;
            case -2:
                UseAllCost();
                break;
            default:
                break;
        }
    }

    #endregion
}

internal enum Players
{
    Self = 0,
    Enemy = 1
}