﻿public class EquipPackServerRequest : ServerRequestBase
{
    public int clientId;
    public CardInfo_Equip cardInfo;
    public int retinueId;
    public int equipID;

    public EquipPackServerRequest()
    {
    }

    public EquipPackServerRequest(int clientId, CardInfo_Equip cardInfo, int retinueId, int equipID)
    {
        this.clientId = clientId;
        this.cardInfo = cardInfo;
        this.retinueId = retinueId;
        this.equipID = equipID;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.SE_EQUIP_PACK_SERVER_REQUEST;
    }

    public override string GetProtocolName()
    {
        return "SE_EQUIP_PACK_SERVER_REQUEST";
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
        writer.WriteSInt32(equipID);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        clientId = reader.ReadSInt32();
        if (reader.ReadByte() == 0x01)
        {
            cardInfo = (CardInfo_Equip) (CardInfo_Base.Deserialze(reader));
        }

        retinueId = reader.ReadSInt32();
        equipID = reader.ReadSInt32();
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [clientId]=" + clientId;
        if (cardInfo == null)
        {
            log += " [PackDown] ";
        }
        else
        {
            log += " [cardInfo.CardID]=" + cardInfo.CardID;
        }

        log += " [retinueId]=" + retinueId;
        log += " [equipID]=" + equipID;
        return log;
    }
}