using System.Collections;
using System.Collections.Generic;

public class RetinueDieRequest : ServerRequestBase
{
    public int clientId;
    public int retinuePlaceIndex;

    public RetinueDieRequest()
    {
    }

    public RetinueDieRequest(int clinetId, int retinuePlaceIndex)
    {
        this.clientId = clinetId;
        this.retinuePlaceIndex = retinuePlaceIndex;
    }

    public override int GetProtocol()
    {
        return NetProtocols.SE_RETINUE_DIE;
    }

    public override string GetProtocolName()
    {
        return "SE_RETINUE_DIE";
    }


    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(clientId);
        writer.WriteSInt32(retinuePlaceIndex);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        clientId = reader.ReadSInt32();
        retinuePlaceIndex = reader.ReadSInt32();
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [clinetId]=" + clientId;
        log += " [retinuePlaceIndex]=" + retinuePlaceIndex;
        return log;
    }
}