using System.Collections;
using System.Collections.Generic;

public class ClientEndRoundRequest : Request
{
    public int clientId;

    public override int GetProtocol()
    {
        return NetProtocols.CLIENT_END_ROUND;
    }

    public ClientEndRoundRequest(int clientId)
    {
        clientId = clientId;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(clientId);
    }
}