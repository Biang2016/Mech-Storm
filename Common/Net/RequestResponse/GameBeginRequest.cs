using System.Collections;
using System.Collections.Generic;

public class GameBeginRequest : Request
{
    public override int GetProtocol()
    {
        return NetProtocols.GAME_BEGIN;
    }

    public GameBeginRequest()
    {
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
       
    }
}
