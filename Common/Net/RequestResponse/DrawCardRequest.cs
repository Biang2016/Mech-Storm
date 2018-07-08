using System.Collections;
using System.Collections.Generic;

public class DrawCardRequest : Request
{
    public int clientId;
    public int cardId;
    public bool isShow;

    public override int GetProtocol()
    {
        return NetProtocols.DRAW_CARD;
    }

    public DrawCardRequest(int clientId, int cardId, bool isShow)
    {
        this.clientId = clientId;
        this.cardId = cardId;
        this.isShow = isShow;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(clientId);
        writer.WriteByte(0x01);
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
}