internal class ClientPlayer : Player
{
    public BoardAreaTypes MyBattleGroundArea; //卡牌所属方的战场区
    public HandManager MyHandManager; //卡牌所属的手部区管理器
    internal BattleGroundManager MyBattleGroundManager; //卡牌所属方的战场区域管理器
    internal BoardAreaTypes MyHandArea; //卡牌所属的手部区
    internal Players WhichPlayer;
    public int ClientId;

    internal ClientPlayer(int costLeft, int costMax, int lifeLeft, int lifeMax, int magicLeft, int magicMax, Players whichPlayer) : base(costLeft, costMax, lifeLeft, lifeMax, magicLeft, magicMax)
    {
        WhichPlayer = whichPlayer;
        MyHandArea = whichPlayer == Players.Self ? BoardAreaTypes.SelfHandArea : BoardAreaTypes.EnemyHandArea;
        MyBattleGroundArea = whichPlayer == Players.Self ? BoardAreaTypes.SelfBattleGroundArea : BoardAreaTypes.EnemyBattleGroundArea;
        MyHandManager = whichPlayer == Players.Self ? GameBoardManager.Instance.SelfHandManager : GameBoardManager.Instance.EnemyHandManager;
        MyBattleGroundManager = whichPlayer == Players.Self ? GameBoardManager.Instance.SelfBattleGroundManager : GameBoardManager.Instance.EnemyBattleGroundManager;
        MyHandManager.ClientPlayer = this;
        MyBattleGroundManager.ClientPlayer = this;
    }

    #region Cost

    protected override void OnCostChanged()
    {
        if (this == RoundManager.Instance.SelfClientPlayer)
        {
            RoundManager.Instance.SelfCostText.text = "Cost: " + CostLeft + "/" + CostMax;
        }
        else if (this == RoundManager.Instance.EnemyClientPlayer)
        {
            RoundManager.Instance.EnemyCostText.text = "Cost: " + CostLeft + "/" + CostMax;
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

    #region Life

    protected override void OnLifeChanged()
    {
        if (this == RoundManager.Instance.SelfClientPlayer)
        {
            RoundManager.Instance.SelfLifeText.text = "Life: " + LifeLeft + "/" + LifeMax;
        }
        else if (this == RoundManager.Instance.EnemyClientPlayer)
        {
            RoundManager.Instance.EnemyLifeText.text = "Life: " + LifeLeft + "/" + LifeMax;
        }
    }

    public void DoChangeLife(PlayerLifeChangeRequest request)
    {
        if (request.change == PlayerLifeChangeRequest.LifeChangeFlag.Left)
        {
            AddLife(request.addLife_left);
        }
    }

    #endregion

    #region Magic

    protected override void OnMagicChanged()
    {
        if (this == RoundManager.Instance.SelfClientPlayer)
        {
            RoundManager.Instance.SelfMagicText.text = "Magic: " + MagicLeft + "/" + MagicMax;
        }
        else if (this == RoundManager.Instance.EnemyClientPlayer)
        {
            RoundManager.Instance.EnemyMagicText.text = "Magic: " + MagicLeft + "/" + MagicMax;
        }
    }

    public void DoChangeMagic(PlayerMagicChangeRequest request)
    {
        if (request.change == PlayerMagicChangeRequest.MagicChangeFlag.Left)
        {
            AddMagic(request.addMagic_left);
        }
    }

    #endregion
}

internal enum Players
{
    Self = 0,
    Enemy = 1
}