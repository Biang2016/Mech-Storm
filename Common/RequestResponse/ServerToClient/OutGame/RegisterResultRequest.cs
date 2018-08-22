public class RegisterResultRequest : ServerRequestBase
{
    public bool isSuccess;

    public RegisterResultRequest()
    {
    }

    public RegisterResultRequest(bool isSuccess)
    {
        this.isSuccess = isSuccess;
    }

    public override int GetProtocol()
    {
        return NetProtocols.REGISTER_RESULT_REQUEST;
    }

    public override string GetProtocolName()
    {
        return "REGISTER_RESULT_REQUEST";
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteByte(isSuccess ? (byte) 0x01 : (byte) 0x00);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        isSuccess = reader.ReadByte() == 0x01;
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [isSuccess]=" + isSuccess;
        return log;
    }
}