using System.Collections;
using System.Collections.Generic;

public class CardDeckResponse : Response
{
    public int clientId;
    public CardDeckInfo cardDeckInfo;

    public override int GetProtocol()
    {
        return NetProtocols.CARD_DECK_INFO;
    }

    public override string GetProtocolName()
    {
        return GetType().FullName;
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        clientId = reader.ReadSInt32();
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
        string log = "";
        log += "[clientId]" + clientId;
        return log;
    }
}