public class BuildRequest : ClientRequestBase
{
    public BuildInfo BuildInfo;

    public BuildRequest() : base()
    {
    }

    public BuildRequest(int clientId, BuildInfo BuildInfo) : base(clientId)
    {
        this.BuildInfo = BuildInfo;
    }

    public override int GetProtocol()
    {
        return NetProtocols.BUILD_REQUEST;
    }

    public override string GetProtocolName()
    {
        return "BUILD_REQUEST";
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