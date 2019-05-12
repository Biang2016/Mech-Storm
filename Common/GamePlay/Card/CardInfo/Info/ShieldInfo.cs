using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public struct ShieldInfo
{
    public int Armor;
    public int Shield;
    public ShieldTypes ShieldType;
    public bool IsDefense;

    public ShieldInfo(int armor, int shield, ShieldTypes shieldType, bool isDefense)
    {
        Armor = armor;
        Shield = shield;
        ShieldType = shieldType;
        IsDefense = isDefense;
    }

    public void Serialize(DataStream writer)
    {
        writer.WriteSInt32(Armor);
        writer.WriteSInt32(Shield);
        writer.WriteSInt32((int) ShieldType);
        writer.WriteByte((byte) (IsDefense ? 0x01 : 0x00));
    }

    public static ShieldInfo Deserialze(DataStream reader)
    {
        int Armor = reader.ReadSInt32();
        int Shield = reader.ReadSInt32();
        ShieldTypes ShieldType = (ShieldTypes) reader.ReadSInt32();
        bool IsDefense = reader.ReadByte() == 0x01;
        return new ShieldInfo(Armor, Shield, ShieldType, IsDefense);
    }
}

[JsonConverter(typeof(StringEnumConverter))]
public enum ShieldTypes
{
    None = 0,
    Armor = 1,
    Shield = 2,
    Mixed = 3
}