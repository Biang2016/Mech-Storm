public class ClientIdRequest : ServerRequestBase
{
    public int givenClientId;
    public string serverVersion;

    public ClientIdRequest()
    {
    }

    public ClientIdRequest(int givenClientId, string serverVersion)
    {
        this.givenClientId = givenClientId;
        this.serverVersion = serverVersion;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.CLIENT_ID_REQUEST;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(givenClientId);
        writer.WriteString8(serverVersion);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        givenClientId = reader.ReadSInt32();
        serverVersion = reader.ReadString8();
    }
}