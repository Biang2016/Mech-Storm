public class LogoutResultRequest : ServerRequestBase
{
    public string username;
    public bool isSuccess;

    public LogoutResultRequest()
    {
    }

    public LogoutResultRequest(string username, bool isSuccess)
    {
        this.username = username;
        this.isSuccess = isSuccess;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.LOGOUT_RESULT_REQUEST;
    }

    public override string GetProtocolName()
    {
        return "LOGOUT_RESULT_REQUEST";
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteString8(username);
        writer.WriteByte(isSuccess ? (byte) 0x01 : (byte) 0x00);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        username = reader.ReadString8();
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