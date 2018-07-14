using System.Collections;
using System.Collections.Generic;

public class CardDeckRequest : ClientRequestBaseBase
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
        return NetProtocols.CARD_DECK_INFO;
    }

    public override string GetProtocolName()
    {
        return "CARD_DECK_INFO";
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(cardDeckInfo.CardNumber);
        foreach (int cardID in cardDeckInfo.CardIDs)
        {
            writer.WriteSInt32(cardID);
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

        cardDeckInfo = new CardDeckInfo(cardIDs);
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [CardIDs] ";
        foreach (int cardID in cardDeckInfo.CardIDs)
        {
            log += cardID + " ";
        }

        return log;
    }
}