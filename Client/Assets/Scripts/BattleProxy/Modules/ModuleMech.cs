using System;
using System.Collections.Generic;

internal class ModuleMech : ModuleBase, ILife
{
    protected override void Initiate()
    {
        m_MechLeftLife = CardInfo.LifeInfo.Life;
        m_MechTotalLife = CardInfo.LifeInfo.Life;
        M_MechAttack = CardInfo.BattleInfo.BasicAttack;
        M_MechArmor = CardInfo.BattleInfo.BasicArmor;
        M_MechShield = CardInfo.BattleInfo.BasicShield;
        AttackTimesThisRound = CardInfo.MechInfo.IsCharger ? (IsFrenzy ? 2 : 1) : 0;
        IsFirstRound = true;
        CannotAttackBecauseDie = false;
        M_IsDead = false;
    }

    protected override void InitializeSideEffects()
    {
        foreach (SideEffectExecute see in CardInfo.SideEffectBundle.SideEffectExecutes)
        {
            foreach (SideEffectBase se in see.SideEffectBases)
            {
                se.Player = BattlePlayer;
                se.M_ExecutorInfo = new ExecutorInfo(
                    BattlePlayer.ClientId,
                    sideEffectExecutorID: see.ID,
                    mechId: M_MechID
                );
            }
        }

        foreach (SideEffectExecute see in CardInfo.SideEffectBundle_BattleGroundAura.SideEffectExecutes)
        {
            foreach (SideEffectBase se in see.SideEffectBases)
            {
                se.Player = BattlePlayer;
                se.M_ExecutorInfo = new ExecutorInfo(
                    BattlePlayer.ClientId,
                    sideEffectExecutorID: see.ID,
                    mechId: M_MechID
                );
            }
        }
    }

    public override CardInfo_Base GetCurrentCardInfo()
    {
        return new CardInfo_Mech(
            cardID: CardInfo.CardID,
            baseInfo: CardInfo.BaseInfo,
            upgradeInfo: CardInfo.UpgradeInfo,
            lifeInfo: CardInfo.LifeInfo,
            battleInfo: CardInfo.BattleInfo,
            mechInfo: CardInfo.MechInfo,
            sideEffectBundle: CardInfo.SideEffectBundle,
            sideEffectBundle_BattleGroundAura: CardInfo.SideEffectBundle_BattleGroundAura);
    }

    #region 属性

    private int m_MechID;

    public int M_MechID
    {
        get { return m_MechID; }
        set { m_MechID = value; }
    }

    public int M_ClientTempMechID { get; set; }

    public (int, bool) M_TargetMechID
    {
        get
        {
            bool isTemp = M_ClientTempMechID != (int) Const.SpecialMechID.ClientTempMechIDNormal;
            int targetMechID = isTemp ? M_ClientTempMechID : M_MechID;
            return (targetMechID, isTemp);
        }
    }

    private bool m_IsDead;

    public bool M_IsDead
    {
        get { return m_IsDead; }
        set { m_IsDead = value; }
    }

    private int m_ImmuneLeftRounds = 0;

    public int M_ImmuneLeftRounds
    {
        get { return m_ImmuneLeftRounds; }
        set
        {
            if (m_ImmuneLeftRounds != value)
            {
                MechImmuneStateRequest request = new MechImmuneStateRequest(BattlePlayer.ClientId, M_MechID, value);
                BattlePlayer.GameManager.Broadcast_AddRequestToOperationResponse(request);
            }

            m_ImmuneLeftRounds = value;
        }
    }

    private int m_InactivityRounds = 0;

    public int M_InactivityRounds
    {
        get { return m_InactivityRounds; }
        set
        {
            if (m_InactivityRounds != value)
            {
                MechInactivityStateRequest request = new MechInactivityStateRequest(BattlePlayer.ClientId, M_MechID, value);
                BattlePlayer.GameManager.Broadcast_AddRequestToOperationResponse(request);
                m_InactivityRounds = value;
                CheckCanAttack();
            }
        }
    }

    protected virtual void OnLifeChanged(int change, bool isOverflow)
    {
        MechAttributesChangeRequest request = new MechAttributesChangeRequest(BattlePlayer.ClientId, M_MechID, addLeftLife: change);
        BattlePlayer.MyClientProxy.BattleGameManager.Broadcast_AddRequestToOperationResponse(request);
    }

    protected virtual void OnHeal(int change, bool isOverflow)
    {
        OnBeHealed(change);
    }

    protected virtual void OnDamage(int change)
    {
        OnBeDamaged(change);
    }

    protected virtual void OnMaxLifeChanged(int change)
    {
        MechAttributesChangeRequest request = new MechAttributesChangeRequest(BattlePlayer.ClientId, M_MechID, addMaxLife: change);
        BattlePlayer.MyClientProxy.BattleGameManager.Broadcast_AddRequestToOperationResponse(request);
    }

    protected virtual void OnMaxLifeIncrease(int change)
    {
    }

    protected virtual void OnMaxLifeReduce(int change)
    {
    }

    protected virtual void LifeChange(int change, bool trigger = true)
    {
        if (M_IsDead) return;
        bool isOverflow = m_MechLeftLife + change > m_MechTotalLife;
        if (change > 0)
        {
            change = Math.Min(m_MechTotalLife - m_MechLeftLife, change);
            m_MechLeftLife += change;
            if (trigger) OnHeal(change, isOverflow);
        }
        else
        {
            change = Math.Max(-m_MechLeftLife, change);
            m_MechLeftLife += change;
            if (trigger) OnDamage(-change);
        }

        if (trigger) OnLifeChanged(change, isOverflow);
    }

    protected virtual void MaxLifeChange(int change, bool trigger = true)
    {
        if (M_IsDead) return;
        if (change > 0)
        {
            m_MechTotalLife += change;
            if (trigger) OnMaxLifeIncrease(change);
        }
        else
        {
            change = Math.Max(-m_MechTotalLife, change);
            m_MechTotalLife += change;

            if (trigger) OnMaxLifeReduce(-change);
            if (m_MechTotalLife < m_MechLeftLife)
            {
                int lifeChange = m_MechLeftLife - m_MechTotalLife;
                m_MechLeftLife = m_MechTotalLife;
                if (trigger) OnLifeChanged(lifeChange, false);
            }
        }

        if (trigger) OnMaxLifeChanged(change);
    }

