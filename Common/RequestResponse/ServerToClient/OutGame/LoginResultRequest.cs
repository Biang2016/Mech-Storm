public class LoginResultRequest : ServerRequestBase
{
    public string username;
    public StateCodes stateCode;

    public LoginResultRequest()
    {
    }

    public LoginResultRequest(string username, StateCodes stateCode)
    {
        this.username = username;
        this.stateCode = stateCode;
    }

    public override NetProtocols GetProtocol()
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
        writer.WriteSInt32((int) stateCode);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        username = reader.ReadString16();
        stateCode = (StateCodes) reader.ReadSInt32();
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [username]=" + username;
        log += " [isSuccess]=" + stateCode;
        return log;
    }

    public enum StateCodes
    {
        Success,
        UnexistedUser,
        WrongPassword,
        AlreadyOnline
    }
}