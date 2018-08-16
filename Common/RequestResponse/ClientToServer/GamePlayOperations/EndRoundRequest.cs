public class EndRoundRequest : ClientRequestBase
{
    public EndRoundRequest()
    {
    }

    public EndRoundRequest(int clientId) : base(clientId)
    {
    }

    public override int GetProtocol()
    {
        return NetProtocols.END_ROUND_REQUEST;
    }

    public override string GetProtocolName()
    {
        return "END_ROUND_REQUEST";
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        return log;
    }
}