using System.Collections;
using System.Collections.Generic;
using MyCardGameCommon;

public class SummonRetinueRequest : ClientRequestBase
{
    public int handCardInstanceId;
    public int battleGroundIndex;
    public Vector3 lastDragPosition;

    public SummonRetinueRequest()
    {
    }

    public SummonRetinueRequest(int clientId, int handCardInstanceId, int battleGroundIndex, Vector3 lastDragPosition) : base(clientId)
    {
        this.handCardInstanceId = handCardInstanceId;
        this.battleGroundIndex = battleGroundIndex;
        this.lastDragPosition = lastDragPosition;
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
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        handCardInstanceId = reader.ReadSInt32();
        battleGroundIndex = reader.ReadSInt32();
        lastDragPosition = Vector3.Deserialize(reader);
    }

    public override string DeserializeLog()
    {
        string log = "";
        log += " [handCardInstanceId]=" + handCardInstanceId;
        log += " [battleGroundIndex]=" + battleGroundIndex;
        log += " [lastDragPosition]=" + lastDragPosition;
        return log;
    }
}