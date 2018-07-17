using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

internal class ServerModuleRetinue : ServerModuleBase
{
    public override void Initiate(CardInfo_Base cardInfo, ServerPlayer serverPlayer)
    {
        M_RetinueName = cardInfo.BaseInfo.CardName;
        M_RetinueDesc = cardInfo.BaseInfo.CardDesc;
        M_RetinueLeftLife = cardInfo.LifeInfo.Life;
        M_RetinueTotalLife = cardInfo.LifeInfo.Life;
        M_RetinueAttack = cardInfo.BattleInfo.BasicAttack;
        M_RetinueArmor = cardInfo.BattleInfo.BasicArmor;
        M_RetinueShield = cardInfo.BattleInfo.BasicShield;
        base.Initiate(cardInfo, serverPlayer);
    }

    public override CardInfo_Base GetCurrentCardInfo()
    {
        return new CardInfo_Retinue(CardInfo.CardID, CardInfo.BaseInfo, CardInfo.UpgradeInfo, CardInfo.LifeInfo, CardInfo.BattleInfo, CardInfo.SlotInfo);
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
                RetinueAttributesChangeRequest request = new RetinueAttributesChangeRequest(ServerPlayer.ClientId, M_RetinuePlaceIndex, RetinueAttributesChangeRequest.RetinueAttributesChangeFlag.LeftLife, addLeftLife: m_RetinueLeftLife - before);
                ServerPlayer.MyClientProxy.MyServerGameManager.Broadcast_AddRequestToOperationResponse(request);
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
                RetinueAttributesChangeRequest request = new RetinueAttributesChangeRequest(ServerPlayer.ClientId, M_RetinuePlaceIndex, RetinueAttributesChangeRequest.RetinueAttributesChangeFlag.MaxLife, addMaxLife: m_RetinueTotalLife - before);
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
            if (isInitialized && before != m_RetinueAttack)
            {
                RetinueAttributesChangeRequest request = new RetinueAttributesChangeRequest(ServerPlayer.ClientId, M_RetinuePlaceIndex, RetinueAttributesChangeRequest.RetinueAttributesChangeFlag.Attack, addAttack: m_RetinueAttack - before);
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
            if (isInitialized && before != m_RetinueArmor)
            {
                RetinueAttributesChangeRequest request = new RetinueAttributesChangeRequest(ServerPlayer.ClientId, M_RetinuePlaceIndex, RetinueAttributesChangeRequest.RetinueAttributesChangeFlag.Armor, addArmor: m_RetinueArmor - before);
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
            if (isInitialized && before != m_RetinueShield)
            {
                RetinueAttributesChangeRequest request = new RetinueAttributesChangeRequest(ServerPlayer.ClientId, M_RetinuePlaceIndex, RetinueAttributesChangeRequest.RetinueAttributesChangeFlag.Shield, addShield: m_RetinueShield - before);
                ServerPlayer.MyClientProxy.MyServerGameManager.Broadcast_AddRequestToOperationResponse(request);
            }
        }
    }

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
                On_WeaponEquiped();
            }
            else if (m_Weapon != value)
            {
                On_WeaponChanged(value);
                return;
            }

            m_Weapon = value;
        }
    }

    void On_WeaponDown()
    {
    }

    void On_WeaponEquiped()
    {
    }

    void On_WeaponChanged(ServerModuleWeapon newWeapon)
    {
        M_Weapon.ChangeWeapon(newWeapon, ref m_Weapon);
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
                On_ShieldEquiped();
            }
            else if (m_Shield != value)
            {
                On_ShieldChanged(value);
                return;
            }

            m_Shield = value;
        }
    }

    void On_ShieldDown()
    {
    }

    void On_ShieldEquiped()
    {
    }

    void On_ShieldChanged(ServerModuleShield newShield) //更换防具时机体基础护甲护盾恢复
    {
        M_Shield.ChangeShield(newShield, ref m_Shield);
    }

    #endregion


    internal ServerModuleShield M_Pack;
    internal ServerModuleShield M_MA;

    #endregion

    #region 模块交互

    private bool canAttack = false;

    internal bool CanAttack
    {
        get { return canAttack; }

        set { canAttack = value; }
    }

    public void OnBeginRound()
    {
        if (M_Weapon == null && M_RetinueAttack != 0)
        {
            CanAttack = true;
        }

        if (M_Weapon != null && M_Weapon.M_WeaponAttack + M_RetinueAttack != 0)
        {
            CanAttack = true;
            M_Weapon.CanAttack = true;
        }
    }

    public void OnEndRound()
    {
        CanAttack = false;
        if (M_Weapon != null) M_Weapon.CanAttack = false;
    }

    public void BeAttacked(int attackNumber)
    {
        int remainAttackNumber = attackNumber;
        if (M_Shield != null)
        {
            remainAttackNumber = M_Shield.ShieldBeAttacked(remainAttackNumber);
            if (remainAttackNumber == 0) return;
        }

        if (M_Shield != null)
        {
            remainAttackNumber = M_Shield.ArmorBeAttacked(remainAttackNumber);
            if (remainAttackNumber == 0) return;
        }
        else
        {
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
                    M_RetinueArmor = (int)(M_RetinueArmor - remainAttackNumber);
                    remainAttackNumber = 0;
                    return;
                }
                else
                {
                    remainAttackNumber -= M_RetinueArmor;
                    M_RetinueArmor = 0;
                }
            }
        }

        if (M_RetinueLeftLife <= remainAttackNumber)
        {
            M_RetinueLeftLife -= M_RetinueLeftLife;
            remainAttackNumber -= M_RetinueLeftLife;
            ServerPlayer.MyBattleGroundManager.RemoveRetinue(this);
        }
        else
        {
            M_RetinueLeftLife -= remainAttackNumber;
            remainAttackNumber = 0;
            return;
        }
    }

    public List<int> AllModulesAttack()
    {
        List<int> ASeriesOfAttacks = new List<int>();
        if (M_Weapon != null)
        {
            ASeriesOfAttacks.AddRange(M_Weapon.WeaponAttack());
        }
        else
        {
            ASeriesOfAttacks.Add(M_RetinueAttack);
        }

        CanAttack = false;
        return ASeriesOfAttacks;
    }

    #endregion
}