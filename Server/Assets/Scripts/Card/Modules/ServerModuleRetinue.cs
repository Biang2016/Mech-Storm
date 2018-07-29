using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

internal class ServerModuleRetinue : ServerModuleBase
{
    public override void Initiate(CardInfo_Base cardInfo, ServerPlayer serverPlayer)
    {
        M_RetinueName = cardInfo.BaseInfo.CardName;
        M_RetinueDesc = cardInfo.BaseInfo.CardDescRaw;
        M_RetinueLeftLife = cardInfo.LifeInfo.Life;
        M_RetinueTotalLife = cardInfo.LifeInfo.Life;
        M_RetinueAttack = cardInfo.BattleInfo.BasicAttack;
        M_RetinueArmor = cardInfo.BattleInfo.BasicArmor;
        M_RetinueShield = cardInfo.BattleInfo.BasicShield;

        M_IsDead = false;
        base.Initiate(cardInfo, serverPlayer);

        foreach (SideEffectBase se in CardInfo.SideEffects_OnDie)
        {
            se.Player = ServerPlayer;
        }

        foreach (SideEffectBase se in CardInfo.SideEffects_OnSummoned)
        {
            se.Player = ServerPlayer;
        }
    }

    public override CardInfo_Base GetCurrentCardInfo()
    {
        return new CardInfo_Retinue(CardInfo.CardID, CardInfo.BaseInfo, CardInfo.UpgradeInfo, CardInfo.LifeInfo, CardInfo.BattleInfo, CardInfo.SlotInfo, CardInfo.SideEffects_OnDie, CardInfo.SideEffects_OnSummoned);
    }

    #region 属性

    private int m_RetinueID;

    public int M_RetinueID
    {
        get { return m_RetinueID; }
        set { m_RetinueID = value; }
    }

    private bool m_IsDead;

    public bool M_IsDead
    {
        get { return m_IsDead; }
        set { m_IsDead = value; }
    }

    private string m_RetinueName;

    public string M_RetinueName
    {
        get { return m_RetinueName; }
        set { m_RetinueName = value; }
    }

    private string m_RetinueDesc;

    public string M_RetinueDesc
    {
        get { return m_RetinueDesc; }
        set { m_RetinueDesc = value; }
    }

    private int m_RetinueLeftLife;

    public int M_RetinueLeftLife
    {
        get { return m_RetinueLeftLife; }
        set
        {
            int before = m_RetinueLeftLife;
            m_RetinueLeftLife = value;
            if (isInitialized && before != m_RetinueLeftLife)
            {
                RetinueAttributesChangeRequest request = new RetinueAttributesChangeRequest(ServerPlayer.ClientId, M_RetinueID, RetinueAttributesChangeRequest.RetinueAttributesChangeFlag.LeftLife, addLeftLife: m_RetinueLeftLife - before);
                ServerPlayer.MyClientProxy.MyServerGameManager.Broadcast_AddRequestToOperationResponse(request);
            }

            if (before < m_RetinueLeftLife)
            {
                OnBeHealed(m_RetinueLeftLife - before);
            }

            if (before > m_RetinueLeftLife)
            {
                OnBeDamaged(before - m_RetinueLeftLife);
            }

            if (m_RetinueLeftLife <= 0)
            {
                OnDie();
                ServerPlayer.MyBattleGroundManager.RemoveRetinue(this);
            }
        }
    }

    private int m_RetinueTotalLife;

    public int M_RetinueTotalLife
    {
        get { return m_RetinueTotalLife; }
        set
        {
            int before = m_RetinueTotalLife;
            m_RetinueTotalLife = value;
            if (isInitialized && before != m_RetinueTotalLife)
            {
                RetinueAttributesChangeRequest request = new RetinueAttributesChangeRequest(ServerPlayer.ClientId, M_RetinueID, RetinueAttributesChangeRequest.RetinueAttributesChangeFlag.MaxLife, addMaxLife: m_RetinueTotalLife - before);
                ServerPlayer.MyClientProxy.MyServerGameManager.Broadcast_AddRequestToOperationResponse(request);
            }
        }
    }

    private int m_RetinueAttack;

    public int M_RetinueAttack
    {
        get { return m_RetinueAttack; }
        set
        {
            int before = m_RetinueAttack;
            m_RetinueAttack = value;
            if (M_Weapon != null)
            {
                M_Weapon.CardInfo.WeaponInfo.Attack = value;
            }

            if (isInitialized && before != m_RetinueAttack)
            {
                RetinueAttributesChangeRequest request = new RetinueAttributesChangeRequest(ServerPlayer.ClientId, M_RetinueID, RetinueAttributesChangeRequest.RetinueAttributesChangeFlag.Attack, addAttack: m_RetinueAttack - before);
                ServerPlayer.MyClientProxy.MyServerGameManager.Broadcast_AddRequestToOperationResponse(request);
            }
        }
    }

