public class StandaloneStartLevelRequest : ClientRequestBase
{
    public int ClientID;
    public int BuildID;
    public int ChapterID;
    public int LevelID;

    public StandaloneStartLevelRequest()
    {
    }

    public StandaloneStartLevelRequest(int clientID, int buildID, int chapterID, int levelID) : base(clientID)
    {
        ClientID = clientID;
        BuildID = buildID;
        ChapterID = chapterID;
        LevelID = levelID;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.STANDALONE_START_LEVEL_REQUEST;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(BuildID);
        writer.WriteSInt32(ChapterID);
        writer.WriteSInt32(LevelID);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        BuildID = reader.ReadSInt32();
        ChapterID = reader.ReadSInt32();
        LevelID = reader.ReadSInt32();
    }
}