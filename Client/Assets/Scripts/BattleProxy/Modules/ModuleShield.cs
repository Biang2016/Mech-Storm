internal class ModuleShield : ModuleEquip
{
    protected override void Initiate()
    {
        M_ShieldType = CardInfo.ShieldInfo.ShieldType;
    }

    private ShieldTypes m_ShieldType;

    public ShieldTypes M_ShieldType
    {
        get { return m_ShieldType; }

        set { m_ShieldType = value; }
    }
}