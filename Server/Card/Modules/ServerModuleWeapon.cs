internal class ServerModuleWeapon : ServerModuleBase
{
    internal ServerModuleRetinue M_ModuleRetinue;

    protected override void Initiate()
    {
        M_WeaponType = CardInfo.WeaponInfo.WeaponType;
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

    private int m_WeaponPlaceIndex;

    public int M_WeaponPlaceIndex
    {
        get { return m_WeaponPlaceIndex; }
        set { m_WeaponPlaceIndex = value; }
    }

    private WeaponTypes m_WeaponType;

    public WeaponTypes M_WeaponType
    {
        get { return m_WeaponType; }

        set { m_WeaponType = value; }
    }

    #endregion
}