using System.Collections;
using System.Collections.Generic;

internal class ServerModuleShield : ServerModuleBase
{
    #region 各模块、自身数值和初始化

    internal ServerModuleRetinue M_ModuleRetinue;

    public override void Initiate(CardInfo_Base cardInfo, ServerPlayer serverPlayer)
    {
        base.Initiate(cardInfo, serverPlayer);
        M_ShieldName = CardInfo_Shield.textToVertical(((CardInfo_Shield) cardInfo).CardName);
        M_ShieldType = ((CardInfo_Shield) cardInfo).M_ShieldType;
        M_ShieldArmor = ((CardInfo_Shield) cardInfo).Armor + M_ModuleRetinue.M_RetinueArmor;
        M_ShieldArmorMax = ((CardInfo_Shield) cardInfo).ArmorMax + M_ModuleRetinue.M_RetinueArmor;
        M_ShieldShield = ((CardInfo_Shield) cardInfo).Shield + M_ModuleRetinue.M_RetinueShield;
        M_ShieldShieldMax = ((CardInfo_Shield) cardInfo).ShieldMax + M_ModuleRetinue.M_RetinueShield;
    }

    public override CardInfo_Base GetCurrentCardInfo()
    {
        return new CardInfo_Shield(CardInfo.CardID, CardInfo.CardName, CardInfo.CardDesc, CardInfo.Cost, CardInfo.DragPurpose, CardInfo.CardType, CardInfo.CardColor, CardInfo.UpgradeID, CardInfo.CardLevel, ((CardInfo_Shield) CardInfo).M_ShieldType, ((CardInfo_Shield) CardInfo).Armor, ((CardInfo_Shield) CardInfo).ArmorMax, ((CardInfo_Shield) CardInfo).Shield, ((CardInfo_Shield) CardInfo).ShieldMax);
    }

    private string m_ShieldName;

    public string M_ShieldName
    {
        get { return m_ShieldName; }

        set
        {
            m_ShieldName = value;
        }
    }

    private ShieldType m_ShieldType;

    public ShieldType M_ShieldType
    {
        get { return m_ShieldType; }

        set { m_ShieldType = value; }
    }

    private int m_ShieldArmor;

    public int M_ShieldArmor
    {
        get { return m_ShieldArmor; }

        set
        {
            m_ShieldArmor = value;
        }
    }

    private int m_ShieldArmorMax;

    public int M_ShieldArmorMax
    {
        get { return m_ShieldArmorMax; }

        set
        {
            m_ShieldArmorMax = value;
        }
    }

    private int m_ShieldShield;

    public int M_ShieldShield
    {
        get { return m_ShieldShield; }

        set
        {
            m_ShieldShield = value;
        }
    }

    private int m_ShieldShieldMax;

    public int M_ShieldShieldMax
    {
        get { return m_ShieldShieldMax; }

        set
        {
            m_ShieldShieldMax = value;
        }
    }

    #endregion

    #region 模块交互

    #region  防具更换

    public void ChangeShield(ServerModuleShield newShield, ref ServerModuleShield resultShield)
    {
        if (AllCards.IsASeries(CardInfo, newShield.CardInfo))
        {
            if (CardInfo.CardLevel == newShield.CardInfo.CardLevel)
            {
                CardInfo_Shield m_currentInfo = (CardInfo_Shield) GetCurrentCardInfo();
                CardInfo_Shield upgradeShieldCardInfo = (CardInfo_Shield) AllCards.GetCard(CardInfo.UpgradeID);
                Initiate(upgradeShieldCardInfo, ServerPlayer);
                M_ShieldShield = m_currentInfo.Shield + ((CardInfo_Shield) newShield.CardInfo).Shield;
                M_ShieldArmor = m_currentInfo.Armor + ((CardInfo_Shield) newShield.CardInfo).Armor;
                //newShield.PoolRecycle();
                resultShield = this;
            }
            else if (CardInfo.CardLevel > newShield.CardInfo.CardLevel)
            {
                M_ShieldShield = M_ShieldShield + ((CardInfo_Shield) newShield.CardInfo).Shield;
                M_ShieldArmor = M_ShieldArmor + ((CardInfo_Shield) newShield.CardInfo).Armor;
                //newShield.PoolRecycle();
                resultShield = this;
            }
            else
            {
                resultShield = newShield;
                newShield.M_ShieldShield = M_ShieldShield + ((CardInfo_Shield) newShield.CardInfo).Shield;
                newShield.M_ShieldArmor = M_ShieldArmor + ((CardInfo_Shield) newShield.CardInfo).Shield;
                //PoolRecycle();
            }
        }
        else
        {
            resultShield = newShield;
            //PoolRecycle();
        }
    }

    #endregion

    #region 攻击防御相关

    public int ShieldBeAttacked(int attackValue)
    {
        bool isTrigger = false;
        bool isDead = false;

        int remainAttackValue = attackValue;
        if (remainAttackValue == 0) return 0;
        if (m_ShieldShield > 0)
        {
            if (m_ShieldShield > remainAttackValue)
            {
                m_ShieldShield--;
                remainAttackValue = 0;
            }
            else
            {
                remainAttackValue -= m_ShieldShield;
                m_ShieldShield /= 2;
            }

            isTrigger = true;
        }

        if (m_ShieldShield == 0 && m_ShieldArmor == 0)
        {
            isDead = true;
            M_ModuleRetinue.M_Shield = null;
        }

        return remainAttackValue;
    }

    public int ArmorBeAttacked(int attackValue)
    {
        bool isTrigger = false;
        bool isDead = false;
        int remainAttackValue = attackValue;
        if (remainAttackValue == 0) return 0;
        if (m_ShieldArmor > 0)
        {
            if (m_ShieldArmor >= remainAttackValue)
            {
                m_ShieldArmor -= remainAttackValue;
                remainAttackValue = 0;
            }
            else
            {
                remainAttackValue = remainAttackValue - m_ShieldArmor;
                m_ShieldArmor = 0;
            }

            isTrigger = true;
        }

        if (M_ShieldShield == 0 && M_ShieldArmor == 0)
        {
            isDead = true;
            M_ModuleRetinue.M_Shield = null;
        }

        return remainAttackValue;
    }

    #endregion

    #endregion
}

