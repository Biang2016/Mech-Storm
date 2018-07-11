using System.Collections;
using System.Collections.Generic;

public class DrawCardRequest : ServerRequestBase
{
    public int clientId;
    public int cardId;
    public bool isShow;

    public DrawCardRequest()
    {

    }

    public DrawCardRequest(int clientId, int cardId, bool isShow)
    {
        this.clientId = clientId;
        this.cardId = cardId;
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
        if (isShow)
        {
            writer.WriteByte(0x01);
            writer.WriteSInt32(cardId);
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
        if (reader.ReadByte() == 0x01)
        {
            isShow = true;
            cardId = reader.ReadSInt32();
        }
        else
        {
            isShow = false;
        }
    }

    public override string DeserializeLog()
    {
        string log = "";
        log += " [clientId] " + clientId;
        if (isShow)
        {
            log += " [cardId] " + cardId;
        }
        return log;
    }
}