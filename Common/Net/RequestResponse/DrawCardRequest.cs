using System.Collections;
using System.Collections.Generic;

public class DrawCardRequest : Request
{
    public int clientId;
    public int cardId;

    public override int GetProtocol()
    {
        return NetProtocols.DRAW_CARD;
    }

    public DrawCardRequest(int clientId,int cardId)
    {
        this.clientId = clientId;
        this.cardId = cardId;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(clientId);
        writer.WriteSInt32(cardId);
    }
}