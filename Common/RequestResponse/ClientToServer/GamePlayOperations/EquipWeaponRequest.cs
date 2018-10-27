
public class EquipWeaponRequest : ClientRequestBase
{
    public int handCardInstanceId;
    public int retinueId;
    public int weaponPlaceIndex;

    public EquipWeaponRequest()
    {
    }

    public EquipWeaponRequest(int clientId, int handCardInstanceId, int retinueId, int weaponPlaceIndex) : base(clientId)
    {
        this.handCardInstanceId = handCardInstanceId;
        this.retinueId = retinueId;
        this.weaponPlaceIndex = weaponPlaceIndex;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.EQUIP_WEAPON_REQUEST;
    }

    public override string GetProtocolName()
    {
        return "EQUIP_WEAPON_REQUEST";
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(handCardInstanceId);
        writer.WriteSInt32(retinueId);
        writer.WriteSInt32(weaponPlaceIndex);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        handCardInstanceId = reader.ReadSInt32();
        retinueId = reader.ReadSInt32();
        weaponPlaceIndex = reader.ReadSInt32();
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [handCardInstanceId]=" + handCardInstanceId;
        log += " [retinueId]=" + retinueId;
        log += " [weaponPlaceIndex]=" + weaponPlaceIndex;
        return log;
    }
}