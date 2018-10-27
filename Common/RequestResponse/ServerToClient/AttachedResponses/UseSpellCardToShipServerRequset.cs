
public class UseSpellCardToShipServerRequset : ServerRequestBase
{
    public int clientId;
    public int handCardInstanceId;
    public int targetClientId;

    public UseSpellCardToShipServerRequset()
    {
    }

    public UseSpellCardToShipServerRequset(int clientId, int handCardInstanceId, int targetClientId) 
    {
        this.clientId = clientId;
        this.handCardInstanceId = handCardInstanceId;
        this.targetClientId = targetClientId;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.SE_USE_SPELLCARD_TO_SHIP_SERVER_REQUEST;
    }

    public override string GetProtocolName()
    {
        return "SE_USE_SPELLCARD_TO_SHIP_SERVER_REQUEST";
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(handCardInstanceId);
        writer.WriteSInt32(targetClientId);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        handCardInstanceId = reader.ReadSInt32();
        targetClientId = reader.ReadSInt32();
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [handCardInstanceId]=" + handCardInstanceId;
        log += " [targetClientId]=" + targetClientId;
        return log;
    }
}