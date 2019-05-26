using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

public class Proxy : ProxyBase
{
    protected override void Response()
    {
    }

    public string Username;

    private ClientStates clientState;

    public ClientStates ClientState
    {
        get { return clientState; }
        set
        {
            if (clientState != value)
            {
                OnClientStateChange(value);
                clientState = value;
                ClientLog.Instance.PrintClientStates("Client states: " + ClientState);
            }
        }
    }

    public delegate void ClientStateEventHandler(ClientStates clientStates);

    public static ClientStateEventHandler OnClientStateChange;

    public Proxy(Socket socket, int clientId, bool isStopReceive) : base(socket, clientId, isStopReceive)
    {
    }

    public void CancelMatch()
    {
        CancelMatchRequest request = new CancelMatchRequest(ClientID);
        SendMessage(request);
        ClientState = ClientStates.Login;
    }

    public void LeaveGame()
    {
        LeaveGameRequest request = new LeaveGameRequest(ClientID);
        SendMessage(request);
        ClientState = ClientStates.Login;
    }

    public void OnSendBuildInfo(BuildInfo buildInfo)
    {
        BuildRequest req = new BuildRequest(ClientID, buildInfo, SelectBuildManager.Instance.CurrentGameMode == SelectBuildManager.GameMode.Single);
        SendMessage(req);
        ClientState = ClientStates.Login;
    }

    public void OnBeginMatch()
    {
        ClientRequestBase req = null;
        req = new MatchRequest(ClientID, SelectBuildManager.Instance.CurrentSelectedBuildInfo.BuildID);
        SendMessage(req);
        ClientState = ClientStates.Matching;
    }

    public void OnBeginSingleMode(int storyPaceID)
    {
        ClientState = ClientStates.Matching;
        ClientRequestBase req = new MatchStandaloneRequest(ClientID, SelectBuildManager.Instance.CurrentSelectedBuildInfo.BuildID, storyPaceID);
        SendMessage(req);
    }

    #region 收发基础组件

    private Queue<ClientRequestBase> SendRequestsQueue = new Queue<ClientRequestBase>();

    private ParameterizedThreadStart SendRequest;

    public void Send() // 每帧调用
    {
        if (SendRequestsQueue.Count > 0)
        {
            ClientRequestBase request = SendRequestsQueue.Dequeue();
            Thread thread = new Thread(SendRequest);
            thread.IsBackground = true;
            thread.Start(request);
        }
    }

    public delegate void SendMessageDelegate(ClientRequestBase request);

    public SendMessageDelegate SendMessage;

    public enum MessageTarget
    {
        LocalGameProxy,
        Server,
    }

    public void SwitchSendMessageTarget(MessageTarget messageTarget, ParameterizedThreadStart sendRequest)
    {
        SendRequest = sendRequest;
        if (messageTarget == MessageTarget.LocalGameProxy)
        {
            SendMessage = SendMessageToLocalGameProxy;
        }
        else if (messageTarget == MessageTarget.Server)
        {
            SendMessage = SendMessageToServer;
        }
    }

    private void SendMessageToServer(ClientRequestBase request)
    {
        SendRequestsQueue.Enqueue(request);
    }

    private void SendMessageToLocalGameProxy(ClientRequestBase request)
    {
        SendRequest(request);
    }

