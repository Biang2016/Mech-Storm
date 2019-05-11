public class MechAttackMechRequest_ResponseBundle : ResponseBundleBase
{
    public MechAttackMechRequest_ResponseBundle()
    {
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.MECH_ATTACK_MECH_REQUEST_RESPONSE;
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