    #region ILife

    public void AddLife(int addLifeValue)
    {
        MaxLifeChange(addLifeValue);
        LifeChange(addLifeValue);
    }

    public void Heal(int healValue)
    {
        LifeChange(healValue);
    }

    public void Damage(int damage)
    {
        BeAttacked(damage);
        CheckAlive();
    }

    public void Change(int changeValue)
    {
        LifeChange(changeValue);
    }

    public void HealAll()
    {
        LifeChange(m_MechTotalLife - m_MechLeftLife);
    }

    public void ChangeMaxLife(int change)
    {
        MaxLifeChange(change);
    }

    private int m_MechLeftLife;

    public int M_MechLeftLife
    {
        get { return m_MechLeftLife; }
    }

    private int m_MechTotalLife;

    public int M_MechTotalLife
    {
        get { return m_MechTotalLife; }
    }

    public bool CheckAlive()
    {
        if (M_MechLeftLife == 0)
        {
            OnDieTogether();
            return false;
        }

        return true;
    }

    private int m_MechAttack;

    public int M_MechAttack
    {
        get { return m_MechAttack; }
        set
        {
            int before = m_MechAttack;
            m_MechAttack = value;
            if (M_Weapon != null)
            {
                M_Weapon.CardInfo.WeaponInfo.Attack = value;
            }

            if (isInitialized && before != m_MechAttack)
            {
                MechAttributesChangeRequest request = new MechAttributesChangeRequest(BattlePlayer.ClientId, M_MechID, addAttack: m_MechAttack - before);
                BattlePlayer.MyClientProxy.BattleGameManager.Broadcast_AddRequestToOperationResponse(request);
            }

            CheckCanAttack();
        }
    }

    private int m_MechWeaponEnergy;

    public int M_MechWeaponEnergy
    {
        get { return m_MechWeaponEnergy; }
        set
        {
            if (M_Weapon == null)
            {
                m_MechWeaponEnergy = 0;
            }
            else
            {
                int before = m_MechWeaponEnergy;
                m_MechWeaponEnergy = value;
                M_Weapon.CardInfo.WeaponInfo.Energy = value;
                if (value == 0)
                {
                    M_Weapon = null;
                }

                if (isInitialized && before != m_MechWeaponEnergy)
                {
                    int beforeAttack = m_MechAttack;
                    if (m_MechWeaponEnergy == 0) m_MechAttack = CardInfo.BattleInfo.BasicAttack;
                    else
                    {
                        m_MechAttack = CardInfo.BattleInfo.BasicAttack + (M_Weapon?.CardInfo.WeaponInfo.Attack ?? 0);
                    }

                    MechAttributesChangeRequest request = new MechAttributesChangeRequest(BattlePlayer.ClientId, M_MechID, addAttack: m_MechAttack - beforeAttack, addWeaponEnergy: m_MechWeaponEnergy - before);
                    BattlePlayer.MyClientProxy.BattleGameManager.Broadcast_AddRequestToOperationResponse(request);
                }

                CheckCanAttack();
            }
        }
    }

    private int m_MechWeaponEnergyMax;

    public int M_MechWeaponEnergyMax
    {
        get { return m_MechWeaponEnergyMax; }
        set
        {
            if (M_Weapon == null)
            {
                m_MechWeaponEnergyMax = 0;
            }
            else
            {
                int before = m_MechWeaponEnergyMax;
                m_MechWeaponEnergyMax = value;
                M_Weapon.CardInfo.WeaponInfo.EnergyMax = value;

                if (isInitialized && before != m_MechWeaponEnergyMax)
                {
                    MechAttributesChangeRequest request = new MechAttributesChangeRequest(BattlePlayer.ClientId, M_MechID, addWeaponEnergyMax: m_MechWeaponEnergyMax - before);
                    BattlePlayer.MyClientProxy.BattleGameManager.Broadcast_AddRequestToOperationResponse(request);
                }

                CheckCanAttack();
            }
        }
    }

    private int m_MechArmor;

    public int M_MechArmor
    {
        get { return m_MechArmor; }
        set
        {
            int before = m_MechArmor;
            m_MechArmor = value;
            if (M_Shield != null)
            {
                M_Shield.CardInfo.ShieldInfo.Armor = value;
            }

            if (isInitialized && before != m_MechArmor)
            {
                MechAttributesChangeRequest request = new MechAttributesChangeRequest(BattlePlayer.ClientId, M_MechID, addArmor: m_MechArmor - before);
                BattlePlayer.MyClientProxy.BattleGameManager.Broadcast_AddRequestToOperationResponse(request);
            }

            if (M_Shield != null)
            {
                if (value <= 0)
                {
                    if (M_MechShield <= 0)
                    {
                        M_Shield = null;
                    }
                }
            }
        }
    }

    public int MechShieldFull;

    private int m_MechShield;

    public int M_MechShield
    {
        get { return m_MechShield; }
        set
        {
            MechShieldFull = Math.Max(value, MechShieldFull);
            int before = m_MechShield;
            m_MechShield = value;
            if (M_Shield != null)
            {
                M_Shield.CardInfo.ShieldInfo.Shield = value;
            }

            if (isInitialized && before != m_MechShield)
            {
                MechAttributesChangeRequest request = new MechAttributesChangeRequest(BattlePlayer.ClientId, M_MechID, addShield: m_MechShield - before);
                BattlePlayer.MyClientProxy.BattleGameManager.Broadcast_AddRequestToOperationResponse(request);
            }

            if (M_Shield != null)
            {
                if (value <= 0)
                {
                    if (M_MechArmor <= 0)
                    {
                        M_Shield = null;
                    }
                }
            }
        }
    }

    #endregion

