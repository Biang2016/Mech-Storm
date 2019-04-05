using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

public class Proxy : ProxyBase
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
                clientState = value;
                ClientLog.Instance.PrintClientStates("Client states: " + ClientState);
            }
        }
    }

    public delegate void ClientStateEventHandler(ClientStates clientStates);

    public static ClientStateEventHandler OnClientStateChange;

    public bool IsSuperAccount = false;

    public Proxy(Socket socket, int clientId, bool isStopReceive) : base(socket, clientId, isStopReceive)
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
        LeaveGameRequest request = new LeaveGameRequest(ClientId);
        SendMessage(request);
        ClientState = ClientStates.Login;
    }

    public void OnSendBuildInfo(BuildInfo buildInfo)
    {
        BuildRequest req = new BuildRequest(ClientId, buildInfo, SelectBuildManager.Instance.GameMode_State == SelectBuildManager.GameMode.Single);
        SendMessage(req);
        ClientState = ClientStates.Login;
    }

    public void OnBeginMatch()
    {
        ClientRequestBase req = null;
        req = new MatchRequest(ClientId, SelectBuildManager.Instance.CurrentSelectedBuildButton.BuildInfo.BuildID);
        SendMessage(req);
        ClientState = ClientStates.Matching;
    }

    public void OnBeginSingleMode(int storyPaceID)
    {
        ClientRequestBase req = null;
        req = new MatchStandAloneRequest(ClientId, SelectBuildManager.Instance.CurrentSelectedBuildButton.BuildInfo.BuildID, storyPaceID);
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
                    Client.ServerVersion = request.serverVersion;
                    if (Client.Instance.ClientInvalid)
                    {
                        UIManager.Instance.GetBaseUIForm<LoginPanel>().ShowUpdateConfirmPanel();
                    }
                    else
                    {
                        ClientVersionValidRequest validRequest = new ClientVersionValidRequest(ClientId);
                        SendMessage(validRequest);
                        ClientState = ClientStates.GetId;
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
                            IsSuperAccount = Username == "ServerAdmin" || Username == "StoryAdmin";
                            ClientState = ClientStates.Login;
                            NoticeManager.Instance.ShowInfoPanelTop(LanguageManager.Instance.GetText("Proxy_LoginSuccess"), 0, 0.5f);
                            UIManager.Instance.CloseUIForms<LoginPanel>();
                            UIManager.Instance.ShowUIForms<StartMenuPanel>().SetState(StartMenuPanel.States.Show_Main);
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
                        UIManager.Instance.CloseUIForms<StartMenuPanel>();
                        SelectBuildManager.Instance.M_StateMachine.SetState(SelectBuildManager.StateMachine.States.Hide);
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
                    //StoryPanel.Instance.InitiateStoryCanvas(request.Story);
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
                    SelectBuildManager.Instance.M_CurrentOnlineCompete = new SelectBuildManager.OnlineCompete();
                    SelectBuildManager.Instance.M_CurrentOnlineCompete.OnlineGamePlaySettings = request.OnlineGamePlaySettings;
                    SelectBuildManager.Instance.M_CurrentOnlineCompete.OnlineBuildInfos = request.OnlineBuildInfos;
                    StoryManager.Instance.InitializeStory(request.Story);
                    break;
                }
                case NetProtocols.BEAT_ENEMY_REQUSET:
                {
                    BeatEnemyRequest request = (BeatEnemyRequest) r;
                    StoryManager.Instance.SetStoryPaceBeated(request.StoryPaceID);
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
        else
        {
            ResponseBundleBase request = (ResponseBundleBase) r;
            if (request is GameStart_ResponseBundle)
            {
                RoundManager.Instance.Initialize();
            }

            if (request is EndRoundRequest_ResponseBundle)
            {
                if (ClientId == ((EndRoundRequest_ResponseBundle) request).ClientID)
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