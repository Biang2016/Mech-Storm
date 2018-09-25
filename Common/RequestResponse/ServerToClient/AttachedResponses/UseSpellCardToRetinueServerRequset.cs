using MyCardGameCommon;

public class UseSpellCardToRetinueServerRequset : ServerRequestBase
{
    public int clientId;
    public int handCardInstanceId;
    public Vector3 lastDragPosition;
    public int targetRetinueId; //-2表示无目标
    public bool isTargetRetinueIdTempId; //目标ID是否也是临时ID
    public int clientRetinueTempId; //客户端临时ID号，用于预召唤随从的匹配

    public UseSpellCardToRetinueServerRequset()
    {
    }

    public UseSpellCardToRetinueServerRequset(int clientId, int handCardInstanceId, Vector3 lastDragPosition, int targetRetinueId, bool isTargetRetinueIdTempId, int clientRetinueTempId) 
    {
        this.clientId = clientId;
        this.handCardInstanceId = handCardInstanceId;
        this.lastDragPosition = lastDragPosition;
        this.targetRetinueId = targetRetinueId;
        this.isTargetRetinueIdTempId = isTargetRetinueIdTempId;
        this.clientRetinueTempId = clientRetinueTempId;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.SE_USE_SPELLCARD_TO_RETINUE_SERVER_REQUEST;
    }

    public override string GetProtocolName()
    {
        return "SE_USE_SPELLCARD_TO_RETINUE_SERVER_REQUEST";
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(handCardInstanceId);
        lastDragPosition.Serialize(writer);
        writer.WriteSInt32(targetRetinueId);
        writer.WriteByte(isTargetRetinueIdTempId ? (byte) 0x01 : (byte) 0x00);
        writer.WriteSInt32(clientRetinueTempId);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        handCardInstanceId = reader.ReadSInt32();
        lastDragPosition = Vector3.Deserialize(reader);
        targetRetinueId = reader.ReadSInt32();
        isTargetRetinueIdTempId = reader.ReadByte() == 0x01;
        clientRetinueTempId = reader.ReadSInt32();
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [handCardInstanceId]=" + handCardInstanceId;
        log += " [lastDragPosition]=" + lastDragPosition;
        log += " [targetRetinueId]=" + targetRetinueId;
        log += " [isTargetRetinueIdTempId]=" + isTargetRetinueIdTempId;
        log += " [clientRetinueTempId]=" + clientRetinueTempId;
        return log;
    }
}