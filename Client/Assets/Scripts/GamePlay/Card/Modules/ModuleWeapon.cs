using System.Collections;
using TMPro;
using UnityEngine;

public class ModuleWeapon : ModuleEquip
{
    [SerializeField] private GameObject M_GunIcon;
    [SerializeField] private GameObject M_SwordIcon;
    [SerializeField] private GameObject M_SniperGunIcon;

    [SerializeField] private TextMeshPro WeaponAttackText;
    [SerializeField] private TextMeshPro WeaponEnergyText;
    [SerializeField] private TextMeshPro WeaponEnergyMaxText;

    [SerializeField] private DragComponent M_DragComponent;

    public override void Initiate(CardInfo_Base cardInfo, ClientPlayer clientPlayer)
    {
        base.Initiate(cardInfo, clientPlayer);
        M_WeaponName = cardInfo.BaseInfo.CardNames[LanguageManager.Instance.GetCurrentLanguage()];
        M_WeaponType = cardInfo.WeaponInfo.WeaponType;
        M_WeaponAttack = cardInfo.WeaponInfo.Attack;
        M_WeaponEnergyMax = cardInfo.WeaponInfo.EnergyMax;
        M_WeaponEnergy = cardInfo.WeaponInfo.Energy;
        if (M_WeaponType == WeaponTypes.Gun)
        {
            if (M_GunIcon) M_GunIcon.SetActive(true);
            if (M_SwordIcon) M_SwordIcon.SetActive(false);
            if (M_SniperGunIcon) M_SniperGunIcon.SetActive(false);
        }
        else if (M_WeaponType == WeaponTypes.Sword)
        {
            if (M_GunIcon) M_GunIcon.SetActive(false);
            if (M_SwordIcon) M_SwordIcon.SetActive(true);
            if (M_SniperGunIcon) M_SniperGunIcon.SetActive(false);
        }
        else if (M_WeaponType == WeaponTypes.SniperGun)
        {
            if (M_GunIcon) M_GunIcon.SetActive(false);
            if (M_SwordIcon) M_SwordIcon.SetActive(false);
            if (M_SniperGunIcon) M_SniperGunIcon.SetActive(true);
        }
    }

    public override void SetPreview()
    {
        base.SetPreview();
        M_WeaponAttack = M_WeaponAttack;
        M_WeaponEnergyMax = M_WeaponEnergyMax;
        M_WeaponEnergy = M_WeaponEnergy;
        if (M_Bloom) M_Bloom.gameObject.SetActive(true);
    }

    public override void SetNoPreview()
    {
        base.SetNoPreview();
        M_WeaponEnergy = M_WeaponEnergy;
        M_WeaponEnergyMax = M_WeaponEnergyMax;
        M_WeaponEnergy = M_WeaponEnergy;
    }

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
            Name.text = LanguageManager.Instance.IsEnglish ? "" : Utils.TextToVertical(value);
            Name_en.text = LanguageManager.Instance.IsEnglish ? value : "";
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
            WeaponAttackText.text = M_ModuleMech.M_MechAttack.ToString();
            M_DragComponent.enabled = M_ModuleMech.M_MechAttack != 0;
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
        WeaponEnergyText.text = value.ToString();

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

            WeaponEnergyMaxText.text = m_WeaponEnergyMax.ToString();
        }
    }

    public void OnAttack() //特效
    {
    }

    public void OnWeaponEquiped()
    {
        EquipAnim.SetTrigger("WeaponEquiped");
    }
}