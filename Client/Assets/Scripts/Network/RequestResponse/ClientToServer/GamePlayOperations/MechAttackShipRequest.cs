public class MechAttackShipRequest : ClientRequestBase
{
    public int AttackMechId;

    public MechAttackShipRequest()
    {
    }

    public MechAttackShipRequest(int clientId, int mechId) : base(clientId)
    {
        AttackMechId = mechId;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.MECH_ATTACK_SHIP_REQUEST;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(AttackMechId);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        AttackMechId = reader.ReadSInt32();
    }
}