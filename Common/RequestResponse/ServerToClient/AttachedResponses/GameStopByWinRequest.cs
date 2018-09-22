public class GameStopByWinRequest : ServerRequestBase
{
    public int winnerClientId;

    public GameStopByWinRequest()
    {
    }

    public GameStopByWinRequest(int winnerClientId)
    {
        this.winnerClientId = winnerClientId;
    }
    public override NetProtocols GetProtocol()
    {
        return NetProtocols.GAME_STOP_BY_WIN_REQUEST;
    }

	public override string GetProtocolName()
	{
        return "GAME_STOP_BY_WIN_REQUEST";
	}

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(winnerClientId);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        winnerClientId = reader.ReadSInt32();
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [winnerClientId]=" + winnerClientId;
        return log;
    }
}