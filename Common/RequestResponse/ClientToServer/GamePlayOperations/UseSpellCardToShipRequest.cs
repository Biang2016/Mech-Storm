using MyCardGameCommon;

public class UseSpellCardToShipRequest : ClientRequestBase
{
    public int handCardInstanceId;
    public Vector3 lastDragPosition;
    public int targetClientId;

    public UseSpellCardToShipRequest()
    {
    }

    public UseSpellCardToShipRequest(int clientId, int handCardInstanceId, Vector3 lastDragPosition, int targetClientId) : base(clientId)
    {
        this.handCardInstanceId = handCardInstanceId;
        this.lastDragPosition = lastDragPosition;
        this.targetClientId = targetClientId;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.USE_SPELLCARD_TO_SHIP_REQUEST;
    }

    public override string GetProtocolName()
    {
        return "USE_SPELLCARD_TO_SHIP_REQUEST";
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(handCardInstanceId);
        lastDragPosition.Serialize(writer);
        writer.WriteSInt32(targetClientId);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        handCardInstanceId = reader.ReadSInt32();
        lastDragPosition = Vector3.Deserialize(reader);
        targetClientId = reader.ReadSInt32();
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [handCardInstanceId]=" + handCardInstanceId;
        log += " [lastDragPosition]=" + lastDragPosition;
        log += " [targetClientId]=" + targetClientId;
        return log;
    }
}