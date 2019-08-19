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

    void OnDestroy()
    {
        Proxy.OnClientStateChange = null;
    }

#if UNITY_EDITOR
    private static bool LocalSerializeRequest = true;
#else
    private static bool LocalSerializeRequest = true;
#endif

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
            if (LocalSerializeRequest)
            {
                ClientLog.Instance.Print("LocalSerializeRequest On.");
                Proxy.SwitchSendMessageTarget(Proxy.MessageTarget.LocalGameProxy, SendToLocalGameProxyWithSerialization);
                GameProxy = new GameProxy(999, "Player1", NetworkManager.ClientVersion, SendFromLocalGameProxyToClientWithSerialization, ClientLog.Instance);
            }
            else
            {
                Proxy.SwitchSendMessageTarget(Proxy.MessageTarget.LocalGameProxy, SendToLocalGameProxy);
                GameProxy = new GameProxy(999, "Player1", NetworkManager.ClientVersion, SendFromLocalGameProxyToClient, ClientLog.Instance);
            }

            GameProxy.SendClientIDRequest();
        }
    }

    private void SendToLocalGameProxy(object obj)
    {
        GameProxy.ReceiveRequest((ClientRequestBase) obj);
    }

    private void SendToLocalGameProxyWithSerialization(object obj)
    {
        byte[] data = NetworkManager.SerializeRequest((ClientRequestBase) obj);
        DataStream stream = new DataStream(data, true);
        ProtoManager.TryLocalDeserialize(stream, GameProxy.ReceiveRequest);
    }

    private void SendFromLocalGameProxyToClientWithSerialization(ServerRequestBase request)
    {
        byte[] data = NetworkManager.SerializeRequest(request);
        DataStream stream = new DataStream(data, true);
        ProtoManager.TryLocalDeserialize(stream, Proxy.Response);
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