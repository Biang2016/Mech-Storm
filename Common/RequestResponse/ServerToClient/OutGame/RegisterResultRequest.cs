public class RegisterResultRequest : ServerRequestBase
{
    public int clientId;
    public bool isSuccess;

    public RegisterResultRequest()
    {
    }

    public RegisterResultRequest(int clientId, bool isSuccess)
    {
        this.clientId = clientId;
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
        writer.WriteSInt32(clientId);
        writer.WriteByte(isSuccess ? (byte) 0x01 : (byte) 0x00);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        clientId = reader.ReadSInt32();
        isSuccess = reader.ReadByte() == 0x01;
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [clientId]=" + clientId;
        log += " [isSuccess]=" + isSuccess;
        return log;
    }
}