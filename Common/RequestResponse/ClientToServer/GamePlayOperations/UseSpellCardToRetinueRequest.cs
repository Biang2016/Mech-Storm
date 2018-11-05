
public class UseSpellCardToRetinueRequest : ClientRequestBase
{
    public int handCardInstanceId;
    public int targetRetinueId; //-2表示无目标
    public bool isTargetRetinueIdTempId; //目标ID是否也是临时ID
    public int clientRetinueTempId; //客户端临时ID号，用于预召唤随从的匹配

    public UseSpellCardToRetinueRequest()
    {
    }

    public UseSpellCardToRetinueRequest(int clientId,int handCardInstanceId, int targetRetinueId, bool isTargetRetinueIdTempId, int clientRetinueTempId):base(clientId)
    {
        this.handCardInstanceId = handCardInstanceId;
        this.targetRetinueId = targetRetinueId;
        this.isTargetRetinueIdTempId = isTargetRetinueIdTempId;
        this.clientRetinueTempId = clientRetinueTempId;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.USE_SPELLCARD_TO_RETINUE_REQUEST;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(handCardInstanceId);
        writer.WriteSInt32(targetRetinueId);
        writer.WriteByte(isTargetRetinueIdTempId ? (byte) 0x01 : (byte) 0x00);
        writer.WriteSInt32(clientRetinueTempId);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        handCardInstanceId = reader.ReadSInt32();
        targetRetinueId = reader.ReadSInt32();
        isTargetRetinueIdTempId = reader.ReadByte() == 0x01;
        clientRetinueTempId = reader.ReadSInt32();
    }

}