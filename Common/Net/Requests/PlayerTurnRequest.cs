using System.Collections;
using System.Collections.Generic;

public class PlayerTurnRequest : Request
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
        return NetProtocols.PLAYER_TURN;
    }

	public override string GetProtocolName()
	{
        return "PLAYER_TURN";
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
        string log = "";
        log += " [clientId] " + clientId;
        return log;
    }
}