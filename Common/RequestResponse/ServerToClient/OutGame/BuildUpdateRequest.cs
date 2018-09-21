public class BuildUpdateRequest : ServerRequestBase
{
    public BuildInfo BuildInfo;

    public BuildUpdateRequest() 
    {
    }

    public BuildUpdateRequest(BuildInfo BuildInfo)
    {
        this.BuildInfo = BuildInfo;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.BUILD_UPDATE_RESPONSE;
    }

    public override string GetProtocolName()
    {
        return "BUILD_UPDATE_RESPONSE";
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        BuildInfo.Serialize(writer);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        BuildInfo = BuildInfo.Deserialize(reader);
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += BuildInfo.DeserializeLog();
        return log;
    }
}