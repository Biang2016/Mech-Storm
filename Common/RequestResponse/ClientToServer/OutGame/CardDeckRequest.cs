public class CardDeckRequest : ClientRequestBase
{
    public CardDeckInfo cardDeckInfo;

    public CardDeckRequest() : base()
    {
    }

    public CardDeckRequest(int clientId, CardDeckInfo cardDeckInfo) : base(clientId)
    {
        this.cardDeckInfo = cardDeckInfo;
    }

    public override int GetProtocol()
    {
        return NetProtocols.CARD_DECK_REQUEST;
    }

    public override string GetProtocolName()
    {
        return "CARD_DECK_REQUEST";
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(cardDeckInfo.CardNumber);
        foreach (int cardID in cardDeckInfo.CardIDs)
        {
            writer.WriteSInt32(cardID);
        }

        writer.WriteSInt32(cardDeckInfo.BeginRetinueIDs.Length);
        foreach (int retinueID in cardDeckInfo.BeginRetinueIDs)
        {
            writer.WriteSInt32(retinueID);
        }
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        int cardDeckSize = reader.ReadSInt32();
        int[] cardIDs = new int[cardDeckSize];
        for (int i = 0; i < cardDeckSize; i++)
        {
            cardIDs[i] = reader.ReadSInt32();
        }

        int beginRetinueIDSize = reader.ReadSInt32();
        int[] beginRetinueIDs = new int[beginRetinueIDSize];
        for (int i = 0; i < beginRetinueIDSize; i++)
        {
            beginRetinueIDs[i] = reader.ReadSInt32();
        }

        cardDeckInfo = new CardDeckInfo(cardIDs, beginRetinueIDs);
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [CardIDs]=";
        foreach (int cardID in cardDeckInfo.CardIDs)
        {
            log += cardID + " ";
        }
        log += " [BeginRetinueIDs]=";
        foreach (int cardID in cardDeckInfo.BeginRetinueIDs)
        {
            log += cardID + " ";
        }

        return log;
    }
}