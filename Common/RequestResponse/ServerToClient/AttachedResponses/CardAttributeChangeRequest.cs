public class CardAttributeChangeRequest : ServerRequestBase
{
    public int clientId;
    public int cardInstanceId;
    public int costChange;
    public int magicChange;
    public int effectFactor;

    public CardAttributeChangeRequest()
    {
    }

    public CardAttributeChangeRequest(int clientId, int cardInstanceId, int costChange, int magicChange, int effectFactor)
    {
        this.clientId = clientId;
        this.cardInstanceId = cardInstanceId;
        this.costChange = costChange;
        this.magicChange = magicChange;
        this.effectFactor = effectFactor;
    }

    public override int GetProtocol()
    {
        return NetProtocols.SE_CARD_INFO_CHANGE;
    }

    public override string GetProtocolName()
    {
        return "SE_CARD_INFO_CHANGE";
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(clientId);
        writer.WriteSInt32(cardInstanceId);
        writer.WriteSInt32(costChange);
        writer.WriteSInt32(magicChange);
        writer.WriteSInt32(effectFactor);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        clientId = reader.ReadSInt32();
        cardInstanceId = reader.ReadSInt32();
        costChange = reader.ReadSInt32();
        magicChange = reader.ReadSInt32();
        effectFactor = reader.ReadSInt32();
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [clientId]=" + clientId;
        log += " [cardInstanceId]=" + cardInstanceId;
        log += " [costChange]=" + costChange;
        log += " [magicChange]=" + magicChange;
        log += " [effectFactor]=" + effectFactor;
        return log;
    }
}