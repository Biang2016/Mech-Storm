using System;
using System.Collections.Generic;

internal class ServerModuleRetinue : ServerModuleBase
{
    protected override void Initiate()
    {
        M_RetinueName = CardInfo.BaseInfo.CardName;
        M_RetinueDesc = CardInfo.BaseInfo.CardDescRaw;
        M_RetinueLeftLife = CardInfo.LifeInfo.Life;
        M_RetinueTotalLife = CardInfo.LifeInfo.Life;
        M_RetinueAttack = CardInfo.BattleInfo.BasicAttack;
        M_RetinueArmor = CardInfo.BattleInfo.BasicArmor;
        M_RetinueShield = CardInfo.BattleInfo.BasicShield;
        M_IsDead = false;
    }

    protected override void InitializeSideEffects()
    {
        foreach (SideEffectBundle.SideEffectExecute see in CardInfo.SideEffects.GetSideEffects())
        {
            see.SideEffectBase.Player = ServerPlayer;
            see.SideEffectBase.M_ExecuterInfo = new SideEffectBase.ExecuterInfo(ServerPlayer.ClientId, retinueId: M_RetinueID);
        }
    }

    public override CardInfo_Base GetCurrentCardInfo()
    {
        return new CardInfo_Retinue(
            cardID: CardInfo.CardID,
            baseInfo: CardInfo.BaseInfo,
            slotType: CardInfo.M_SlotType,
            upgradeInfo: CardInfo.UpgradeInfo,
            lifeInfo: CardInfo.LifeInfo,
            battleInfo: CardInfo.BattleInfo,
            slotInfo: CardInfo.SlotInfo,
            sideEffects: CardInfo.SideEffects);
    }

    #region 属性

    private int m_RetinueID;

    public int M_RetinueID
    {
        get { return m_RetinueID; }
        set { m_RetinueID = value; }
    }


    private int m_UsedClientRetinueTempId;

