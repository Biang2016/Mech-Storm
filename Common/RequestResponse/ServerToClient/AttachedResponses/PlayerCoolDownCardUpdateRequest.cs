public class PlayerCoolDownCardUpdateRequest : ServerRequestBase
{
    public int clientId;
    public CardDeck.CoolingDownCard coolingDownCard;

    public PlayerCoolDownCardUpdateRequest()
    {
    }

    public PlayerCoolDownCardUpdateRequest(int clientId, CardDeck.CoolingDownCard coolingDownCard)
    {
        this.clientId = clientId;
        this.coolingDownCard = coolingDownCard;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.SE_PLAYER_COOLDOWNCARD_UPDATE_REQUEST;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(clientId);
        coolingDownCard.Serialize(writer);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        clientId = reader.ReadSInt32();
        coolingDownCard = CardDeck.CoolingDownCard.Deserialize(reader);
    }

}