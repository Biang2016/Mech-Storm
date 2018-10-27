
public class EquipPackRequest : ClientRequestBase
{
    public int handCardInstanceId;
    public int retinueID;
    public int packPlaceIndex;

    public EquipPackRequest()
    {
    }

    public EquipPackRequest(int clientId, int handCardInstanceId, int retinueID, int packPlaceIndex) : base(clientId)
    {
        this.handCardInstanceId = handCardInstanceId;
        this.retinueID = retinueID;
        this.packPlaceIndex = packPlaceIndex;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.EQUIP_PACK_REQUEST;
    }

    public override string GetProtocolName()
    {
        return "EQUIP_PACK_REQUEST";
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(handCardInstanceId);
        writer.WriteSInt32(retinueID);
        writer.WriteSInt32(packPlaceIndex);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        handCardInstanceId = reader.ReadSInt32();
        retinueID = reader.ReadSInt32();
        packPlaceIndex = reader.ReadSInt32();
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [handCardInstanceId]=" + handCardInstanceId;
        log += " [retinueID]=" + retinueID;
        log += " [packPlaceIndex]=" + packPlaceIndex;
        return log;
    }
}