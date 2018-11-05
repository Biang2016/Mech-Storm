public class CreateBuildRequestResponse : ServerRequestBase
{
    public int buildId;

    public CreateBuildRequestResponse()
    {
    }


    public CreateBuildRequestResponse( int buildId)
    {
        this.buildId = buildId;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.CREATE_BUILD_REQUEST_RESPONSE;
    }


    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(buildId);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        buildId = reader.ReadSInt32();
    }

}