internal class ServerModulePack : ServerModuleBase
{
    internal ServerModuleRetinue M_ModuleRetinue;

    protected override void Initiate()
    {
    }

    protected override void InitializeSideEffects()
    {
        foreach (SideEffectExecute see in CardInfo.SideEffectBundle.SideEffectExecutes)
        {
            see.SideEffectBase.Player = ServerPlayer;
            see.SideEffectBase.M_ExecuterInfo = new SideEffectBase.ExecuterInfo(
                clientId: ServerPlayer.ClientId,
                sideEffectExecutorID: see.ID,
                retinueId: M_ModuleRetinue.M_RetinueID,
                equipId: M_EquipID
            );
        }

        foreach (SideEffectExecute see in CardInfo.SideEffectBundle_OnBattleGround.SideEffectExecutes)
        {
            see.SideEffectBase.Player = ServerPlayer;
            see.SideEffectBase.M_ExecuterInfo = new SideEffectBase.ExecuterInfo(
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

    private int m_PackPlaceIndex;

    public int M_PackPlaceIndex
    {
        get { return m_PackPlaceIndex; }
        set { m_PackPlaceIndex = value; }
    }

    private int m_EquipID;

    public int M_EquipID
    {
        get { return m_EquipID; }

        set { m_EquipID = value; }
    }

    #endregion
}