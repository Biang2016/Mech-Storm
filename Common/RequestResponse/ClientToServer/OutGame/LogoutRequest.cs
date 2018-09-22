public class LogoutRequest : ClientRequestBase
{
    public string username;

    public LogoutRequest() : base()
    {
    }

    public LogoutRequest(int clientId, string username) : base(clientId)
    {
        this.username = username;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.LOGOUT_REQUEST;
    }

    public override string GetProtocolName()
    {
        return "LOGOUT_REQUEST";
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteString16(username);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        username = reader.ReadString16();
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [username]=" + username;
        return log;
    }
}