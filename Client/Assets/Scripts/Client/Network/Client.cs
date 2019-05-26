public class Client : MonoSingleton<Client>
{
    private Proxy proxy;

    public Proxy Proxy
    {
        get
        {
            if (proxy == null)
            {
                proxy = new Proxy(null, 0, false);
            }

            return proxy;
        }
        set => proxy = value;
    }

    public GameProxy GameProxy;

    public void SetNetwork(bool isOnline)
    {
        if (isOnline)
        {
            Proxy.SwitchSendMessageTarget(Proxy.MessageTarget.Server, NetworkManager.Instance.Send);
        }
        else
        {
            NetworkManager.Instance.Closed();
            NetworkManager.Instance.TerminateConnection();
            Proxy.Socket = null;
            Proxy.SwitchSendMessageTarget(Proxy.MessageTarget.LocalGameProxy, SendToLocalGameProxy);
            GameProxy = new GameProxy(999, "Player1", NetworkManager.ClientVersion, SendFromLocalGameProxyToClient, ClientLog.Instance);
            GameProxy.SendClientIDRequest();
        }
    }

    private void SendToLocalGameProxy(object obj)
    {
        GameProxy.ReceiveRequest((ClientRequestBase) obj);
    }

    private void SendFromLocalGameProxyToClient(ServerRequestBase request)
    {
        Proxy.Response(request);
    }

    public bool IsStandalone => Proxy.Socket == null;

    public bool IsLogin() //登录且未开始游戏，包括单机、在线状态
    {
        return (Proxy.ClientState == ProxyBase.ClientStates.Login || Proxy.ClientState == ProxyBase.ClientStates.Matching);
    }

    public bool IsMatching() //是否正在匹配，仅适用于在线
    {
        return !IsStandalone && Proxy.ClientState == ProxyBase.ClientStates.Matching;
    }

    public bool IsPlaying() //是否开始游戏，包括单机、在线状态
    {
        return Proxy.ClientState == ProxyBase.ClientStates.Playing;
    }
}