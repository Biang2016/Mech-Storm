public class EndRoundRequest : ClientRequestBase
{
    public EndRoundRequest()
    {
    }

    public EndRoundRequest(int clientId) : base(clientId)
    {
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.END_ROUND_REQUEST;
    }
}