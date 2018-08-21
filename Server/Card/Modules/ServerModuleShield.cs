internal class ServerModuleShield : ServerModuleBase
{
    #region 各模块、自身数值和初始化

    internal ServerModuleRetinue M_ModuleRetinue;

    public override void Initiate(CardInfo_Base cardInfo, ServerPlayer serverPlayer)
    {
        M_ShieldName = Utils.TextToVertical(((CardInfo_Shield)cardInfo).BaseInfo.CardName);
        M_ShieldType = cardInfo.ShieldInfo.ShieldType;
        base.Initiate(cardInfo, serverPlayer);
    }

    public override CardInfo_Base GetCurrentCardInfo()
    {
        return new CardInfo_Shield(CardInfo.CardID, CardInfo.BaseInfo, CardInfo.UpgradeInfo, CardInfo.ShieldInfo, CardInfo.SideEffects_OnDie);
    }

    #region 属性

    private int m_ShieldPlaceIndex;

    public int M_ShieldPlaceIndex
    {
        get { return m_ShieldPlaceIndex; }
        set { m_ShieldPlaceIndex = value; }
    }

    private string m_ShieldName;

    public string M_ShieldName
    {
        get { return m_ShieldName; }

        set { m_ShieldName = value; }
    }

    private ShieldTypes m_ShieldType;

    public ShieldTypes M_ShieldType
    {
        get { return m_ShieldType; }

        set { m_ShieldType = value; }
    }

    #endregion

    #endregion

    #region 模块交互

    #endregion
}