
public class EquipWeaponRequest : ClientRequestBase
{
    public int handCardInstanceId;
    public int mechId;

    public EquipWeaponRequest()
    {
    }

    public EquipWeaponRequest(int clientId, int handCardInstanceId, int mechId) : base(clientId)
    {
        this.handCardInstanceId = handCardInstanceId;
        this.mechId = mechId;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.EQUIP_WEAPON_REQUEST;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(handCardInstanceId);
        writer.WriteSInt32(mechId);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        handCardInstanceId = reader.ReadSInt32();
        mechId = reader.ReadSInt32();
    }
}