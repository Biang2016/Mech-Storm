using System.Collections;
using System.Collections.Generic;

public class SummonRetinueRequest_Response : ClientOperationResponseBase
{
    public SummonRetinueRequest_Response()
    {

    }

    public override int GetProtocol()
    {
        return NetProtocols.SUMMON_RETINUE_REQUEST_RESPONSE;
    }

	public override string GetProtocolName()
	{
        return "SUMMON_RETINUE_REQUEST_RESPONSE";
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