using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class GameStart_Response : ClientOperationResponseBase
{
    public GameStart_Response()
    {
    }
    public override int GetProtocol()
    {
        return NetProtocols.GAME_START_RESPONSE;
    }
    public override string GetProtocolName()
    {
        return "GAME_START_RESPONSE";
    }
    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        return log;
    }
}