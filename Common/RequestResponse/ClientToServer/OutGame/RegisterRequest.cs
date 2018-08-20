public class RegisterRequest : ClientRequestBase
{
    public string username;
    public int password;

    public RegisterRequest() : base()
    {
    }

    public RegisterRequest(string username, int password) : base(0)
    {
        this.username = username;
        this.password = password;
    }

    public override int GetProtocol()
    {
        return NetProtocols.REGISTER_REQUEST;
    }

    public override string GetProtocolName()
    {
        return "REGISTER_REQUEST";
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteString16(username);
        writer.WriteSInt32(password);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        username = reader.ReadString16();
        password = reader.ReadSInt32();
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [username]=" + username;
        return log;
    }
}