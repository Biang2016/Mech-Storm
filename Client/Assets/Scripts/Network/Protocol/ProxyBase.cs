using System.Net.Sockets;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public abstract class ProxyBase
{
    //发包读包基础
    public Socket Socket;
    public int ClientID;
    public BuildInfo CurrentBuildInfo;
    public DataHolder DataHolder = new DataHolder();
    public bool IsStopReceive;

    protected ProxyBase(Socket socket, int clientID, bool isStopReceive)
    {
        Socket = socket;
        ClientID = clientID;
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