public struct BattleInfo
{
    public int BasicAttack;
    public int BasicShield;
    public int BasicArmor;

    public BattleInfo(int basicAttack, int basicShield, int basicArmor)
    {
        BasicAttack = basicAttack;
        BasicShield = basicShield;
        BasicArmor = basicArmor;
    }

    public void Serialize(DataStream writer)
    {
        writer.WriteSInt32(BasicAttack);
        writer.WriteSInt32(BasicShield);
        writer.WriteSInt32(BasicArmor);
    }

    public static BattleInfo Deserialze(DataStream reader)
    {
        int BasicAttack = reader.ReadSInt32();
        int BasicShield = reader.ReadSInt32();
        int BasicArmor = reader.ReadSInt32();
        return new BattleInfo(BasicAttack, BasicShield, BasicArmor);
    }
}