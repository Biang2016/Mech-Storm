using System.Collections;
using System.Collections.Generic;

public class EquipShieldServerRequest : ServerRequestBase
{
    public int clientId;
    public CardInfo_Shield cardInfo;
    public int retinueId;
    public int shieldPlaceIndex;

    public EquipShieldServerRequest()
    {
    }

    public EquipShieldServerRequest(int clientId, CardInfo_Shield cardInfo, int retinueId, int shieldPlaceIndex)
    {
        this.clientId = clientId;
        this.cardInfo = cardInfo;
        this.retinueId = retinueId;
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
        if (cardInfo == null)
        {
            writer.WriteByte(0x00);
        }
        else
        {
            writer.WriteByte(0x01);
            cardInfo.Serialize(writer);
        }
        writer.WriteSInt32(retinueId);
        writer.WriteSInt32(shieldPlaceIndex);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        clientId = reader.ReadSInt32();
        if (reader.ReadByte() == 0x01)
        {
            cardInfo = (CardInfo_Shield)(CardInfo_Base.Deserialze(reader));
        }
        retinueId = reader.ReadSInt32();
        shieldPlaceIndex = reader.ReadSInt32();
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [clientId]=" + clientId;
        if (cardInfo == null)
        {
            log += " [ShieldDown] ";
        }
        else
        {
            log += " [cardInfo.CardID]=" + cardInfo.CardID;
        }
        log += " [retinueId]=" + retinueId;
        log += " [shieldPlaceIndex]=" + shieldPlaceIndex;
        return log;
    }
}