using System.Collections;
using System.Collections.Generic;

public class EquipShieldRequest_ResponseBundle : ResponseBundleBase
{
    public EquipShieldRequest_ResponseBundle()
    {
    }


    public override int GetProtocol()
    {
        return NetProtocols.EQUIP_SHIELD_REQUEST_RESPONSE;
    }

    public override string GetProtocolName()
    {
        return "EQUIP_SHIELD_REQUEST_RESPONSE";
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