public class MechOnAttackRequest : ServerRequestBase
{
    public int clientId;
    public int mechId;
    public int targetMechId;
    public WeaponTypes weaponType;

    public MechOnAttackRequest()
    {
    }

    public MechOnAttackRequest(int clientId, int mechId, int targetMechId, WeaponTypes weaponType)
    {
        this.clientId = clientId;
        this.mechId = mechId;
        this.targetMechId = targetMechId;
        this.weaponType = weaponType;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.SE_MECH_ONATTACK;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(clientId);
        writer.WriteSInt32(mechId);
        writer.WriteSInt32(targetMechId);
        writer.WriteSInt32((int) weaponType);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        clientId = reader.ReadSInt32();
        mechId = reader.ReadSInt32();
        targetMechId = reader.ReadSInt32();
        weaponType = (WeaponTypes) reader.ReadSInt32();
    }
}