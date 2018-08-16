using MyCardGameCommon;

public class EquipShieldRequest : ClientRequestBase
{
    public int handCardInstanceId;
    public int retinueID;
    public int shieldPlaceIndex;
    public Vector3 lastDragPosition;

    public EquipShieldRequest()
    {
    }

    public EquipShieldRequest(int clientId, int handCardInstanceId, int retinueID, int shieldPlaceIndex, Vector3 lastDragPosition) : base(clientId)
    {
        this.handCardInstanceId = handCardInstanceId;
        this.retinueID = retinueID;
        this.shieldPlaceIndex = shieldPlaceIndex;
        this.lastDragPosition = lastDragPosition;
    }

    public override int GetProtocol()
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
        writer.WriteSInt32(shieldPlaceIndex);
        lastDragPosition.Serialize(writer);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        handCardInstanceId = reader.ReadSInt32();
        retinueID = reader.ReadSInt32();
        shieldPlaceIndex = reader.ReadSInt32();
        lastDragPosition = Vector3.Deserialize(reader);
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [handCardInstanceId]=" + handCardInstanceId;
        log += " [retinueID]=" + retinueID;
        log += " [shieldPlaceIndex]=" + shieldPlaceIndex;
        log += " [lastDragPosition]=" + lastDragPosition;
        return log;
    }
}