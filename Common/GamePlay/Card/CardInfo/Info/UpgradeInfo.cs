public struct UpgradeInfo
{
    public int UpgradeCardID;
    public int DegradeCardID;
    public int CardLevel;
    public int CardLevelMax;

    public UpgradeInfo(int upgradeCardID, int degradeCardID, int cardLevel, int cardLevelMax)
    {
        UpgradeCardID = upgradeCardID;
        DegradeCardID = degradeCardID;
        CardLevel = cardLevel;
        CardLevelMax = cardLevelMax;
    }

    public void Serialize(DataStream writer)
    {
        writer.WriteSInt32(UpgradeCardID);
        writer.WriteSInt32(DegradeCardID);
        writer.WriteSInt32(CardLevel);
        writer.WriteSInt32(CardLevelMax);
    }

    public static UpgradeInfo Deserialze(DataStream reader)
    {
        int UpgradeCardID = reader.ReadSInt32();
        int DegradeCardID = reader.ReadSInt32();
        int CardLevel = reader.ReadSInt32();
        int CardLevelMax = reader.ReadSInt32();
        return new UpgradeInfo(UpgradeCardID, DegradeCardID, CardLevel, CardLevelMax);
    }
}