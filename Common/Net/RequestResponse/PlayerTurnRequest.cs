using System.Collections;
using System.Collections.Generic;

public class PlayerTurnRequest : Request
{
    public int clientId;

    public override int GetProtocol()
    {
        return NetProtocols.PLAYER_TURN;
    }

    public PlayerTurnRequest(int clientId)
    {
        clientId = clientId;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(clientId);
    }
}