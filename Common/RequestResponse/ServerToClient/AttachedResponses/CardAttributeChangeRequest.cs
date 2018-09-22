public class CardAttributeChangeRequest : ServerRequestBase
{
    public int clientId;
    public int cardInstanceId;
    public int metalChange;
    public int energyChange;
    public int effectFactor;

    public CardAttributeChangeRequest()
    {
    }

    public CardAttributeChangeRequest(int clientId, int cardInstanceId, int metalChange, int energyChange, int effectFactor)
    {
        this.clientId = clientId;
        this.cardInstanceId = cardInstanceId;
        this.metalChange = metalChange;
        this.energyChange = energyChange;
        this.effectFactor = effectFactor;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.SE_CARD_ATTR_CHANGE;
    }

    public override string GetProtocolName()
    {
        return "SE_CARD_ATTR_CHANGE";
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(clientId);
        writer.WriteSInt32(cardInstanceId);
        writer.WriteSInt32(metalChange);
        writer.WriteSInt32(energyChange);
        writer.WriteSInt32(effectFactor);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        clientId = reader.ReadSInt32();
        cardInstanceId = reader.ReadSInt32();
        metalChange = reader.ReadSInt32();
        energyChange = reader.ReadSInt32();
        effectFactor = reader.ReadSInt32();
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [clientId]=" + clientId;
        log += " [cardInstanceId]=" + cardInstanceId;
        log += " [metalChange]=" + metalChange;
        log += " [energyChange]=" + energyChange;
        log += " [effectFactor]=" + effectFactor;
        return log;
    }
}