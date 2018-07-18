using System.Collections;
using System.Collections.Generic;
using System.Security.AccessControl;

public class RetinueAttackRetinueServerRequest : ServerRequestBase
{
    public int AttackRetinueClientId;
    public int AttackRetinuePlaceIndex;
    public int BeAttackedRetinueClientId;
    public int BeAttackedRetinuePlaceIndex;

    public RetinueAttackRetinueServerRequest()
    {
    }

    public RetinueAttackRetinueServerRequest(int attackRetinueClientId, int retinuePlaceIndex, int beAttackedRetinueClientId, int beAttackedRetinuePlaceIndex)
    {
        AttackRetinueClientId = attackRetinueClientId;
        AttackRetinuePlaceIndex = retinuePlaceIndex;
        BeAttackedRetinueClientId = beAttackedRetinueClientId;
        BeAttackedRetinuePlaceIndex = beAttackedRetinuePlaceIndex;
    }

    public override int GetProtocol()
    {
        return NetProtocols.SE_RETINUE_ATTACK_RETINUE_SERVER_REQUEST;
    }

    public override string GetProtocolName()
    {
        return "SE_RETINUE_ATTACK_RETINUE_SERVER_REQUEST";
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(AttackRetinueClientId);
        writer.WriteSInt32(AttackRetinuePlaceIndex);
        writer.WriteSInt32(BeAttackedRetinueClientId);
        writer.WriteSInt32(BeAttackedRetinuePlaceIndex);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        AttackRetinueClientId = reader.ReadSInt32();
        AttackRetinuePlaceIndex = reader.ReadSInt32();
        BeAttackedRetinueClientId = reader.ReadSInt32();
        BeAttackedRetinuePlaceIndex = reader.ReadSInt32();
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [AttackRetinueClientId]=" + AttackRetinueClientId;
        log += " [RetinuePlaceIndex]=" + AttackRetinuePlaceIndex;
        log += " [BeAttackedRetinueClientId]=" + BeAttackedRetinueClientId;
        log += " [BeAttackedRetinuePlaceIndex]=" + BeAttackedRetinuePlaceIndex;
        return log;
    }
}