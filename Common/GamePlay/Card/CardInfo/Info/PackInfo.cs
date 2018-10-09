public struct PackInfo
{
    public bool IsFrenzy;
    public bool IsDefence;
    public bool IsSniper;
    public int DodgeProp;

    public PackInfo(bool isFrenzy, bool isDefence, bool isSniper, int dodgeProp)
    {
        IsFrenzy = isFrenzy;
        IsDefence = isDefence;
        IsSniper = isSniper;
        DodgeProp = dodgeProp;
    }

    public void Serialize(DataStream writer)
    {
        writer.WriteByte((byte) (IsFrenzy ? 0x01 : 0x00));
        writer.WriteByte((byte) (IsDefence ? 0x01 : 0x00));
        writer.WriteByte((byte) (IsSniper ? 0x01 : 0x00));
        writer.WriteSInt32(DodgeProp);
    }

    public static PackInfo Deserialze(DataStream reader)
    {
        bool IsFrenzy = reader.ReadByte() == 0x01;
        bool IsDefence = reader.ReadByte() == 0x01;
        bool IsSniper = reader.ReadByte() == 0x01;
        int DodgeProp = reader.ReadSInt32();
        return new PackInfo(IsFrenzy, IsDefence, IsSniper, DodgeProp);
    }
}