using System.Collections;
using System.Collections.Generic;

public class ClientIdRequest : Request
{
    public int clientId;
    public ClientIdPurpose purpose;

    public ClientIdRequest()
    {
    }

    public ClientIdRequest(int clientId, ClientIdPurpose purpose)
    {
        this.clientId = clientId;
        this.purpose = purpose;
    }
    public override int GetProtocol()
    {
        return NetProtocols.SEND_CLIENT_ID;
    }

    public override string GetProtocolName()
    {
        return "SEND_CLIENT_ID";
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

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        this.clientId = reader.ReadSInt32();
        byte tmp = reader.ReadByte();
        if (tmp == 0x00)
        {
            purpose = ClientIdPurpose.RegisterClientId;
        }
        else if (tmp == 0x01)
        {
            purpose = ClientIdPurpose.MatchGames;
        }
    }

    public override string DeserializeLog()
    {
        string log = "";
        log += "[clientId]" + clientId;
        log += "[purpose]" + purpose;
        return log;
    }
}

public enum ClientIdPurpose
{
    RegisterClientId = 0,
    MatchGames = 1
}