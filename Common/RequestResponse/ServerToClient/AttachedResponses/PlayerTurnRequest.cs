public class PlayerTurnRequest : ServerRequestBase
{
    public int clientId;

    public PlayerTurnRequest()
    {

    }

    public PlayerTurnRequest(int clientId)
    {
        this.clientId = clientId;
    }
    public override int GetProtocol()
    {
        return NetProtocols.SE_PLAYER_TURN;
    }

	public override string GetProtocolName()
	{
        return "SE_PLAYER_TURN";
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

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [clientId]=" + clientId;
        return log;
    }
}