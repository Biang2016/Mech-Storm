﻿public class ClientMoneyRequest : ServerRequestBase
{
    public int clientMoney;

    public ClientMoneyRequest()
    {
    }

    public ClientMoneyRequest(int clientMoney)
    {
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
        writer.WriteSInt32(clientMoney);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        clientMoney = reader.ReadSInt32();
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [clientMoney]=" + clientMoney;
        return log;
    }
}