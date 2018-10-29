
public class EquipShieldRequest : ClientRequestBase
{
    public int handCardInstanceId;
    public int retinueID;

    public EquipShieldRequest()
    {
    }

    public EquipShieldRequest(int clientId, int handCardInstanceId, int retinueID) : base(clientId)
    {
        this.handCardInstanceId = handCardInstanceId;
        this.retinueID = retinueID;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.EQUIP_SHIELD_REQUEST;
    }

    public override string GetProtocolName()
    {
        return "EQUIP_SHIELD_REQUEST";
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(handCardInstanceId);
        writer.WriteSInt32(retinueID);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        handCardInstanceId = reader.ReadSInt32();
        retinueID = reader.ReadSInt32();
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [handCardInstanceId]=" + handCardInstanceId;
        log += " [retinueID]=" + retinueID;
        return log;
    }
}