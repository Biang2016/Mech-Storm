public class UseSpellCardServerRequest : ServerRequestBase
{
    public int clientId;
    public int handCardInstanceId;
    public CardInfo_Base cardInfo;

    public UseSpellCardServerRequest()
    {
    }

    public UseSpellCardServerRequest(int clientId, int handCardInstanceId, CardInfo_Base cardInfo)
    {
        this.clientId = clientId;
        this.handCardInstanceId = handCardInstanceId;
        this.cardInfo = cardInfo;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.SE_USE_SPELLCARD_SERVER_REQUEST;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(handCardInstanceId);
        cardInfo.Serialize(writer);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        handCardInstanceId = reader.ReadSInt32();
        cardInfo = CardInfo_Base.Deserialze(reader);
    }
}