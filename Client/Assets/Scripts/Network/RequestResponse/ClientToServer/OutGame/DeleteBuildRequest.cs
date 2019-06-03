public class DeleteBuildRequest : ClientRequestBase
{
    public int buildID;
    public bool isSingle;

    public DeleteBuildRequest() : base()
    {
    }

    public DeleteBuildRequest(int clientID, int buildID, bool isSingle) : base(clientID)
    {
        this.buildID = buildID;
        this.isSingle = isSingle;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.DELETE_BUILD_REQUEST;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(buildID);
        writer.WriteByte((byte) (isSingle ? 0x01 : 0x00));
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        buildID = reader.ReadSInt32();
        isSingle = reader.ReadByte() == 0x01;
    }
}