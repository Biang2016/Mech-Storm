internal abstract class ModuleBase
{
    internal BattlePlayer BattlePlayer;
    internal CardInfo_Base CardInfo; //卡牌原始数值信息
    protected bool isInitialized = false;

    public void Initiate(CardInfo_Base cardInfo, BattlePlayer battlePlayer)
    {
        BattlePlayer = battlePlayer;
        CardInfo = cardInfo.Clone();
        Stars = CardInfo.UpgradeInfo.CardLevel;
        InitializeSideEffects();
        Initiate();
        isInitialized = true;
        BattlePlayer.GameManager.EventManager.RegisterEvent(CardInfo.SideEffectBundle);
    }

    protected abstract void Initiate();

    protected abstract void InitializeSideEffects();

    public void UnRegisterSideEffect()
    {
        BattlePlayer.GameManager.EventManager.UnRegisterEvent(CardInfo.SideEffectBundle);
    }

    public abstract CardInfo_Base GetCurrentCardInfo();

    #region 属性

    protected int stars;

    public virtual int Stars
    {
        get { return stars; }

        set { stars = value; }
    }

    public int OriginCardInstanceId
    {
        get => originCardInstanceId;
        set => originCardInstanceId = value;
    }

    private int originCardInstanceId;

    #endregion
}