    public void Response(RequestBase r)
    {
        ClientLog.Instance.PrintReceive("Server: " + r.DeserializeLog());
        if (!(r is ResponseBundleBase))
        {
            switch (r.GetProtocol())
            {
                case NetProtocols.CLIENT_ID_REQUEST:
                {
                    ClientIdRequest request = (ClientIdRequest) r;
                    ClientID = request.givenClientId;
                    NetworkManager.ServerVersion = request.serverVersion;
                    if (NetworkManager.Instance.ClientInvalid)
                    {
                        UIManager.Instance.GetBaseUIForm<LoginPanel>().ShowUpdateConfirmPanel();
                    }
                    else
                    {
                        ClientState = ClientStates.GetId;
                        ClientVersionValidRequest validRequest = new ClientVersionValidRequest(ClientID);
                        SendMessage(validRequest);

                        if (Client.Instance.IsStandalone)
                        {
                            LoginRequest request1 = new LoginRequest(Client.Instance.Proxy.ClientID, "Player", "");
                            SendMessage(request1);
                        }
                    }

                    break;
                }
                case NetProtocols.REGISTER_RESULT_REQUEST:
                {
                    RegisterResultRequest request = (RegisterResultRequest) r;
                    if (request.isSuccess)
                    {
                        NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("Proxy_RegisterSuccess"), 0, 0.5f);
                    }
                    else
                    {
                        NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("Proxy_UserNameUsed"), 0, 0.5f);
                    }

                    break;
                }
                case NetProtocols.LOGIN_RESULT_REQUEST:
                {
                    LoginResultRequest request = (LoginResultRequest) r;
                    switch (request.stateCode)
                    {
                        case LoginResultRequest.StateCodes.Success:
                        {
                            Username = request.username;
                            ClientState = ClientStates.Login;
                            NoticeManager.Instance.ShowInfoPanelTop(LanguageManager.Instance.GetText("Proxy_LoginSuccess"), 0, 0.5f);
                            if (Client.Instance.IsStandalone)
                            {
                                UIManager.Instance.ShowUIForms<StartMenuPanel>().SetState(StartMenuPanel.States.Show_Main_Standalone);
                            }
                            else
                            {
                                UIManager.Instance.ShowUIForms<StartMenuPanel>().SetState(StartMenuPanel.States.Show_Main_Online);
                            }

                            break;
                        }
                        case LoginResultRequest.StateCodes.WrongPassword:
                        {
                            NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("Proxy_WrongPassword"), 0, 0.5f);
                            break;
                        }
                        case LoginResultRequest.StateCodes.UnexistedUser:
                        {
                            NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("Proxy_UserNameNotExisted"), 0, 0.5f);
                            break;
                        }
                        case LoginResultRequest.StateCodes.AlreadyOnline:
                        {
                            NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("Proxy_AlreadyLogin"), 0, 0.5f);
                            break;
                        }
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
                        NoticeManager.Instance.ShowInfoPanelTop(LanguageManager.Instance.GetText("Proxy_LogOutSuccess"), 0, 0.5f);
                        Client.Instance.Proxy.ClientState = ClientStates.GetId;
                        UIManager.Instance.CloseUIForm<StartMenuPanel>();
                        UIManager.Instance.CloseUIForm<SelectBuildPanel>();
                        UIManager.Instance.ShowUIForms<LoginPanel>();
                    }
                    else
                    {
                        NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("Proxy_LogOutFailed"), 0, 0.5f);
                    }

                    break;
                }
                case NetProtocols.START_NEW_STORY_REQUEST_RESPONSE:
                {
                    StartNewStoryRequestResponse request = (StartNewStoryRequestResponse) r;
                    StoryManager.Instance.InitializeStory(request.Story);
                    SelectBuildManager.Instance.SwitchGameMode(SelectBuildManager.GameMode.Single, true);
                    UIManager.Instance.GetBaseUIForm<StartMenuPanel>().SetState(StartMenuPanel.States.Show_Single_HasStory);
                    AudioManager.Instance.SoundPlay("sfx/OnStoryStart");
                    break;
                }
                case NetProtocols.CREATE_BUILD_REQUEST_RESPONSE:
                {
                    CreateBuildRequestResponse request = (CreateBuildRequestResponse) r;
                    SelectBuildManager.Instance.OnCreateNewBuildResponse(request.buildInfo);
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
                    OnlineManager.Instance.OnlineBuildInfos = request.OnlineBuildInfos;
                    OnlineManager.Instance.OnlineGamePlaySettings = request.OnlineGamePlaySettings;
                    StoryManager.Instance.InitializeStory(request.Story);
                    break;
                }
                case NetProtocols.BEAT_ENEMY_REQUSET:
                {
                    BeatEnemyRequest request = (BeatEnemyRequest) r;
                    StoryManager.Instance.SetStoryPaceBeated(request.LevelID);
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
                case NetProtocols.GAME_STOP_BY_SERVER_ERROR_REQUEST:
                {
                    RoundManager.Instance.OnGameStopByServerError((GameStopByServerErrorRequest) r);
                    break;
                }
            }
        }
        else // 战斗内行为全部交由RoundManager处理
        {
            ResponseBundleBase request = (ResponseBundleBase) r;
            if (request is GameStart_ResponseBundle)
            {
                RoundManager.Instance.Initialize();
            }

            if (request is EndRoundRequest_ResponseBundle)
            {
                if (ClientID == ((EndRoundRequest_ResponseBundle) request).ClientID)
                {
                    RoundManager.Instance.EndRound();
                }
            }

            foreach (ServerRequestBase attachedRequest in request.AttachedRequests) //请求预处理，提取关键信息，如机甲死亡、弃牌等会影响客户端交互的信息
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