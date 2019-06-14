public struct MAInfo
{
    public bool IsFrenzy;
    public bool IsDefense;
    public bool IsSniper;

    public MAInfo(bool isFrenzy, bool isDefense, bool isSniper)
    {
        IsFrenzy = isFrenzy;
        IsDefense = isDefense;
        IsSniper = isSniper;
    }

    public void Serialize(DataStream writer)
    {
        writer.WriteByte((byte) (IsFrenzy ? 0x01 : 0x00));
        writer.WriteByte((byte) (IsDefense ? 0x01 : 0x00));
        writer.WriteByte((byte) (IsSniper ? 0x01 : 0x00));
    }

    public static MAInfo Deserialze(DataStream reader)
    {
        bool IsFrenzy = reader.ReadByte() == 0x01;
        bool IsDefense = reader.ReadByte() == 0x01;
        bool IsSniper = reader.ReadByte() == 0x01;
        return new MAInfo(IsFrenzy, IsDefense, IsSniper);
    }
}