public class CreateBuildRequestResponse : ServerRequestBase
{
    public BuildInfo buildInfo;

    public CreateBuildRequestResponse()
    {
    }

    public CreateBuildRequestResponse(BuildInfo buildInfo)
    {
        this.buildInfo = buildInfo;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.CREATE_BUILD_REQUEST_RESPONSE;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        buildInfo.Serialize(writer);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        buildInfo = BuildInfo.Deserialize(reader);
    }
}