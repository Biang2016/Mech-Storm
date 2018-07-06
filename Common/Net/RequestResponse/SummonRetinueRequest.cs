using System.Collections;
using System.Collections.Generic;

public class SummonRetinueRequest : Request
{
    public int clientId;
    public CardInfo_Retinue cardInfo;
    public int retinuePlaceIndex;

    public override int GetProtocol()
    {
        return NetProtocols.SUMMON_RETINUE;
    }

    public SummonRetinueRequest(int clientId, CardInfo_Retinue cardInfo, int retinuePlaceIndex)
    {
        this.clientId = clientId;
        this.cardInfo = cardInfo;
        this.retinuePlaceIndex = retinuePlaceIndex;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(clientId);
        writer.WriteSInt32(cardInfo.CardID);
        writer.WriteSInt32(retinuePlaceIndex);
    }
}