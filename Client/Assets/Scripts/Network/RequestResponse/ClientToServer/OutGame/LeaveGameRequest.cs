public class LeaveGameRequest : ClientRequestBase
{
    public LeaveGameRequest()
    {
    }

    public LeaveGameRequest(int clientId) : base(clientId)
    {
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.LEAVE_GAME_REQUEST;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
    }
}