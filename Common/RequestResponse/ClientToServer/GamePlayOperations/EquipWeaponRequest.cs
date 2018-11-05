
public class EquipWeaponRequest : ClientRequestBase
{
    public int handCardInstanceId;
    public int retinueId;

    public EquipWeaponRequest()
    {
    }

    public EquipWeaponRequest(int clientId, int handCardInstanceId, int retinueId) : base(clientId)
    {
        this.handCardInstanceId = handCardInstanceId;
        this.retinueId = retinueId;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.EQUIP_WEAPON_REQUEST;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(handCardInstanceId);
        writer.WriteSInt32(retinueId);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        handCardInstanceId = reader.ReadSInt32();
        retinueId = reader.ReadSInt32();
    }
}