    private int m_RetinueWeaponEnergy;

    public int M_RetinueWeaponEnergy
    {
        get { return m_RetinueWeaponEnergy; }
        set
        {
            int before = m_RetinueWeaponEnergy;
            m_RetinueWeaponEnergy = value;
            if (M_Weapon != null)
            {
                M_Weapon.CardInfo.WeaponInfo.Energy = value;
            }

            if (isInitialized && before != m_RetinueWeaponEnergy)
            {
                RetinueAttributesChangeRequest request = new RetinueAttributesChangeRequest(ServerPlayer.ClientId, M_RetinueID, RetinueAttributesChangeRequest.RetinueAttributesChangeFlag.WeaponEnergy, addWeaponEnergy: m_RetinueWeaponEnergy - before);
                ServerPlayer.MyClientProxy.MyServerGameManager.Broadcast_AddRequestToOperationResponse(request);
            }
        }
    }

    private int m_RetinueWeaponEnergyMax;

    public int M_RetinueWeaponEnergyMax
    {
        get { return m_RetinueWeaponEnergyMax; }
        set
        {
            int before = m_RetinueWeaponEnergyMax;
            m_RetinueWeaponEnergyMax = value;
            if (M_Weapon != null)
            {
                M_Weapon.CardInfo.WeaponInfo.EnergyMax = value;
            }

            if (isInitialized && before != m_RetinueWeaponEnergyMax)
            {
                RetinueAttributesChangeRequest request = new RetinueAttributesChangeRequest(ServerPlayer.ClientId, M_RetinueID, RetinueAttributesChangeRequest.RetinueAttributesChangeFlag.WeaponEnergyMax, addWeaponEnergyMax: m_RetinueWeaponEnergyMax - before);
                ServerPlayer.MyClientProxy.MyServerGameManager.Broadcast_AddRequestToOperationResponse(request);
            }
        }
    }

    private int m_RetinueArmor;

    public int M_RetinueArmor
    {
        get { return m_RetinueArmor; }
        set
        {
            int before = m_RetinueArmor;
            m_RetinueArmor = value;
            if (M_Shield != null)
            {
                M_Shield.CardInfo.ShieldInfo.Armor = value;
            }

            if (isInitialized && before != m_RetinueArmor)
            {
                RetinueAttributesChangeRequest request = new RetinueAttributesChangeRequest(ServerPlayer.ClientId, M_RetinueID, RetinueAttributesChangeRequest.RetinueAttributesChangeFlag.Armor, addArmor: m_RetinueArmor - before);
                ServerPlayer.MyClientProxy.MyServerGameManager.Broadcast_AddRequestToOperationResponse(request);
            }
        }
    }

    private int m_RetinueShield;

    public int M_RetinueShield
    {
        get { return m_RetinueShield; }
        set
        {
            int before = m_RetinueShield;
            m_RetinueShield = value;
            if (M_Shield != null)
            {
                M_Shield.CardInfo.ShieldInfo.Shield = value;
            }

            if (isInitialized && before != m_RetinueShield)
            {
                RetinueAttributesChangeRequest request = new RetinueAttributesChangeRequest(ServerPlayer.ClientId, M_RetinueID, RetinueAttributesChangeRequest.RetinueAttributesChangeFlag.Shield, addShield: m_RetinueShield - before);
                ServerPlayer.MyClientProxy.MyServerGameManager.Broadcast_AddRequestToOperationResponse(request);
            }
        }
    }

    #endregion

    #region 拼装上的模块

    #region 武器相关

    private ServerModuleWeapon m_Weapon;

    public ServerModuleWeapon M_Weapon
    {
        get { return m_Weapon; }
        set
        {
            if (m_Weapon != null && value == null)
            {
                On_WeaponDown();
            }
            else if (m_Weapon == null && value != null)
            {
                On_WeaponEquiped(value);
            }
            else if (m_Weapon != value)
            {
                On_WeaponChanged(value);
                return;
            }
        }
    }

    void On_WeaponDown()
    {
        m_Weapon = null;
        EquipWeaponServerRequest request = new EquipWeaponServerRequest(ServerPlayer.ClientId, null, M_RetinueID, 0);
        ServerPlayer.MyClientProxy.MyServerGameManager.Broadcast_AddRequestToOperationResponse(request);
    }

    void On_WeaponEquiped(ServerModuleWeapon newWeapon)
    {
        m_Weapon = newWeapon;
        EquipWeaponServerRequest request = new EquipWeaponServerRequest(ServerPlayer.ClientId, (CardInfo_Weapon) newWeapon.GetCurrentCardInfo(), M_RetinueID, newWeapon.M_WeaponPlaceIndex);
        ServerPlayer.MyClientProxy.MyServerGameManager.Broadcast_AddRequestToOperationResponse(request);

        M_RetinueAttack += newWeapon.CardInfo.WeaponInfo.Attack;
        M_RetinueWeaponEnergy += newWeapon.CardInfo.WeaponInfo.Energy;
        M_RetinueWeaponEnergyMax += newWeapon.CardInfo.WeaponInfo.EnergyMax;
    }

