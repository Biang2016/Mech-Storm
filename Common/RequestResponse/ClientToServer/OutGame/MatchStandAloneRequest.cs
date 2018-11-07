public class MatchStandAloneRequest : ClientRequestBase
{
    public int ClientID;
    public int BuildID;
    public int LevelID;
    public int BossPicID;


    public MatchStandAloneRequest()
    {
    }

    public MatchStandAloneRequest(int clientID, int buildID, int levelID, int bossPicID) : base(clientID)
    {
        ClientID = clientID;
        BuildID = buildID;
        LevelID = levelID;
        BossPicID = bossPicID;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.MATCH_STANDALONE_REQUEST;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(BuildID);
        writer.WriteSInt32(LevelID);
        writer.WriteSInt32(BossPicID);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        BuildID = reader.ReadSInt32();
        LevelID = reader.ReadSInt32();
        BossPicID = reader.ReadSInt32();
    }
}