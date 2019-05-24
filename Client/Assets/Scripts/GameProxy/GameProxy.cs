/// <summary>
/// Proxy of all game events like battles, stories, builds, card edits and so on.
/// Including BattleProxy, BuildProxy, StoryProxy ...
/// Its duty is to allocate request/response to each sub-proxy.
///
/// If client plays standalone, then the GameProxy inside Client-end would start.
/// If client plays through Internet, then the GameProxy inside Server-end would start and the one inside Client-end would be disabled.
/// </summary>
public class GameProxy
{
    public delegate void SendMessageDelegate(ServerRequestBase request);

    public SendMessageDelegate SendMessage;

    public BattleProxy BattleProxy;

    public GameProxy(int clientID, string serverVersion, SendMessageDelegate sendMessageDelegate)
    {
        SendMessage = sendMessageDelegate;
        ClientIdRequest request = new ClientIdRequest(clientID, serverVersion);
        SendMessage(request);
    }

    public void ReceiveRequest(ClientRequestBase request)
    {
        switch (request)
        {
            case ClientVersionValidRequest _:
                LoginResultRequest response = new LoginResultRequest("local player", LoginResultRequest.StateCodes.Success);
                SendMessage(response);
                break;
        }
    }
}