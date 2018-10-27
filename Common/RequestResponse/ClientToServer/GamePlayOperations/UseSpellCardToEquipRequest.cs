
public class UseSpellCardToEquipRequest : ClientRequestBase
{
    public int handCardInstanceId;
    public int targetEquipId;

    public UseSpellCardToEquipRequest()
    {
    }

    public UseSpellCardToEquipRequest(int clientId, int handCardInstanceId,  int targetEquipId) : base(clientId)
    {
        this.handCardInstanceId = handCardInstanceId;
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
        writer.WriteSInt32(targetEquipId);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        handCardInstanceId = reader.ReadSInt32();
        targetEquipId = reader.ReadSInt32();
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [handCardInstanceId]=" + handCardInstanceId;
        log += " [targetEquipId]=" + targetEquipId;
        return log;
    }
}