using System.Collections;
using System.Collections.Generic;

public class SetPlayerRequest : ServerRequestBase
{
    public int clientId;
    public int costLeft;
    public int costMax;

    public SetPlayerRequest()
    {

    }

    public SetPlayerRequest(int clientId, int costLeft, int costMax)
    {
        this.clientId = clientId;
        this.costLeft = costLeft;
        this.costMax = costMax;
    }

    public override int GetProtocol()
    {
        return NetProtocols.SE_SET_PLAYER;
    }

    public override string GetProtocolName()
    {
        return "SE_SET_PLAYER";
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(clientId);
        writer.WriteSInt32(costLeft);
        writer.WriteSInt32(costMax);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        clientId = reader.ReadSInt32();
        costLeft = reader.ReadSInt32();
        costMax = reader.ReadSInt32();
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [clientId]=" + clientId;
        log += " [costLeft]=" + costLeft;
        log += " [costMax]=" + costMax;
        return log;
    }
}