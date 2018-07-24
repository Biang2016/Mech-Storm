using System.Collections;
using System.Collections.Generic;

public class EquipShieldRequest : ClientRequestBase
{
    public int handCardIndex;
    public int battleGroundIndex;
    public int shieldPlaceIndex;

    public EquipShieldRequest()
    {
    }

    public EquipShieldRequest(int clientId, int handCardIndex, int battleGroundIndex, int shieldPlaceIndex) :base(clientId)
    {
        this.handCardIndex = handCardIndex;
        this.battleGroundIndex = battleGroundIndex;
        this.shieldPlaceIndex = shieldPlaceIndex;
    }

    public override int GetProtocol()
    {
        return NetProtocols.EQUIP_SHIELD_REQUEST;
    }

    public override string GetProtocolName()
    {
        return "EQUIP_SHIELD_REQUEST";
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(handCardIndex);
        writer.WriteSInt32(battleGroundIndex);
        writer.WriteSInt32(shieldPlaceIndex);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        handCardIndex = reader.ReadSInt32();
        battleGroundIndex = reader.ReadSInt32();
        shieldPlaceIndex = reader.ReadSInt32();
    }

    public override string DeserializeLog()
    {
        string log = "";
        log += " [handCardIndex]=" + handCardIndex;
        log += " [battleGroundIndex]=" + battleGroundIndex;
        log += " [shieldPlaceIndex]=" + shieldPlaceIndex;
        return log;
    }
}