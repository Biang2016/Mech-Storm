using MyCardGameCommon;

public class SummonRetinueRequest : ClientRequestBase
{
    public int handCardInstanceId;
    public int battleGroundIndex;
    public Vector3 lastDragPosition;
    public int targetRetinueId; //-2表示无目标
    public bool isTargetRetinueIdTempId; //目标ID是否也是临时ID
    public int clientRetinueTempId; //客户端临时ID号，用于预召唤随从的匹配

    public SummonRetinueRequest()
    {
    }

    public SummonRetinueRequest(int clientId, int handCardInstanceId, int battleGroundIndex, Vector3 lastDragPosition, int targetRetinueId, bool isTargetRetinueIdTempId, int clientRetinueTempId) : base(clientId)
    {
        this.handCardInstanceId = handCardInstanceId;
        this.battleGroundIndex = battleGroundIndex;
        this.lastDragPosition = lastDragPosition;
        this.targetRetinueId = targetRetinueId;
        this.isTargetRetinueIdTempId = isTargetRetinueIdTempId;
        this.clientRetinueTempId = clientRetinueTempId;
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
        writer.WriteByte(isTargetRetinueIdTempId ? (byte) 0x01 : (byte) 0x00);
        writer.WriteSInt32(clientRetinueTempId);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        handCardInstanceId = reader.ReadSInt32();
        battleGroundIndex = reader.ReadSInt32();
        lastDragPosition = Vector3.Deserialize(reader);
        targetRetinueId = reader.ReadSInt32();
        isTargetRetinueIdTempId = reader.ReadByte() == 0x01;
        clientRetinueTempId = reader.ReadSInt32();
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [handCardInstanceId]=" + handCardInstanceId;
        log += " [battleGroundIndex]=" + battleGroundIndex;
        log += " [lastDragPosition]=" + lastDragPosition;
        log += " [targetRetinueId]=" + targetRetinueId;
        log += " [isTargetRetinueIdTempId]=" + isTargetRetinueIdTempId;
        log += " [clientRetinueTempId]=" + clientRetinueTempId;
        return log;
    }
}