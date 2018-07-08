using System.Collections;
using System.Collections.Generic;

public class GameBeginResponse : Response
{

    public override int GetProtocol()
    {
        return NetProtocols.GAME_BEGIN;
    }

    public override string GetProtocolName()
    {
        return GetType().FullName;
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