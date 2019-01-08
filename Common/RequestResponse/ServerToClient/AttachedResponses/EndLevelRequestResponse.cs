public class EndLevelRequestResponse : ServerRequestBase
{
    public EndLevelRequestResponse()
    {
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.END_LEVEL_REQUEST_RESPONSE;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
    }
}