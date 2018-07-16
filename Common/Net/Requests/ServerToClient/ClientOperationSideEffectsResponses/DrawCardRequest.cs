using System.Collections;
using System.Collections.Generic;

public class DrawCardRequest : ServerRequestBase
{
    public int clientId;
    public int cardCount;
    public int[] cardIds;
    public bool isShow;

    public DrawCardRequest()
    {
    }

    public DrawCardRequest(int clientId, int cardId, bool isShow)
    {
        this.clientId = clientId;
        this.cardCount = 1;
        this.cardIds = new int[] {cardId};
        this.isShow = isShow;
    }

    public DrawCardRequest(int clientId, List<int> cardIds, bool isShow)
    {
        this.clientId = clientId;
        this.cardCount = cardIds.Count;
        this.cardIds = cardIds.ToArray();
        this.isShow = isShow;
    }

    public override int GetProtocol()
    {
        return NetProtocols.DRAW_CARD;
    }

    public override string GetProtocolName()
    {
        return "DRAW_CARD";
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(clientId);
        writer.WriteSInt32(cardCount);
        if (isShow)
        {
            writer.WriteByte(0x01);
            foreach (int cardId in cardIds)
            {
                writer.WriteSInt32(cardId);
            }
        }
        else
        {
            writer.WriteByte(0x00);
        }
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        clientId = reader.ReadSInt32();
        cardCount = reader.ReadSInt32();
        if (reader.ReadByte() == 0x01)
        {
            isShow = true;
            cardIds = new int[cardCount];
            for (int i = 0; i < cardCount; i++)
            {
                cardIds[i] = reader.ReadSInt32();
            }
        }
        else
        {
            isShow = false;
        }
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [clientId] " + clientId;
        log += " [cardCount] " + cardCount;
        if (isShow)
        {
            log += " [cardId] ";
            foreach (int cardId in cardIds)
            {
                log += cardId + ", ";
            }
        }

        return log;
    }
}