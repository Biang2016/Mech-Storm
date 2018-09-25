using MyCardGameCommon;

public class UseSpellCardToEquipServerRequset : ServerRequestBase
{
    public int clientId;
    public int handCardInstanceId;
    public Vector3 lastDragPosition;
    public int targetEquipId;

    public UseSpellCardToEquipServerRequset()
    {
    }

    public UseSpellCardToEquipServerRequset(int clientId, int handCardInstanceId, Vector3 lastDragPosition, int targetEquipId) 
    {
        this.clientId = clientId;
        this.handCardInstanceId = handCardInstanceId;
        this.lastDragPosition = lastDragPosition;
        this.targetEquipId = targetEquipId;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.SE_USE_SPELLCARD_TO_EQUIP_SERVER_REQUEST;
    }

    public override string GetProtocolName()
    {
        return "SE_USE_SPELLCARD_TO_EQUIP_SERVER_REQUEST";
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