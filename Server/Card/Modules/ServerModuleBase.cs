internal abstract class ServerModuleBase
{
    internal ServerPlayer ServerPlayer;
    internal CardInfo_Base CardInfo; //卡牌原始数值信息
    protected bool isInitialized = false;

    public void Initiate(CardInfo_Base cardInfo, ServerPlayer serverPlayer)
    {
        ServerPlayer = serverPlayer;
        CardInfo = cardInfo.Clone();
        Stars = CardInfo.UpgradeInfo.CardLevel;
        InitializeSideEffects();
        Initiate();
        isInitialized = true;
        ServerPlayer.MyGameManager.EventManager.RegisterEvent(CardInfo.SideEffectBundle_OnBattleGround);
    }

    protected abstract void Initiate();

    protected abstract void InitializeSideEffects();

    public void UnRegisterSideEffect()
    {
        ServerPlayer.MyGameManager.EventManager.UnRegisterEvent(CardInfo.SideEffectBundle_OnBattleGround);
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