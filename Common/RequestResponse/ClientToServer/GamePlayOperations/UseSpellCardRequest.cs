
public class UseSpellCardRequest : ClientRequestBase
{
    public int handCardInstanceId;

    public UseSpellCardRequest()
    {
    }

    public UseSpellCardRequest(int clientId,int handCardInstanceId):base(clientId)
    {
        this.handCardInstanceId = handCardInstanceId;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.USE_SPELLCARD_REQUEST;
    }

    public override string GetProtocolName()
    {
        return "USE_SPELLCARD_REQUEST";
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(handCardInstanceId);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        handCardInstanceId = reader.ReadSInt32();
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [handCardInstanceId]=" + handCardInstanceId;
        return log;
    }
}