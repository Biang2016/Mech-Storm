public class MatchStandAloneRequest : ClientRequestBase
{
    public int buildID;
    public bool isResume;

    public MatchStandAloneRequest() : base()
    {
    }

    public MatchStandAloneRequest(int clientID, int buildID, bool isResume) : base(clientID)
    {
        this.buildID = buildID;
        this.isResume = isResume;
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
        writer.WriteSInt32((byte) (isResume ? 0x01 : 0x00));
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        buildID = reader.ReadSInt32();
        isResume = reader.ReadByte() == 0x01;
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [buildID]=" + buildID;
        log += " [isResume]=" + isResume;
        return log;
    }
}