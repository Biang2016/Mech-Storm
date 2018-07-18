using System.Collections;
using System.Collections.Generic;

public class BattleGroundAddRetinueRequest : ServerRequestBase
{
    public int clientId;
    public CardInfo_Retinue cardInfo;
    public int battleGroundIndex;

    public BattleGroundAddRetinueRequest()
    {
    }

    public BattleGroundAddRetinueRequest(int clientId, CardInfo_Retinue cardInfo, int battleGroundIndex)
    {
        this.clientId = clientId;
        this.cardInfo = cardInfo;
        this.battleGroundIndex = battleGroundIndex;
    }

    public override int GetProtocol()
    {
        return NetProtocols.SE_BATTLEGROUND_ADD_RETINUE;
    }

    public override string GetProtocolName()
    {
        return "SE_BATTLEGROUND_ADD_RETINUE";
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(clientId);
        writer.WriteSInt32(cardInfo.CardID);
        writer.WriteSInt32(battleGroundIndex);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        clientId = reader.ReadSInt32();
        cardInfo = (CardInfo_Retinue) AllCards.GetCard(reader.ReadSInt32());
        battleGroundIndex = reader.ReadSInt32();
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [clientId]=" + clientId;
        log += " [cardInfo.CardID]=" + cardInfo.CardID;
        log += " [battleGroundIndex]=" + battleGroundIndex;
        return log;
    }
}