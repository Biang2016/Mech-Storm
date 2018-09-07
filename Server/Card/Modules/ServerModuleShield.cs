internal class ServerModuleShield : ServerModuleBase
{
    internal ServerModuleRetinue M_ModuleRetinue;

    protected override void Initiate()
    {
        M_ShieldName = Utils.TextToVertical(((CardInfo_Equip) CardInfo).BaseInfo.CardName);
        M_ShieldType = CardInfo.ShieldInfo.ShieldType;
    }

    protected override void InitializeSideEffects()
    {
        foreach (SideEffectBundle.SideEffectExecute see in CardInfo.SideEffects.GetSideEffects())
        {
            see.SideEffectBase.Player = ServerPlayer;
            see.SideEffectBase.M_ExecuterInfo = new SideEffectBase.ExecuterInfo(ServerPlayer.ClientId);
        }
    }

    public override CardInfo_Base GetCurrentCardInfo()
    {
        return new CardInfo_Equip(
            cardID: CardInfo.CardID,
            baseInfo: CardInfo.BaseInfo,
            upgradeInfo: CardInfo.UpgradeInfo,
            slotType: ((CardInfo_Equip) CardInfo).M_SlotType,
            weaponInfo: CardInfo.WeaponInfo,
            shieldInfo: CardInfo.ShieldInfo,
            sideEffects: CardInfo.SideEffects);
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
}