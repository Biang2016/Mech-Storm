internal class ServerModuleWeapon : ServerModuleBase
{
    #region 各模块、自身数值和初始化

    internal ServerModuleRetinue M_ModuleRetinue;

    public override void Initiate(CardInfo_Base cardInfo, ServerPlayer serverPlayer)
    {
        M_WeaponName = Utils.TextToVertical(cardInfo.BaseInfo.CardName);
        M_WeaponType = cardInfo.WeaponInfo.WeaponType;
        base.Initiate(cardInfo, serverPlayer);
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
            sideEffects_OnEndRound: CardInfo.SideEffects_OnEndRound,
            sideEffects_OnPlayOut: CardInfo.SideEffects_OnPlayOut,
            sideEffects_OnSummoned: CardInfo.SideEffects_OnSummoned,
            sideEffects_OnDie: CardInfo.SideEffects_OnDie);
    }


    #region 属性

    private int m_WeaponPlaceIndex;

    public int M_WeaponPlaceIndex
    {
        get { return m_WeaponPlaceIndex; }
        set { m_WeaponPlaceIndex = value; }
    }

    private string m_WeaponName;

    public string M_WeaponName
    {
        get { return m_WeaponName; }

        set { m_WeaponName = value; }
    }

    private WeaponTypes m_WeaponType;

    public WeaponTypes M_WeaponType
    {
        get { return m_WeaponType; }

        set { m_WeaponType = value; }
    }

    #endregion

    #endregion

    #region 模块交互

    #endregion
}