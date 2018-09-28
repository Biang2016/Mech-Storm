public struct WeaponInfo
{
    public int Energy;
    public int EnergyMax;
    public int Attack;
    public WeaponTypes WeaponType;

    public WeaponInfo(int energy, int energyMax, int attack, WeaponTypes weaponType)
    {
        Energy = energy;
        EnergyMax = energyMax;
        Attack = attack;
        WeaponType = weaponType;
    }

    public void Serialize(DataStream writer)
    {
        writer.WriteSInt32(Energy);
        writer.WriteSInt32(EnergyMax);
        writer.WriteSInt32(Attack);
        writer.WriteSInt32((int) WeaponType);
    }

    public static WeaponInfo Deserialze(DataStream reader)
    {
        int Energy = reader.ReadSInt32();
        int EnergyMax = reader.ReadSInt32();
        int Attack = reader.ReadSInt32();
        WeaponTypes WeaponType = (WeaponTypes) reader.ReadSInt32();
        return new WeaponInfo(Energy, EnergyMax, Attack, WeaponType);
    }
}

public enum WeaponTypes
{
    None = 0,
    Sword = 1,
    Gun = 2,
    SniperGun = 3,
}