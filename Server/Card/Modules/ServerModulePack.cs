﻿internal class ServerModulePack : ServerModuleBase
{
    internal ServerModuleRetinue M_ModuleRetinue;

    protected override void Initiate()
    {
    }

    protected override void InitializeSideEffects()
    {
        foreach (SideEffectExecute see in CardInfo.SideEffectBundle.SideEffectExecutes)
        {
            foreach (SideEffectBase se in see.SideEffectBases)
            {
                se.Player = ServerPlayer;
                se.M_ExecutorInfo = new ExecutorInfo(
                    clientId: ServerPlayer.ClientId,
                    sideEffectExecutorID: see.ID,
                    retinueId: M_ModuleRetinue.M_RetinueID,
                    equipId: M_EquipID
                );
            }
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
            sideEffectBundle: CardInfo.SideEffectBundle);
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