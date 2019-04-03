public class MatchStandAloneRequest : ClientRequestBase
{
    public int ClientID;
    public int BuildID;
    public int ChapterID;
    public int EnemyPicID;


    public MatchStandAloneRequest()
    {
    }

    public MatchStandAloneRequest(int clientID, int buildID, int chapterID, int enemyPicID) : base(clientID)
    {
        ClientID = clientID;
        BuildID = buildID;
        ChapterID = chapterID;
        EnemyPicID = enemyPicID;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.MATCH_STANDALONE_REQUEST;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(BuildID);
        writer.WriteSInt32(ChapterID);
        writer.WriteSInt32(EnemyPicID);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        BuildID = reader.ReadSInt32();
        ChapterID = reader.ReadSInt32();
        EnemyPicID = reader.ReadSInt32();
    }
}