public struct MAInfo
{
    public bool IsFrenzy;
    public bool IsDefence;
    public bool IsSniper;

    public MAInfo(bool isFrenzy, bool isDefence, bool isSniper)
    {
        IsFrenzy = isFrenzy;
        IsDefence = isDefence;
        IsSniper = isSniper;
    }

    public void Serialize(DataStream writer)
    {
        writer.WriteByte((byte) (IsFrenzy ? 0x01 : 0x00));
        writer.WriteByte((byte) (IsDefence ? 0x01 : 0x00));
        writer.WriteByte((byte) (IsSniper ? 0x01 : 0x00));
    }

    public static MAInfo Deserialze(DataStream reader)
    {
        bool IsFrenzy = reader.ReadByte() == 0x01;
        bool IsDefence = reader.ReadByte() == 0x01;
        bool IsSniper = reader.ReadByte() == 0x01;
        return new MAInfo(IsFrenzy, IsDefence, IsSniper);
    }
}