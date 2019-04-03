public class EndBattleRequestResponse : ServerRequestBase
{
    public EndBattleRequestResponse()
    {
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.END_BATTLE_REQUEST_RESPONSE;
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