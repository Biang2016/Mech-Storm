public struct BattleInfo
{
    public int BasicAttack;
    public int BasicShield;
    public int BasicArmor;
    public bool IsSoldier;

    public BattleInfo(int basicAttack, int basicShield, int basicArmor, bool isSoldier)
    {
        BasicAttack = basicAttack;
        BasicShield = basicShield;
        BasicArmor = basicArmor;
        IsSoldier = isSoldier;
    }

    public void Serialize(DataStream writer)
    {
        writer.WriteSInt32(BasicAttack);
        writer.WriteSInt32(BasicShield);
        writer.WriteSInt32(BasicArmor);
        writer.WriteByte(IsSoldier ? (byte) 0x01 : (byte) 0x00);
    }

    public static BattleInfo Deserialze(DataStream reader)
    {
        int BasicAttack = reader.ReadSInt32();
        int BasicShield = reader.ReadSInt32();
        int BasicArmor = reader.ReadSInt32();
        bool IsSoldier = reader.ReadByte() == 0x01;
        return new BattleInfo(BasicAttack, BasicShield, BasicArmor, IsSoldier);
    }
}