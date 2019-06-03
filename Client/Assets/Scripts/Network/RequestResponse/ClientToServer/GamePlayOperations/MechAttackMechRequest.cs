public class MechAttackMechRequest : ClientRequestBase
{
    public int AttackMechId;
    public int BeAttackedMechClientId;
    public int BeAttackedMechId;

    public MechAttackMechRequest()
    {
    }

    public MechAttackMechRequest(int clientId, int mechId, int beAttackedMechClientId, int beAttackedMechId) : base(clientId)
    {
        AttackMechId = mechId;
        BeAttackedMechClientId = beAttackedMechClientId;
        BeAttackedMechId = beAttackedMechId;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.MECH_ATTACK_MECH_REQUEST;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(AttackMechId);
        writer.WriteSInt32(BeAttackedMechClientId);
        writer.WriteSInt32(BeAttackedMechId);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        AttackMechId = reader.ReadSInt32();
        BeAttackedMechClientId = reader.ReadSInt32();
        BeAttackedMechId = reader.ReadSInt32();
    }
}