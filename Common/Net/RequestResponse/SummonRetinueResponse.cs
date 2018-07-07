using System.Collections;
using System.Collections.Generic;

public class SummonRetinueResponse : Response
{
    public int clientId;
    public CardInfo_Retinue cardInfo;
    public int handCardIndex;
    public int battleGroundIndex;

    public override int GetProtocol()
    {
        return NetProtocols.SUMMON_RETINUE;
    }

    public override string GetProtocolName()
    {
        return GetType().FullName;
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        clientId = reader.ReadSInt32();
        cardInfo = (CardInfo_Retinue) AllCards.AC.GetCard(reader.ReadSInt32());
        handCardIndex = reader.ReadSInt32();
        battleGroundIndex = reader.ReadSInt32();
    }

    public override string DeserializeLog()
    {
        string log = "";
        log += "[clientId]" + clientId;
        log += "[cardInfo.CardID]" + cardInfo.CardID;
        log += "[handCardIndex]" + handCardIndex;
        log += "[battleGroundIndex]" + battleGroundIndex;
        return log;
    }
}