using MyCardGameCommon;

public class UseSpellCardToShipRequest : ClientRequestBase
{
    public int handCardInstanceId;
    public Vector3 lastDragPosition;

    public UseSpellCardToShipRequest()
    {
    }

    public UseSpellCardToShipRequest(int clientId,int handCardInstanceId, Vector3 lastDragPosition):base(clientId)
    {
        this.handCardInstanceId = handCardInstanceId;
        this.lastDragPosition = lastDragPosition;
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
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        handCardInstanceId = reader.ReadSInt32();
        lastDragPosition = Vector3.Deserialize(reader);
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [handCardInstanceId]=" + handCardInstanceId;
        log += " [lastDragPosition]=" + lastDragPosition;
        return log;
    }
}