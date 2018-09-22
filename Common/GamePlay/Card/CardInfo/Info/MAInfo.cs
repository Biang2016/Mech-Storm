public struct MAInfo
{
    public void Serialize(DataStream writer)
    {
    }

    public static MAInfo Deserialze(DataStream reader)
    {
        return new MAInfo();
    }
}
