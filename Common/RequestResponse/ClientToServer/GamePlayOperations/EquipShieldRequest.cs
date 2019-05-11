
public class EquipShieldRequest : ClientRequestBase
{
    public int handCardInstanceId;
    public int mechID;

    public EquipShieldRequest()
    {
    }

    public EquipShieldRequest(int clientId, int handCardInstanceId, int mechID) : base(clientId)
    {
        this.handCardInstanceId = handCardInstanceId;
        this.mechID = mechID;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.EQUIP_SHIELD_REQUEST;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(handCardInstanceId);
        writer.WriteSInt32(mechID);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        handCardInstanceId = reader.ReadSInt32();
        mechID = reader.ReadSInt32();
    }
}