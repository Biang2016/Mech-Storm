
public class UseSpellCardToMechRequest : ClientRequestBase
{
    public int handCardInstanceId;
    public int targetMechId; //-2表示无目标
    public bool isTargetMechIdTempId; //目标ID是否也是临时ID
    public int clientMechTempId; //客户端临时ID号，用于预召唤随从的匹配

    public UseSpellCardToMechRequest()
    {
    }

    public UseSpellCardToMechRequest(int clientId,int handCardInstanceId, int targetMechId, bool isTargetMechIdTempId, int clientMechTempId):base(clientId)
    {
        this.handCardInstanceId = handCardInstanceId;
        this.targetMechId = targetMechId;
        this.isTargetMechIdTempId = isTargetMechIdTempId;
        this.clientMechTempId = clientMechTempId;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.USE_SPELLCARD_TO_MECH_REQUEST;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(handCardInstanceId);
        writer.WriteSInt32(targetMechId);
        writer.WriteByte(isTargetMechIdTempId ? (byte) 0x01 : (byte) 0x00);
        writer.WriteSInt32(clientMechTempId);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        handCardInstanceId = reader.ReadSInt32();
        targetMechId = reader.ReadSInt32();
        isTargetMechIdTempId = reader.ReadByte() == 0x01;
        clientMechTempId = reader.ReadSInt32();
    }

}