    public int M_UsedClientRetinueTempId //曾用过的客户端临时Id
    {
        get { return m_UsedClientRetinueTempId; }
        set { m_UsedClientRetinueTempId = value; }
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
            if (M_IsDead) return;
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
                OnDieTogather();
            }
        }
    }


    private int m_RetinueTotalLife;

    public int M_RetinueTotalLife
    {
        get { return m_RetinueTotalLife; }
        set
        {
            if (M_IsDead) return;
            int before = m_RetinueTotalLife;
            m_RetinueTotalLife = value;
            if (isInitialized && before != m_RetinueTotalLife)
            {
                RetinueAttributesChangeRequest request = new RetinueAttributesChangeRequest(ServerPlayer.ClientId, M_RetinueID, RetinueAttributesChangeRequest.RetinueAttributesChangeFlag.MaxLife, addMaxLife: m_RetinueTotalLife - before);
                ServerPlayer.MyClientProxy.MyServerGameManager.Broadcast_AddRequestToOperationResponse(request);
            }
        }
    }

    public bool CheckAlive()
    {
        if (M_RetinueLeftLife == 0)
        {
            OnDieTogather();
            return false;
        }

        return true;
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

    public int RetinueShieldFull;

    private int m_RetinueShield;

    public int M_RetinueShield
    {
        get { return m_RetinueShield; }
        set
        {
            RetinueShieldFull = Math.Max(value, RetinueShieldFull);
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
        if (m_Weapon != null)
        {
            ServerPlayer.MyCardDeckManager.M_CurrentCardDeck.RecycleCardInstanceID(m_Weapon.OriginCardInstanceId);
            m_Weapon = null;
            EquipWeaponServerRequest request = new EquipWeaponServerRequest(ServerPlayer.ClientId, null, M_RetinueID, 0);
            ServerPlayer.MyClientProxy.MyServerGameManager.Broadcast_AddRequestToOperationResponse(request);
        }
    }

    void On_WeaponEquiped(ServerModuleWeapon newWeapon)
    {
        m_Weapon = newWeapon;
        EquipWeaponServerRequest request = new EquipWeaponServerRequest(ServerPlayer.ClientId, (CardInfo_Equip) newWeapon.GetCurrentCardInfo(), M_RetinueID, newWeapon.M_WeaponPlaceIndex);
        ServerPlayer.MyClientProxy.MyServerGameManager.Broadcast_AddRequestToOperationResponse(request);

        M_RetinueAttack += newWeapon.CardInfo.WeaponInfo.Attack;
        M_RetinueWeaponEnergyMax += newWeapon.CardInfo.WeaponInfo.EnergyMax;
        M_RetinueWeaponEnergy += newWeapon.CardInfo.WeaponInfo.Energy;
    }

    void On_WeaponChanged(ServerModuleWeapon newWeapon)
    {
        if (m_Weapon != null)
        {
            ServerPlayer.MyCardDeckManager.M_CurrentCardDeck.RecycleCardInstanceID(m_Weapon.OriginCardInstanceId);
        }

        m_Weapon = newWeapon;
        EquipWeaponServerRequest request = new EquipWeaponServerRequest(ServerPlayer.ClientId, (CardInfo_Equip) newWeapon.GetCurrentCardInfo(), M_RetinueID, newWeapon.M_WeaponPlaceIndex);
        ServerPlayer.MyClientProxy.MyServerGameManager.Broadcast_AddRequestToOperationResponse(request);

        M_RetinueAttack = CardInfo.BattleInfo.BasicAttack + newWeapon.CardInfo.WeaponInfo.Attack;
        M_RetinueWeaponEnergyMax = newWeapon.CardInfo.WeaponInfo.EnergyMax;
        M_RetinueWeaponEnergy = newWeapon.CardInfo.WeaponInfo.Energy;
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
        if (m_Shield != null)
        {
            ServerPlayer.MyCardDeckManager.M_CurrentCardDeck.RecycleCardInstanceID(m_Shield.OriginCardInstanceId);
        }

        m_Shield = null;
        EquipShieldServerRequest request = new EquipShieldServerRequest(ServerPlayer.ClientId, null, M_RetinueID, 0);
        ServerPlayer.MyClientProxy.MyServerGameManager.Broadcast_AddRequestToOperationResponse(request);
    }

    void On_ShieldEquiped(ServerModuleShield newShield)
    {
        m_Shield = newShield;
        EquipShieldServerRequest request = new EquipShieldServerRequest(ServerPlayer.ClientId, (CardInfo_Equip) newShield.GetCurrentCardInfo(), M_RetinueID, newShield.M_ShieldPlaceIndex);
        ServerPlayer.MyClientProxy.MyServerGameManager.Broadcast_AddRequestToOperationResponse(request);

        M_RetinueArmor += newShield.CardInfo.ShieldInfo.Armor;
        M_RetinueShield += newShield.CardInfo.ShieldInfo.Shield;
    }

    void On_ShieldChanged(ServerModuleShield newShield) //更换防具时机体基础护甲护盾恢复
    {
        if (m_Shield != null)
        {
            ServerPlayer.MyCardDeckManager.M_CurrentCardDeck.RecycleCardInstanceID(m_Shield.OriginCardInstanceId);
        }
        m_Shield = newShield;
        EquipShieldServerRequest request = new EquipShieldServerRequest(ServerPlayer.ClientId, (CardInfo_Equip) newShield.GetCurrentCardInfo(), M_RetinueID, newShield.M_ShieldPlaceIndex);
        ServerPlayer.MyClientProxy.MyServerGameManager.Broadcast_AddRequestToOperationResponse(request);

        M_RetinueShield = CardInfo.BattleInfo.BasicShield + newShield.CardInfo.ShieldInfo.Shield;
        M_RetinueArmor = CardInfo.BattleInfo.BasicArmor + newShield.CardInfo.ShieldInfo.Armor;
    }

    #endregion


    internal ServerModuleShield M_Pack;
    internal ServerModuleShield M_MA;

    #endregion

    #region 模块交互

    public void BeAttacked(int attackNumber) //攻击和被攻击仅发送伤害数值给客户端，具体计算分别处理(这里是被攻击，指的是攻击动作，不是掉血事件)
    {
        if (M_IsDead) return;
        OnBeAttacked();
        int remainAttackNumber = attackNumber;

        if (M_RetinueShield > 0)
        {
            if (M_RetinueShield >= remainAttackNumber)
            {
                m_RetinueShield = M_RetinueShield - remainAttackNumber;
                remainAttackNumber = 0;
                return;
            }
            else
            {
                remainAttackNumber -= M_RetinueShield;
                m_RetinueShield = 0;
            }
        }

        if (M_RetinueArmor > 0)
        {
            if (M_RetinueArmor >= remainAttackNumber)
            {
                m_RetinueArmor = M_RetinueArmor - remainAttackNumber;
                remainAttackNumber = 0;
                return;
            }
            else
            {
                remainAttackNumber -= M_RetinueArmor;
                m_RetinueArmor = 0;
            }
        }

        if (M_RetinueLeftLife <= remainAttackNumber)
        {
            m_RetinueLeftLife -= M_RetinueLeftLife;
            remainAttackNumber -= M_RetinueLeftLife;
        }
        else
        {
            m_RetinueLeftLife -= remainAttackNumber;
            remainAttackNumber = 0;
            return;
        }
    }

    private void OnBeAttacked()
    {
    }

    public void Attack(ServerModuleRetinue targetModuleRetinue, bool isStrickBack) //服务器客户单分别计算
    {
        if (M_IsDead) return;
        OnAttack();
        int damage = 0;

        if (M_Weapon != null && M_RetinueWeaponEnergy != 0) //有武器避免反击
        {
            switch (M_Weapon.M_WeaponType)
            {
                case WeaponTypes.Sword:
                    damage = M_RetinueAttack * M_RetinueWeaponEnergy;
                    targetModuleRetinue.BeAttacked(damage);
                    OnMakeDamage(damage);
                    if (M_RetinueWeaponEnergy < M_RetinueWeaponEnergyMax) m_RetinueWeaponEnergy++;
                    break;
                case WeaponTypes.Gun:
                    int tmp = M_RetinueWeaponEnergy;
                    for (int i = 0; i < tmp; i++)
                    {
                        targetModuleRetinue.BeAttacked(M_RetinueAttack);
                        OnMakeDamage(M_RetinueAttack);
                        m_RetinueWeaponEnergy--;
                    }

                    break;
            }
        }
        else //如果没有武器，则受到反击
        {
            damage = M_RetinueAttack;
            targetModuleRetinue.BeAttacked(damage);
            OnMakeDamage(damage);
            if (!isStrickBack) targetModuleRetinue.Attack(this, true); //攻击对方且接受对方反击，但反击不能再反击
        }

        //死亡结算
        if (isStrickBack) return; //逻辑集中在攻击方处理，反击方不处理后续效果


        if (M_RetinueLeftLife == 0 && targetModuleRetinue.M_RetinueLeftLife != 0) //攻击方挂了
        {
            OnDieTogather();
        }
        else if (M_RetinueLeftLife != 0 && targetModuleRetinue.M_RetinueLeftLife == 0) //反击方挂了
        {
            targetModuleRetinue.OnDieTogather();
        }
        else if (M_RetinueLeftLife == 0 && targetModuleRetinue.M_RetinueLeftLife == 0) //全挂了
        {
            if (M_RetinueID > targetModuleRetinue.M_RetinueID) //随从上场顺序决定死亡顺序
            {
                OnDieTogather();
                targetModuleRetinue.OnDieTogather();
            }
            else
            {
                targetModuleRetinue.OnDieTogather();
                OnDieTogather();
            }
        }
    }

    public void OnDieTogather()
    {
        if (M_IsDead) return;
        M_IsDead = true;
        ServerPlayer.MyGameManager.AddDieTogatherRetinuesInfo(M_RetinueID);
        SideEffectBase.ExecuterInfo info = new SideEffectBase.ExecuterInfo(ServerPlayer.ClientId, retinueId: M_RetinueID);
        ServerPlayer.MyGameManager.EventManager.Invoke(SideEffectBundle.TriggerTime.OnRetinueDie, info);
        if (CardInfo.BattleInfo.IsSoldier) ServerPlayer.MyGameManager.EventManager.Invoke(SideEffectBundle.TriggerTime.OnSoldierDie, info);
        else ServerPlayer.MyGameManager.EventManager.Invoke(SideEffectBundle.TriggerTime.OnHeroDie, info);

        ServerPlayer.MyGameManager.EventManager.UnRegisterEvent(CardInfo.SideEffects);
    }

    private void OnMakeDamage(int damage)
    {
    }

    private void OnBeDamaged(int i)
    {
    }

    private void OnBeHealed(int i)
    {
    }

    public void AttackShip(ServerPlayer ship) //计算结果全部由服务器下发
    {
        if (M_IsDead) return;
        OnAttack();
        int damage = 0;

        if (M_Weapon != null && M_RetinueWeaponEnergy != 0) //有武器避免反击
        {
            switch (M_Weapon.M_WeaponType)
            {
                case WeaponTypes.Sword:
                    damage = M_RetinueAttack * M_RetinueWeaponEnergy;
                    ship.DamageLifeAboveZero(damage);
                    OnMakeDamage(damage);
                    if (M_RetinueWeaponEnergy < M_RetinueWeaponEnergyMax) M_RetinueWeaponEnergy++;
                    break;
                case WeaponTypes.Gun:
                    int tmp = M_RetinueWeaponEnergy;
                    for (int i = 0; i < tmp; i++)
                    {
                        ship.DamageLifeAboveZero(M_RetinueAttack);
                        OnMakeDamage(M_RetinueAttack);
                        M_RetinueWeaponEnergy--;
                    }

                    break;
            }
        }
        else
        {
            damage = M_RetinueAttack;
            ship.DamageLifeAboveZero(damage);
            OnMakeDamage(damage);
        }

        if (M_RetinueLeftLife == 0) //攻击方挂了
        {
            OnDieTogather();
        }
    }

    private void OnAttack()
    {
    }

    #endregion
}