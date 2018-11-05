public class HeartBeatRequest : ServerRequestBase
{
    public HeartBeatRequest()
    {
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.HEART_BEAT_REQUEST;
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