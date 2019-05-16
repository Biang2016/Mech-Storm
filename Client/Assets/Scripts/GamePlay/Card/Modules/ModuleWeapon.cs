using System.Collections;
using TMPro;
using UnityEngine;

public class ModuleWeapon : ModuleEquip
{
    [SerializeField] private SpriteRenderer M_WeaponTypeIcon;
    [SerializeField] private Sprite[] WeaponTypeIcons;

    [SerializeField] private TextMeshPro WeaponAttackText;
    [SerializeField] private TextMeshPro WeaponEnergyText;
    [SerializeField] private TextMeshPro WeaponEnergyMaxText;

    public override void Initiate(CardInfo_Base cardInfo, ClientPlayer clientPlayer)
    {
        base.Initiate(cardInfo, clientPlayer);
        M_WeaponType = cardInfo.WeaponInfo.WeaponType;
        M_WeaponAttack = cardInfo.WeaponInfo.Attack;
        M_WeaponEnergyMax = cardInfo.WeaponInfo.EnergyMax;
        M_WeaponEnergy = cardInfo.WeaponInfo.Energy;
    }

    #region Preview Details

    public override void SetPreview()
    {
        base.SetPreview();
        M_WeaponAttack = M_WeaponAttack;
        M_WeaponEnergyMax = M_WeaponEnergyMax;
        M_WeaponEnergy = M_WeaponEnergy;
        if (M_Bloom) M_Bloom.gameObject.SetActive(true);
        if (M_WeaponType == WeaponTypes.Gun)
        {
            M_WeaponTypeIcon.sprite = WeaponTypeIcons[1];
        }
        else if (M_WeaponType == WeaponTypes.Sword)
        {
            M_WeaponTypeIcon.sprite = WeaponTypeIcons[0];
        }
        else if (M_WeaponType == WeaponTypes.SniperGun)
        {
            M_WeaponTypeIcon.sprite = WeaponTypeIcons[2];
        }
    }

    public override void SetNoPreview()
    {
        base.SetNoPreview();
        M_WeaponEnergy = M_WeaponEnergy;
        M_WeaponEnergyMax = M_WeaponEnergyMax;
        M_WeaponEnergy = M_WeaponEnergy;
    }

    #endregion

    #region 属性

    public override CardInfo_Equip GetCurrentCardInfo()
    {
        CardInfo_Equip currentCI = (CardInfo_Equip) CardInfo.Clone();
        currentCI.WeaponInfo.Attack = M_WeaponAttack;
        currentCI.WeaponInfo.Energy = M_WeaponEnergy;
        currentCI.WeaponInfo.EnergyMax = M_WeaponEnergyMax;
        return currentCI;
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
            if (WeaponAttackText) WeaponAttackText.text = M_ModuleMech.M_MechAttack.ToString();
            DragComponent.enabled = M_ModuleMech.M_MechAttack != 0;
        }
    }

    private int m_WeaponEnergy;

    public int M_WeaponEnergy
    {
        get { return m_WeaponEnergy; }

        set
        {
            m_WeaponEnergy = Mathf.Min(value, M_WeaponEnergyMax);
            BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_WeaponEnergyChange(m_WeaponEnergy), "Co_WeaponEnergyChange");
        }
    }

    IEnumerator Co_WeaponEnergyChange(int value)
    {
        if (WeaponEnergyText) WeaponEnergyText.text = value.ToString();

        if (value == 0)
        {
            BeDimColor();
        }
        else
        {
            BeBrightColor();
        }

        yield return null;
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
    }

    private int m_WeaponEnergyMax;

    public int M_WeaponEnergyMax
    {
        get { return m_WeaponEnergyMax; }

        set
        {
            m_WeaponEnergyMax = value;

            if (WeaponEnergyMaxText) WeaponEnergyMaxText.text = m_WeaponEnergyMax.ToString();
        }
    }

    #endregion

    public void OnWeaponEquipped()
    {
        EquipAnim.SetTrigger("WeaponEquipped");
    }
}