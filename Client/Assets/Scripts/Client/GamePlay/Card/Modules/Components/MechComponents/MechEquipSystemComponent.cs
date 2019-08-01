using UnityEngine;

public class MechEquipSystemComponent : MechComponentBase
{
    [SerializeField] private Transform[] EquipPivots;

    protected override void Child_Initialize()
    {
    }

    protected override void Reset()
    {
        PoolRecycle();
    }

    public void PoolRecycle()
    {
        if (M_Weapon)
        {
            M_Weapon.PoolRecycle();
            M_Weapon = null;
        }

        if (M_Shield)
        {
            M_Shield.PoolRecycle();
            M_Shield = null;
        }

        if (M_Pack)
        {
            M_Pack.PoolRecycle();
            M_Pack = null;
        }

        if (M_MA)
        {
            M_MA.PoolRecycle();
            M_MA = null;
        }
    }

    void Awake()
    {
        Reset();
    }

    protected override void SetSortingIndexOfCard(int cardSortingIndex)
    {
    }

    #region 拼装上的模块

    internal bool IsAllEquipExceptMA
    {
        get
        {
            if (Mech.CardInfo.MechInfo.Slots[0] != SlotTypes.None && M_Weapon == null) return false;
            if (Mech.CardInfo.MechInfo.Slots[1] != SlotTypes.None && M_Shield == null) return false;
            if (Mech.CardInfo.MechInfo.Slots[2] != SlotTypes.None && M_Pack == null) return false;
            return true;
        }
    }

    public ModuleEquip GetEquipBySlotType(SlotTypes slotType)
    {
        switch (slotType)
        {
            case SlotTypes.Weapon:
                return M_Weapon;
            case SlotTypes.Shield:
                return M_Shield;
            case SlotTypes.Pack:
                return M_Pack;
            case SlotTypes.MA:
                return M_MA;
        }

        return null;
    }

    #region 武器相关

    private void EquipSystemRefresh()
    {
        Mech.MechAttrShapesComponent.Initialize(Mech);
    }

    private ModuleWeapon m_Weapon;

    public ModuleWeapon M_Weapon
    {
        get { return m_Weapon; }
        set
        {
            if (m_Weapon && !value)
            {
                m_Weapon.PoolRecycle();
                m_Weapon = value;
                On_WeaponDown();
            }
            else if (!m_Weapon && value)
            {
                m_Weapon = value;
                On_WeaponEquipped();
            }
            else if (m_Weapon != value)
            {
                m_Weapon.PoolRecycle();
                m_Weapon = value;
                On_WeaponChanged();
            }

            EquipSystemRefresh();
        }
    }

