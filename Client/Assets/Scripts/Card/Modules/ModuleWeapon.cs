using System.Collections;
using UnityEngine;

public class ModuleWeapon : ModuleBase
{
    public override void PoolRecycle()
    {
        base.PoolRecycle();
        if (WeaponEquipAnim) WeaponEquipAnim.SetTrigger("Hide");
    }

    void Awake()
    {
        gameObjectPool = GameObjectPoolManager.Instance.Pool_ModuleWeaponPool;
    }

    #region 各模块、自身数值和初始化

    internal ModuleRetinue M_ModuleRetinue;
    [SerializeField] private TextMesh WeaponName;
    [SerializeField] private TextMesh WeaponName_en;
    [SerializeField] private Renderer M_Bloom;
    [SerializeField] private Renderer M_BloomSE;
    [SerializeField] private Renderer M_BloomSE_Sub;

    [SerializeField] private GameObject M_GunIcon;
    [SerializeField] private GameObject M_SwordIcon;

    [SerializeField] private GameObject Block_WeaponAttack;
    protected GameObject GoNumberSet_WeaponAttack;
    protected CardNumberSet CardNumberSet_WeaponAttack;

    [SerializeField] private GameObject Block_WeaponEnergy;
    protected GameObject GoNumberSet_WeaponEnergy;
    protected CardNumberSet CardNumberSet_WeaponEnergy;

    [SerializeField] private GameObject Block_WeaponEnergyMax;
    protected GameObject GoNumberSet_WeaponEnergyMax;
    protected CardNumberSet CardNumberSet_WeaponEnergyMax;

    [SerializeField] private Animator WeaponEquipAnim;

    [SerializeField] private Animator WeaponBloomSEAnim;
    [SerializeField] private Animator WeaponBloomSE_SubAnim;

    public int M_EquipID;

    public override void Initiate(CardInfo_Base cardInfo, ClientPlayer clientPlayer)
    {
        base.Initiate(cardInfo, clientPlayer);
        M_WeaponName = GameManager.Instance.isEnglish ? cardInfo.BaseInfo.CardName_en : cardInfo.BaseInfo.CardName;
        M_WeaponType = cardInfo.WeaponInfo.WeaponType;
        M_WeaponAttack = cardInfo.WeaponInfo.Attack;
        M_WeaponEnergyMax = cardInfo.WeaponInfo.EnergyMax;
        M_WeaponEnergy = cardInfo.WeaponInfo.Energy;

        if (M_Bloom) M_Bloom.gameObject.SetActive(false);
        if (M_BloomSE) M_BloomSE.gameObject.SetActive(false);
        if (M_WeaponType == WeaponTypes.Gun)
        {
            if (M_GunIcon) M_GunIcon.SetActive(true);
            if (M_SwordIcon) M_SwordIcon.SetActive(false);
        }
        else if (M_WeaponType == WeaponTypes.Sword)
        {
            if (M_GunIcon) M_GunIcon.SetActive(false);
            if (M_SwordIcon) M_SwordIcon.SetActive(true);
        }
    }

    public override void ChangeColor(Color color)
    {
        base.ChangeColor(color);
        ClientUtils.ChangeColor(M_Bloom, color);
        ClientUtils.ChangeColor(M_BloomSE, color);
    }

    private NumberSize my_NumberSize_Attack = NumberSize.Big;
    private NumberSize my_NumberSize_Energy = NumberSize.Medium;
    private NumberSize my_NumberSize_EnergyMax = NumberSize.Medium;
    private CardNumberSet.TextAlign my_TextAlign_Attack = CardNumberSet.TextAlign.Center;
    private CardNumberSet.TextAlign my_TextAlign_Energy = CardNumberSet.TextAlign.Left;
    private CardNumberSet.TextAlign my_TextAlign_EnergyMax = CardNumberSet.TextAlign.Right;

    public void SetPreview()
    {
        my_NumberSize_Attack = NumberSize.Small;
        my_TextAlign_Attack = CardNumberSet.TextAlign.Center;
        my_NumberSize_Energy = NumberSize.Small;
        my_TextAlign_Energy = CardNumberSet.TextAlign.Left;
        my_NumberSize_EnergyMax = NumberSize.Small;
        my_TextAlign_EnergyMax = CardNumberSet.TextAlign.Right;
        M_WeaponAttack = M_WeaponAttack;
        M_WeaponEnergyMax = M_WeaponEnergyMax;
        M_WeaponEnergy = M_WeaponEnergy;
        if (M_Bloom) M_Bloom.gameObject.SetActive(true);
    }

    public void SetNoPreview()
    {
        my_NumberSize_Attack = NumberSize.Big;
        my_TextAlign_Attack = CardNumberSet.TextAlign.Center;
        my_NumberSize_Energy = NumberSize.Medium;
        my_TextAlign_Energy = CardNumberSet.TextAlign.Left;
        my_NumberSize_EnergyMax = NumberSize.Medium;
        my_TextAlign_EnergyMax = CardNumberSet.TextAlign.Right;
        M_WeaponEnergy = M_WeaponEnergy;
        M_WeaponEnergyMax = M_WeaponEnergyMax;
        M_WeaponEnergy = M_WeaponEnergy;
    }

