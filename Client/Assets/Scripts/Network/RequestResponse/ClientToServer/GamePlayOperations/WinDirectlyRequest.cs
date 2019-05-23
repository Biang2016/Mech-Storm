public class WinDirectlyRequest : ClientRequestBase
{
    public WinDirectlyRequest()
    {
    }

    public WinDirectlyRequest(int clientId) : base(clientId)
    {
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.WIN_DIRECTLY_REQUEST;
    }

}