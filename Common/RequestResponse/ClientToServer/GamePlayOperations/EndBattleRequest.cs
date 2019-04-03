public class EndBattleRequest : ClientRequestBase
{
    public EndBattleRequest()
    {
    }

    public EndBattleRequest(int clientId) : base(clientId)
    {
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.END_BATTLE_REQUEST;
    }

}