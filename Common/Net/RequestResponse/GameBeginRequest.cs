using System.Collections;
using System.Collections.Generic;

public class GameBeginRequest : Request
{
    public GameBeginRequest()
    {
    }

    public override int GetProtocol()
    {
        return NetProtocols.GAME_BEGIN;
    }

    public override string GetProtocolName()
    {
        return "GAME_BEGIN";
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
        string log = "";
        return log;
    }
}
