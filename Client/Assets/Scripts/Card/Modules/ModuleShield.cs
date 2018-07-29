using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal class ModuleShield : ModuleBase
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

    #region 各模块、自身数值和初始化

    internal ModuleRetinue M_ModuleRetinue;
    public TextMesh ShieldName;
    public GameObject M_Bloom;

    public GameObject Block_ShieldArmor;
    protected GameObject GoNumberSet_ShieldArmor;
    protected CardNumberSet CardNumberSet_ShieldArmor;

    public GameObject Block_ShieldShield;
    protected GameObject GoNumberSet_ShieldShield;
    protected CardNumberSet CardNumberSet_ShieldShield;

    public override void Initiate(CardInfo_Base cardInfo, ClientPlayer clientPlayer)
    {
        base.Initiate(cardInfo, clientPlayer);
        M_ShieldName = CardInfo_Base.textToVertical(cardInfo.BaseInfo.CardName);
        M_ShieldType = cardInfo.ShieldInfo.ShieldType;
        M_ShieldArmor = cardInfo.ShieldInfo.Armor;
        M_ShieldShield = cardInfo.ShieldInfo.Shield;
        if (M_Bloom) M_Bloom.SetActive(false);
    }

    private NumberSize my_NumberSize_Armor = NumberSize.Medium;
    private NumberSize my_NumberSize_Shield = NumberSize.Medium;
    private CardNumberSet.TextAlign my_TextAlign_Armor = CardNumberSet.TextAlign.Center;
    private CardNumberSet.TextAlign my_TextAlign_Shield = CardNumberSet.TextAlign.Center;

    public void SetPreview()
    {
        my_NumberSize_Armor = NumberSize.Small;
        my_NumberSize_Shield = NumberSize.Small;
        my_TextAlign_Armor = CardNumberSet.TextAlign.Left;
        my_TextAlign_Shield = CardNumberSet.TextAlign.Left;
        M_ShieldArmor = M_ShieldArmor;
        M_ShieldShield = M_ShieldShield;
    }

    public void SetNoPreview()
    {
        my_NumberSize_Armor = NumberSize.Medium;
        my_NumberSize_Shield = NumberSize.Medium;
        my_TextAlign_Armor = CardNumberSet.TextAlign.Center;
        my_TextAlign_Shield = CardNumberSet.TextAlign.Center;
        M_ShieldArmor = M_ShieldArmor;
        M_ShieldShield = M_ShieldShield;
    }

    #endregion

    #region 属性

    private string m_ShieldName;

    public string M_ShieldName
    {
        get { return m_ShieldName; }

        set
        {
            m_ShieldName = value;
            ShieldName.text = m_ShieldName;
        }
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
            m_ShieldArmor = value;
            if (Block_ShieldArmor)
            {
                initiateNumbers(ref GoNumberSet_ShieldArmor, ref CardNumberSet_ShieldArmor, my_NumberSize_Armor, my_TextAlign_Armor, Block_ShieldArmor, '+');
                CardNumberSet_ShieldArmor.Number = m_ShieldArmor;
            }
        }
    }

    private int m_ShieldShield;

    public int M_ShieldShield
    {
        get { return m_ShieldShield; }

        set
        {
            m_ShieldShield = value;
            if (Block_ShieldShield)
            {
                initiateNumbers(ref GoNumberSet_ShieldShield, ref CardNumberSet_ShieldShield, my_NumberSize_Shield, my_TextAlign_Shield, Block_ShieldShield, '+');
                CardNumberSet_ShieldShield.Number = m_ShieldShield;
            }
        }
    }

    #endregion

    #region 模块交互

    #region 攻击防御相关

    IEnumerator Co_DelayPoolRecycle()
    {
        yield return new WaitForSeconds(0.5F);
        PoolRecycle();
    }

    public override void DragComponent_SetStates(ref bool canDrag, ref DragPurpose dragPurpose)
    {
        canDrag = false;
        dragPurpose = CardInfo.BaseInfo.DragPurpose;
    }

    #endregion

    #region 交互UX

    public override void MouseHoverComponent_OnMouseEnterImmediately(Vector3 mousePosition)
    {
        base.MouseHoverComponent_OnMouseEnterImmediately(mousePosition);
        if (M_Bloom) M_Bloom.SetActive(true);
    }

    public override void MouseHoverComponent_OnMouseLeaveImmediately()
    {
        base.MouseHoverComponent_OnMouseLeaveImmediately();
        if (M_Bloom) M_Bloom.SetActive(false);
    }

    #endregion

    #endregion
}