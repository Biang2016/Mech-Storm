public class LoginRequest : ClientRequestBase
{
    public string username;
    public string password;

    public LoginRequest() : base()
    {
    }

    public LoginRequest(string username, string password) : base(0)
    {
        this.username = username;
        this.password = password;
    }

    public override int GetProtocol()
    {
        return NetProtocols.LOGIN_REQUEST;
    }

    public override string GetProtocolName()
    {
        return "LOGIN_REQUEST";
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteString16(username);
        writer.WriteString16(password);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        username = reader.ReadString16();
        password = reader.ReadString16();
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [username]=" + username;
        return log;
    }
}