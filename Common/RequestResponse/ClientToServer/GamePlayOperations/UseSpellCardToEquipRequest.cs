using MyCardGameCommon;

public class UseSpellCardToEquipRequest : ClientRequestBase
{
    public int handCardInstanceId;
    public Vector3 lastDragPosition;
    public int targetEquipId;

    public UseSpellCardToEquipRequest()
    {
    }

    public UseSpellCardToEquipRequest(int clientId, int handCardInstanceId, Vector3 lastDragPosition, int targetEquipId) : base(clientId)
    {
        this.handCardInstanceId = handCardInstanceId;
        this.lastDragPosition = lastDragPosition;
        this.targetEquipId = targetEquipId;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.USE_SPELLCARD_TO_EQUIP_REQUEST;
    }

    public override string GetProtocolName()
    {
        return "USE_SPELLCARD_TO_EQUIP_REQUEST";
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(handCardInstanceId);
        lastDragPosition.Serialize(writer);
        writer.WriteSInt32(targetEquipId);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        handCardInstanceId = reader.ReadSInt32();
        lastDragPosition = Vector3.Deserialize(reader);
        targetEquipId = reader.ReadSInt32();
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [handCardInstanceId]=" + handCardInstanceId;
        log += " [lastDragPosition]=" + lastDragPosition;
        log += " [targetEquipId]=" + targetEquipId;
        return log;
    }
}