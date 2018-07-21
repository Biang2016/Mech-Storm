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
    public Animator M_ShieldHitAnim;
    public Animator M_ArmorHitAnim;

    public GameObject Block_ShieldArmor;
    protected GameObject GoNumberSet_ShieldArmor;
    protected CardNumberSet CardNumberSet_ShieldArmor;

    public GameObject Block_ShieldShield;
    protected GameObject GoNumberSet_ShieldShield;
    protected CardNumberSet CardNumberSet_ShieldShield;

    public GameObject Block_ShieldShieldMax;
    protected GameObject GoNumberSet_ShieldShieldMax;
    protected CardNumberSet CardNumberSet_ShieldShieldMax;

    public override void Initiate(CardInfo_Base cardInfo, ClientPlayer clientPlayer)
    {
        base.Initiate(cardInfo, clientPlayer);
        M_ShieldName = CardInfo_Base.textToVertical(cardInfo.BaseInfo.CardName);
        M_ShieldType = cardInfo.ShieldInfo.ShieldType;
        M_ShieldArmor = cardInfo.ShieldInfo.Armor + (M_ModuleRetinue != null ? M_ModuleRetinue.M_RetinueArmor : 0);
        M_ShieldShield = cardInfo.ShieldInfo.Shield + (M_ModuleRetinue != null ? M_ModuleRetinue.M_RetinueShield : 0);
        M_ShieldShieldMax = cardInfo.ShieldInfo.ShieldMax + (M_ModuleRetinue != null ? M_ModuleRetinue.M_RetinueShield : 0);
        if (M_Bloom) M_Bloom.SetActive(false);
    }

    public override CardInfo_Base GetCurrentCardInfo()
    {
        return new CardInfo_Shield(CardInfo.CardID, CardInfo.BaseInfo, CardInfo.UpgradeInfo, CardInfo.ShieldInfo, CardInfo.SideEffects_OnDie);
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
        my_TextAlign_Armor = CardNumberSet.TextAlign.Left;
        my_TextAlign_ArmorMax = CardNumberSet.TextAlign.Right;
        my_TextAlign_Shield = CardNumberSet.TextAlign.Left;
        my_TextAlign_ShieldMax = CardNumberSet.TextAlign.Right;
        M_ShieldArmor = M_ShieldArmor;
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
        M_ShieldShield = M_ShieldShield;
        M_ShieldShieldMax = M_ShieldShieldMax;
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
            if (m_ShieldArmor > value) BattleEffectsManager.BEM.EffectsShow(Co_ArmorBeAttacked(value <= 0));
            m_ShieldArmor = value;
            initiateNumbers(ref GoNumberSet_ShieldArmor, ref CardNumberSet_ShieldArmor, my_NumberSize_Armor, my_TextAlign_Armor, Block_ShieldArmor);
            CardNumberSet_ShieldArmor.Number = m_ShieldArmor;
        }
    }

    private int m_ShieldShield;

    public int M_ShieldShield
    {
        get { return m_ShieldShield; }

        set
        {
            if (m_ShieldShield > value) BattleEffectsManager.BEM.EffectsShow(Co_ShieldBeAttacked(value <= 0));
            m_ShieldShield = value;
            initiateNumbers(ref GoNumberSet_ShieldShield, ref CardNumberSet_ShieldShield, my_NumberSize_Shield, my_TextAlign_Shield, Block_ShieldShield);
            CardNumberSet_ShieldShield.Number = m_ShieldShield;
        }
    }

    private int m_ShieldShieldMax;

    public int M_ShieldShieldMax
    {
        get { return m_ShieldShieldMax; }

        set
        {
            m_ShieldShieldMax = value;
            initiateNumbers(ref GoNumberSet_ShieldShieldMax, ref CardNumberSet_ShieldShieldMax, my_NumberSize_ShieldMax, my_TextAlign_ShieldMax, Block_ShieldShieldMax, '/');
            CardNumberSet_ShieldShieldMax.Number = m_ShieldShieldMax;
        }
    }

    #endregion

    #region 模块交互

    #region  防具更换

    public void ChangeShield(ModuleShield newShield, ref ModuleShield resultShield)
    {
        if (AllCards.IsASeries(CardInfo, newShield.CardInfo))
        {
            if (CardInfo.UpgradeInfo.CardLevel == newShield.CardInfo.UpgradeInfo.CardLevel)
            {
                CardInfo_Shield m_currentInfo = (CardInfo_Shield)GetCurrentCardInfo();
                CardInfo_Shield upgradeShieldCardInfo = (CardInfo_Shield)AllCards.GetCard(CardInfo.UpgradeInfo.UpgradeCardID);
                Initiate(upgradeShieldCardInfo, ClientPlayer);
                M_ShieldShield = m_currentInfo.ShieldInfo.Shield + newShield.CardInfo.ShieldInfo.Shield;
                M_ShieldArmor = m_currentInfo.ShieldInfo.Armor + newShield.CardInfo.ShieldInfo.Armor;
                newShield.PoolRecycle();
                resultShield = this;
            }
            else if (CardInfo.UpgradeInfo.CardLevel > newShield.CardInfo.UpgradeInfo.CardLevel)
            {
                M_ShieldShield = M_ShieldShield + newShield.CardInfo.ShieldInfo.Shield;
                M_ShieldArmor = M_ShieldArmor + newShield.CardInfo.ShieldInfo.Armor;
                newShield.PoolRecycle();
                resultShield = this;
            }
            else
            {
                resultShield = newShield;
                newShield.M_ShieldShield = M_ShieldShield + newShield.CardInfo.ShieldInfo.Shield;
                newShield.M_ShieldArmor = M_ShieldArmor + newShield.CardInfo.ShieldInfo.Shield;
                PoolRecycle();
            }
        }
        else
        {
            resultShield = newShield;
            PoolRecycle();
        }
    }

    #endregion

    #region 攻击防御相关

    IEnumerator Co_ShieldBeAttacked(bool isDead)
    {
        M_ShieldHitAnim.SetTrigger("BeHit");
        yield return new WaitForSeconds(1F);
        M_ShieldShield = M_ShieldShield;
        if (isDead) PoolRecycle();
        yield return null;
        BattleEffectsManager.BEM.EffectEnd();
    }

    IEnumerator Co_ArmorBeAttacked(bool isDead)
    {
        M_ArmorHitAnim.SetTrigger("BeHit");
        yield return new WaitForSeconds(1F);
        if (isDead) PoolRecycle();
        BattleEffectsManager.BEM.EffectEnd();
    }

    IEnumerator DelayPoolRecycle()
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