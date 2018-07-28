using System.Collections;
using System.Collections.Generic;

public class UseCardRequest : ServerRequestBase
{
    public int clientId;
    public int handCardInstanceId;

    public UseCardRequest()
    {
    }

    public UseCardRequest(int clientId, int handCardInstanceId)
    {
        this.clientId = clientId;
        this.handCardInstanceId = handCardInstanceId;
    }

    public override int GetProtocol()
    {
        return NetProtocols.SE_USE_CARD;
    }

    public override string GetProtocolName()
    {
        return "SE_USE_CARD";
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(clientId);
        writer.WriteSInt32(handCardInstanceId);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        clientId = reader.ReadSInt32();
        handCardInstanceId = reader.ReadSInt32();
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [clientId]=" + clientId;
        log += " [handCardInstanceId]=" + handCardInstanceId;
        return log;
    }
}