using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

internal class Proxy : ProxyBase
{
    private Queue<ClientRequestBase> SendRequestsQueue = new Queue<ClientRequestBase>();

    protected override void Response()
    {
    }

    public string Username;

    public override ClientStates ClientState
    {
        get { return clientState; }
        set
        {
            if (clientState != value)
            {
                OnClientStateChange(value);
            }

            clientState = value;
            ClientLog.Instance.PrintClientStates("Client states: " + ClientState);
        }
    }

    public delegate void ClientStateEventHandler(ClientStates clientStates);

    public static ClientStateEventHandler OnClientStateChange;

    public Proxy(Socket socket, int clientId, int clientMoney, bool isStopReceive) : base(socket, clientId, isStopReceive)
    {
    }

    public void CancelMatch()
    {
        CancelMatchRequest request = new CancelMatchRequest(ClientId);
        SendMessage(request);
        ClientState = ClientStates.Login;
    }

    public void LeaveGame()
    {
        LeaveGameRequest request = new LeaveGameRequest();
        SendMessage(request);
        ClientState = ClientStates.Login;
    }

    public void OnSendBuildInfo(BuildInfo buildInfo)
    {
        BuildRequest req = new BuildRequest(ClientId, buildInfo);
        SendMessage(req);
        ClientState = ClientStates.Login;
    }

    public void OnBeginMatch()
    {
        MatchRequest req = new MatchRequest(ClientId, SelectBuildManager.Instance.CurrentSelectedBuildButton.BuildInfo.BuildID);
        SendMessage(req);
        ClientState = ClientStates.Matching;
    }

    #region 收发基础组件

    public void Send() //每帧调用
    {
        if (SendRequestsQueue.Count > 0)
        {
            ClientRequestBase request = SendRequestsQueue.Dequeue();
            Thread thread = new Thread(Client.Instance.Send);
            thread.IsBackground = true;
            thread.Start(request);
        }
    }

    public void SendMessage(ClientRequestBase request)
    {
        SendRequestsQueue.Enqueue(request);
    }

    public void Response(Socket socket, RequestBase r)
    {
        ClientLog.Instance.PrintReceive("Server: " + r.DeserializeLog());
        if (!(r is ResponseBundleBase))
        {
            switch (r.GetProtocol())
            {
                case NetProtocols.CLIENT_ID_REQUEST:
                {
                    ClientIdRequest request = (ClientIdRequest) r;
                    ClientId = request.givenClientId;
                    ClientState = ClientStates.GetId;
                    break;
                }
                case NetProtocols.REGISTER_RESULT_REQUEST:
                {
                    RegisterResultRequest request = (RegisterResultRequest) r;
                    if (request.isSuccess)
                    {
                        NoticeManager.Instance.ShowInfoPanelCenter("注册成功", 0, 0.5f);
                    }
                    else
                    {
                        NoticeManager.Instance.ShowInfoPanelCenter("该用户名已被注册", 0, 0.5f);
                    }

                    break;
                }
                case NetProtocols.LOGIN_RESULT_REQUEST:
                {
                    LoginResultRequest request = (LoginResultRequest) r;
                    if (request.isSuccess)
                    {
                        Username = request.username;
                        ClientState = ClientStates.Login;
                        NoticeManager.Instance.ShowInfoPanelTop("登录成功", 0, 0.5f);
                        LoginManager.Instance.M_StateMachine.SetState(LoginManager.StateMachine.States.Hide);
                        StartMenuManager.Instance.M_StateMachine.SetState(StartMenuManager.StateMachine.States.Show);
                    }
                    else
                    {
                        NoticeManager.Instance.ShowInfoPanelCenter("登录失败，请检查用户名或密码", 0, 0.5f);
                    }

                    break;
                }
                case NetProtocols.LOGOUT_RESULT_REQUEST:
                {
                    LogoutResultRequest request = (LogoutResultRequest) r;
                    if (request.isSuccess)
                    {
                        Username = "";
                        ClientState = ClientStates.GetId;
                        NoticeManager.Instance.ShowInfoPanelTop("退出成功", 0, 0.5f);
                        Client.Instance.Proxy.ClientState = ClientStates.GetId;
                        StartMenuManager.Instance.M_StateMachine.SetState(StartMenuManager.StateMachine.States.Hide);
                        LoginManager.Instance.M_StateMachine.SetState(LoginManager.StateMachine.States.Show);
                    }
                    else
                    {
                        NoticeManager.Instance.ShowInfoPanelCenter("退出失败", 0, 0.5f);
                    }

                    break;
                }
                case NetProtocols.CREATE_BUILD_REQUEST_RESPONSE:
                {
                    CreateBuildRequestResponse request = (CreateBuildRequestResponse) r;
                    SelectBuildManager.Instance.OnCreateNewBuildResponse(request.buildId);
                    break;
                }
                case NetProtocols.BUILD_UPDATE_RESPONSE:
                {
                    BuildUpdateRequest request = (BuildUpdateRequest) r;
                    SelectBuildManager.Instance.RefreshSomeBuild(request.BuildInfo);
                    break;
                }
                case NetProtocols.DELETE_BUILD_REQUEST_RESPONSE:
                {
                    DeleteBuildRequestResponse request = (DeleteBuildRequestResponse) r;
                    SelectBuildManager.Instance.OnDeleteBuildResponse(request.buildID);
                    break;
                }
                case NetProtocols.CLIENT_BUILDINFOS_REQUEST:
                {
                    ClientBuildInfosRequest request = (ClientBuildInfosRequest) r;
                    SelectBuildManager.Instance.InitAllMyBuildInfos(request.buildInfos);
                    break;
                }
                case NetProtocols.GAME_STOP_BY_LEAVE_REQUEST:
                {
                    GameStopByLeaveRequest request = (GameStopByLeaveRequest) r;
                    RoundManager.Instance.OnGameStopByLeave(request);
                    break;
                }
                case NetProtocols.RANDOM_NUMBER_SEED_REQUEST:
                {
                    RoundManager.Instance.OnRandomNumberSeed((RandomNumberSeedRequest) r);
                    break;
                }
            }
        }
        else
        {
            ResponseBundleBase request = (ResponseBundleBase) r;
            foreach (ServerRequestBase attachedRequest in request.AttachedRequests) //请求预处理，提取关键信息，如随从死亡、弃牌等会影响客户端交互的信息
            {
                RoundManager.Instance.ResponseToSideEffects_PrePass(attachedRequest);
            }

            foreach (ServerRequestBase attachedRequest in request.AttachedRequests)
            {
                RoundManager.Instance.ResponseToSideEffects(attachedRequest);
            }
        }
    }

    #endregion
}