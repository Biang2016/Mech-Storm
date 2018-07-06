using System.Collections;
using System.Collections.Generic;

public class GetACardRequest : Request
{
    public int clientId;
    public int cardId;

    public override int GetProtocol()
    {
        return NetProtocols.GET_A_CARD;
    }

    public GetACardRequest(int clientId,int cardId)
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