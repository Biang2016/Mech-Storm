using MyCardGameCommon;

public class EquipMARequest : ClientRequestBase
{
    public int handCardInstanceId;
    public int retinueID;
    public int maPlaceIndex;
    public Vector3 lastDragPosition;

    public EquipMARequest()
    {
    }

    public EquipMARequest(int clientId, int handCardInstanceId, int retinueID, int maPlaceIndex, Vector3 lastDragPosition) : base(clientId)
    {
        this.handCardInstanceId = handCardInstanceId;
        this.retinueID = retinueID;
        this.maPlaceIndex = maPlaceIndex;
        this.lastDragPosition = lastDragPosition;
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
        lastDragPosition.Serialize(writer);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        handCardInstanceId = reader.ReadSInt32();
        retinueID = reader.ReadSInt32();
        maPlaceIndex = reader.ReadSInt32();
        lastDragPosition = Vector3.Deserialize(reader);
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [handCardInstanceId]=" + handCardInstanceId;
        log += " [retinueID]=" + retinueID;
        log += " [maPlaceIndex]=" + maPlaceIndex;
        log += " [lastDragPosition]=" + lastDragPosition;
        return log;
    }
}