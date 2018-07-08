using System.Collections;
using System.Collections.Generic;

public class ClientIdRequest : Request
{
    public int clientId;
    public ClientIdPurpose purpose;

    public override int GetProtocol()
    {
        return NetProtocols.SEND_CLIENT_ID;
    }

    public ClientIdRequest(int clientId, ClientIdPurpose purpose)
    {
        this.clientId = clientId;
        this.purpose = purpose;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(clientId);
        if (purpose == ClientIdPurpose.RegisterClientId)
        {
            writer.WriteByte(0x00);
        }
        else if (purpose == ClientIdPurpose.MatchGames)
        {
            writer.WriteByte(0x01);
        }
    }
}

public enum ClientIdPurpose
{
    RegisterClientId = 0,
    MatchGames = 1
}