    public void EquipWeapon(CardInfo_Equip cardInfo, int equipId)
    {
        ModuleWeapon newWeapon = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.ModuleWeapon].AllocateGameObject<ModuleWeapon>(transform);
        newWeapon.M_ModuleMech = Mech;
        newWeapon.Initiate(cardInfo, Mech.ClientPlayer);
        newWeapon.M_EquipID = equipId;
        newWeapon.transform.position = EquipPivots[0].position;
        M_Weapon = newWeapon;
    }

    private void On_WeaponDown()
    {
        AudioManager.Instance.SoundPlay("sfx/OnEquipDown");
        Mech.MechAttrShapesComponent.OnAttrShapeShow();
    }

    private void On_WeaponEquipped()
    {
        M_Weapon.OnEquipped();
        Mech.MechAttrShapesComponent.OnAttrShapeShow();
        AudioManager.Instance.SoundPlay("sfx/OnEquipWeapon");
    }

    private void On_WeaponChanged()
    {
        M_Weapon.OnEquipped();
        Mech.MechAttrShapesComponent.OnAttrShapeShow();
        AudioManager.Instance.SoundPlay("sfx/OnEquipWeapon");
    }

    #endregion

    #region 防具相关

    private ModuleShield m_Shield;

    public ModuleShield M_Shield
    {
        get { return m_Shield; }
        set
        {
            if (m_Shield && !value)
            {
                m_Shield.PoolRecycle();
                m_Shield = value;
                On_ShieldDown();
            }
            else if (!m_Shield && value)
            {
                m_Shield = value;
                On_ShieldEquipped();
            }
            else if (m_Shield != value)
            {
                m_Shield.PoolRecycle();
                m_Shield = value;
                On_ShieldChanged();
            }

            EquipSystemRefresh();
        }
    }

    public void EquipShield(CardInfo_Equip cardInfo, int equipId)
    {
        ModuleShield newShield = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.ModuleShield].AllocateGameObject<ModuleShield>(transform);
        newShield.M_ModuleMech = Mech;
        newShield.Initiate(cardInfo, Mech.ClientPlayer);
        newShield.M_EquipID = equipId;
        newShield.transform.position = EquipPivots[1].position;
        M_Shield = newShield;
    }

    private void On_ShieldDown()
    {
        AudioManager.Instance.SoundPlay("sfx/OnEquipDown");
        Mech.MechAttrShapesComponent.OnAttrShapeShow();
    }

    private void On_ShieldEquipped()
    {
        M_Shield.OnEquipped();
        Mech.MechAttrShapesComponent.OnAttrShapeShow();
        AudioManager.Instance.SoundPlay("sfx/OnEquipShield");
    }

    private void On_ShieldChanged()
    {
        M_Shield.OnEquipped();
        Mech.MechAttrShapesComponent.OnAttrShapeShow();
        AudioManager.Instance.SoundPlay("sfx/OnEquipShield");
    }

    #endregion

    #region 飞行背包相关

    private ModulePack m_Pack;

    public ModulePack M_Pack
    {
        get { return m_Pack; }
        set
        {
            if (m_Pack && !value)
            {
                m_Pack.PoolRecycle();
                m_Pack = value;
                On_PackDown();
            }
            else if (!m_Pack && value)
            {
                m_Pack = value;
                On_PackEquipped();
            }
            else if (m_Pack != value)
            {
                m_Pack.PoolRecycle();
                m_Pack = value;
                On_PackChanged();
            }

            EquipSystemRefresh();
        }
    }

    public void EquipPack(CardInfo_Equip cardInfo, int equipId)
    {
        ModulePack newPack = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.ModulePack].AllocateGameObject<ModulePack>(transform);
        newPack.M_ModuleMech = Mech;
        newPack.Initiate(cardInfo, Mech.ClientPlayer);
        newPack.M_EquipID = equipId;
        newPack.transform.position = EquipPivots[2].position;
        M_Pack = newPack;
    }

    private void On_PackDown()
    {
        AudioManager.Instance.SoundPlay("sfx/OnEquipDown");
    }

    private void On_PackEquipped()
    {
        M_Pack.OnEquipped();
        AudioManager.Instance.SoundPlay("sfx/OnEquipPack");
    }

    private void On_PackChanged()
    {
        M_Pack.OnEquipped();
        AudioManager.Instance.SoundPlay("sfx/OnEquipPack");
    }

    #endregion

    #region MA相关

    private ModuleMA m_MA;

    public ModuleMA M_MA
    {
        get { return m_MA; }
        set
        {
            if (m_MA && !value)
            {
                m_MA.PoolRecycle();
                m_MA = value;
                On_MADown();
            }
            else if (!m_MA && value)
            {
                m_MA = value;
                On_MAEquipped();
            }
            else if (m_MA != value)
            {
                m_MA.PoolRecycle();
                m_MA = value;
                On_MAChanged();
            }

            EquipSystemRefresh();
        }
    }

    public void EquipMA(CardInfo_Equip cardInfo, int equipId)
    {
        ModuleMA newMA = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.ModuleMA].AllocateGameObject<ModuleMA>(transform);
        newMA.M_ModuleMech = Mech;
        newMA.Initiate(cardInfo, Mech.ClientPlayer);
        newMA.M_EquipID = equipId;
        newMA.transform.position = EquipPivots[3].position;
        M_MA = newMA;
    }

    private void On_MADown()
    {
        AudioManager.Instance.SoundPlay("sfx/OnEquipDown");
        Mech.MechSwordShieldArmorComponent.MA_BG.SetActive(false);
    }

    private void On_MAEquipped()
    {
        M_MA.OnEquipped();
        Mech.MechSwordShieldArmorComponent.MA_BG.SetActive(true);
        AudioManager.Instance.SoundPlay("sfx/OnEquipMA");
    }

    private void On_MAChanged()
    {
        M_MA.OnEquipped();
        AudioManager.Instance.SoundPlay("sfx/OnEquipMA");
    }

    #endregion

    #endregion
}