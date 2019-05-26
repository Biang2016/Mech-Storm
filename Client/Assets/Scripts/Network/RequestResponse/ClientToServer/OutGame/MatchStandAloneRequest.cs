public class MatchStandaloneRequest : ClientRequestBase
{
    public int ClientID;
    public int BuildID;
    public int LevelID;


    public MatchStandaloneRequest()
    {
    }

    public MatchStandaloneRequest(int clientID, int buildID, int levelID) : base(clientID)
    {
        ClientID = clientID;
        BuildID = buildID;
        LevelID = levelID;
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
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        BuildID = reader.ReadSInt32();
        LevelID = reader.ReadSInt32();
    }
}