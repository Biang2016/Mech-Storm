public class MatchStandAloneRequest : ClientRequestBase
{
    public int buildID;

    public MatchStandAloneRequest() : base()
    {
    }

    public MatchStandAloneRequest(int clientId, int buildID) : base(clientId)
    {
        this.buildID = buildID;
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
        writer.WriteSInt32(buildID);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        buildID = reader.ReadSInt32();
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [buildID]=" + buildID;
        return log;
    }
}