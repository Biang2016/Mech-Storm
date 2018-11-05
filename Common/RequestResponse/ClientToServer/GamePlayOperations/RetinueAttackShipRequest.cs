public class RetinueAttackShipRequest : ClientRequestBase
{
    public int AttackRetinueId;

    public RetinueAttackShipRequest()
    {
    }

    public RetinueAttackShipRequest(int clientId, int retinueId) : base(clientId)
    {
        AttackRetinueId = retinueId;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.RETINUE_ATTACK_SHIP_REQUEST;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(AttackRetinueId);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        AttackRetinueId = reader.ReadSInt32();
    }
}