public class RetinueOnAttackShipRequest : ServerRequestBase
{
    public int clientId;
    public int retinueId;
    public int targetClientId;
    public WeaponTypes weaponType;

    public RetinueOnAttackShipRequest()
    {
    }

    public RetinueOnAttackShipRequest(int clientId, int retinueId, int targetClientId, WeaponTypes weaponType)
    {
        this.clientId = clientId;
        this.retinueId = retinueId;
        this.targetClientId = targetClientId;
        this.weaponType = weaponType;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.SE_RETINUE_ONATTACKSHIP;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(clientId);
        writer.WriteSInt32(retinueId);
        writer.WriteSInt32(targetClientId);
        writer.WriteSInt32((int) weaponType);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        clientId = reader.ReadSInt32();
        retinueId = reader.ReadSInt32();
        targetClientId = reader.ReadSInt32();
        weaponType = (WeaponTypes) reader.ReadSInt32();
    }

}