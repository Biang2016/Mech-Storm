using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

internal class Proxy : ProxyBase
{
    private Queue<ClientRequestBase> SendRequestsQueue = new Queue<ClientRequestBase>();
    private Queue<ServerRequestBase> ReceiveRequestsQueue = new Queue<ServerRequestBase>();

    public override ClientStates ClientState
    {
        get { return clientState; }
        set
        {
            clientState = value;
            ClientLog.CL.PrintClientStates("Client states: " + ClientState);
        }
    }

    public Proxy(Socket socket, int clientId, bool isStopReceive) : base(socket, clientId, isStopReceive)
    {
    }

    #region 收发基础组件

    public void Send() //每帧调用
    {
        if (SendRequestsQueue.Count > 0)
        {
            ClientRequestBase request = SendRequestsQueue.Dequeue();
            Thread thread = new Thread(Client.CS.Send);
            thread.IsBackground = true;
            thread.Start(request);
        }
    }

    public void SendMessage(ClientRequestBase request)
    {
        SendRequestsQueue.Enqueue(request);
    }

    public void ReceiveMessage(ServerRequestBase request)
    {
        ReceiveRequestsQueue.Enqueue(request);
        Response();
    }

    #endregion

    protected override void Response()
    {
        ServerRequestBase r = ReceiveRequestsQueue.Dequeue();
        if (r is ClientIdRequest)
        {
            ClientIdRequest request = (ClientIdRequest) r;
            ClientId = request.givenClientId;
            ClientState = ClientStates.GetId;
        }
        else if (r is PlayerRequest)
        {
            ClientState = ClientStates.Playing;
            RoundManager.RM.Initialize();
            RoundManager.RM.InitializePlayers((PlayerRequest) r);
        }
        else if (r is PlayerTurnRequest)
        {
            RoundManager.RM.SetPlayerTurn((PlayerTurnRequest) r);
        }
        else if (r is PlayerCostRequest)
        {
            RoundManager.RM.SetPlayersCost((PlayerCostRequest) r);
        }
        else if (r is DrawCardRequest)
        {
            RoundManager.RM.OnPlayerDrawCard((DrawCardRequest) r);
        }
        else if (r is SummonRetinueRequest_Response)
        {
            RoundManager.RM.OnPlayerSummonRetinue((SummonRetinueRequest_Response) r);
        }

        if (r is GameStopByLeaveRequest)
        {
            RoundManager.RM.OnGameStop();
        }
    }

    public void CancelMatch()
    {
        CancelMatchRequest request = new CancelMatchRequest(ClientId);
        SendMessage(request);
        ClientState = ClientStates.SubmitCardDeck;
    }

    public void LeaveGame()
    {
        LeaveGameRequest request = new LeaveGameRequest();
        SendMessage(request);
        ClientState = ClientStates.SubmitCardDeck;
        //...
    }


    public void OnSendCardDeck(CardDeckInfo cardDeckInfo)
    {
        CardDeckRequest req = new CardDeckRequest(ClientId, cardDeckInfo);
        SendMessage(req);
        ClientState = ClientStates.SubmitCardDeck;
    }

    public void OnBeginMatch()
    {
        MatchRequest req = new MatchRequest(ClientId);
        SendMessage(req);
        ClientState = ClientStates.Matching;
    }
}