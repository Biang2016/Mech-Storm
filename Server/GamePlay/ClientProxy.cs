using System;
using System.Collections.Generic;
using System.Net.Sockets;
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
        set
        {
            ClientStates before = ClientState;
            clientState = value;
            ServerLog.PrintClientStates("客户 " + ClientId + " 状态切换: " + before + " -> " + ClientState);
        }
    }

    public ClientProxy(Socket socket, int clientId, int clientMoney, bool isStopReceive) : base(socket, clientId, clientMoney, isStopReceive)
    {
    }

    private bool isClosed = false;

    public void OnClose()
    {
        if (isClosed) return;

        MyServerGameManager?.OnStopGame(this); //先结束对应的游戏

        if (Socket != null)
        {
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
        OnSendMessage();
    }

    private void OnSendMessage()
    {
        Thread sendMessageThread = new Thread(SendMessage);
        sendMessageThread.IsBackground = true;
        sendMessageThread.Start();
    }

    private void SendMessage()
    {
        lock (SendRequestsQueue)
        {
            if (SendRequestsQueue.Count > 0)
            {
                try
                {
                    Thread thread = new Thread(Server.SV.DoSendToClient);
                    SendMsg msg = new SendMsg(Socket, SendRequestsQueue.Dequeue(), ClientId);
                    thread.IsBackground = true;
                    thread.Start(msg);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
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

    public ResponseBundleBase CurrentClientRequestResponseBundle; //目前正在生成的响应包

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
                case RegisterRequest _:
                    ServerLog.PrintClientStates("客户 " + ClientId + " 状态: " + ClientState);
                    if (ClientState != ClientStates.Nothing)
                    {
                        Server.SV.SGMM.RemoveGame(this);
                        ClientState = ClientStates.Nothing;
                    }

                    if (ClientState == ClientStates.Nothing)
                    {
                        RegisterRequest request = (RegisterRequest) r;
                        RegisterResultRequest response;
                        if (Server.SV.UserTable.ContainsKey(request.username))
                        {
                            response = new RegisterResultRequest(ClientId, false);
                        }
                        else
                        {
                            Server.SV.UserTable.Add(request.username, request.password);
                            response = new RegisterResultRequest(ClientId, true);
                        }

                        SendMessage(response);
                    }

                    break;
                case LoginRequest _:
                    ServerLog.PrintClientStates("客户 " + ClientId + " 状态: " + ClientState);
                    if (ClientState != ClientStates.Nothing)
                    {
                        Server.SV.SGMM.RemoveGame(this);
                        ClientState = ClientStates.Nothing;
                    }

                    if (ClientState == ClientStates.Nothing)
                    {
                        LoginRequest request = (LoginRequest) r;
                        LoginResultRequest response;
                        if (Server.SV.UserTable.ContainsKey(request.username))
                        {
                            if (Server.SV.UserTable[request.username] == request.password)
                            {
                                response = new LoginResultRequest(request.username, ClientId, true);
                                if (!Server.SV.LoginUserTable.ContainsKey(request.username))
                                {
                                    Server.SV.LoginUserTable.Add(request.username, request.password);
                                }
                                ClientState = ClientStates.Login;
                            }
                            else
                            {
                                response = new LoginResultRequest(request.username, ClientId, false);
                            }
                        }
                        else
                        {
                            response = new LoginResultRequest(request.username, ClientId, false);
                        }

                        SendMessage(response);
                    }

                    break;
                case CardDeckRequest _:
                    ServerLog.PrintClientStates("客户 " + ClientId + " 状态: " + ClientState);
                    if (ClientState == ClientStates.Playing)
                    {
                        Server.SV.SGMM.RemoveGame(this);
                        ClientState = ClientStates.Login;
                    }

                    if (ClientState == ClientStates.Login || ClientState == ClientStates.SubmitCardDeck)
                    {
                        CardDeckRequest request = (CardDeckRequest) r;
                        CardDeckInfo = request.cardDeckInfo;
                        ClientState = ClientStates.SubmitCardDeck;
                    }

                    break;
                case MatchRequest _:
                    ServerLog.PrintClientStates("客户 " + ClientId + " 状态: " + ClientState);
                    if (ClientState == ClientStates.Playing)
                    {
                        Server.SV.SGMM.RemoveGame(this);
                        ClientState = ClientStates.SubmitCardDeck;
                    }

                    if (ClientState == ClientStates.SubmitCardDeck)
                    {
                        ClientState = ClientStates.Matching;
                        Server.SV.SGMM.OnClientMatchGames(this);
                    }

                    break;
                case CancelMatchRequest _:
                    ServerLog.PrintClientStates("客户 " + ClientId + " 状态: " + ClientState);
                    if (ClientState == ClientStates.Playing)
                    {
                        Server.SV.SGMM.RemoveGame(this);
                        ClientState = ClientStates.Matching;
                    }

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
                    case UseSpellCardRequest _:
                        MyServerGameManager?.OnClientUseSpellCardRequest((UseSpellCardRequest) r);
                        break;
                    case LeaveGameRequest _: //正常退出游戏请求
                        MyServerGameManager?.OnLeaveGameRequest((LeaveGameRequest) r);
                        break;
                    case RetinueAttackRetinueRequest _:
                        MyServerGameManager?.OnClientRetinueAttackRetinueRequest((RetinueAttackRetinueRequest) r);
                        break;
                }
            }
        }
    }
}