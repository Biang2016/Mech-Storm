public class MechCanAttackRequest : ServerRequestBase
{
    public int clientId;
    public int mechId;
    public bool canAttack;

    public MechCanAttackRequest()
    {
    }

    public MechCanAttackRequest(int clientId, int mechId, bool canAttack)
    {
        this.clientId = clientId;
        this.mechId = mechId;
        this.canAttack = canAttack;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.SE_MECH_CANATTACK;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(clientId);
        writer.WriteSInt32(mechId);
        writer.WriteByte((byte) (canAttack ? 0x01 : 0x00));
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        clientId = reader.ReadSInt32();
        mechId = reader.ReadSInt32();
        canAttack = reader.ReadByte() == 0x01;
    }
}