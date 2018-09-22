public class BattleGroundAddRetinueRequest : ServerRequestBase
{
    public int clientId;
    public CardInfo_Retinue cardInfo;
    public int battleGroundIndex;
    public int retinueId;
    public int clientRetinueTempId;//客户端预召唤随从的匹配Id

    public BattleGroundAddRetinueRequest()
    {
    }

    public BattleGroundAddRetinueRequest(int clientId, CardInfo_Retinue cardInfo, int battleGroundIndex, int retinueId, int clientRetinueTempId)
    {
        this.clientId = clientId;
        this.cardInfo = cardInfo;
        this.battleGroundIndex = battleGroundIndex;
        this.retinueId = retinueId;
        this.clientRetinueTempId = clientRetinueTempId;
    }

    public override NetProtocols GetProtocol()
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
        cardInfo.Serialize(writer);
        writer.WriteSInt32(battleGroundIndex);
        writer.WriteSInt32(retinueId);
        writer.WriteSInt32(clientRetinueTempId);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        clientId = reader.ReadSInt32();
        cardInfo = (CardInfo_Retinue) (CardInfo_Base.Deserialze(reader));
        battleGroundIndex = reader.ReadSInt32();
        retinueId = reader.ReadSInt32();
        clientRetinueTempId = reader.ReadSInt32();
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [clientId]=" + clientId;
        log += " [cardInfo.CardID]=" + cardInfo.CardID;
        log += " [battleGroundIndex]=" + battleGroundIndex;
        log += " [retinueId]=" + retinueId;
        log += " [clientRetinueTempId]=" + clientRetinueTempId;
        return log;
    }
}