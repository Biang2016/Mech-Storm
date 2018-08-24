public class DeleteBuildRequestResponse : ServerRequestBase
{
    public int buildID;

    public DeleteBuildRequestResponse() : base()
    {
    }

    public DeleteBuildRequestResponse(int buildID)
    {
        this.buildID = buildID;
    }

    public override int GetProtocol()
    {
        return NetProtocols.DELETE_BUILD_REQUEST_RESPONSE;
    }

    public override string GetProtocolName()
    {
        return "DELETE_BUILD_REQUEST_RESPONSE";
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