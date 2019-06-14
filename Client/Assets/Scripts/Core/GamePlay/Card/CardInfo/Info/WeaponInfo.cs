using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public struct WeaponInfo
{
    public int Energy;
    public int EnergyMax;
    public int Attack;
    public WeaponTypes WeaponType;
    public bool IsSentry; // 防御的哨戒炮，不可主动攻击
    public bool IsFrenzy; // 是否是狂暴的，每回合可攻击两次

    public WeaponInfo(int energy, int energyMax, int attack, WeaponTypes weaponType, bool isSentry, bool isFrenzy)
    {
        Energy = energy;
        EnergyMax = energyMax;
        Attack = attack;
        WeaponType = weaponType;
        IsSentry = isSentry;
        IsFrenzy = isFrenzy;
    }

    public void Serialize(DataStream writer)
    {
        writer.WriteSInt32(Energy);
        writer.WriteSInt32(EnergyMax);
        writer.WriteSInt32(Attack);
        writer.WriteSInt32((int) WeaponType);
        writer.WriteByte((byte) (IsSentry ? 0x01 : 0x00));
        writer.WriteByte((byte) (IsFrenzy ? 0x01 : 0x00));
    }

    public static WeaponInfo Deserialze(DataStream reader)
    {
        int Energy = reader.ReadSInt32();
        int EnergyMax = reader.ReadSInt32();
        int Attack = reader.ReadSInt32();
        WeaponTypes WeaponType = (WeaponTypes) reader.ReadSInt32();
        bool IsSentry = reader.ReadByte() == 0x01;
        bool IsFrenzy = reader.ReadByte() == 0x01;
        return new WeaponInfo(Energy, EnergyMax, Attack, WeaponType, IsSentry, IsFrenzy);
    }
}

[JsonConverter(typeof(StringEnumConverter))]
public enum WeaponTypes
{
    None = 0,
    Sword = 1,
    Gun = 2,
    SniperGun = 3,
}