using System.Collections;
using System.Collections.Generic;
using MyCardGameCommon;

public class SummonRetinueRequest : ClientRequestBase
{
    public int handCardInstanceId;
    public int battleGroundIndex;
    public Vector3 lastDragPosition;
    public int targetRetinueId; //-2表示无目标

    public SummonRetinueRequest()
    {
    }

    public SummonRetinueRequest(int clientId, int handCardInstanceId, int battleGroundIndex, Vector3 lastDragPosition, int targetRetinueId) : base(clientId)
    {
        this.handCardInstanceId = handCardInstanceId;
        this.battleGroundIndex = battleGroundIndex;
        this.lastDragPosition = lastDragPosition;
        this.targetRetinueId = targetRetinueId;
    }

    public override int GetProtocol()
    {
        return NetProtocols.SUMMON_RETINUE_REQUEST;
    }

    public override string GetProtocolName()
    {
        return "SUMMON_RETINUE_REQUEST";
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(handCardInstanceId);
        writer.WriteSInt32(battleGroundIndex);
        lastDragPosition.Serialize(writer);
        writer.WriteSInt32(targetRetinueId);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        handCardInstanceId = reader.ReadSInt32();
        battleGroundIndex = reader.ReadSInt32();
        lastDragPosition = Vector3.Deserialize(reader);
        targetRetinueId = reader.ReadSInt32();
    }

    public override string DeserializeLog()
    {
        string log = "";
        log += " [handCardInstanceId]=" + handCardInstanceId;
        log += " [battleGroundIndex]=" + battleGroundIndex;
        log += " [lastDragPosition]=" + lastDragPosition;
        log += " [targetRetinueId]=" + targetRetinueId;
        return log;
    }
}