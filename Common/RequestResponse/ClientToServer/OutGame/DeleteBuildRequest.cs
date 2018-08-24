public class DeleteBuildRequest : ClientRequestBase
{
    public int buildID;

    public DeleteBuildRequest() : base()
    {
    }

    public DeleteBuildRequest(int clientID, int buildID) : base(clientID)
    {
        this.buildID = buildID;
    }

    public override int GetProtocol()
    {
        return NetProtocols.DELETE_BUILD_REQUEST;
    }

    public override string GetProtocolName()
    {
        return "DELETE_BUILD_REQUEST";
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