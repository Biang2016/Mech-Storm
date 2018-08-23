public struct BuildInfo
{
    public int BuildID;

    public string BuildName;

    public int[] CardIDs;

    public int[] BeginRetinueIDs;

    public int BuildConsumeMoney;

    public int Life;

    public int Magic;

    public BuildInfo(int buildID, string buildName, int[] cardIDs, int[] beginRetinueIDs, int buildConsumeMoney, int life, int magic)
    {
        BuildID = buildID;
        BuildName = buildName;
        CardIDs = cardIDs;
        BeginRetinueIDs = beginRetinueIDs;
        BuildConsumeMoney = buildConsumeMoney;
        Life = life;
        Magic = magic;
    }

    public void Serialize(DataStream writer)
    {
        writer.WriteSInt32(BuildID);
        writer.WriteString16(BuildName);
        writer.WriteSInt32(CardIDs.Length);
        foreach (int cardID in CardIDs)
        {
            writer.WriteSInt32(cardID);
        }

        writer.WriteSInt32(BeginRetinueIDs.Length);
        foreach (int beginRetinueID in BeginRetinueIDs)
        {
            writer.WriteSInt32(beginRetinueID);
        }

        writer.WriteSInt32(BuildConsumeMoney);
        writer.WriteSInt32(Life);
        writer.WriteSInt32(Magic);
    }

    public static BuildInfo Deserialize(DataStream reader)
    {
        int BuildID = reader.ReadSInt32();
        string BuildName = reader.ReadString16();

        int cardIdCount = reader.ReadSInt32();
        int[] CardIDs = new int[cardIdCount];
        for (int i = 0; i < cardIdCount; i++)
        {
            CardIDs[i] = reader.ReadSInt32();
        }

        int ceginRetinueIDCount = reader.ReadSInt32();
        int[] BeginRetinueIDs = new int[ceginRetinueIDCount];
        for (int i = 0; i < ceginRetinueIDCount; i++)
        {
            BeginRetinueIDs[i] = reader.ReadSInt32();
        }

        int BuildConsumeMoney = reader.ReadSInt32();
        int Life = reader.ReadSInt32();
        int Magic = reader.ReadSInt32();

        BuildInfo buildInfo = new BuildInfo(BuildID, BuildName, CardIDs, BeginRetinueIDs, BuildConsumeMoney, Life, Magic);
        return buildInfo;
    }

    public string DeserializeLog()
    {
        string log = " <BuildInfo>";
        log += " [CardIDs]=";
        foreach (int cardID in CardIDs)
        {
            log += cardID + " ";
        }

        log += " [BeginRetinueIDs]=";
        foreach (int cardID in BeginRetinueIDs)
        {
            log += cardID + " ";
        }

        log += " [BuildConsumeMoney]=" + BuildConsumeMoney;
        log += " [Life]=" + Life;
        log += " [Magic]=" + Magic;
        log += "</BuildInfo> ";
        return log;
    }
}