    void On_WeaponChanged(ServerModuleWeapon newWeapon)
    {
        if (AllCards.IsASeries(m_Weapon.CardInfo, newWeapon.CardInfo)) //同类型+1/+1
        {
            if (newWeapon.CardInfo.UpgradeInfo.CardLevel > m_Weapon.CardInfo.UpgradeInfo.CardLevel)
            {
                m_Weapon = newWeapon;
                EquipWeaponServerRequest request = new EquipWeaponServerRequest(ServerPlayer.ClientId, (CardInfo_Weapon) newWeapon.GetCurrentCardInfo(), M_RetinueID, newWeapon.M_WeaponPlaceIndex);
                ServerPlayer.MyClientProxy.MyServerGameManager.Broadcast_AddRequestToOperationResponse(request);
            }

            M_RetinueAttack += 1;
            M_RetinueWeaponEnergy += 1;
            M_RetinueWeaponEnergyMax += 1;
        }
        else //不同类型替换
        {
            m_Weapon = newWeapon;
            EquipWeaponServerRequest request = new EquipWeaponServerRequest(ServerPlayer.ClientId, (CardInfo_Weapon) newWeapon.GetCurrentCardInfo(), M_RetinueID, newWeapon.M_WeaponPlaceIndex);
            ServerPlayer.MyClientProxy.MyServerGameManager.Broadcast_AddRequestToOperationResponse(request);

            M_RetinueAttack = CardInfo.BattleInfo.BasicAttack + newWeapon.CardInfo.WeaponInfo.Attack;
            M_RetinueWeaponEnergy = newWeapon.CardInfo.WeaponInfo.Energy;
            M_RetinueWeaponEnergyMax = newWeapon.CardInfo.WeaponInfo.EnergyMax;
        }
    }

    #endregion


    #region 防具相关

    private ServerModuleShield m_Shield;

    public ServerModuleShield M_Shield
    {
        get { return m_Shield; }
        set
        {
            if (m_Shield != null && value == null)
            {
                On_ShieldDown();
            }
            else if (m_Shield == null && value != null)
            {
                On_ShieldEquiped(value);
            }
            else if (m_Shield != value)
            {
                On_ShieldChanged(value);
                return;
            }
        }
    }

    void On_ShieldDown()
    {
        m_Shield = null;
        EquipShieldServerRequest request = new EquipShieldServerRequest(ServerPlayer.ClientId, null, M_RetinueID, 0);
        ServerPlayer.MyClientProxy.MyServerGameManager.Broadcast_AddRequestToOperationResponse(request);
    }

    void On_ShieldEquiped(ServerModuleShield newShield)
    {
        m_Shield = newShield;
        EquipShieldServerRequest request = new EquipShieldServerRequest(ServerPlayer.ClientId, (CardInfo_Shield) newShield.GetCurrentCardInfo(), M_RetinueID, newShield.M_ShieldPlaceIndex);
        ServerPlayer.MyClientProxy.MyServerGameManager.Broadcast_AddRequestToOperationResponse(request);

        M_RetinueArmor += newShield.CardInfo.ShieldInfo.Armor;
        M_RetinueShield += newShield.CardInfo.ShieldInfo.Shield;
    }

    void On_ShieldChanged(ServerModuleShield newShield) //更换防具时机体基础护甲护盾恢复
    {
        if (AllCards.IsASeries(m_Shield.CardInfo, newShield.CardInfo)) //同类型直接叠加
        {
            if (newShield.CardInfo.UpgradeInfo.CardLevel > m_Shield.CardInfo.UpgradeInfo.CardLevel)
            {
                m_Shield = newShield;
                EquipShieldServerRequest request = new EquipShieldServerRequest(ServerPlayer.ClientId, (CardInfo_Shield) newShield.GetCurrentCardInfo(), M_RetinueID, newShield.M_ShieldPlaceIndex);
                ServerPlayer.MyClientProxy.MyServerGameManager.Broadcast_AddRequestToOperationResponse(request);
            }

            M_RetinueShield += newShield.CardInfo.ShieldInfo.Shield;
            M_RetinueArmor += newShield.CardInfo.ShieldInfo.Armor;
        }
        else //不同类型替换
        {
            m_Shield = newShield;
            EquipShieldServerRequest request = new EquipShieldServerRequest(ServerPlayer.ClientId, (CardInfo_Shield) newShield.GetCurrentCardInfo(), M_RetinueID, newShield.M_ShieldPlaceIndex);
            ServerPlayer.MyClientProxy.MyServerGameManager.Broadcast_AddRequestToOperationResponse(request);

            M_RetinueShield = CardInfo.BattleInfo.BasicShield + newShield.CardInfo.ShieldInfo.Shield;
            M_RetinueArmor = CardInfo.BattleInfo.BasicArmor + newShield.CardInfo.ShieldInfo.Armor;
        }
    }

