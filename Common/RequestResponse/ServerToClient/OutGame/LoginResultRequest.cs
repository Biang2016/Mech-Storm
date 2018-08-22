public class LoginResultRequest : ServerRequestBase
{
    public string username;
    public bool isSuccess;

    public LoginResultRequest()
    {
    }

    public LoginResultRequest(string username, bool isSuccess)
    {
        this.username = username;
        this.isSuccess = isSuccess;
    }

    public override int GetProtocol()
    {
        return NetProtocols.LOGIN_RESULT_REQUEST;
    }

    public override string GetProtocolName()
    {
        return "LOGIN_RESULT_REQUEST";
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteString16(username);
        writer.WriteByte(isSuccess ? (byte) 0x01 : (byte) 0x00);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        username = reader.ReadString16();
        isSuccess = reader.ReadByte() == 0x01;
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [username]=" + username;
        log += " [isSuccess]=" + isSuccess;
        return log;
    }
}