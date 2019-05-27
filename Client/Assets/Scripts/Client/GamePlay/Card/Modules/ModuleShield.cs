using TMPro;
using UnityEngine;

public class ModuleShield : ModuleEquip
{
    [SerializeField] private TextMeshPro ArmorText;
    [SerializeField] private TextMeshPro ShieldText;

    public override void Initiate(CardInfo_Base cardInfo, ClientPlayer clientPlayer)
    {
        base.Initiate(cardInfo, clientPlayer);
        M_ShieldType = cardInfo.ShieldInfo.ShieldType;
        M_ShieldArmor = cardInfo.ShieldInfo.Armor;
        M_ShieldShield = cardInfo.ShieldInfo.Shield;
    }

    #region Preview Details

    public override void SetPreview()
    {
        base.SetPreview();
        //Todo 改字大小？
        M_ShieldArmor = M_ShieldArmor;
        M_ShieldShield = M_ShieldShield;
    }

    public override void SetNoPreview()
    {
        base.SetNoPreview();
        //Todo 改字大小？
        M_ShieldArmor = M_ShieldArmor;
        M_ShieldShield = M_ShieldShield;
    }

    #endregion

    #region 属性

    public override void OnEquipped()
    {
        EquipAnim.SetTrigger("Equipped");
    }

    public override CardInfo_Equip GetCurrentCardInfo()
    {
        CardInfo_Equip currentCI = (CardInfo_Equip) CardInfo.Clone();
        currentCI.ShieldInfo.Armor = M_ShieldArmor;
        currentCI.ShieldInfo.Shield = M_ShieldShield;
        return currentCI;
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
            if (ArmorText) ArmorText.text = m_ShieldArmor.ToString();
        }
    }

    private int m_ShieldShield;

    public int M_ShieldShield
    {
        get { return m_ShieldShield; }

        set
        {
            m_ShieldShield = value;
            if (ShieldText) ShieldText.text = m_ShieldShield.ToString();
        }
    }

    #endregion
}