    #endregion


    internal ServerModuleShield M_Pack;
    internal ServerModuleShield M_MA;

    #endregion

    #region 模块交互

    public void BeAttacked(int attackNumber)
    {
        OnBeAttacked();
        int remainAttackNumber = attackNumber;

        if (M_RetinueShield > 0)
        {
            if (M_RetinueShield >= remainAttackNumber)
            {
                M_RetinueShield--;
                remainAttackNumber = 0;
                return;
            }
            else
            {
                remainAttackNumber -= M_RetinueShield;
                M_RetinueShield /= 2;
            }
        }

        if (M_RetinueArmor > 0)
        {
            if (M_RetinueArmor >= remainAttackNumber)
            {
                M_RetinueArmor = (int) (M_RetinueArmor - remainAttackNumber);
                remainAttackNumber = 0;
                return;
            }
            else
            {
                remainAttackNumber -= M_RetinueArmor;
                M_RetinueArmor = 0;
            }
        }

        if (M_RetinueLeftLife <= remainAttackNumber)
        {
            M_RetinueLeftLife -= M_RetinueLeftLife;
            remainAttackNumber -= M_RetinueLeftLife;
        }
        else
        {
            M_RetinueLeftLife -= remainAttackNumber;
            remainAttackNumber = 0;
            return;
        }
    }

    public List<int> AllModulesAttack() //全模块攻击
    {
        OnAttack();
        List<int> ASeriesOfAttacks = new List<int>();

        if (M_Weapon != null)
        {
            switch (M_Weapon.M_WeaponType)
            {
                case WeaponTypes.Sword:
                    ASeriesOfAttacks.Add(M_RetinueAttack * M_RetinueWeaponEnergy);
                    if (M_RetinueWeaponEnergy < M_RetinueWeaponEnergyMax) M_RetinueWeaponEnergy++;
                    break;
                case WeaponTypes.Gun:
                    int tmp = M_RetinueWeaponEnergy;
                    for (int i = 0; i < tmp; i++)
                    {
                        ASeriesOfAttacks.Add(M_RetinueAttack);
                        M_RetinueWeaponEnergy--;
                    }

                    break;
            }
        }
        else
        {
            ASeriesOfAttacks.Add(M_RetinueAttack);
        }

        foreach (int aSeriesOfAttack in ASeriesOfAttacks)
        {
            OnMakeDamage(aSeriesOfAttack);
        }

        return ASeriesOfAttacks;
    }

    #endregion

    #region 特效、技能

    public void OnSummoned()
    {
        foreach (SideEffectBase se in CardInfo.SideEffects_OnSummoned)
        {
            ServerPlayer.MyGameManager.EnqueueSideEffect(se);
        }

        ServerPlayer.MyGameManager.ExecuteAllSideEffects();
    }

    public void OnDie() //被单杀触发
    {
        if (M_IsDead) return;
        foreach (SideEffectBase se in CardInfo.SideEffects_OnDie)
        {
            ServerPlayer.MyGameManager.EnqueueSideEffect(se);
        }

        RetinueDieRequest request = new RetinueDieRequest(new List<int> {M_RetinueID});
        ServerPlayer.MyClientProxy.MyServerGameManager.Broadcast_AddRequestToOperationResponse(request);

        M_IsDead = true;
        ServerPlayer.MyGameManager.AddDieTogatherRetinuesInfo(M_RetinueID); 
        ServerPlayer.MyGameManager.ExecuteAllSideEffects();
    }

    public void OnDieTogather() //被群杀时触发
    {
        if (M_IsDead) return;
        foreach (SideEffectBase se in CardInfo.SideEffects_OnDie) //先入队死亡效果，但不触发，等到所有被群杀的随从的死亡效果都入队之后再触发
        {
            ServerPlayer.MyGameManager.EnqueueSideEffect(se);
        }

        ServerPlayer.MyGameManager.AddDieTogatherRetinuesInfo(M_RetinueID); //入队死亡信息，整个结算结束后一并发送request
        M_IsDead = true;
    }

    public void OnAttack()
    {
    }

    public void OnMakeDamage(int damage)
    {
    }

    public void OnBeAttacked()
    {
    }

    public void OnBeDamaged(int damage)
    {
    }

    public void OnBeHealed(int healValue)
    {
    }

    public void OnBeginRound()
    {
    }

    public void OnEndRound()
    {
    }

    #endregion
}