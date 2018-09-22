public class EquipPackRequest_ResponseBundle : ResponseBundleBase
{
    public EquipPackRequest_ResponseBundle()
    {
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.EQUIP_PACK_REQUEST_RESPONSE;
    }

    public override string GetProtocolName()
    {
        return "EQUIP_PACK_REQUEST_RESPONSE";
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