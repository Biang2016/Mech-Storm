public class BuildRequest : ClientRequestBase
{
    public BuildInfo BuildInfo;
    public bool isSingle;

    public BuildRequest() : base()
    {
    }

    public BuildRequest(int clientId, BuildInfo BuildInfo, bool isSingle) : base(clientId)
    {
        this.BuildInfo = BuildInfo;
        this.isSingle = isSingle;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.BUILD_REQUEST;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        BuildInfo.Serialize(writer);
        writer.WriteByte((byte) (isSingle ? 0x01 : 0x00));
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        BuildInfo = BuildInfo.Deserialize(reader);
        isSingle = reader.ReadByte() == 0x01;
    }
}