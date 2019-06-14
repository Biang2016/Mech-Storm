public class BattleGroundAddMechRequest : ServerRequestBase
{
    public int clientId;
    public CardInfo_Mech cardInfo;
    public int battleGroundIndex;
    public int mechId;
    public int clientMechTempId; //客户端预召唤随从的匹配Id

    public BattleGroundAddMechRequest()
    {
    }

    public BattleGroundAddMechRequest(int clientId, CardInfo_Mech cardInfo, int battleGroundIndex, int mechId, int clientMechTempId)
    {
        this.clientId = clientId;
        this.cardInfo = cardInfo;
        this.battleGroundIndex = battleGroundIndex;
        this.mechId = mechId;
        this.clientMechTempId = clientMechTempId;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.SE_BATTLEGROUND_ADD_MECH;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(clientId);
        cardInfo.Serialize(writer);
        writer.WriteSInt32(battleGroundIndex);
        writer.WriteSInt32(mechId);
        writer.WriteSInt32(clientMechTempId);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        clientId = reader.ReadSInt32();
        cardInfo = (CardInfo_Mech) (CardInfo_Base.Deserialze(reader));
        battleGroundIndex = reader.ReadSInt32();
        mechId = reader.ReadSInt32();
        clientMechTempId = reader.ReadSInt32();
    }
}