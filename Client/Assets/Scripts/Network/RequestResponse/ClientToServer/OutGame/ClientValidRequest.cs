public class ClientVersionValidRequest : ClientRequestBase
{
    public ClientVersionValidRequest() : base()
    {
    }

    public ClientVersionValidRequest(int clientId) : base(clientId)
    {
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.CLIENT_VERSION_VALID_REQUEST;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
    }

}