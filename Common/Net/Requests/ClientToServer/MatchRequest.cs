using System.Collections;
using System.Collections.Generic;

public class MatchRequest : ClientRequestBase
{
    public MatchRequest() : base()
    {
    }

    public MatchRequest(int clientId) : base(clientId)
    {
    }

    public override int GetProtocol()
    {
        return NetProtocols.Match;
    }

    public override string GetProtocolName()
    {
        return "Match";
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