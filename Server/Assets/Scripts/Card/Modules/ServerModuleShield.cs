using System.Collections;
using System.Collections.Generic;

internal class ServerModuleShield : ServerModuleBase
{
    #region 各模块、自身数值和初始化

    internal ServerModuleRetinue M_ModuleRetinue;

    public override void Initiate(CardInfo_Base cardInfo, ServerPlayer serverPlayer)
    {
        M_ShieldName = CardInfo_Shield.textToVertical(((CardInfo_Shield) cardInfo).CardName);
        M_ShieldType = ((CardInfo_Shield) cardInfo).M_ShieldType;
        M_ShieldArmor = ((CardInfo_Shield) cardInfo).Armor + M_ModuleRetinue.M_RetinueArmor;
        M_ShieldArmorMax = ((CardInfo_Shield) cardInfo).ArmorMax + M_ModuleRetinue.M_RetinueArmor;
        M_ShieldShield = ((CardInfo_Shield) cardInfo).Shield + M_ModuleRetinue.M_RetinueShield;
        M_ShieldShieldMax = ((CardInfo_Shield) cardInfo).ShieldMax + M_ModuleRetinue.M_RetinueShield;
        base.Initiate(cardInfo, serverPlayer);
    }

    public override CardInfo_Base GetCurrentCardInfo()
    {
        return new CardInfo_Shield(CardInfo.CardID, CardInfo.CardName, CardInfo.CardDesc, CardInfo.Cost, CardInfo.DragPurpose, CardInfo.CardType, CardInfo.CardColor, CardInfo.UpgradeID, CardInfo.CardLevel, ((CardInfo_Shield) CardInfo).M_ShieldType, ((CardInfo_Shield) CardInfo).Armor, ((CardInfo_Shield) CardInfo).ArmorMax, ((CardInfo_Shield) CardInfo).Shield, ((CardInfo_Shield) CardInfo).ShieldMax);
    }

    #region 属性

    private int m_ShieldPlaceIndex;

    public int M_ShieldPlaceIndex
    {
        get { return m_ShieldPlaceIndex; }
        set { m_ShieldPlaceIndex = value; }
    }

    private string m_ShieldName;

    public string M_ShieldName
    {
        get { return m_ShieldName; }

        set { m_ShieldName = value; }
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
            int before = m_ShieldArmor;
            m_ShieldArmor = value;
            if (isInitialized && before != m_ShieldArmor)
            {
                ShieldAttributesRequest request = new ShieldAttributesRequest(ServerPlayer.ClientId, M_RetinuePlaceIndex, M_ShieldPlaceIndex, ShieldAttributesRequest.ShieldAttributesChangeFlag.Armor, addArmor: m_ShieldArmor - before);
                ServerPlayer.MyClientProxy.SendMessage(request);
                ServerPlayer.MyEnemyPlayer.MyClientProxy.SendMessage(request);
            }
        }
    }

    private int m_ShieldArmorMax;

    public int M_ShieldArmorMax
    {
        get { return m_ShieldArmorMax; }

        set
        {
            int before = m_ShieldArmorMax;
            m_ShieldArmorMax = value;
            if (isInitialized && before != m_ShieldArmorMax)
            {
                ShieldAttributesRequest request = new ShieldAttributesRequest(ServerPlayer.ClientId, M_RetinuePlaceIndex, M_ShieldPlaceIndex, ShieldAttributesRequest.ShieldAttributesChangeFlag.ArmorMax, addArmorMax: m_ShieldArmorMax - before);
                ServerPlayer.MyClientProxy.SendMessage(request);
                ServerPlayer.MyEnemyPlayer.MyClientProxy.SendMessage(request);
            }
        }
    }

    private int m_ShieldShield;

    public int M_ShieldShield
    {
        get { return m_ShieldShield; }

        set
        {
            int before = m_ShieldShield;
            m_ShieldShield = value;
            if (isInitialized && before != m_ShieldShield)
            {
                ShieldAttributesRequest request = new ShieldAttributesRequest(ServerPlayer.ClientId, M_RetinuePlaceIndex, M_ShieldPlaceIndex, ShieldAttributesRequest.ShieldAttributesChangeFlag.Shield, addShield: m_ShieldShield - before);
                ServerPlayer.MyClientProxy.SendMessage(request);
                ServerPlayer.MyEnemyPlayer.MyClientProxy.SendMessage(request);
            }
        }
    }

    private int m_ShieldShieldMax;

    public int M_ShieldShieldMax
    {
        get { return m_ShieldShieldMax; }

        set
        {
            int before = m_ShieldShieldMax;
            m_ShieldShieldMax = value;
            if (isInitialized&&before !=m_ShieldShieldMax)
            {
                ShieldAttributesRequest request = new ShieldAttributesRequest(ServerPlayer.ClientId, M_RetinuePlaceIndex, M_ShieldPlaceIndex, ShieldAttributesRequest.ShieldAttributesChangeFlag.ShieldMax, addShieldMax: m_ShieldShieldMax - before);
                ServerPlayer.MyClientProxy.SendMessage(request);
                ServerPlayer.MyEnemyPlayer.MyClientProxy.SendMessage(request);
            }
        }
    }

    #endregion

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
        if (M_ShieldShield > 0)
        {
            if (M_ShieldShield > remainAttackValue)
            {
                M_ShieldShield--;
                remainAttackValue = 0;
            }
            else
            {
                remainAttackValue -= M_ShieldShield;
                M_ShieldShield /= 2;
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

    public int ArmorBeAttacked(int attackValue)
    {
        bool isTrigger = false;
        bool isDead = false;
        int remainAttackValue = attackValue;
        if (remainAttackValue == 0) return 0;
        if (M_ShieldArmor > 0)
        {
            if (M_ShieldArmor >= remainAttackValue)
            {
                M_ShieldArmor -= remainAttackValue;
                remainAttackValue = 0;
            }
            else
            {
                remainAttackValue = remainAttackValue - M_ShieldArmor;
                M_ShieldArmor = 0;
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