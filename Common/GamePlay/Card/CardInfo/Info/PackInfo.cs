public struct PackInfo
{
    public void Serialize(DataStream writer)
    {
    }

    public static PackInfo Deserialze(DataStream reader)
    {
        return new PackInfo();
    }
}
