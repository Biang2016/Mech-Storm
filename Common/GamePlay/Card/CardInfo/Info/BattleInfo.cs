public struct BattleInfo
{
    public int BasicAttack;
    public int BasicShield;
    public int BasicArmor;
    public bool IsSodier;

    public BattleInfo(int basicAttack, int basicShield, int basicArmor, bool isSodier)
    {
        BasicAttack = basicAttack;
        BasicShield = basicShield;
        BasicArmor = basicArmor;
        IsSodier = isSodier;
    }

    public void Serialize(DataStream writer)
    {
        writer.WriteSInt32(BasicAttack);
        writer.WriteSInt32(BasicShield);
        writer.WriteSInt32(BasicArmor);
        writer.WriteByte(IsSodier ? (byte) 0x01 : (byte) 0x00);
    }

    public static BattleInfo Deserialze(DataStream reader)
    {
        int BasicAttack = reader.ReadSInt32();
        int BasicShield = reader.ReadSInt32();
        int BasicArmor = reader.ReadSInt32();
        bool IsSodier = reader.ReadByte() == 0x01;
        return new BattleInfo(BasicAttack, BasicShield, BasicArmor, IsSodier);
    }
}