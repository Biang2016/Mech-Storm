using System.Collections;
using System.Collections.Generic;

public class ClientIdResponse : Response
{
    public int clientId;
    public ClientIdPurpose purpose;

    public override int GetProtocol()
    {
        return NetProtocols.SEND_CLIENT_ID;
    }

    public override string GetProtocolName()
    {
        return GetType().FullName;
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