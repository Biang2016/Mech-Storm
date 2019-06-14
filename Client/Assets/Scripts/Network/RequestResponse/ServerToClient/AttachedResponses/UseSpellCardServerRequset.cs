public class UseSpellCardServerRequset : ServerRequestBase
{
    public int clientId;
    public int handCardInstanceId;

    public UseSpellCardServerRequset()
    {
    }

    public UseSpellCardServerRequset(int clientId, int handCardInstanceId)
    {
        this.clientId = clientId;
        this.handCardInstanceId = handCardInstanceId;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.SE_USE_SPELLCARD_SERVER_REQUEST;
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
}