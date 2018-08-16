public struct ShieldInfo
{
    public int Armor;
    public int Shield;
    public ShieldTypes ShieldType;

    public ShieldInfo(int armor, int shield, ShieldTypes shieldType)
    {
        Armor = armor;
        Shield = shield;
        ShieldType = shieldType;
    }

    public void Serialize(DataStream writer)
    {
        writer.WriteSInt32(Armor);
        writer.WriteSInt32(Shield);
        writer.WriteSInt32((int) ShieldType);
    }

    public static ShieldInfo Deserialze(DataStream reader)
    {
        int Armor = reader.ReadSInt32();
        int Shield = reader.ReadSInt32();
        ShieldTypes ShieldType = (ShieldTypes) reader.ReadSInt32();
        return new ShieldInfo(Armor, Shield, ShieldType);
    }
}

public enum ShieldTypes
{
    Armor = 0,
    Shield = 1,
    Mixed = 2
}