public class EndRoundRequest_ResponseBundle : ResponseBundleBase
{
    public EndRoundRequest_ResponseBundle()
    {
    }
    public override NetProtocols GetProtocol()
    {
        return NetProtocols.END_ROUND_REQUEST_RESPONSE;
    }
}