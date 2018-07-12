using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

internal class ClientProxy : ProxyBase
{
    public Queue<ServerRequestBase> SendRequestsQueue = new Queue<ServerRequestBase>();
    public Queue<ClientRequestBase> ReceiveRequestsQueue = new Queue<ClientRequestBase>();

    public ServerGameManager MyServerGameManager;
    public ServerCardDeckManager MyServerCardDeckManager;

    public CardDeckInfo CardDeckInfo;

    public ServerPlayer MyServerPlayer;

    public ClientProxy(Socket socket, int clientId, bool isStopReceive) : base(socket, clientId, isStopReceive)
    {
        ClientIdRequest request = new ClientIdRequest(clientId);
        SendRequestsQueue.Enqueue(request);
        ClientState = ClientStates.GetId;
        Thread sendMessageThread = new Thread(SendMessage);
        sendMessageThread.IsBackground = true;
        sendMessageThread.Start();
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

    /// <summary>
    /// 此类中处理进入游戏前的所有Request
    /// 进入游戏后将所有Request发给ServerGameManager处理
    /// </summary>
    public override void Response()
    {
        while (ReceiveRequestsQueue.Count > 0)
        {
            ClientRequestBase r = ReceiveRequestsQueue.Dequeue();
            //以下是进入游戏前的请求
            if (r is CardDeckRequest)
            {
                if (ClientState == ClientStates.GetId)
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
            else if (r is ResetClientRequest)
            {

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
                    MyServerGameManager.OnClientSummonRetinueRequest((SummonRetinueRequest)r);
                }
            }
        }
    }
}