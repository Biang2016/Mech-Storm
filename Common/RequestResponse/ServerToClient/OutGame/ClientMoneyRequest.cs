public class ClientMoneyRequest : ServerRequestBase
{
    public int givenClientId;
    public int clientMoney;

    public ClientMoneyRequest()
    {
    }

    public ClientMoneyRequest(int givenClientId, int clientMoney)
    {
        this.givenClientId = givenClientId;
        this.clientMoney = clientMoney;
    }

    public override int GetProtocol()
    {
        return NetProtocols.CLIENT_MONEY_REQUEST;
    }

    public override string GetProtocolName()
    {
        return "CLIENT_MONEY_REQUEST";
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(givenClientId);
        writer.WriteSInt32(clientMoney);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        givenClientId = reader.ReadSInt32();
        clientMoney = reader.ReadSInt32();
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [givenClientId]=" + givenClientId;
        log += " [clientMoney]=" + clientMoney;
        return log;
    }
}