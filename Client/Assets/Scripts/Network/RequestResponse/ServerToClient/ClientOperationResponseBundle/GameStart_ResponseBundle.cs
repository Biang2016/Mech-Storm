public class GameStart_ResponseBundle : ResponseBundleBase
{
    public GameStart_ResponseBundle()
    {
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.GAME_START_RESPONSE;
    }
}