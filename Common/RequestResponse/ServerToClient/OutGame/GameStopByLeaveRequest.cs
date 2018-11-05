public class GameStopByLeaveRequest : ServerRequestBase
{
    public int clientId;

    public GameStopByLeaveRequest()
    {

    }

    public GameStopByLeaveRequest(int clientId)
    {
        this.clientId = clientId;
    }
    public override NetProtocols GetProtocol()
    {
        return NetProtocols.GAME_STOP_BY_LEAVE_REQUEST;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(clientId);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        clientId = reader.ReadSInt32();
    }

}