public struct UpgradeInfo
{
    public int UpgradeCardID;
    public int CardLevel;

    public UpgradeInfo(int upgradeCardID, int cardLevel)
    {
        UpgradeCardID = upgradeCardID;
        CardLevel = cardLevel;
    }

    public void Serialize(DataStream writer)
    {
        writer.WriteSInt32(UpgradeCardID);
        writer.WriteSInt32(CardLevel);
    }

    public static UpgradeInfo Deserialze(DataStream reader)
    {
        int UpgradeCardID = reader.ReadSInt32();
        int CardLevel = reader.ReadSInt32();
        return new UpgradeInfo(UpgradeCardID, CardLevel);
    }
}