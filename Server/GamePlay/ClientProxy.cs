using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

internal class ClientProxy : ProxyBase
{
    private Queue<ServerRequestBase> SendRequestsQueue = new Queue<ServerRequestBase>();
    private Queue<ClientRequestBase> ReceiveRequestsQueue = new Queue<ClientRequestBase>();

    public ServerGameManager MyServerGameManager;

    private string username = "";

    public string UserName
    {
        get { return username; }
    }


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

    public ClientProxy(Socket socket, int clientId, bool isStopReceive) : base(socket, clientId, isStopReceive)
    {
        ClientIdRequest request = new ClientIdRequest(clientId);
        ClientState = ClientStates.GetId;
        SendMessage(request);
    }

    public List<BuildInfo> GetClientBuildInfos(string username)
    {
        return new List<BuildInfo>();
    }

    private bool isClosed = false;

    public void OnClose()
    {
        if (isClosed) return;

        MyServerGameManager?.OnLeaveGame(ClientId); //先结束对应的游戏

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
                    if (ClientState != ClientStates.GetId)
                    {
                        Server.SV.SGMM.RemoveGame(this);
                        ClientState = ClientStates.GetId;
                    }

                    if (ClientState == ClientStates.GetId)
                    {
                        RegisterRequest request = (RegisterRequest) r;

                        bool suc = Database.Instance.AddUser(request.username, request.password);
                        RegisterResultRequest response = new RegisterResultRequest(suc);

                        SendMessage(response);
                    }

                    break;
                case LoginRequest _:
                    ServerLog.PrintClientStates("客户 " + ClientId + " 状态: " + ClientState);
                    if (ClientState != ClientStates.GetId)
                    {
                        Server.SV.SGMM.RemoveGame(this);
                        ClientState = ClientStates.GetId;
                    }

                    if (ClientState == ClientStates.GetId)
                    {
                        LoginRequest request = (LoginRequest) r;
                        LoginResultRequest response;

                        string password = Database.Instance.GetUserPasswordByUsername(request.username);
                        if (password != null)
                        {
                            if (password == request.password)
                            {
                                bool suc = Database.Instance.AddLoginUser(ClientId, request.username);
                                if (suc)
                                {
                                    response = new LoginResultRequest(request.username, LoginResultRequest.StateCodes.Success);
                                    SendMessage(response);
                                    ClientState = ClientStates.Login;
                                    username = request.username;
                                    ClientBuildInfosRequest request1 = new ClientBuildInfosRequest(Database.Instance.GetPlayerBuilds(username));
                                    SendMessage(request1);
                                }
                                else
                                {
                                    response = new LoginResultRequest(request.username, LoginResultRequest.StateCodes.AlreadyOnline);
                                    SendMessage(response);
                                }
                            }
                            else
                            {
                                response = new LoginResultRequest(request.username, LoginResultRequest.StateCodes.WrongPassword);
                                SendMessage(response);
                            }
                        }
                        else
                        {
                            response = new LoginResultRequest(request.username, LoginResultRequest.StateCodes.UnexistedUser);
                            SendMessage(response);
                        }
                    }

                    break;
                case LogoutRequest _:
                {
                    ServerLog.PrintClientStates("客户 " + ClientId + " 状态: " + ClientState);
                    LogoutRequest request = (LogoutRequest) r;
                    LogoutResultRequest response;
                    if (ClientState != ClientStates.GetId)
                    {
                        Server.SV.SGMM.RemoveGame(this);
                        bool suc = Database.Instance.RemoveLoginUser(ClientId);
                        ClientState = ClientStates.GetId;
                        response = new LogoutResultRequest(request.username, suc);
                    }
                    else
                    {
                        response = new LogoutResultRequest(request.username, false);
                    }

                    SendMessage(response);
                    break;
                }

                case BuildRequest _:
                {
                    BuildRequest request = (BuildRequest) r;
                    if (request.BuildInfo.BuildID == -1)
                    {
                        request.BuildInfo.BuildID = Database.Instance.GenerateBuildID();
                        CreateBuildRequestResponse response = new CreateBuildRequestResponse(request.BuildInfo.BuildID);
                        SendMessage(response);
                    }
                    else
                    {
                        BuildUpdateRequest response = new BuildUpdateRequest(request.BuildInfo);
                        SendMessage(response);
                    }

                    string username = Database.Instance.GetUsernameByClientId(ClientId);
                    if (username != null)
                    {
                        Database.Instance.AddOrModifyBuild(username, request.BuildInfo);
                    }
                    else
                    {
                        ServerLog.PrintError("服务器上不存在该登录用户：[ClientID]=" + ClientId);
                    }

                    break;
                }

                case DeleteBuildRequest _:
                {
                    DeleteBuildRequest request = (DeleteBuildRequest) r;
                    Database.Instance.DeleteBuild(username, request.buildID);
                    DeleteBuildRequestResponse response = new DeleteBuildRequestResponse(request.buildID);
                    SendMessage(response);
                    break;
                }

                case MatchRequest _:
                    ServerLog.PrintClientStates("客户 " + ClientId + " 状态: " + ClientState);
                    if (ClientState == ClientStates.Playing)
                    {
                        Server.SV.SGMM.RemoveGame(this);
                        ClientState = ClientStates.Login;
                    }

                    if (ClientState == ClientStates.Login)
                    {
                        MatchRequest request = (MatchRequest) r;
                        CurrentBuildInfo = Database.Instance.GetBuildInfoByID(request.buildID);
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
                        ClientState = ClientStates.Login;
                        Server.SV.SGMM.OnClientCancelMatch(this);
                    }

                    break;
            }

            if (ClientState == ClientStates.Playing)
            {
                if (MyServerGameManager.IsStopped)
                {
                    MyServerGameManager.StopGame();
                    return;
                }
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
                    case EquipPackRequest _:
                        MyServerGameManager?.OnClientEquipPackRequest((EquipPackRequest) r);
                        break;
                    case EquipMARequest _:
                        MyServerGameManager?.OnClientEquipMARequest((EquipMARequest) r);
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
                    case RetinueAttackShipRequest _:
                        MyServerGameManager?.OnClientRetinueAttackShipRequest((RetinueAttackShipRequest) r);
                        break;
                }
            }
        }
    }
}