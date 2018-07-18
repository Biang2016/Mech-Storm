using System.Collections;
using System.Collections.Generic;

public class EquipShieldServerRequest : ServerRequestBase
{
    public int clientId;
    public CardInfo_Shield cardInfo;
    public int battleGroundIndex;
    public int shieldPlaceIndex;

    public EquipShieldServerRequest()
    {
    }

    public EquipShieldServerRequest(int clientId, CardInfo_Shield cardInfo, int battleGroundIndex, int shieldPlaceIndex)
    {
        this.clientId = clientId;
        this.cardInfo = cardInfo;
        this.battleGroundIndex = battleGroundIndex;
        this.shieldPlaceIndex = shieldPlaceIndex;
    }

    public override int GetProtocol()
    {
        return NetProtocols.SE_EQUIP_SHIELD_SERVER_REQUEST;
    }

    public override string GetProtocolName()
    {
        return "EQUIP_SHIELD_SERVER_REQUEST";
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(clientId);
        writer.WriteSInt32(cardInfo.CardID);
        writer.WriteSInt32(battleGroundIndex);
        writer.WriteSInt32(shieldPlaceIndex);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        clientId = reader.ReadSInt32();
        cardInfo = (CardInfo_Shield) AllCards.GetCard(reader.ReadSInt32());
        battleGroundIndex = reader.ReadSInt32();
        shieldPlaceIndex = reader.ReadSInt32();
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [clientId]=" + clientId;
        log += " [cardInfo.CardID]=" + cardInfo.CardID;
        log += " [battleGroundIndex]=" + battleGroundIndex;
        log += " [shieldPlaceIndex]=" + shieldPlaceIndex;
        return log;
    }
}