    #region 属性

    public CardInfo_Equip GetCurrentCardInfo()
    {
        CardInfo_Equip currentCI = (CardInfo_Equip) CardInfo.Clone();
        currentCI.WeaponInfo.Attack = M_WeaponAttack;
        currentCI.WeaponInfo.Energy = M_WeaponEnergy;
        currentCI.WeaponInfo.EnergyMax = M_WeaponEnergyMax;
        return currentCI;
    }

    private string m_WeaponName;

    public string M_WeaponName
    {
        get { return m_WeaponName; }

        set
        {
            m_WeaponName = value;
            WeaponName.text = GameManager.Instance.isEnglish ? "" : Utils.TextToVertical(value);
            WeaponName_en.text = GameManager.Instance.isEnglish ? value : "";
        }
    }

    private WeaponTypes m_WeaponType;

    public WeaponTypes M_WeaponType
    {
        get { return m_WeaponType; }

        set { m_WeaponType = value; }
    }


    private int m_WeaponAttack;

    public int M_WeaponAttack
    {
        get { return m_WeaponAttack; }

        set
        {
            m_WeaponAttack = value;
            if (Block_WeaponAttack)
            {
                initiateNumbers(ref GoNumberSet_WeaponAttack, ref CardNumberSet_WeaponAttack, my_NumberSize_Attack, my_TextAlign_Attack, Block_WeaponAttack, '+');
                CardNumberSet_WeaponAttack.Number = M_ModuleRetinue.M_RetinueAttack;
            }

            if (M_ModuleRetinue.M_RetinueAttack == 0)
            {
                GetComponent<DragComponent>().enabled = false;
            }
            else
            {
                GetComponent<DragComponent>().enabled = true;
            }
        }
    }

    private int m_WeaponEnergy;

    public int M_WeaponEnergy
    {
        get { return m_WeaponEnergy; }

        set
        {
            m_WeaponEnergy = Mathf.Min(value, M_WeaponEnergyMax);
            if (Block_WeaponEnergy)
            {
                initiateNumbers(ref GoNumberSet_WeaponEnergy, ref CardNumberSet_WeaponEnergy, my_NumberSize_Energy, my_TextAlign_Energy, Block_WeaponEnergy);
                CardNumberSet_WeaponEnergy.Number = m_WeaponEnergy;
            }

            if (m_WeaponEnergy == 0)
            {
                GetComponent<DragComponent>().enabled = false;
                BeDimColor();
            }
            else
            {
                GetComponent<DragComponent>().enabled = true;
                BeBrightColor();
            }
        }
    }

    private int m_WeaponEnergyMax;

    public int M_WeaponEnergyMax
    {
        get { return m_WeaponEnergyMax; }

        set
        {
            m_WeaponEnergyMax = value;
            if (Block_WeaponEnergyMax)
            {
                initiateNumbers(ref GoNumberSet_WeaponEnergyMax, ref CardNumberSet_WeaponEnergyMax, my_NumberSize_EnergyMax, my_TextAlign_EnergyMax, Block_WeaponEnergyMax, '/');
                CardNumberSet_WeaponEnergyMax.Number = m_WeaponEnergyMax;
            }
        }
    }

    #endregion

    #endregion

    #region 模块交互

    #region 攻击相关

    public void OnAttack() //特效
    {
    }

    #endregion

    #region 交互UX

    public void OnWeaponEquiped()
    {
        WeaponEquipAnim.SetTrigger("WeaponEquiped");
    }


    public override void MouseHoverComponent_OnHover1Begin(Vector3 mousePosition)
    {
        base.MouseHoverComponent_OnHover1Begin(mousePosition);
        if (M_Bloom) M_Bloom.gameObject.SetActive(true);
    }

    public override void MouseHoverComponent_OnHover1End()
    {
        base.MouseHoverComponent_OnHover1End();
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
        M_BloomSE_Sub.gameObject.SetActive(true);
        WeaponBloomSEAnim.SetTrigger("OnSE");
        WeaponBloomSE_SubAnim.SetTrigger("OnSE");
        ClientUtils.ChangeColor(M_BloomSE, color);
        ClientUtils.ChangeColor(M_BloomSE_Sub, color);
        AudioManager.Instance.SoundPlay("sfx/OnSE");
        yield return new WaitForSeconds(duration);
        WeaponBloomSEAnim.SetTrigger("Reset");
        WeaponBloomSE_SubAnim.SetTrigger("Reset");
        M_BloomSE.gameObject.SetActive(false);
        M_BloomSE_Sub.gameObject.SetActive(false);
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
    }

    #endregion

    #endregion
}