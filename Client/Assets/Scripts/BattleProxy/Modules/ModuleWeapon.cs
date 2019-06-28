internal class ModuleWeapon : ModuleEquip
{
    protected override void Initiate()
    {
        M_WeaponType = CardInfo.WeaponInfo.WeaponType;
    }

    private WeaponTypes m_WeaponType;

    public WeaponTypes M_WeaponType
    {
        get { return m_WeaponType; }

        set { m_WeaponType = value; }
    }
}