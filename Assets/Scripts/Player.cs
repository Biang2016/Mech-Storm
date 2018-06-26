public class Player {
    internal BoardAreaTypes MyBattleGroundArea; //卡牌所属方的战场区
    internal BattleGroundManager MyBattleGroundManager; //卡牌所属方的战场区域管理器
    internal CardDeckManager MyCardDeckManager; //卡牌所属方的牌堆管理器
    internal BoardAreaTypes MyHandArea; //卡牌所属的手部区
    internal HandManager MyHandManager; //卡牌所属的手部区管理器
    internal Players WhichPlayer;


    internal Player(Players whichPlayer)
    {
        WhichPlayer = whichPlayer;
        MyHandArea = whichPlayer == Players.Self ? BoardAreaTypes.SelfHandArea : BoardAreaTypes.EnemyHandArea;
        MyBattleGroundArea = whichPlayer == Players.Self
            ? BoardAreaTypes.SelfBattleGroundArea
            : BoardAreaTypes.EnemyBattleGroundArea;
        MyHandManager = whichPlayer == Players.Self
            ? GameBoardManager.GBM.SelfHandManager
            : GameBoardManager.GBM.EnemyHandManager;
        MyBattleGroundManager = whichPlayer == Players.Self
            ? GameBoardManager.GBM.SelfBattleGroundManager
            : GameBoardManager.GBM.EnemyBattleGroundManager;
        MyCardDeckManager = whichPlayer == Players.Self
            ? GameBoardManager.GBM.SelfCardDeckManager
            : GameBoardManager.GBM.EnemyCardDeckManager;
        MyHandManager.Player = this;
        MyBattleGroundManager.Player = this;
        MyCardDeckManager.Player = this;
        CostMax = GameManager.GM.BeginCost;
        AddAllCost();
    }


    #region Cost

    internal int CostMax { get; set; }

    internal int CostLeft { get; set; }

    public bool UseCost(int useCostNumber)
    {
        if (CostLeft > useCostNumber) {
            CostLeft -= useCostNumber;
            return true;
        }

        return false;
    }

    public void AddCostWithoutLimit(int addCostValue)
    {
        CostLeft += addCostValue;
    }

    public void AddCostWithinMax(int addCostValue)
    {
        if (CostMax - CostLeft > addCostValue)
            CostLeft += addCostValue;
        else
            CostLeft = CostMax;
    }

    public void UseAllCost()
    {
        CostLeft = 0;
    }

    public void AddAllCost()
    {
        CostLeft = CostMax;
    }

    public void IncreaseCostMax(int increaseValue)
    {
        if (CostMax + increaseValue <= GameManager.GM.MaxCost)
            CostMax += increaseValue;
        else
            CostMax = GameManager.GM.MaxCost;
    }

    public void DecreaseCostMax(int decreaseValue)
    {
        if (CostMax <= decreaseValue) {
            CostMax = 0;
        } else {
            CostMax -= decreaseValue;
            if (CostLeft > CostMax) CostLeft = CostMax;
        }
    }

    #endregion
}

internal enum Players {
    Self = 0,
    Enemy = 1
}