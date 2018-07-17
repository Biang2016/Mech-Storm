using System.Collections;
using System.Collections.Generic;

public class EquipShieldRequest_Response : ClientOperationResponseBase
{
    public int clientId;
    public CardInfo_Shield cardInfo;
    public int handCardIndex;
    public int battleGroundIndex;
    public int shieldPlaceIndex;

    public EquipShieldRequest_Response()
    {
    }

    public EquipShieldRequest_Response(int clientId, CardInfo_Shield cardInfo, int handCardIndex, int battleGroundIndex, int shieldPlaceIndex)
    {
        this.clientId = clientId;
        this.cardInfo = cardInfo;
        this.handCardIndex = handCardIndex;
        this.battleGroundIndex = battleGroundIndex;
        this.shieldPlaceIndex = shieldPlaceIndex;
    }

    public override int GetProtocol()
    {
        return NetProtocols.EQUIP_SHIELD_REQUEST_RESPONSE;
    }

    public override string GetProtocolName()
    {
        return "EQUIP_SHIELD_REQUEST_RESPONSE";
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(clientId);
        writer.WriteSInt32(cardInfo.CardID);
        writer.WriteSInt32(handCardIndex);
        writer.WriteSInt32(battleGroundIndex);
        writer.WriteSInt32(shieldPlaceIndex);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        clientId = reader.ReadSInt32();
        cardInfo = (CardInfo_Shield) AllCards.GetCard(reader.ReadSInt32());
        handCardIndex = reader.ReadSInt32();
        battleGroundIndex = reader.ReadSInt32();
        shieldPlaceIndex = reader.ReadSInt32();
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [clientId]=" + clientId;
        log += " [cardInfo.CardID]=" + cardInfo.CardID;
        log += " [handCardIndex]=" + handCardIndex;
        log += " [battleGroundIndex]=" + battleGroundIndex;
        log += " [shieldPlaceIndex]=" + shieldPlaceIndex;
        return log;
    }
}