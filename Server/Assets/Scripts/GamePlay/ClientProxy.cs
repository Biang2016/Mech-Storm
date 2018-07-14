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

    public override ClientStates ClientState
    {
        get => clientState;
        set => clientState = value;
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

        MyServerGameManager?.OnStopGame(this);//先结束对应的游戏

        if (Socket != null)
        {
            if (Socket.Connected) Socket.Shutdown(SocketShutdown.Both);
            Socket.Close();
        }

        SendRequestsQueue.Clear();
        ReceiveRequestsQueue.Clear();

        isClosed = true;
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
            switch (r)
            {
                //以下是进入游戏前的请求
                case CardDeckRequest _:
                    if (ClientState == ClientStates.GetId || ClientState == ClientStates.SubmitCardDeck)
                    {
                        CardDeckRequest request = (CardDeckRequest) r;
                        CardDeckInfo = request.cardDeckInfo;
                        ClientState = ClientStates.SubmitCardDeck;
                    }

                    break;
                case MatchRequest _:
                    if (ClientState == ClientStates.SubmitCardDeck)
                    {
                        ClientState = ClientStates.Matching;
                        Server.SV.SGMM.OnClientMatchGames(this);
                    }

                    break;
                case CancelMatchRequest _:
                    if (ClientState == ClientStates.Matching)
                    {
                        ClientState = ClientStates.SubmitCardDeck;
                        Server.SV.SGMM.OnClientCancelMatch(this);
                    }

                    break;
            }

            if (ClientState == ClientStates.Playing)
            {
                switch (r)
                {
                    case EndRoundRequest _:
                        MyServerGameManager?.OnEndRoundRequest((EndRoundRequest) r);
                        break;
                    case SummonRetinueRequest _:
                        MyServerGameManager?.OnClientSummonRetinueRequest((SummonRetinueRequest) r);
                        break;
                    case EquipWeaponRequest _:
                        MyServerGameManager?.OnClientEquipWeaponRequest((EquipWeaponRequest) r);
                        break;
                    case EquipShieldRequest _:
                        MyServerGameManager?.OnClientEquipShieldRequest((EquipShieldRequest) r);
                        break;
                    case LeaveGameRequest _: //正常退出游戏请求
                        MyServerGameManager?.OnStopGame(this);
                        break;
                    case RetinueAttackRetinueRequest _:
                        MyServerGameManager?.OnClientRetinueAttackRetinueRequest((RetinueAttackRetinueRequest) r);
                        break;
                }
            }
        }
    }
}