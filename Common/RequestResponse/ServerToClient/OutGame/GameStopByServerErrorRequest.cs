public class GameStopByServerErrorRequest : ServerRequestBase
{
    public GameStopByServerErrorRequest()
    {

    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.GAME_STOP_BY_SERVER_ERROR_REQUEST;
    }

	public override string GetProtocolName()
	{
        return "GAME_STOP_BY_SERVER_ERROR_REQUEST";
	}
}