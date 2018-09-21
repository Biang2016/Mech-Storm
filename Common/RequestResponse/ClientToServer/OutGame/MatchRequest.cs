public class MatchRequest : ClientRequestBase
{
    public int buildID;

    public MatchRequest() : base()
    {
    }

    public MatchRequest(int clientId, int buildID) : base(clientId)
    {
        this.buildID = buildID;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.MATCH_REQUEST;
    }

    public override string GetProtocolName()
    {
        return "MATCH_REQUEST";
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