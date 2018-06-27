using System.Collections.Generic;
using UnityEngine;

public class ModuleShield : ModuleBase
{
    void Awake()
    {
        gameObjectPool = GameObjectPoolManager.GOPM.Pool_ModuleShieldPool;
    }

    void Start()
    {

    }

    void Update()
    {

    }

    #region 各模块
    internal ModuleRetinue M_ModuleRetinue;
    public TextMesh ShieldName;
    public GameObject M_Bloom;

    public GameObject Block_ShieldArmor;
    protected GameObject GoNumberSet_ShieldArmor;
    protected CardNumberSet CardNumberSet_ShieldArmor;

    public GameObject Block_ShieldArmorMax;
    protected GameObject GoNumberSet_ShieldArmorMax;
    protected CardNumberSet CardNumberSet_ShieldArmorMax;

    public GameObject Block_ShieldShield;
    protected GameObject GoNumberSet_ShieldShield;
    protected CardNumberSet CardNumberSet_ShieldShield;

    public GameObject Block_ShieldShieldMax;
    protected GameObject GoNumberSet_ShieldShieldMax;
    protected CardNumberSet CardNumberSet_ShieldShieldMax;

    public override void Initiate(CardInfo_Base cardInfo, Player player)
    {
        base.Initiate(cardInfo, player);
        M_ShieldName = CardInfo_Shield.textToVertical(((CardInfo_Shield)cardInfo).CardName);
        M_ShieldType = ((CardInfo_Shield)cardInfo).M_ShieldType;
        M_ShieldArmor = ((CardInfo_Shield)cardInfo).Armor;
        M_ShieldArmorMax = ((CardInfo_Shield)cardInfo).ArmorMax;
        M_ShieldShield = ((CardInfo_Shield)cardInfo).Shield;
        M_ShieldShieldMax = ((CardInfo_Shield)cardInfo).ShieldMax;
        if (M_Bloom) M_Bloom.SetActive(false);
    }

    public override CardInfo_Base GetCurrentCardInfo()
    {
        ModuleCurrentCardInfo = new CardInfo_Shield(CardInfo.CardID, CardInfo.CardName, CardInfo.CardDesc, CardInfo.Cost, CardInfo.HasTarget, CardInfo.CardType,((CardInfo_Shield)CardInfo).M_ShieldType, ((CardInfo_Shield)CardInfo).Armor, ((CardInfo_Shield)CardInfo).ArmorMax, ((CardInfo_Shield)CardInfo).Shield, ((CardInfo_Shield)CardInfo).ShieldMax);
        return ModuleCurrentCardInfo;
    }

    private string m_ShieldName;
    public string M_ShieldName
    {
        get
        {
            return m_ShieldName;
        }

        set
        {
            m_ShieldName = value;
            ShieldName.text = m_ShieldName;
        }
    }

    private ShieldType m_ShieldType;
    public ShieldType M_ShieldType
    {
        get
        {
            return m_ShieldType;
        }

        set
        {
            m_ShieldType = value;
        }
    }

    private NumberSize my_NumberSize_Armor = NumberSize.Medium;
    private NumberSize my_NumberSize_ArmorMax = NumberSize.Medium;
    private NumberSize my_NumberSize_Shield = NumberSize.Medium;
    private NumberSize my_NumberSize_ShieldMax = NumberSize.Medium;
    private CardNumberSet.TextAlign my_TextAlign_Armor = CardNumberSet.TextAlign.Center;
    private CardNumberSet.TextAlign my_TextAlign_ArmorMax = CardNumberSet.TextAlign.Center;
    private CardNumberSet.TextAlign my_TextAlign_Shield = CardNumberSet.TextAlign.Center;
    private CardNumberSet.TextAlign my_TextAlign_ShieldMax = CardNumberSet.TextAlign.Center;

    public void SetPreview()
    {
        my_NumberSize_Armor = NumberSize.Small;
        my_NumberSize_ArmorMax = NumberSize.Small;
        my_NumberSize_Shield = NumberSize.Small;
        my_NumberSize_ShieldMax = NumberSize.Small;
        my_TextAlign_Armor= CardNumberSet.TextAlign.Left;
        my_TextAlign_ArmorMax = CardNumberSet.TextAlign.Right;
        my_TextAlign_Shield = CardNumberSet.TextAlign.Left;
        my_TextAlign_ShieldMax = CardNumberSet.TextAlign.Right;
        M_ShieldArmor = M_ShieldArmor;
        M_ShieldArmorMax = M_ShieldArmorMax;
        M_ShieldShield = M_ShieldShield;
        M_ShieldShieldMax = M_ShieldShieldMax;
    }

    public void SetNoPreview()
    {
        my_NumberSize_Armor = NumberSize.Medium;
        my_NumberSize_ArmorMax = NumberSize.Medium;
        my_NumberSize_Shield = NumberSize.Medium;
        my_NumberSize_ShieldMax = NumberSize.Medium;
        my_TextAlign_Armor = CardNumberSet.TextAlign.Center;
        my_TextAlign_ArmorMax = CardNumberSet.TextAlign.Center;
        my_TextAlign_Shield = CardNumberSet.TextAlign.Center;
        my_TextAlign_ShieldMax = CardNumberSet.TextAlign.Center;
        M_ShieldArmor = M_ShieldArmor;
        M_ShieldArmorMax = M_ShieldArmorMax;
        M_ShieldShield = M_ShieldShield;
        M_ShieldShieldMax = M_ShieldShieldMax;
    }

