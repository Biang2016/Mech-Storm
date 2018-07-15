using System.Collections;
using System.Collections.Generic;

public class CancelMatchRequest : ClientRequestBase
{
    public CancelMatchRequest()
    {
    }

    public CancelMatchRequest(int clientId) : base(clientId)
    {
    }

    public override int GetProtocol()
    {
        return NetProtocols.CANCEL_MATCH;
    }

    public override string GetProtocolName()
    {
        return "CANCEL_MATCH";
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