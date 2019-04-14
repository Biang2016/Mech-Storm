internal class ServerModuleWeapon : ServerModuleBase
{
    internal ServerModuleRetinue M_ModuleRetinue;

    protected override void Initiate()
    {
        M_WeaponType = CardInfo.WeaponInfo.WeaponType;
    }

    protected override void InitializeSideEffects()
    {
        foreach (SideEffectExecute see in CardInfo.SideEffectBundle.SideEffectExecutes)
        {
            see.SideEffectBase.Player = ServerPlayer;
            see.SideEffectBase.M_ExecutorInfo = new ExecutorInfo(
                clientId: ServerPlayer.ClientId,
                sideEffectExecutorID: see.ID,
                retinueId: M_ModuleRetinue.M_RetinueID,
                equipId: M_EquipID
            );
        }

        foreach (SideEffectExecute see in CardInfo.SideEffectBundle_OnBattleGround.SideEffectExecutes)
        {
            see.SideEffectBase.Player = ServerPlayer;
            see.SideEffectBase.M_ExecutorInfo = new ExecutorInfo(
                clientId: ServerPlayer.ClientId,
                sideEffectExecutorID: see.ID,
                retinueId: M_ModuleRetinue.M_RetinueID,
                equipId: M_EquipID
            );
        }
    }

    public override CardInfo_Base GetCurrentCardInfo()
    {
        return new CardInfo_Equip(
            cardID: CardInfo.CardID,
            baseInfo: CardInfo.BaseInfo,
            upgradeInfo: CardInfo.UpgradeInfo,
            equipInfo: CardInfo.EquipInfo,
            weaponInfo: CardInfo.WeaponInfo,
            shieldInfo: CardInfo.ShieldInfo,
            packInfo: CardInfo.PackInfo,
            maInfo: CardInfo.MAInfo,
            sideEffectBundle: CardInfo.SideEffectBundle,
            sideEffectBundle_OnBattleGround: CardInfo.SideEffectBundle_OnBattleGround);
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

    private int m_EquipID;

    public int M_EquipID
    {
        get { return m_EquipID; }

        set { m_EquipID = value; }
    }

    #endregion
}