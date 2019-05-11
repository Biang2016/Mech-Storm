public class MechOnAttackShipRequest : ServerRequestBase
{
    public int clientId;
    public int mechId;
    public int targetClientId;
    public WeaponTypes weaponType;

    public MechOnAttackShipRequest()
    {
    }

    public MechOnAttackShipRequest(int clientId, int mechId, int targetClientId, WeaponTypes weaponType)
    {
        this.clientId = clientId;
        this.mechId = mechId;
        this.targetClientId = targetClientId;
        this.weaponType = weaponType;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.SE_MECH_ONATTACKSHIP;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(clientId);
        writer.WriteSInt32(mechId);
        writer.WriteSInt32(targetClientId);
        writer.WriteSInt32((int) weaponType);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        clientId = reader.ReadSInt32();
        mechId = reader.ReadSInt32();
        targetClientId = reader.ReadSInt32();
        weaponType = (WeaponTypes) reader.ReadSInt32();
    }

}