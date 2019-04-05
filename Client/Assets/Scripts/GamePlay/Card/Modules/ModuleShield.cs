using UnityEngine;

public class ModuleShield : ModuleEquip
{
    void Awake()
    {
        gameObjectPool = GameObjectPoolManager.Instance.PoolDict["ModuleShield"];
    }

    #region 各模块、自身数值和初始化

    [SerializeField] private Transform Block_ShieldArmor;
    protected CardNumberSet CardNumberSet_ShieldArmor;

    [SerializeField] private Transform Block_ShieldShield;
    protected CardNumberSet CardNumberSet_ShieldShield;

    public override void Initiate(CardInfo_Base cardInfo, ClientPlayer clientPlayer)
    {
        base.Initiate(cardInfo, clientPlayer);
        M_ShieldName = cardInfo.BaseInfo.CardNames[LanguageManager.Instance.GetCurrentLanguage()];
        M_ShieldType = cardInfo.ShieldInfo.ShieldType;
        M_ShieldArmor = cardInfo.ShieldInfo.Armor;
        M_ShieldShield = cardInfo.ShieldInfo.Shield;
    }

    private NumberSize my_NumberSize_Armor = NumberSize.Medium;
    private NumberSize my_NumberSize_Shield = NumberSize.Medium;
    private CardNumberSet.TextAlign my_TextAlign_Armor = CardNumberSet.TextAlign.Center;
    private CardNumberSet.TextAlign my_TextAlign_Shield = CardNumberSet.TextAlign.Center;

    public override void SetPreview()
    {
        base.SetPreview();
        my_NumberSize_Armor = NumberSize.Small;
        my_NumberSize_Shield = NumberSize.Small;
        my_TextAlign_Armor = CardNumberSet.TextAlign.Left;
        my_TextAlign_Shield = CardNumberSet.TextAlign.Left;
        M_ShieldArmor = M_ShieldArmor;
        M_ShieldShield = M_ShieldShield;
    }

    public override void SetNoPreview()
    {
        base.SetNoPreview();
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
            Name.text = LanguageManager.Instance.IsEnglish ? "" : Utils.TextToVertical(value);
            Name_en.text = LanguageManager.Instance.IsEnglish ? value : "";
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
                CardNumberSet.InitiateNumbers(ref CardNumberSet_ShieldArmor, my_NumberSize_Armor, my_TextAlign_Armor, Block_ShieldArmor, '+');
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
                CardNumberSet.InitiateNumbers(ref CardNumberSet_ShieldShield, my_NumberSize_Shield, my_TextAlign_Shield, Block_ShieldShield, '+');
                CardNumberSet_ShieldShield.Number = m_ShieldShield;
            }
        }
    }

    #endregion

    public void OnShieldEquiped()
    {
        EquipAnim.SetTrigger("ShieldEquiped");
    }
}