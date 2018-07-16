using System.Collections;
using System.Collections.Generic;

internal class ServerModuleShield : ServerModuleBase
{
    #region 各模块、自身数值和初始化

    internal ServerModuleRetinue M_ModuleRetinue;

    public override void Initiate(CardInfo_Base cardInfo, ServerPlayer serverPlayer)
    {
        M_ShieldName = CardInfo_Base.textToVertical(((CardInfo_Shield)cardInfo).BaseInfo.CardName);
        M_ShieldType = cardInfo.ShieldInfo.ShieldType;
        M_ShieldArmor = cardInfo.ShieldInfo.Armor + M_ModuleRetinue.M_RetinueArmor;
        M_ShieldArmorMax = cardInfo.ShieldInfo.ArmorMax + M_ModuleRetinue.M_RetinueArmor;
        M_ShieldShield = cardInfo.ShieldInfo.Shield + M_ModuleRetinue.M_RetinueShield;
        M_ShieldShieldMax = cardInfo.ShieldInfo.ShieldMax + M_ModuleRetinue.M_RetinueShield;
        base.Initiate(cardInfo, serverPlayer);
    }

    public override CardInfo_Base GetCurrentCardInfo()
    {
        return new CardInfo_Shield(CardInfo.CardID, CardInfo.BaseInfo, CardInfo.UpgradeInfo, CardInfo.ShieldInfo);
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

    private ShieldTypes m_ShieldType;

    public ShieldTypes M_ShieldType
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
                ServerPlayer.MyClientProxy.MyServerGameManager.Broadcast_AddRequestToOperationResponse(request);
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
                ServerPlayer.MyClientProxy.MyServerGameManager.Broadcast_AddRequestToOperationResponse(request);
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
                ServerPlayer.MyClientProxy.MyServerGameManager.Broadcast_AddRequestToOperationResponse(request);
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
            if (isInitialized && before != m_ShieldShieldMax)
            {
                ShieldAttributesRequest request = new ShieldAttributesRequest(ServerPlayer.ClientId, M_RetinuePlaceIndex, M_ShieldPlaceIndex, ShieldAttributesRequest.ShieldAttributesChangeFlag.ShieldMax, addShieldMax: m_ShieldShieldMax - before);
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
            if (CardInfo.UpgradeInfo.CardLevel == newShield.CardInfo.UpgradeInfo.CardLevel)
            {
                CardInfo_Shield m_currentInfo = (CardInfo_Shield)GetCurrentCardInfo();
                CardInfo_Shield upgradeShieldCardInfo = (CardInfo_Shield)AllCards.GetCard(CardInfo.UpgradeInfo.UpgradeCardID);
                Initiate(upgradeShieldCardInfo, ServerPlayer);
                M_ShieldShield = m_currentInfo.ShieldInfo.Shield + newShield.CardInfo.ShieldInfo.Shield;
                M_ShieldArmor = m_currentInfo.ShieldInfo.Armor + newShield.CardInfo.ShieldInfo.Armor;
                //newShield.PoolRecycle();
                resultShield = this;
            }
            else if (CardInfo.UpgradeInfo.CardLevel > newShield.CardInfo.UpgradeInfo.CardLevel)
            {
                M_ShieldShield = M_ShieldShield + newShield.CardInfo.ShieldInfo.Shield;
                M_ShieldArmor = M_ShieldArmor + newShield.CardInfo.ShieldInfo.Armor;
                //newShield.PoolRecycle();
                resultShield = this;
            }
            else
            {
                resultShield = newShield;
                newShield.M_ShieldShield = M_ShieldShield + newShield.CardInfo.ShieldInfo.Shield;
                newShield.M_ShieldArmor = M_ShieldArmor + newShield.CardInfo.ShieldInfo.Shield;
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
        }

        if (M_ShieldShield == 0 && M_ShieldArmor == 0)
        {
            M_ModuleRetinue.M_Shield = null;
        }

        return remainAttackValue;
    }

    public int ArmorBeAttacked(int attackValue)
    {
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
        }

        if (M_ShieldShield == 0 && M_ShieldArmor == 0)
        {
            M_ModuleRetinue.M_Shield = null;
        }

        return remainAttackValue;
    }

    #endregion

    #endregion
}