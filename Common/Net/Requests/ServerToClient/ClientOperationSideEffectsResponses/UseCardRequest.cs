using System.Collections;
using System.Collections.Generic;

public class UseCardRequest : ServerRequestBase
{
    public int clientId;
    public int handCardIndex;

    public UseCardRequest()
    {
    }

    public UseCardRequest(int clientId, int handCardIndex)
    {
        this.clientId = clientId;
        this.handCardIndex = handCardIndex;
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
        writer.WriteSInt32(handCardIndex);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        clientId = reader.ReadSInt32();
        handCardIndex = reader.ReadSInt32();
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [clientId]=" + clientId;
        log += " [handCardIndex]=" + handCardIndex;
        return log;
    }
}