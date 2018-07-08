using System.Collections;
using System.Collections.Generic;

public class SummonRetinueRequest : Request
{
    public int clientId;
    public CardInfo_Retinue cardInfo;
    public int handCardIndex;
    public int battleGroundIndex;

    public override int GetProtocol()
    {
        return NetProtocols.SUMMON_RETINUE;
    }

    public SummonRetinueRequest(int clientId, CardInfo_Retinue cardInfo, int handCardIndex, int battleGroundIndex)
    {
        this.clientId = clientId;
        this.cardInfo = cardInfo;
        this.handCardIndex = handCardIndex;
        this.battleGroundIndex = battleGroundIndex;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(clientId);
        writer.WriteSInt32(cardInfo.CardID);
        writer.WriteSInt32(handCardIndex);
        writer.WriteSInt32(battleGroundIndex);
    }
}