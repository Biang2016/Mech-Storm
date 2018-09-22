public class CardDeckLeftChangeRequest : ServerRequestBase
{
    public int clientId;
    public int left;

    public CardDeckLeftChangeRequest()
    {
    }

    public CardDeckLeftChangeRequest(int clientId, int left)
    {
        this.clientId = clientId;
        this.left = left;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.SE_CARDDECT_LEFT_CHANGE;
    }

    public override string GetProtocolName()
    {
        return "SE_CARDDECT_LEFT_CHANGE";
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(clientId);
        writer.WriteSInt32(left);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        clientId = reader.ReadSInt32();
        left = reader.ReadSInt32();
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [clientId]=" + clientId;
        log += " [left]=" + left;
        return log;
    }
}