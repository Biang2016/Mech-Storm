using System.Collections;
using System.Collections.Generic;

public class ClientEndRoundResponse : Response
{
    public int clientId;

    public override int GetProtocol()
    {
        return NetProtocols.CLIENT_END_ROUND;
    }

    public override string GetProtocolName()
    {
        return GetType().FullName;
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