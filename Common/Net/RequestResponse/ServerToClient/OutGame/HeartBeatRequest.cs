using System.Collections;
using System.Collections.Generic;

public class HeartBeatRequest : ServerRequestBase
{
    public HeartBeatRequest()
    {
    }

    public override int GetProtocol()
    {
        return NetProtocols.HEART_BEAT_REQUEST;
    }

    public override string GetProtocolName()
    {
        return "HEART_BEAT_REQUEST";
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        return log;
    }
}