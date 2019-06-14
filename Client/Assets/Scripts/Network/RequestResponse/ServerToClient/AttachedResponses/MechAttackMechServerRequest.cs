public class MechAttackMechServerRequest : ServerRequestBase
{
    public int AttackMechClientId;
    public int AttackMechId;
    public int BeAttackedMechClientId;
    public int BeAttackedMechId;

    public MechAttackMechServerRequest()
    {
    }

    public MechAttackMechServerRequest(int attackMechClientId, int attackMechId, int beAttackedMechClientId, int beAttackedMechId)
    {
        AttackMechClientId = attackMechClientId;
        AttackMechId = attackMechId;
        BeAttackedMechClientId = beAttackedMechClientId;
        BeAttackedMechId = beAttackedMechId;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.SE_MECH_ATTACK_MECH_SERVER_REQUEST;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(AttackMechClientId);
        writer.WriteSInt32(AttackMechId);
        writer.WriteSInt32(BeAttackedMechClientId);
        writer.WriteSInt32(BeAttackedMechId);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        AttackMechClientId = reader.ReadSInt32();
        AttackMechId = reader.ReadSInt32();
        BeAttackedMechClientId = reader.ReadSInt32();
        BeAttackedMechId = reader.ReadSInt32();
    }
}