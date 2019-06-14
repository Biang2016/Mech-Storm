public class UseSpellCardToEquipServerRequset : ServerRequestBase
{
    public int clientId;
    public int handCardInstanceId;
    public int targetEquipId;

    public UseSpellCardToEquipServerRequset()
    {
    }

    public UseSpellCardToEquipServerRequset(int clientId, int handCardInstanceId, int targetEquipId)
    {
        this.clientId = clientId;
        this.handCardInstanceId = handCardInstanceId;
        this.targetEquipId = targetEquipId;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.SE_USE_SPELLCARD_TO_EQUIP_SERVER_REQUEST;
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
}