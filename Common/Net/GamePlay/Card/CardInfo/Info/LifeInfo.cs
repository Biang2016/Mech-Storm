public struct LifeInfo
{
    public int Life;
    public int TotalLife;

    public LifeInfo(int life, int totalLife)
    {
        Life = life;
        TotalLife = totalLife;
    }

    public void Serialize(DataStream writer)
    {
        writer.WriteSInt32(Life);
        writer.WriteSInt32(TotalLife);
    }

    public static LifeInfo Deserialze(DataStream reader)
    {
        int Life = reader.ReadSInt32();
        int TotalLife = reader.ReadSInt32();
        return new LifeInfo(Life, TotalLife);
    }
}