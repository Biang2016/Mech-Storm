public class WinDirectlyRequest_ResponseBundle : ResponseBundleBase
{
    public WinDirectlyRequest_ResponseBundle()
    {
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.WIN_DIRECTLY_REQUEST_RESPONSE;
    }
}