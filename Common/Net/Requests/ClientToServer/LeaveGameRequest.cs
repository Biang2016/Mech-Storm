using System.Collections;
using System.Collections.Generic;

public class LeaveGameRequest : ClientRequestBaseBase
{
    public LeaveGameRequest()
    {
    }

    public LeaveGameRequest(int clientId) : base(clientId)
    {
    }

    public override int GetProtocol()
    {
        return NetProtocols.LEAVE_GAME;
    }

    public override string GetProtocolName()
    {
        return "LEAVE_GAME";
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