    private int m_ShieldArmor;
    public int M_ShieldArmor 
        {
        get
        {
            return m_ShieldArmor;
        }

        set
        {
            m_ShieldArmor = value;
            initiateNumbers(ref GoNumberSet_ShieldArmor, ref CardNumberSet_ShieldArmor, my_NumberSize_Armor, my_TextAlign_Armor, Block_ShieldArmor);
            CardNumberSet_ShieldArmor.Number = m_ShieldArmor + M_ModuleRetinue.M_RetinueArmor;
        }
    }

    private int m_ShieldArmorMax;
    public int M_ShieldArmorMax {
        get {
            return m_ShieldArmorMax;
        }

        set {
            m_ShieldArmorMax = value;
            initiateNumbers(ref GoNumberSet_ShieldArmorMax, ref CardNumberSet_ShieldArmorMax, my_NumberSize_ArmorMax, my_TextAlign_ArmorMax, Block_ShieldArmorMax,'/');
            CardNumberSet_ShieldArmorMax.Number = m_ShieldArmorMax + M_ModuleRetinue.M_RetinueArmor;
        }
    }

    private int m_ShieldShield;
    public int M_ShieldShield {
        get
        {
            return m_ShieldShield;
        }

        set
        {
            m_ShieldShield = value;
            initiateNumbers(ref GoNumberSet_ShieldShield, ref CardNumberSet_ShieldShield, my_NumberSize_Shield, my_TextAlign_Shield, Block_ShieldShield);
            CardNumberSet_ShieldShield.Number = m_ShieldShield + M_ModuleRetinue.M_RetinueShield;
        }
    }

    private int m_ShieldShieldMax;
    public int M_ShieldShieldMax {
        get {
            return m_ShieldShieldMax;
        }

        set {
            m_ShieldShieldMax = value;
            initiateNumbers(ref GoNumberSet_ShieldShieldMax, ref CardNumberSet_ShieldShieldMax, my_NumberSize_ShieldMax, my_TextAlign_ShieldMax, Block_ShieldShieldMax, '/');
            CardNumberSet_ShieldShieldMax.Number = m_ShieldShieldMax + M_ModuleRetinue.M_RetinueShield;
        }
    }

    #endregion

    #region 模块交互
    public int ShieldBeAttacked(int attackValue)
    {
        int remainAttackValue = attackValue;
        if (M_ShieldShield > 0) {
            if (M_ShieldShield > remainAttackValue) {
                M_ShieldShield --;
                remainAttackValue = 0;
            } else {
                remainAttackValue -= M_ShieldShield;
                M_ShieldShield /= 2;
            }
        }

        if (M_ShieldShield == 0 && M_ShieldArmor == 0) PoolRecycle();
        return remainAttackValue;
    }

    public int ArmorBeAttacked(int attackValue)
    {
        int remainAttackValue = attackValue;
        if (M_ShieldArmor > 0) {
            if (M_ShieldArmor >= remainAttackValue) {
                M_ShieldArmor -= remainAttackValue;
                remainAttackValue = 0;
            } else {
                remainAttackValue = remainAttackValue - M_ShieldArmor;
                M_ShieldArmor = 0;
            }
        }

        if (M_ShieldShield == 0 && M_ShieldArmor == 0)
        {
            M_ModuleRetinue.M_Shield = null;
            PoolRecycle();
        }
        return remainAttackValue;
    }

    public override void DragComponent_OnMouseUp(BoardAreaTypes boardAreaType, List<SlotAnchor> slotAnchors,
        ModuleRetinue moduleRetinue, Vector3 dragLastPosition, Vector3 dragBeginPosition,
        Quaternion dragBeginQuaternion)
    {
        base.DragComponent_OnMouseUp(boardAreaType, slotAnchors, moduleRetinue, dragLastPosition, dragBeginPosition,
            dragBeginQuaternion);
    }

    public override void DragComponent_SetStates(ref bool canDrag, ref bool hasTarget)
    {
        canDrag = false;
        hasTarget = false;
    }

    public override void MouseHoverComponent_OnMouseEnterImmediately(Vector3 mousePosition)
    {
        base.MouseHoverComponent_OnMouseEnterImmediately(mousePosition);
        if (M_Bloom) M_Bloom.SetActive(true);
    }

    public override void MouseHoverComponent_OnMouseOver()
    {
        base.MouseHoverComponent_OnMouseOver();
    }

    public override void MouseHoverComponent_OnMouseLeave()
    {
        base.MouseHoverComponent_OnMouseLeave();
    }

    public override void MouseHoverComponent_OnMouseLeaveImmediately()
    {
        base.MouseHoverComponent_OnMouseLeaveImmediately();
        if (M_Bloom) M_Bloom.SetActive(false);
    }

    #endregion

}

public enum ShieldType {
    Armor=0,
    Shield=1,
    Mixed=2
}