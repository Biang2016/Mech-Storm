public class GameStopByWinRequest : ServerRequestBase
{
    public int WinnerClientId;
    public BattleStatistics BattleStatistics;

    public GameStopByWinRequest()
    {
    }

    public GameStopByWinRequest(int winnerClientId, BattleStatistics battleStatistics)
    {
        WinnerClientId = winnerClientId;
        BattleStatistics = battleStatistics;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.GAME_STOP_BY_WIN_REQUEST;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(WinnerClientId);
        BattleStatistics.Serialize(writer);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        WinnerClientId = reader.ReadSInt32();
        BattleStatistics = BattleStatistics.Deserialize(reader);
    }
}