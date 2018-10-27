
public class EquipMARequest : ClientRequestBase
{
    public int handCardInstanceId;
    public int retinueID;
    public int maPlaceIndex;

    public EquipMARequest()
    {
    }

    public EquipMARequest(int clientId, int handCardInstanceId, int retinueID, int maPlaceIndex) : base(clientId)
    {
        this.handCardInstanceId = handCardInstanceId;
        this.retinueID = retinueID;
        this.maPlaceIndex = maPlaceIndex;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.EQUIP_MA_REQUEST;
    }

    public override string GetProtocolName()
    {
        return "EQUIP_MA_REQUEST";
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(handCardInstanceId);
        writer.WriteSInt32(retinueID);
        writer.WriteSInt32(maPlaceIndex);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        handCardInstanceId = reader.ReadSInt32();
        retinueID = reader.ReadSInt32();
        maPlaceIndex = reader.ReadSInt32();
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [handCardInstanceId]=" + handCardInstanceId;
        log += " [retinueID]=" + retinueID;
        log += " [maPlaceIndex]=" + maPlaceIndex;
        return log;
    }
}