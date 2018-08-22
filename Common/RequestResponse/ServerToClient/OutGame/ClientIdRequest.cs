public class ClientIdRequest : ServerRequestBase
{
    public int givenClientId;

    public ClientIdRequest()
    {
    }

    public ClientIdRequest(int givenClientId)
    {
        this.givenClientId = givenClientId;
    }

    public override int GetProtocol()
    {
        return NetProtocols.CLIENT_ID_REQUEST;
    }

    public override string GetProtocolName()
    {
        return "CLIENT_ID_REQUEST";
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(givenClientId);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        givenClientId = reader.ReadSInt32();
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [givenClientId]=" + givenClientId;
        return log;
    }
}