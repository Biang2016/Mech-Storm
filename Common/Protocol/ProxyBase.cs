using System.Net.Sockets;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public abstract class ProxyBase
{
    //发包读包基础
    public Socket Socket;
    public int ClientId;
    public BuildInfo CurrentBuildInfo;
    public DataHolder DataHolder = new DataHolder();
    public bool IsStopReceive;

    protected ClientStates clientState;

    public virtual ClientStates ClientState
    {
        get { return clientState; }
        set { clientState = value; }
    }

    protected ProxyBase(Socket socket, int clientId, bool isStopReceive)
    {
        Socket = socket;
        ClientId = clientId;
        IsStopReceive = isStopReceive;
    }


    protected abstract void Response();

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ClientStates
    {
        Offline,
        GetId,
        Login,
        Matching,
        Playing,
    }
}