using System.Collections;
using System.Collections.Generic;

public class UseSpellCardRequset_Response : ClientOperationResponseBase
{
    public UseSpellCardRequset_Response()
    {
    }

    public override int GetProtocol()
    {
        return NetProtocols.USE_SPELLCARD_REQUEST_RESPONSE;
    }

    public override string GetProtocolName()
    {
        return "USE_SPELLCARD_REQUEST_RESPONSE";
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