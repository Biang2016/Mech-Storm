using System.Collections;
using System.Collections.Generic;

public class EquipShieldRequest : ClientRequestBase
{
    public CardInfo_Shield cardInfo;
    public int handCardIndex;
    public int battleGroundIndex;
    public int shieldPlaceIndex;

    public EquipShieldRequest()
    {
    }

    public EquipShieldRequest(int clientId, CardInfo_Shield cardInfo, int handCardIndex, int battleGroundIndex, int shieldPlaceIndex) :base(clientId)
    {
        this.cardInfo = cardInfo;
        this.handCardIndex = handCardIndex;
        this.battleGroundIndex = battleGroundIndex;
        this.shieldPlaceIndex = shieldPlaceIndex;
    }

    public override int GetProtocol()
    {
        return NetProtocols.EQUIP_SHIELD;
    }

    public override string GetProtocolName()
    {
        return "EQUIP_SHIELD";
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(cardInfo.CardID);
        writer.WriteSInt32(handCardIndex);
        writer.WriteSInt32(battleGroundIndex);
        writer.WriteSInt32(shieldPlaceIndex);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        cardInfo = (CardInfo_Shield) AllCards.GetCard(reader.ReadSInt32());
        handCardIndex = reader.ReadSInt32();
        battleGroundIndex = reader.ReadSInt32();
        shieldPlaceIndex = reader.ReadSInt32();
    }

    public override string DeserializeLog()
    {
        string log = "";
        log += " [cardInfo.CardID] " + cardInfo.CardID;
        log += " [handCardIndex] " + handCardIndex;
        log += " [battleGroundIndex] " + battleGroundIndex;
        log += " [shieldPlaceIndex] " + shieldPlaceIndex;
        return log;
    }
}