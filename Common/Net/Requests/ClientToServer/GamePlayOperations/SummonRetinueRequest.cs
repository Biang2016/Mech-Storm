using System.Collections;
using System.Collections.Generic;

public class SummonRetinueRequest : ClientRequestBase
{
    public CardInfo_Retinue cardInfo;
    public int handCardIndex;
    public int battleGroundIndex;

    public SummonRetinueRequest()
    {

    }

    public SummonRetinueRequest(int clientId, CardInfo_Retinue cardInfo, int handCardIndex, int battleGroundIndex):base(clientId)
    {
        this.cardInfo = (CardInfo_Retinue)cardInfo.Clone();
        this.handCardIndex = handCardIndex;
        this.battleGroundIndex = battleGroundIndex;
    }
    public override int GetProtocol()
    {
        return NetProtocols.SUMMON_RETINUE;
    }

	public override string GetProtocolName()
	{
        return "SUMMON_RETINUE";
	}

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(cardInfo.CardID);
        writer.WriteSInt32(handCardIndex);
        writer.WriteSInt32(battleGroundIndex);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        cardInfo = (CardInfo_Retinue)AllCards.GetCard(reader.ReadSInt32());
        handCardIndex = reader.ReadSInt32();
        battleGroundIndex = reader.ReadSInt32();
    }

    public override string DeserializeLog()
    {
        string log = "";
        log += " [cardInfo.CardID] " + cardInfo.CardID;
        log += " [handCardIndex] " + handCardIndex;
        log += " [battleGroundIndex] " + battleGroundIndex;
        return log;
    }
}