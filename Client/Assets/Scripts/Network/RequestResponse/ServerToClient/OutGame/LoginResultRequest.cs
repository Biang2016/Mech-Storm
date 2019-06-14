using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

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

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteString8(username);
        writer.WriteSInt32((int) stateCode);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        username = reader.ReadString8();
        stateCode = (StateCodes) reader.ReadSInt32();
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum StateCodes
    {
        Success,
        UnexistedUser,
        WrongPassword,
        AlreadyOnline
    }
}