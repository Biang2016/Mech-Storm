
public class HandCardInfoSyncRequest : ServerRequestBase
{
    public int clientId;
    public int instanceId;
    public CardInfo_Base cardInfo;

    public HandCardInfoSyncRequest()
    {
    }

    public HandCardInfoSyncRequest(int clientId, int instanceId, CardInfo_Base cardInfo)
    {
        this.clientId = clientId;
        this.instanceId = instanceId;
        this.cardInfo = cardInfo;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.SE_HAND_CARDINFO_SYNC;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(clientId);
        writer.WriteSInt32(instanceId);
        cardInfo.Serialize(writer);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        clientId = reader.ReadSInt32();
        instanceId = reader.ReadSInt32();
        cardInfo = CardInfo_Base.Deserialze(reader);
    }
}