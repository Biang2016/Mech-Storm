public class MatchStandAloneRequest : ClientRequestBase
{
    public int ClientID;
    public int BuildID;
    public int LevelID;
    public int BossID;


    public MatchStandAloneRequest()
    {
    }

    public MatchStandAloneRequest(int clientID, int buildID, int levelID, int bossID) : base(clientID)
    {
        ClientID = clientID;
        BuildID = buildID;
        LevelID = levelID;
        BossID = bossID;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.MATCH_STANDALONE_REQUEST;
    }

    public override string GetProtocolName()
    {
        return "MATCH_STANDALONE_REQUEST";
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(BuildID);
        writer.WriteSInt32(LevelID);
        writer.WriteSInt32(BossID);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        BuildID = reader.ReadSInt32();
        LevelID = reader.ReadSInt32();
        BossID = reader.ReadSInt32();
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [BuildID]=" + BuildID;
        log += " [LevelID]=" + LevelID;
        log += " [BossID]=" + BossID;
        return log;
    }
}