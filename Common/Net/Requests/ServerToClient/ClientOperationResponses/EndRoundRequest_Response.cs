using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class EndRoundRequest_Response : ClientOperationResponseBase
{
    public EndRoundRequest_Response()
    {
    }
    public override int GetProtocol()
    {
        return NetProtocols.END_ROUND_RESPONSE;
    }
    public override string GetProtocolName()
    {
        return "END_ROUND_RESPONSE";
    }
    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        return log;
    }
}