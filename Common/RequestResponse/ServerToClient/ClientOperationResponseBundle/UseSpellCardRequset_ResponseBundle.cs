public class UseSpellCardRequset_ResponseBundle : ResponseBundleBase
{
    public UseSpellCardRequset_ResponseBundle()
    {
    }

    public override NetProtocols GetProtocol()
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