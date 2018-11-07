using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
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
            }

            clientState = value;
            ClientLog.Instance.PrintClientStates("Client states: " + ClientState);
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

    public void OnBeginSingleMode(int levelID, int bossPicID)
    {
        ClientRequestBase req = null;
        req = new MatchStandAloneRequest(ClientId, SelectBuildManager.Instance.CurrentSelectedBuildButton.BuildInfo.BuildID, levelID, bossPicID);
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
                        NoticeManager.Instance.ShowInfoPanelCenter(GameManager.Instance.IsEnglish ? "Register Success" : "注册成功", 0, 0.5f);
                    }
                    else
                    {
                        NoticeManager.Instance.ShowInfoPanelCenter(GameManager.Instance.IsEnglish ? "This username has been used" : "该用户名已被注册", 0, 0.5f);
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
                            NoticeManager.Instance.ShowInfoPanelTop(GameManager.Instance.IsEnglish ? "Login Success" : "登录成功", 0, 0.5f);
                            LoginManager.Instance.M_StateMachine.SetState(LoginManager.StateMachine.States.Hide);
                            StartMenuManager.Instance.M_StateMachine.SetState(StartMenuManager.StateMachine.States.Show_Main);
                            break;
                        }
                        case LoginResultRequest.StateCodes.WrongPassword:
                        {
                            NoticeManager.Instance.ShowInfoPanelCenter(GameManager.Instance.IsEnglish ? "Wrong Password" : "您的密码有误", 0, 0.5f);
                            break;
                        }
                        case LoginResultRequest.StateCodes.UnexistedUser:
                        {
                            NoticeManager.Instance.ShowInfoPanelCenter(GameManager.Instance.IsEnglish ? "Username does not Exist" : "该用户名不存在", 0, 0.5f);
                            break;
                        }
                        case LoginResultRequest.StateCodes.AlreadyOnline:
                        {
                            NoticeManager.Instance.ShowInfoPanelCenter(GameManager.Instance.IsEnglish ? "The account is logged in" : "该账号已登录", 0, 0.5f);
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
                        NoticeManager.Instance.ShowInfoPanelTop(GameManager.Instance.IsEnglish ? "Logout Success" : "退出成功", 0, 0.5f);
                        Client.Instance.Proxy.ClientState = ClientStates.GetId;
                        StartMenuManager.Instance.M_StateMachine.SetState(StartMenuManager.StateMachine.States.Hide);
                        SelectBuildManager.Instance.M_StateMachine.SetState(SelectBuildManager.StateMachine.States.Hide);
                        LoginManager.Instance.M_StateMachine.SetState(LoginManager.StateMachine.States.Show);
                    }
                    else
                    {
                        NoticeManager.Instance.ShowInfoPanelCenter(GameManager.Instance.IsEnglish ? "Logout Failed" : "退出失败", 0, 0.5f);
                    }

                    break;
                }
                case NetProtocols.START_NEW_STORY_REQUEST_RESPONSE:
                {
                    StartNewStoryRequestResponse request = (StartNewStoryRequestResponse) r;
                    StoryManager.Instance.InitiateStoryCanvas(request.Story);
                    SelectBuildManager.Instance.SwitchGameMode(SelectBuildManager.GameMode.Single, true);
                    StartMenuManager.Instance.M_StateMachine.RefreshStoryState();
                    AudioManager.Instance.SoundPlay("sfx/OnStoryStart");
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
                    SelectBuildManager.Instance.M_CurrentOnlineCompete = new SelectBuildManager.OnlineCompete();
                    SelectBuildManager.Instance.M_CurrentOnlineCompete.OnlineGamePlaySettings = request.OnlineGamePlaySettings;
                    SelectBuildManager.Instance.M_CurrentOnlineCompete.OnlineBuildInfos = request.OnlineBuildInfos;
                    if (request.HasStory)
                    {
                        StoryManager.Instance.M_CurrentStory = request.Story;
                        StoryManager.Instance.InitiateStoryCanvas(request.Story);
                    }
                    else
                    {
                        StoryManager.Instance.M_CurrentStory = null;
                    }

                    break;
                }
                case NetProtocols.BEAT_BOSS_REQUSET:
                {
                    BeatBossRequest request = (BeatBossRequest) r;
                    StoryManager.Instance.SetLevelBeated(request.LevelID, request.BossPicID);
                    break;
                }
                case NetProtocols.NEXT_LEVEL_BOSSINFO_REQUSET:
                {
                    NextLevelBossesRequest request = (NextLevelBossesRequest) r;
                    StoryManager.Instance.SetLevelKnown(request.LevelID, request.NextLevelBossPicIDs);
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