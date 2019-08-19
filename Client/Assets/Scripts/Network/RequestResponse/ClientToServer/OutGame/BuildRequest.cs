public class BuildRequest : ClientRequestBase
{
    public BuildInfo BuildInfo;
    public bool isSingle;
    public bool isStory;

    public BuildRequest() : base()
    {
    }

    public BuildRequest(int clientId, BuildInfo BuildInfo, bool isSingle, bool isStory) : base(clientId)
    {
        this.BuildInfo = BuildInfo;
        this.isSingle = isSingle;
        this.isStory = isStory;
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
        writer.WriteByte((byte) (isStory ? 0x01 : 0x00));
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        BuildInfo = BuildInfo.Deserialize(reader);
        isSingle = reader.ReadByte() == 0x01;
        isStory = reader.ReadByte() == 0x01;
    }
}