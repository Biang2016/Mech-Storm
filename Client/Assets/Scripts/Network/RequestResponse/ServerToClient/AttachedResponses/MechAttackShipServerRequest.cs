public class MechAttackShipServerRequest : ServerRequestBase
{
    public int AttackMechClientId;
    public int AttackMechId;

    public MechAttackShipServerRequest()
    {
    }

    public MechAttackShipServerRequest(int attackMechClientId, int attackMechId)
    {
        AttackMechClientId = attackMechClientId;
        AttackMechId = attackMechId;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.SE_MECH_ATTACK_SHIP_SERVER_REQUEST;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(AttackMechClientId);
        writer.WriteSInt32(AttackMechId);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        AttackMechClientId = reader.ReadSInt32();
        AttackMechId = reader.ReadSInt32();
    }

}