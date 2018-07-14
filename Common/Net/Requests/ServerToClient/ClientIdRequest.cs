using System.Collections;
using System.Collections.Generic;

public class ClientIdRequest : ServerRequestBaseBase
{
    public int givenClientId;

    public ClientIdRequest()
    {
    }

    public ClientIdRequest(int clientId)
    {
        givenClientId = clientId;
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
        writer.WriteSInt32(givenClientId);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        givenClientId = reader.ReadSInt32();
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [givenClientId] " + givenClientId;
        return log;
    }
}