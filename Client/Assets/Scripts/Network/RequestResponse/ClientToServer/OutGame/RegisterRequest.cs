public class RegisterRequest : ClientRequestBase
{
    public string username;
    public string password;

    public RegisterRequest() : base()
    {
    }

    public RegisterRequest(int clientId, string username, string password) : base(clientId)
    {
        this.username = username;
        this.password = password;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.REGISTER_REQUEST;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteString8(username);
        writer.WriteString8(password);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        username = reader.ReadString8();
        password = reader.ReadString8();
    }
}