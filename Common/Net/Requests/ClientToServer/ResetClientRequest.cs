using System.Collections;
using System.Collections.Generic;

public class ResetClientRequest : ClientRequestBase
{
    public ResetClientRequest() : base()
    {
    }

    public ResetClientRequest(int clientId) : base(clientId)
    {
    }

    public override int GetProtocol()
    {
        return NetProtocols.RESET_GAME;
    }

    public override string GetProtocolName()
    {
        return "RESET_GAME";
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