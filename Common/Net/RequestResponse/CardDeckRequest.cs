using System.Collections;
using System.Collections.Generic;

public class CardDeckRequest : Request
{
    public int clientId;
    public CardDeckInfo cardDeckInfo;

    public override int GetProtocol()
    {
        return NetProtocols.CARD_DECK_INFO;
    }

    public CardDeckRequest(int clientId, CardDeckInfo cardDeckInfo)
    {
        this.clientId = clientId;
        this.cardDeckInfo = cardDeckInfo;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(clientId);
        writer.WriteSInt32(cardDeckInfo.CardNumber);
        foreach (int cardID in cardDeckInfo.CardIDs)
        {
            writer.WriteSInt32(cardID);
        }
    }
}