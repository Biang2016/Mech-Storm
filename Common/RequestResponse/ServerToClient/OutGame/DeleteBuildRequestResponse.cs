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

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.DELETE_BUILD_REQUEST_RESPONSE;
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

}