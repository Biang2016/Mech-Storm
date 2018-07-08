using System.Collections;
using System.Collections.Generic;

public class SummonRetinueRequest : Request
{
    public int clientId;
    public CardInfo_Retinue cardInfo;
    public int handCardIndex;
    public int battleGroundIndex;

    public SummonRetinueRequest()
    {

    }

    public SummonRetinueRequest(int clientId, CardInfo_Retinue cardInfo, int handCardIndex, int battleGroundIndex)
    {
        this.clientId = clientId;
        this.cardInfo = cardInfo;
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
        writer.WriteSInt32(clientId);
        writer.WriteSInt32(cardInfo.CardID);
        writer.WriteSInt32(handCardIndex);
        writer.WriteSInt32(battleGroundIndex);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        clientId = reader.ReadSInt32();
        cardInfo = (CardInfo_Retinue)AllCards.AC.GetCard(reader.ReadSInt32());
        handCardIndex = reader.ReadSInt32();
        battleGroundIndex = reader.ReadSInt32();
    }

    public override string DeserializeLog()
    {
        string log = "";
        log += " [clientId] " + clientId;
        log += " [cardInfo.CardID] " + cardInfo.CardID;
        log += " [handCardIndex] " + handCardIndex;
        log += " [battleGroundIndex] " + battleGroundIndex;
        return log;
    }
}