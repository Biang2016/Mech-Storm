using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public abstract class ProxyBase
{
    //发包读包基础
    public Socket Socket;
    public int ClientId;
    public DataHolder DataHolder = new DataHolder();
    public bool IsStopReceive;

    public ClientStates ClientState;

    protected ProxyBase(Socket socket, int clientId, bool isStopReceive)
    {
        Socket = socket;
        ClientId = clientId;
        IsStopReceive = isStopReceive;
    }

    public abstract void Response();

    public enum ClientStates
    {
        GetId = 0,
        SubmitCardDeck = 1,
        Matching = 2,
        Playing = 3,
    }
}