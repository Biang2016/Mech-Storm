using System.Collections;
using UnityEngine;

public class ModuleShield : ModuleBase
{
    public override void PoolRecycle()
    {
        base.PoolRecycle();
        if (ShieldEquipedAnim) ShieldEquipedAnim.SetTrigger("Hide");
    }

    void Awake()
    {
        gameObjectPool = GameObjectPoolManager.Instance.Pool_ModuleShieldPool;
    }

    #region 各模块、自身数值和初始化

    internal ModuleRetinue M_ModuleRetinue;
    [SerializeField] private TextMesh ShieldName;
    [SerializeField] private TextMesh ShieldName_en;
    [SerializeField] private Renderer M_Bloom;
    [SerializeField] private Renderer M_BloomSE;

    [SerializeField] private GameObject Block_ShieldArmor;
    protected GameObject GoNumberSet_ShieldArmor;
    protected CardNumberSet CardNumberSet_ShieldArmor;

    [SerializeField] private GameObject Block_ShieldShield;
    protected GameObject GoNumberSet_ShieldShield;
    protected CardNumberSet CardNumberSet_ShieldShield;

    [SerializeField] private Animator ShieldEquipedAnim;

    public int M_EquipID;

    public override void Initiate(CardInfo_Base cardInfo, ClientPlayer clientPlayer)
    {
        base.Initiate(cardInfo, clientPlayer);
        M_ShieldName = GameManager.Instance.isEnglish ? cardInfo.BaseInfo.CardName_en : cardInfo.BaseInfo.CardName;
        M_ShieldType = cardInfo.ShieldInfo.ShieldType;
        M_ShieldArmor = cardInfo.ShieldInfo.Armor;
        M_ShieldShield = cardInfo.ShieldInfo.Shield;
        if (M_Bloom) M_Bloom.gameObject.SetActive(false);
        if (M_BloomSE) M_BloomSE.gameObject.SetActive(false);
    }

    public override void ChangeColor(Color color)
    {
        base.ChangeColor(color);
        ClientUtils.ChangeColor(M_Bloom, color);
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
        if (M_Bloom) M_Bloom.gameObject.SetActive(true);
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

    public CardInfo_Equip GetCurrentCardInfo()
    {
        CardInfo_Equip currentCI = (CardInfo_Equip) CardInfo.Clone();
        currentCI.ShieldInfo.Armor = M_ShieldArmor;
        currentCI.ShieldInfo.Shield = M_ShieldShield;
        return currentCI;
    }

    private string m_ShieldName;

    public string M_ShieldName
    {
        get { return m_ShieldName; }

        set
        {
            m_ShieldName = value;
            ShieldName.text = GameManager.Instance.isEnglish ? "" : Utils.TextToVertical(value);
            ShieldName_en.text = GameManager.Instance.isEnglish ? value : "";
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

    public void OnShieldEquiped()
    {
        ShieldEquipedAnim.SetTrigger("ShieldEquiped");
    }

    public override void MouseHoverComponent_OnHoverBegin(Vector3 mousePosition)
    {
        base.MouseHoverComponent_OnHoverBegin(mousePosition);
        if (M_Bloom) M_Bloom.gameObject.SetActive(true);
    }

    public override void MouseHoverComponent_OnHoverEnd()
    {
        base.MouseHoverComponent_OnHoverEnd();
        if (M_Bloom) M_Bloom.gameObject.SetActive(false);
    }

    #endregion

    #region SE

    public override void OnShowEffects(SideEffectBundle.TriggerTime triggerTime, SideEffectBundle.TriggerRange triggerRange)
    {
        BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_ShowSideEffectBloom(ClientUtils.HTMLColorToColor("#FFFFFF"), 0.8f), "ShowSideEffectBloom");
    }

    IEnumerator Co_ShowSideEffectBloom(Color color, float duration)
    {
        M_BloomSE.gameObject.SetActive(true);
        ClientUtils.ChangeColor(M_BloomSE, color);
        yield return new WaitForSeconds(duration);
        M_BloomSE.gameObject.SetActive(false);
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
    }

    #endregion

    #endregion
}