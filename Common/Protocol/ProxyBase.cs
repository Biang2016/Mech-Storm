﻿using System.Net.Sockets;

public abstract class ProxyBase
{
    //发包读包基础
    public Socket Socket;
    public int ClientId;
    public DataHolder DataHolder = new DataHolder();
    public bool IsStopReceive;

    protected ClientStates clientState;

    public virtual ClientStates ClientState
    {
        get => clientState;
        set
        {
            clientState = value;
        }
    }

    protected ProxyBase(Socket socket, int clientId, bool isStopReceive)
    {
        Socket = socket;
        ClientId = clientId;
        IsStopReceive = isStopReceive;
    }


    protected abstract void Response();

    public enum ClientStates
    {
        Nothing = 0,
        GetId = 1,
        SubmitCardDeck = 2,
        Matching = 3,
        Playing = 4,
    }
}