    #endregion

    #region 拼装上的模块

    internal bool IsAllEquipExceptMA
    {
        get
        {
            if (CardInfo.MechInfo.Slots[0] != SlotTypes.None && M_Weapon == null) return false;
            if (CardInfo.MechInfo.Slots[1] != SlotTypes.None && M_Shield == null) return false;
            if (CardInfo.MechInfo.Slots[2] != SlotTypes.None && M_Pack == null) return false;
            return true;
        }
    }

    #region 武器相关

    private ModuleWeapon m_Weapon;

    public ModuleWeapon M_Weapon
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
            }

            CheckCanAttack();
        }
    }

    void On_WeaponDown()
    {
        if (m_Weapon != null)
        {
            if (!M_Weapon.CardInfo.BaseInfo.IsTemp) BattlePlayer.CardDeckManager.CardDeck.RecycleCardInstanceID(m_Weapon.OriginCardInstanceId);
            BattlePlayer.GameManager.EventManager.Invoke(SideEffectExecute.TriggerTime.OnEquipDie, new ExecutorInfo(BattlePlayer.ClientId, M_MechID, equipId: m_Weapon.M_EquipID));
            m_Weapon.UnRegisterSideEffect();

            EquipWeaponServerRequest request = new EquipWeaponServerRequest(BattlePlayer.ClientId, null, M_MechID, m_Weapon.M_EquipID);
            BattlePlayer.MyClientProxy.BattleGameManager.Broadcast_AddRequestToOperationResponse(request);
            m_Weapon = null;

            int att_before = m_MechAttack;
            int we_before = m_MechWeaponEnergy;
            int weMax_before = m_MechWeaponEnergyMax;

            m_MechAttack = CardInfo.BattleInfo.BasicAttack;
            m_MechWeaponEnergy = 0;
            m_MechWeaponEnergyMax = 0;

            MechAttributesChangeRequest request2 = new MechAttributesChangeRequest(BattlePlayer.ClientId, M_MechID, addAttack: m_MechAttack - att_before, addWeaponEnergy: m_MechWeaponEnergy - we_before, addWeaponEnergyMax: m_MechWeaponEnergyMax - weMax_before);
            BattlePlayer.MyClientProxy.BattleGameManager.Broadcast_AddRequestToOperationResponse(request2);
        }
    }

    void On_WeaponEquiped(ModuleWeapon newWeapon)
    {
        m_Weapon = newWeapon;
        BattlePlayer.GameManager.EventManager.Invoke(SideEffectExecute.TriggerTime.OnEquipEquiped, new ExecutorInfo(BattlePlayer.ClientId, M_MechID, equipId: m_Weapon.M_EquipID));
        EquipWeaponServerRequest request = new EquipWeaponServerRequest(BattlePlayer.ClientId, (CardInfo_Equip) newWeapon.GetCurrentCardInfo(), M_MechID, m_Weapon.M_EquipID);
        BattlePlayer.MyClientProxy.BattleGameManager.Broadcast_AddRequestToOperationResponse(request);

        RefreshAttackTime();
        int att_before = m_MechAttack;
        int we_before = m_MechWeaponEnergy;
        int weMax_before = m_MechWeaponEnergyMax;
        m_MechAttack += newWeapon.CardInfo.WeaponInfo.Attack;
        m_MechWeaponEnergyMax += newWeapon.CardInfo.WeaponInfo.EnergyMax;
        m_MechWeaponEnergy += newWeapon.CardInfo.WeaponInfo.Energy;
        if (m_MechWeaponEnergy == 0) m_MechAttack = CardInfo.BattleInfo.BasicAttack;

        MechAttributesChangeRequest request2 = new MechAttributesChangeRequest(BattlePlayer.ClientId, M_MechID, addAttack: m_MechAttack - att_before, addWeaponEnergy: m_MechWeaponEnergy - we_before, addWeaponEnergyMax: m_MechWeaponEnergyMax - weMax_before);
        BattlePlayer.MyClientProxy.BattleGameManager.Broadcast_AddRequestToOperationResponse(request2);
    }

    void On_WeaponChanged(ModuleWeapon newWeapon)
    {
        if (m_Weapon != null)
        {
            if (!M_Weapon.CardInfo.BaseInfo.IsTemp) BattlePlayer.CardDeckManager.CardDeck.RecycleCardInstanceID(m_Weapon.OriginCardInstanceId);
            m_Weapon.UnRegisterSideEffect();
        }

        m_Weapon = newWeapon;
        BattlePlayer.GameManager.EventManager.Invoke(SideEffectExecute.TriggerTime.OnEquipEquiped, new ExecutorInfo(BattlePlayer.ClientId, M_MechID, equipId: m_Weapon.M_EquipID));
        EquipWeaponServerRequest request = new EquipWeaponServerRequest(BattlePlayer.ClientId, (CardInfo_Equip) newWeapon.GetCurrentCardInfo(), M_MechID, m_Weapon.M_EquipID);
        BattlePlayer.MyClientProxy.BattleGameManager.Broadcast_AddRequestToOperationResponse(request);

        RefreshAttackTime();
        int att_before = m_MechAttack;
        int we_before = m_MechWeaponEnergy;
        int weMax_before = m_MechWeaponEnergyMax;
        m_MechAttack = CardInfo.BattleInfo.BasicAttack + newWeapon.CardInfo.WeaponInfo.Attack;
        m_MechWeaponEnergyMax = newWeapon.CardInfo.WeaponInfo.EnergyMax;
        m_MechWeaponEnergy = newWeapon.CardInfo.WeaponInfo.Energy;
        if (m_MechWeaponEnergy == 0) m_MechAttack = CardInfo.BattleInfo.BasicAttack;

        MechAttributesChangeRequest request2 = new MechAttributesChangeRequest(BattlePlayer.ClientId, M_MechID, addAttack: m_MechAttack - att_before, addWeaponEnergy: m_MechWeaponEnergy - we_before, addWeaponEnergyMax: m_MechWeaponEnergyMax - weMax_before);
        BattlePlayer.MyClientProxy.BattleGameManager.Broadcast_AddRequestToOperationResponse(request2);
    }

    void RefreshAttackTime()
    {
        if (AttackTimesThisRound > 0) //如果攻击次数还未用完
        {
            if (!CardInfo.MechInfo.IsFrenzy && M_Weapon.CardInfo.WeaponInfo.IsFrenzy) //如果枪械为狂暴状态(如果机甲为狂暴则无效)，则增加攻击次数到2
            {
                AttackTimesThisRound = 2;
            }
            else if (M_Weapon.CardInfo.WeaponInfo.IsSentry) //如果枪械为哨戒模式，则攻击次数清零
            {
                AttackTimesThisRound = 0;
            }
        }
    }

    #endregion

    #region 防具相关

    private ModuleShield m_Shield;

    public ModuleShield M_Shield
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
            }

            CheckCanAttack();
        }
    }

    void On_ShieldDown()
    {
        if (m_Shield != null)
        {
            if (!M_Shield.CardInfo.BaseInfo.IsTemp) BattlePlayer.CardDeckManager.CardDeck.RecycleCardInstanceID(m_Shield.OriginCardInstanceId);
            BattlePlayer.GameManager.EventManager.Invoke(SideEffectExecute.TriggerTime.OnEquipDie, new ExecutorInfo(BattlePlayer.ClientId, M_MechID, equipId: m_Shield.M_EquipID));
            m_Shield.UnRegisterSideEffect();

            EquipShieldServerRequest request = new EquipShieldServerRequest(BattlePlayer.ClientId, null, M_MechID, m_Shield.M_EquipID);
            BattlePlayer.MyClientProxy.BattleGameManager.Broadcast_AddRequestToOperationResponse(request);
            m_Shield = null;

            int addShield = 0;
            if (m_MechShield >= CardInfo.BattleInfo.BasicShield)
            {
                addShield = CardInfo.BattleInfo.BasicShield - m_MechShield;
            }

            int addArmor = 0;
            if (m_MechArmor >= CardInfo.BattleInfo.BasicArmor)
            {
                addArmor = CardInfo.BattleInfo.BasicArmor - m_MechArmor;
            }

            MechAttributesChangeRequest request2 = new MechAttributesChangeRequest(BattlePlayer.ClientId, M_MechID, addShield: addShield, addArmor: addArmor);
            BattlePlayer.MyClientProxy.BattleGameManager.Broadcast_AddRequestToOperationResponse(request2);
        }
    }

    void On_ShieldEquiped(ModuleShield newShield)
    {
        m_Shield = newShield;
        BattlePlayer.GameManager.EventManager.Invoke(SideEffectExecute.TriggerTime.OnEquipEquiped, new ExecutorInfo(BattlePlayer.ClientId, M_MechID, equipId: m_Shield.M_EquipID));
        EquipShieldServerRequest request = new EquipShieldServerRequest(BattlePlayer.ClientId, (CardInfo_Equip) newShield.GetCurrentCardInfo(), M_MechID, m_Shield.M_EquipID);
        BattlePlayer.MyClientProxy.BattleGameManager.Broadcast_AddRequestToOperationResponse(request);

        int shield_before = m_MechShield;
        int armor_before = m_MechArmor;
        m_MechArmor += newShield.CardInfo.ShieldInfo.Armor;
        m_MechShield += newShield.CardInfo.ShieldInfo.Shield;

        MechAttributesChangeRequest request2 = new MechAttributesChangeRequest(BattlePlayer.ClientId, M_MechID, addShield: m_MechShield - shield_before, addArmor: m_MechArmor - armor_before);
        BattlePlayer.MyClientProxy.BattleGameManager.Broadcast_AddRequestToOperationResponse(request2);
    }

    void On_ShieldChanged(ModuleShield newShield) //更换防具时机体基础护甲护盾恢复
    {
        if (m_Shield != null)
        {
            if (!M_Shield.CardInfo.BaseInfo.IsTemp) BattlePlayer.CardDeckManager.CardDeck.RecycleCardInstanceID(m_Shield.OriginCardInstanceId);
            m_Shield.UnRegisterSideEffect();
        }

        m_Shield = newShield;
        BattlePlayer.GameManager.EventManager.Invoke(SideEffectExecute.TriggerTime.OnEquipEquiped, new ExecutorInfo(BattlePlayer.ClientId, M_MechID, equipId: m_Shield.M_EquipID));
        EquipShieldServerRequest request = new EquipShieldServerRequest(BattlePlayer.ClientId, (CardInfo_Equip) newShield.GetCurrentCardInfo(), M_MechID, m_Shield.M_EquipID);
        BattlePlayer.MyClientProxy.BattleGameManager.Broadcast_AddRequestToOperationResponse(request);

        int shield_before = m_MechShield;
        int armor_before = m_MechArmor;
        m_MechShield = CardInfo.BattleInfo.BasicShield + newShield.CardInfo.ShieldInfo.Shield;
        m_MechArmor = CardInfo.BattleInfo.BasicArmor + newShield.CardInfo.ShieldInfo.Armor;

        MechAttributesChangeRequest request2 = new MechAttributesChangeRequest(BattlePlayer.ClientId, M_MechID, addShield: m_MechShield - shield_before, addArmor: m_MechArmor - armor_before);
        BattlePlayer.MyClientProxy.BattleGameManager.Broadcast_AddRequestToOperationResponse(request2);
    }

    #endregion

    #region 飞行背包相关

    private ModulePack m_Pack;

    public ModulePack M_Pack
    {
        get { return m_Pack; }
        set
        {
            if (m_Pack != null && value == null)
            {
                On_PackDown();
            }
            else if (m_Pack == null && value != null)
            {
                On_PackEquiped(value);
            }
            else if (m_Pack != value)
            {
                On_PackChanged(value);
            }

            CheckCanAttack();
        }
    }

    void On_PackDown()
    {
        if (m_Pack != null)
        {
            if (!M_Pack.CardInfo.BaseInfo.IsTemp) BattlePlayer.CardDeckManager.CardDeck.RecycleCardInstanceID(m_Pack.OriginCardInstanceId);
            BattlePlayer.GameManager.EventManager.Invoke(SideEffectExecute.TriggerTime.OnEquipDie, new ExecutorInfo(BattlePlayer.ClientId, M_MechID, equipId: m_Pack.M_EquipID));
            m_Pack.UnRegisterSideEffect();

            EquipPackServerRequest request = new EquipPackServerRequest(BattlePlayer.ClientId, null, M_MechID, m_Pack.M_EquipID);
            BattlePlayer.MyClientProxy.BattleGameManager.Broadcast_AddRequestToOperationResponse(request);
            m_Pack = null;
        }
    }

    void On_PackEquiped(ModulePack newPack)
    {
        m_Pack = newPack;
        BattlePlayer.GameManager.EventManager.Invoke(SideEffectExecute.TriggerTime.OnEquipEquiped, new ExecutorInfo(BattlePlayer.ClientId, M_MechID, equipId: m_Pack.M_EquipID));
        EquipPackServerRequest request = new EquipPackServerRequest(BattlePlayer.ClientId, (CardInfo_Equip) newPack.GetCurrentCardInfo(), M_MechID, m_Pack.M_EquipID);
        BattlePlayer.MyClientProxy.BattleGameManager.Broadcast_AddRequestToOperationResponse(request);
    }

    void On_PackChanged(ModulePack newPack)
    {
        if (m_Pack != null)
        {
            if (!M_Pack.CardInfo.BaseInfo.IsTemp) BattlePlayer.CardDeckManager.CardDeck.RecycleCardInstanceID(m_Pack.OriginCardInstanceId);
            m_Pack.UnRegisterSideEffect();
        }

        m_Pack = newPack;
        BattlePlayer.GameManager.EventManager.Invoke(SideEffectExecute.TriggerTime.OnEquipEquiped, new ExecutorInfo(BattlePlayer.ClientId, M_MechID, equipId: m_Pack.M_EquipID));
        EquipPackServerRequest request = new EquipPackServerRequest(BattlePlayer.ClientId, (CardInfo_Equip) newPack.GetCurrentCardInfo(), M_MechID, m_Pack.M_EquipID);
        BattlePlayer.MyClientProxy.BattleGameManager.Broadcast_AddRequestToOperationResponse(request);
    }

    #endregion

    #region MA相关

    private ModuleMA m_MA;

    public ModuleMA M_MA
    {
        get { return m_MA; }
        set
        {
            if (m_MA != null && value == null)
            {
                On_MADown();
            }
            else if (m_MA == null && value != null)
            {
                On_MAEquiped(value);
            }
            else if (m_MA != value)
            {
                On_MAChanged(value);
            }

            CheckCanAttack();
        }
    }

    void On_MADown()
    {
        if (m_MA != null)
        {
            if (!M_MA.CardInfo.BaseInfo.IsTemp) BattlePlayer.CardDeckManager.CardDeck.RecycleCardInstanceID(m_MA.OriginCardInstanceId);
            BattlePlayer.GameManager.EventManager.Invoke(SideEffectExecute.TriggerTime.OnEquipDie, new ExecutorInfo(BattlePlayer.ClientId, M_MechID, equipId: m_MA.M_EquipID));
            m_MA.UnRegisterSideEffect();

            EquipMAServerRequest request = new EquipMAServerRequest(BattlePlayer.ClientId, null, M_MechID, m_MA.M_EquipID);
            BattlePlayer.MyClientProxy.BattleGameManager.Broadcast_AddRequestToOperationResponse(request);
            m_MA = null;
        }
    }

    void On_MAEquiped(ModuleMA newMA)
    {
        m_MA = newMA;
        BattlePlayer.GameManager.EventManager.Invoke(SideEffectExecute.TriggerTime.OnEquipEquiped, new ExecutorInfo(BattlePlayer.ClientId, M_MechID, equipId: m_MA.M_EquipID));
        EquipMAServerRequest request = new EquipMAServerRequest(BattlePlayer.ClientId, (CardInfo_Equip) newMA.GetCurrentCardInfo(), M_MechID, m_MA.M_EquipID);
        BattlePlayer.MyClientProxy.BattleGameManager.Broadcast_AddRequestToOperationResponse(request);
    }

    void On_MAChanged(ModuleMA newMA)
    {
        if (m_MA != null)
        {
            if (!M_MA.CardInfo.BaseInfo.IsTemp) BattlePlayer.CardDeckManager.CardDeck.RecycleCardInstanceID(m_MA.OriginCardInstanceId);
            m_MA.UnRegisterSideEffect();
        }

        m_MA = newMA;
        BattlePlayer.GameManager.EventManager.Invoke(SideEffectExecute.TriggerTime.OnEquipEquiped, new ExecutorInfo(BattlePlayer.ClientId, M_MechID, equipId: m_MA.M_EquipID));
        EquipMAServerRequest request = new EquipMAServerRequest(BattlePlayer.ClientId, (CardInfo_Equip) newMA.GetCurrentCardInfo(), M_MechID, m_MA.M_EquipID);
        BattlePlayer.MyClientProxy.BattleGameManager.Broadcast_AddRequestToOperationResponse(request);
    }

    #endregion

    #endregion

    #region MechType

    public bool IsFrenzy
    {
        get
        {
            return CardInfo.MechInfo.IsFrenzy ||
                   (M_Weapon != null && M_Weapon.CardInfo.WeaponInfo.IsFrenzy) ||
                   (M_Pack != null && M_Pack.CardInfo.PackInfo.IsFrenzy) ||
                   (M_MA != null && M_MA.CardInfo.MAInfo.IsFrenzy);
        }
    }

    public bool IsSentry
    {
        get { return (M_Weapon != null && M_Weapon.CardInfo.WeaponInfo.IsSentry); }
    }

    public bool IsSniper
    {
        get
        {
            return CardInfo.MechInfo.IsSniper ||
                   (M_Pack != null && M_Pack.CardInfo.PackInfo.IsSniper) ||
                   (M_MA != null && M_MA.CardInfo.MAInfo.IsSniper);
        }
    }

    public bool IsDefender
    {
        get
        {
            return CardInfo.MechInfo.IsDefense ||
                   (M_Shield != null && M_Shield.CardInfo.ShieldInfo.IsDefense) ||
                   (M_Pack != null && M_Pack.CardInfo.PackInfo.IsDefense) ||
                   (M_MA != null && M_MA.CardInfo.MAInfo.IsDefense);
        }
    }

    public int DodgeProp
    {
        get
        {
            if (M_Pack != null) return M_Pack.CardInfo.PackInfo.DodgeProp;
            return 0;
        }
    }

    #endregion

    #region 模块交互

    private bool canAttack;

    public bool CanAttack
    {
        get { return canAttack; }
        set
        {
            if (canAttack && !value)
            {
                BattlePlayer.BattleGroundManager.CanAttackMechs.Remove(M_MechID);
            }

            if (!canAttack && value)
            {
                BattlePlayer.BattleGroundManager.CanAttackMechs.Add(M_MechID);
            }

            if (canAttack != value)
            {
                canAttack = value;
                MechCanAttackRequest request = new MechCanAttackRequest(BattlePlayer.ClientId, M_MechID, value);
                BattlePlayer.GameManager.Broadcast_AddRequestToOperationResponse(request);
            }
        }
    }

    public bool CheckMechCanAttackMe(ModuleMech attackMech)
    {
        if (M_ImmuneLeftRounds != 0) return false;
        if (attackMech.M_Weapon != null && attackMech.M_Weapon.M_WeaponType == WeaponTypes.SniperGun && attackMech.M_MechWeaponEnergy != 0) return true; //狙击枪可以越过嘲讽机甲，其他武器只能攻击嘲讽机甲
        if (BattlePlayer.BattleGroundManager.HasDefenseMech && !IsDefender) return false;
        return true;
    }

    private bool isFirstRound = true; //是否是召唤的第一回合

    public bool IsFirstRound
    {
        get { return isFirstRound; }
        set
        {
            isFirstRound = value;
            CheckCanAttack();
        }
    }

    private bool cannotAttackBecauseDie = false; //是否已预先判定死亡

    public bool CannotAttackBecauseDie
    {
        get { return cannotAttackBecauseDie; }
        set
        {
            cannotAttackBecauseDie = value;
            CheckCanAttack();
        }
    }

    private int attackTimesThisRound = 1; //本回合攻击次数

    public int AttackTimesThisRound
    {
        get => attackTimesThisRound;
        set
        {
            attackTimesThisRound = value;
            CheckCanAttack();
        }
    }

    private void CheckCanAttack()
    {
        bool res = true;
        res &= BattlePlayer.GameManager.CurrentPlayer == BattlePlayer;
        res &= !IsFirstRound || (IsFirstRound && CanCharge);
        res &= (M_InactivityRounds == 0);
        res &= (!CannotAttackBecauseDie);
        res &= AttackTimesThisRound > 0;
        res &= (M_MechAttack > 0);
        res &= !EndRound;

        CanAttack = res;
    }

    private bool canCharge = false; //冲锋

    public bool CanCharge
    {
        get => canCharge;
        set => canCharge = value;
    }

    private bool endRound = false; //回合结束后

    public bool EndRound
    {
        get => endRound;
        set => endRound = value;
    }

    public void BeAttacked(int attackNumber) //攻击和被攻击仅发送伤害数值给客户端，具体计算分别处理
    {
        if (M_IsDead) return;
        OnBeAttacked();
        if (M_ImmuneLeftRounds != 0) attackNumber = 0; //免疫状态不受伤
        int remainAttackNumber = attackNumber;

        //小于等于护盾的伤害的全部免除，护盾无任何损失，大于护盾的伤害，每超过一点，护盾受到一点伤害，如果扣为0，则护盾破坏
        if (M_MechShield > 0)
        {
            if (M_MechShield >= remainAttackNumber)
            {
                ShieldDefenseDamage(remainAttackNumber, M_MechShield);
                remainAttackNumber = 0;
                return;
            }
            else
            {
                int shieldDecrease = remainAttackNumber - M_MechShield;
                remainAttackNumber -= M_MechShield;
                ShieldDefenseDamage(M_MechShield, M_MechShield);
                M_MechShield -= Math.Min(m_MechShield, shieldDecrease);
                if (m_MechShield == 0 && m_MechArmor == 0)
                {
                    M_Shield = null;
                }
            }
        }

        if (M_MechArmor > 0)
        {
            if (M_MechArmor >= remainAttackNumber)
            {
                M_MechArmor = M_MechArmor - remainAttackNumber;
                remainAttackNumber = 0;
                return;
            }
            else
            {
                remainAttackNumber -= M_MechArmor;
                M_MechArmor = 0;
                if (m_MechShield == 0 && m_MechArmor == 0)
                {
                    M_Shield = null;
                }
            }
        }

        if (M_MechLeftLife <= remainAttackNumber)
        {
            LifeChange(-m_MechLeftLife);
            remainAttackNumber -= M_MechLeftLife;
        }
        else
        {
            LifeChange(-remainAttackNumber);
            remainAttackNumber = 0;
            return;
        }
    }

    private void ShieldDefenseDamage(int decreaseValue, int shieldValue)
    {
        MechShieldDefenseRequest request = new MechShieldDefenseRequest(BattlePlayer.ClientId, M_MechID, decreaseValue, shieldValue);
        BattlePlayer.GameManager.Broadcast_AddRequestToOperationResponse(request);
    }

    private void OnBeAttacked()
    {
    }

    public void OnDodge()
    {
        MechDodgeRequest request = new MechDodgeRequest(BattlePlayer.ClientId, M_MechID);
        BattlePlayer.GameManager.Broadcast_AddRequestToOperationResponse(request);
    }

    private enum AttackLevel
    {
        Sword = 0,
        Gun = 1,
        SniperGun = 2
    }

    private AttackLevel M_AttackLevel
    {
        get
        {
            if (M_Weapon == null || M_MechWeaponEnergy == 0) return AttackLevel.Sword;
            if (M_Weapon.M_WeaponType == WeaponTypes.Gun) return AttackLevel.Gun;
            if (M_Weapon.M_WeaponType == WeaponTypes.SniperGun) return AttackLevel.SniperGun;
            return AttackLevel.Sword;
        }
    }

    public bool BeforeAttack(ModuleMech targetMech, bool isCounterAttack)
    {
        if (M_IsDead) return false;
        if (!isCounterAttack) OnAttack();
        return true;
    }

    void OnAttack(WeaponTypes weaponType, int targetMechId)
    {
        MechOnAttackRequest request = new MechOnAttackRequest(BattlePlayer.ClientId, M_MechID, targetMechId, weaponType);
        BattlePlayer.GameManager.Broadcast_AddRequestToOperationResponse(request);
    }

    void OnAttackShip(WeaponTypes weaponType, int targetClientId)
    {
        MechOnAttackShipRequest request = new MechOnAttackShipRequest(BattlePlayer.ClientId, M_MechID, targetClientId, weaponType);
        BattlePlayer.GameManager.Broadcast_AddRequestToOperationResponse(request);
    }

    public void Attack(ModuleMech targetMech, bool isCounterAttack) //服务器客户单分别计算
    {
        int damage = 0;

        bool canCounter = !isCounterAttack && M_AttackLevel <= targetMech.M_AttackLevel; //对方能否反击

        if (M_Weapon != null && M_MechWeaponEnergy != 0)
        {
            switch (M_Weapon.M_WeaponType)
            {
                case WeaponTypes.Sword:
                {
                    damage = M_MechAttack * M_MechWeaponEnergy;
                    if (!isCounterAttack) OnAttack(WeaponTypes.Sword, targetMech.M_MechID); //机甲特效
                    Random rd = new Random();
                    int dodgeRandomNumber = rd.Next(0, 100);
                    if (dodgeRandomNumber < DodgeProp) //闪避成功
                    {
                        targetMech.OnDodge();
                    }
                    else
                    {
                        targetMech.BeAttacked(damage);
                        targetMech.OnBeDamaged(damage);
                        OnMakeDamage(damage);
                        if (M_MechWeaponEnergy < M_MechWeaponEnergyMax) M_MechWeaponEnergy++;
                    }

                    if (canCounter) targetMech.Attack(this, true); //对方反击
                    break;
                }

                case WeaponTypes.Gun: //有远程武器避免反击
                {
                    int repeatTimes = M_MechWeaponEnergy;
                    if (isCounterAttack) //如果是用枪反击
                    {
                        if (IsFrenzy) //如果是狂暴枪，反击2次
                        {
                            repeatTimes = 2;
                        }
                        else //如果是用枪反击，只反击一个子弹
                        {
                            repeatTimes = 1;
                        }
                    }

                    for (int i = 0; i < repeatTimes; i++)
                    {
                        OnAttack(WeaponTypes.Gun, targetMech.M_MechID); //机甲特效
                        Random rd = new Random();
                        int dodgeRandomNumber = rd.Next(0, 100);
                        if (dodgeRandomNumber < DodgeProp) //闪避成功
                        {
                            targetMech.OnDodge();
                        }
                        else
                        {
                            targetMech.BeAttacked(M_MechAttack);
                            targetMech.OnBeDamaged(damage);
                            OnMakeDamage(M_MechAttack);
                        }

                        M_MechWeaponEnergy--;
                        if (targetMech.M_MechLeftLife <= 0 || M_MechWeaponEnergy <= 0) break;
                    }

                    if (canCounter) targetMech.Attack(this, true); //对方反击
                    break;
                }

                case WeaponTypes.SniperGun:
                {
                    if (isCounterAttack) break; //狙击枪无法反击他人攻击
                    OnAttack(WeaponTypes.SniperGun, targetMech.M_MechID); //机甲特效
                    Random rd = new Random();
                    int dodgeRandomNumber = rd.Next(0, 100);
                    if (dodgeRandomNumber < DodgeProp) //闪避成功
                    {
                        targetMech.OnDodge();
                    }
                    else
                    {
                        targetMech.BeAttacked(M_MechAttack);
                        targetMech.OnBeDamaged(damage);
                        OnMakeDamage(M_MechAttack);
                    }

                    M_MechWeaponEnergy--;
                    if (targetMech.M_MechLeftLife <= 0) break;
                    if (canCounter) targetMech.Attack(this, true); //对方反击
                    break;
                }
            }
        }
        else //没有武器
        {
            damage = M_MechAttack;
            if (!isCounterAttack) OnAttack(WeaponTypes.None, targetMech.M_MechID); //机甲特效
            Random rd = new Random();
            int dodgeRandomNumber = rd.Next(0, 100);
            if (dodgeRandomNumber < DodgeProp) //闪避成功
            {
                targetMech.OnDodge();
            }
            else
            {
                targetMech.BeAttacked(damage);
                targetMech.OnBeDamaged(damage);
                OnMakeDamage(damage);
            }

            if (canCounter) targetMech.Attack(this, true); //对方反击
        }

        AttackTimesThisRound -= 1;

        //死亡结算
        if (isCounterAttack) return; //逻辑集中在攻击方处理，反击方不处理后续效果

        if (M_MechLeftLife == 0 && targetMech.M_MechLeftLife != 0) //攻击方挂了
        {
            OnDieTogether();
        }
        else if (M_MechLeftLife != 0 && targetMech.M_MechLeftLife == 0) //反击方挂了
        {
            targetMech.OnDieTogether();
            ExecutorInfo ei = new ExecutorInfo(BattlePlayer.ClientId, mechId: M_MechID, targetMechIds: new List<int> {targetMech.M_MechID});
            if (CardInfo.MechInfo.IsSoldier) BattlePlayer.GameManager.EventManager.Invoke(SideEffectExecute.TriggerTime.OnSoldierKill, ei);
            else BattlePlayer.GameManager.EventManager.Invoke(SideEffectExecute.TriggerTime.OnHeroKill, ei);
        }
        else if (M_MechLeftLife == 0 && targetMech.M_MechLeftLife == 0) //全挂了
        {
            if (M_MechID > targetMech.M_MechID) //随从上场顺序决定死亡顺序
            {
                OnDieTogether();
                targetMech.OnDieTogether();
            }
            else
            {
                targetMech.OnDieTogether();
                OnDieTogether();
            }
        }
    }

    public void OnDieTogether()
    {
        if (M_IsDead) return;
        M_IsDead = true;
        M_Weapon = null;
        M_Shield = null;
        M_Pack = null;
        M_MA = null;
        BattlePlayer.GameManager.AddDieTogetherMechsInfo(M_MechID);
        ExecutorInfo info = new ExecutorInfo(BattlePlayer.ClientId, mechId: M_MechID);
        if (CardInfo.MechInfo.IsSoldier) BattlePlayer.GameManager.EventManager.Invoke(SideEffectExecute.TriggerTime.OnSoldierDie, info);
        else BattlePlayer.GameManager.EventManager.Invoke(SideEffectExecute.TriggerTime.OnHeroDie, info);
    }

    public void UnregisterEvent()
    {
        BattlePlayer.GameManager.EventManager.UnRegisterEvent(CardInfo.SideEffectBundle);
        BattlePlayer.GameManager.EventManager.UnRegisterEvent(CardInfo.SideEffectBundle_BattleGroundAura);
    }

    private void OnMakeDamage(int damage)
    {
        ExecutorInfo ei = new ExecutorInfo(BattlePlayer.ClientId, mechId: M_MechID);

        if (CardInfo.MechInfo.IsSoldier)
        {
            BattlePlayer.GameManager.EventManager.Invoke(SideEffectExecute.TriggerTime.OnSoldierMakeDamage, new ExecutorInfo(BattlePlayer.ClientId, mechId: M_MechID));
        }
        else
        {
            BattlePlayer.GameManager.EventManager.Invoke(SideEffectExecute.TriggerTime.OnHeroMakeDamage, new ExecutorInfo(BattlePlayer.ClientId, mechId: M_MechID));
        }
    }

    private void OnBeDamaged(int i)
    {
        if (CardInfo.MechInfo.IsSoldier)
        {
            BattlePlayer.GameManager.EventManager.Invoke(SideEffectExecute.TriggerTime.OnSoldierInjured, new ExecutorInfo(BattlePlayer.ClientId, mechId: M_MechID));
        }
        else
        {
            BattlePlayer.GameManager.EventManager.Invoke(SideEffectExecute.TriggerTime.OnHeroInjured, new ExecutorInfo(BattlePlayer.ClientId, mechId: M_MechID));
        }
    }

    private void OnBeHealed(int i)
    {
        if (CardInfo.MechInfo.IsSoldier)
        {
            BattlePlayer.GameManager.EventManager.Invoke(SideEffectExecute.TriggerTime.OnSoldierBeHealed, new ExecutorInfo(BattlePlayer.ClientId, mechId: M_MechID));
        }
        else
        {
            BattlePlayer.GameManager.EventManager.Invoke(SideEffectExecute.TriggerTime.OnHeroBeHealed, new ExecutorInfo(BattlePlayer.ClientId, mechId: M_MechID));
        }
    }

    public void AttackShip(BattlePlayer ship) //计算结果全部由服务器下发
    {
        if (M_IsDead) return;
        OnAttack();
        int damage = 0;

        if (M_Weapon != null && M_MechWeaponEnergy != 0)
        {
            switch (M_Weapon.M_WeaponType)
            {
                case WeaponTypes.Sword:
                    OnAttackShip(WeaponTypes.Sword, ship.ClientId);
                    damage = M_MechAttack * M_MechWeaponEnergy;
                    ship.Damage(damage);
                    OnMakeDamage(damage);
                    if (M_MechWeaponEnergy < M_MechWeaponEnergyMax) M_MechWeaponEnergy++;
                    break;
                case WeaponTypes.Gun:
                    int tmp = M_MechWeaponEnergy;
                    for (int i = 0; i < tmp; i++)
                    {
                        OnAttackShip(WeaponTypes.Gun, ship.ClientId);
                        ship.Damage(M_MechAttack);
                        OnMakeDamage(M_MechAttack);
                        M_MechWeaponEnergy--;
                    }

                    break;
                case WeaponTypes.SniperGun:
                    OnAttackShip(WeaponTypes.SniperGun, ship.ClientId);
                    ship.Damage(M_MechAttack);
                    OnMakeDamage(M_MechAttack);
                    M_MechWeaponEnergy--;
                    break;
            }
        }
        else
        {
            OnAttackShip(WeaponTypes.None, ship.ClientId);
            damage = M_MechAttack;
            ship.Damage(damage);
            OnMakeDamage(damage);
        }

        if (M_MechLeftLife == 0) //攻击方挂了
        {
            OnDieTogether();
        }

        AttackTimesThisRound -= 1;
    }

    private void OnAttack()
    {
        ExecutorInfo ei = new ExecutorInfo(clientId: BattlePlayer.ClientId, mechId: M_MechID);
        if (CardInfo.MechInfo.IsSoldier)
        {
            BattlePlayer.GameManager.EventManager.Invoke(SideEffectExecute.TriggerTime.OnSoldierAttack, ei);
        }
        else
        {
            BattlePlayer.GameManager.EventManager.Invoke(SideEffectExecute.TriggerTime.OnHeroAttack, ei);
        }
    }

    public void OnBeginRound()
    {
        EndRound = false;
        AttackTimesThisRound = IsSentry ? 0 : (IsFrenzy ? 2 : 1);
    }

    public void OnEndRound()
    {
        EndRound = true;
        IsFirstRound = false;
    }

    #endregion
}