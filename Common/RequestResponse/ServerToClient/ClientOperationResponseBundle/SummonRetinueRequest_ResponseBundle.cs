public class SummonRetinueRequest_ResponseBundle : ResponseBundleBase
{
    public SummonRetinueRequest_ResponseBundle()
    {

    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.SUMMON_RETINUE_REQUEST_RESPONSE;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
    }


}