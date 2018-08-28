internal class ClientPlayer : Player
{
    public BoardAreaTypes MyBattleGroundArea; //卡牌所属方的战场区
    public HandManager MyHandManager; //卡牌所属的手部区管理器
    internal BattleGroundManager MyBattleGroundManager; //卡牌所属方的战场区域管理器
    internal CostLifeMagiceManager MyCostLifeMagiceManager; //Cost、Magic、Life条的管理器
    internal BoardAreaTypes MyHandArea; //卡牌所属的手部区
    internal Players WhichPlayer;
    public int ClientId;
    public bool IsInitialized = false;

    internal ClientPlayer(int costLeft, int costMax, int lifeLeft, int lifeMax, int magicLeft, int magicMax, Players whichPlayer) : base(costLeft, costMax, lifeLeft, lifeMax, magicLeft, magicMax)
    {
        WhichPlayer = whichPlayer;
        MyHandArea = whichPlayer == Players.Self ? BoardAreaTypes.SelfHandArea : BoardAreaTypes.EnemyHandArea;
        MyBattleGroundArea = whichPlayer == Players.Self ? BoardAreaTypes.SelfBattleGroundArea : BoardAreaTypes.EnemyBattleGroundArea;
        MyHandManager = whichPlayer == Players.Self ? GameBoardManager.Instance.SelfHandManager : GameBoardManager.Instance.EnemyHandManager;
        MyBattleGroundManager = whichPlayer == Players.Self ? GameBoardManager.Instance.SelfBattleGroundManager : GameBoardManager.Instance.EnemyBattleGroundManager;
        MyCostLifeMagiceManager = whichPlayer == Players.Self ? GameBoardManager.Instance.SelfCostLifeMagiceManager : GameBoardManager.Instance.EnemyCostLifeMagiceManager;
        MyHandManager.ClientPlayer = this;
        MyCostLifeMagiceManager.ClientPlayer = this;
        MyBattleGroundManager.ClientPlayer = this;
        IsInitialized = true;
        SetTotalLife();
        SetTotalMagic();
        OnCostChanged();
        OnLifeChanged();
        OnMagicChanged();
    }

    #region Cost

    protected override void OnCostChanged()
    {
        if (IsInitialized)
        {
            MyCostLifeMagiceManager.SetCost(CostLeft);
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
        if (IsInitialized) MyCostLifeMagiceManager.SetLife(LifeLeft);
    }

    protected void SetTotalLife()
    {
        if (IsInitialized) MyCostLifeMagiceManager.SetTotalLife(LifeMax);
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
        if (IsInitialized) MyCostLifeMagiceManager.SetMagic(MagicLeft);
    }

    protected void SetTotalMagic()
    {
        if (IsInitialized) MyCostLifeMagiceManager.SetTotalMagic(MagicMax);
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