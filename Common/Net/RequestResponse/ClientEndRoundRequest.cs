using System.Collections;
using System.Collections.Generic;

public class ClientEndRoundRequest : Request
{
    public int clientId;

    public ClientEndRoundRequest()
    {

    }

    public ClientEndRoundRequest(int clientId)
    {
        this.clientId = clientId;
    }

    public override int GetProtocol()
    {
        return NetProtocols.CLIENT_END_ROUND;
    }

    public override string GetProtocolName()
    {
        return "CLIENT_END_ROUND";
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
        log += "[clientId]" + clientId;
        return log;
    }
}