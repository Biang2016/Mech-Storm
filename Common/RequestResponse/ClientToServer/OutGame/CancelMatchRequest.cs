public class CancelMatchRequest : ClientRequestBase
{
    public CancelMatchRequest()
    {
    }

    public CancelMatchRequest(int clientId) : base(clientId)
    {
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.CANCEL_MATCH_REQUEST;
    }

}