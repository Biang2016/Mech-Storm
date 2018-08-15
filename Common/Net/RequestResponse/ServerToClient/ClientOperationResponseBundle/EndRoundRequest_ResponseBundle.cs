using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class EndRoundRequest_ResponseBundle : ResponseBundleBase
{
    public EndRoundRequest_ResponseBundle()
    {
    }
    public override int GetProtocol()
    {
        return NetProtocols.END_ROUND_REQUEST_RESPONSE;
    }
    public override string GetProtocolName()
    {
        return "END_ROUND_REQUEST_RESPONSE";
    }
    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        return log;
    }
}