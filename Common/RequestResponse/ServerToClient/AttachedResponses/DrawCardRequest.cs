using System.Collections.Generic;

public class DrawCardRequest : ServerRequestBase
{
    public int clientId;
    public List<CardIdAndInstanceId> cardInfos = new List<CardIdAndInstanceId>();
    public bool isShow;

    public DrawCardRequest()
    {
    }

    public DrawCardRequest(int clientId, CardIdAndInstanceId cardInfo, bool isShow)
    {
        this.clientId = clientId;
        this.cardInfos.Add(cardInfo);
        this.isShow = isShow;
    }

    public DrawCardRequest(int clientId, List<CardIdAndInstanceId> cardInfos, bool isShow)
    {
        this.clientId = clientId;
        this.cardInfos.AddRange(cardInfos.ToArray());
        this.isShow = isShow;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.SE_DRAW_CARD;
    }

    public override string GetProtocolName()
    {
        return "SE_DRAW_CARD";
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(clientId);
        writer.WriteSInt32(cardInfos.Count);
        if (isShow)
        {
            writer.WriteByte(0x01);
            foreach (CardIdAndInstanceId cardInfo in cardInfos)
            {
                writer.WriteSInt32(cardInfo.CardId);
                writer.WriteSInt32(cardInfo.CardInstanceId);
            }
        }
        else
        {
            writer.WriteByte(0x00);
            foreach (CardIdAndInstanceId cardInfo in cardInfos)
            {
                writer.WriteSInt32(cardInfo.CardInstanceId);
            }
        }
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        clientId = reader.ReadSInt32();
        int cardCount = reader.ReadSInt32();
        if (reader.ReadByte() == 0x01)
        {
            isShow = true;
            for (int i = 0; i < cardCount; i++)
            {
                int cardId = reader.ReadSInt32();
                int cardInstanceId = reader.ReadSInt32();
                cardInfos.Add(new CardIdAndInstanceId(cardId, cardInstanceId));
            }
        }
        else
        {
            isShow = false;
            for (int i = 0; i < cardCount; i++)
            {
                int cardInstanceId = reader.ReadSInt32();
                cardInfos.Add(new CardIdAndInstanceId(999, cardInstanceId));
            }
        }
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [clientId]=" + clientId;
        log += " [cardCount]=" + cardInfos.Count;
        if (isShow)
        {
            log += " [cardInfo]=";
            foreach (CardIdAndInstanceId cardInfo in cardInfos)
            {
                log += cardInfo.CardId + "[" + cardInfo.CardInstanceId + "], ";
            }
        }

        return log;
    }

    public struct CardIdAndInstanceId
    {
        public int CardId;
        public int CardInstanceId;

        public CardIdAndInstanceId(int cardId, int cardInstanceId)
        {
            CardId = cardId;
            CardInstanceId = cardInstanceId;
        }
    }
}