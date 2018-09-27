internal class ServerModuleMA : ServerModuleBase
{
    internal ServerModuleRetinue M_ModuleRetinue;

    protected override void Initiate()
    {
    }

    protected override void InitializeSideEffects()
    {
        foreach (SideEffectBundle.SideEffectExecute see in CardInfo.SideEffects.GetSideEffects())
        {
            see.SideEffectBase.Player = ServerPlayer;
            see.SideEffectBase.M_ExecuterInfo = new SideEffectBase.ExecuterInfo(clientId: ServerPlayer.ClientId, retinueId: M_ModuleRetinue.M_RetinueID, equipId: M_EquipID);
        }

        foreach (SideEffectBundle.SideEffectExecute see in CardInfo.SideEffects_OnBattleGround.GetSideEffects())
        {
            see.SideEffectBase.Player = ServerPlayer;
            see.SideEffectBase.M_ExecuterInfo = new SideEffectBase.ExecuterInfo(clientId: ServerPlayer.ClientId, retinueId: M_ModuleRetinue.M_RetinueID, equipId: M_EquipID);
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
            sideEffects: CardInfo.SideEffects,
            sideEffects_OnBattleGround: CardInfo.SideEffects_OnBattleGround);
    }


    #region 属性

    private int m_MAPlaceIndex;

    public int M_MAPlaceIndex
    {
        get { return m_MAPlaceIndex; }
        set { m_MAPlaceIndex = value; }
    }

    private int m_EquipID;

    public int M_EquipID
    {
        get { return m_EquipID; }

        set { m_EquipID = value; }
    }

    #endregion
}