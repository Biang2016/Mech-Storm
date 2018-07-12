using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

internal class ClientProxy : ProxyBase
{
    private Queue<ServerRequestBase> SendRequestsQueue = new Queue<ServerRequestBase>();
    private Queue<ClientRequestBase> ReceiveRequestsQueue = new Queue<ClientRequestBase>();

    public ServerGameManager MyServerGameManager;

    public CardDeckInfo CardDeckInfo;

    public ServerPlayer MyServerPlayer;

    public override ClientStates ClientState
    {
        get { return clientState; }
        set { clientState = value; }
    }

    public ClientProxy(Socket socket, int clientId, bool isStopReceive) : base(socket, clientId, isStopReceive)
    {
        ClientIdRequest request = new ClientIdRequest(clientId);
        SendMessage(request);
        ClientState = ClientStates.GetId;
        Thread sendMessageThread = new Thread(SendMessage);
        sendMessageThread.IsBackground = true;
        sendMessageThread.Start();
    }

    private bool isClosed = false;

    public void OnClose()
    {
        if (isClosed) return;
        isClosed = true;
        Server.SV.ClientsDict.Remove(ClientId);
        if (Socket != null)
        {
            Socket.Close();
        }

        MyServerPlayer = null;
        if (MyServerGameManager != null)
        {
            MyServerGameManager.OnStopGame(this);
        }

        MyServerGameManager = null;
        SendRequestsQueue.Clear();
        ReceiveRequestsQueue.Clear();
    }

    public void SendMessage(ServerRequestBase request)
    {
        if (isClosed) return;
        SendRequestsQueue.Enqueue(request);
    }

    private void SendMessage()
    {
        while (true)
        {
            if (SendRequestsQueue.Count > 0)
            {
                lock (SendRequestsQueue)
                {
                    if (SendRequestsQueue.Count > 0)
                    {
                        Thread thread = new Thread(Server.SV.DoSendToClient);
                        SendMsg msg = new SendMsg(Socket, SendRequestsQueue.Dequeue());
                        thread.IsBackground = true;
                        thread.Start(msg);
                    }
                }
            }
        }
    }

    public void ReceiveMessage(ClientRequestBase request)
    {
        if (isClosed) return;
        ReceiveRequestsQueue.Enqueue(request);
        Response();
    }

    /// <summary>
    /// 此类中处理进入游戏前的所有Request
    /// 进入游戏后将所有Request发给ServerGameManager处理
    /// </summary>
    protected override void Response()
    {
        while (ReceiveRequestsQueue.Count > 0)
        {
            ClientRequestBase r = ReceiveRequestsQueue.Dequeue();
            //以下是进入游戏前的请求
            if (r is CardDeckRequest)
            {
                if (ClientState == ClientStates.GetId || ClientState == ClientStates.SubmitCardDeck)
                {
                    CardDeckRequest request = (CardDeckRequest) r;
                    CardDeckInfo = request.cardDeckInfo;
                    ClientState = ClientStates.SubmitCardDeck;
                }
            }
            else if (r is MatchRequest)
            {
                if (ClientState == ClientStates.SubmitCardDeck)
                {
                    ClientState = ClientStates.Matching;
                    Server.SV.SGMM.OnClientMatchGames(this);
                }
            }
            else if (r is CancelMatchRequest)
            {
                if (ClientState == ClientStates.Matching)
                {
                    ClientState = ClientStates.SubmitCardDeck;
                    Server.SV.SGMM.OnClientCancelMatch(this);
                }
            }

            //以下是进入游戏后的请求
            else if (r is ClientEndRoundRequest)
            {
                if (ClientState == ClientStates.Playing)
                {
                    MyServerGameManager.OnEndRoundRequest((ClientEndRoundRequest) r);
                }
            }
            else if (r is SummonRetinueRequest)
            {
                if (ClientState == ClientStates.Playing)
                {
                    MyServerGameManager.OnClientSummonRetinueRequest((SummonRetinueRequest) r);
                }
            }
            else if (r is LeaveGameRequest)
            {
                if (ClientState == ClientStates.Playing)
                {
                    if (MyServerGameManager != null) MyServerGameManager.OnStopGame(this);
                }
            }
        }
    }
}