public class EndLevelRequest : ClientRequestBase
{
    public EndLevelRequest()
    {
    }

    public EndLevelRequest(int clientId) : base(clientId)
    {
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.END_LEVEL_REQUEST;
    }

}