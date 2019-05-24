using System;
using System.Collections.Generic;
using System.Net.Sockets;

internal class ClientProxy : ProxyBase
{
    protected Queue<ServerRequestBase> SendRequestsQueue = new Queue<ServerRequestBase>();
    protected Queue<ClientRequestBase> ReceiveRequestsQueue = new Queue<ClientRequestBase>();

    internal BattleClientProxy BattleClientProxy;

    private bool clientVersionValid = false;
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

            ServerLog.Instance.PrintClientStates("Client " + ClientId + " state change: " + before + " -> " + ClientState);
        }
    }

    public ClientProxy(Socket socket, int clientId, bool isStopReceive) : base(socket, clientId, isStopReceive)
    {
        ClientIdRequest request = new ClientIdRequest(clientId, Server.ServerVersion);
        ClientState = ClientStates.GetId;
        SendMessage(request);

        BattleClientProxy = new BattleClientProxy(socket, Server.ServerVersion, clientId, isStopReceive);
    }

    private bool isClosed = false;

    public void OnClose()
    {
        if (isClosed) return;

        Socket?.Close();

        SendRequestsQueue.Clear();
        ReceiveRequestsQueue.Clear();

        isClosed = true;
    }

    public virtual void SendMessage(ServerRequestBase request)
    {
        if (isClosed) return;
        SendRequestsQueue.Enqueue(request);
        if (SendRequestsQueue.Count > 0)
        {
            try
            {
                SendMsg msg = new SendMsg(Socket, SendRequestsQueue.Dequeue(), ClientId);
                Server.SV.DoSendToClient(msg);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    public virtual void ReceiveMessage(ClientRequestBase request)
    {
        if (isClosed) return;
        ReceiveRequestsQueue.Enqueue(request);
        Response();
    }

    public ResponseBundleBase CurrentClientRequestResponseBundle;

    /// <summary>
    /// Dispose of all request received before game start
    /// ServerGameManager will dispose of others during games.
    /// </summary>
    protected override void Response()
    {
        while (ReceiveRequestsQueue.Count > 0)
        {
            ClientRequestBase r = ReceiveRequestsQueue.Dequeue();
            switch (r)
            {
                case ClientVersionValidRequest _:

                    ServerLog.Instance.PrintClientStates("Client " + ClientId + " version valid.");
                    clientVersionValid = true;
                    break;
                case RegisterRequest request:
                    if (clientVersionValid)
                    {
                        ServerLog.Instance.PrintClientStates("Client " + ClientId + " state: " + ClientState);

                        if (ClientState != ClientStates.GetId)
                        {
                            Server.SV.SGMM.RemoveGame(this);
                            ClientState = ClientStates.GetId;
                        }

                        if (ClientState == ClientStates.GetId)
                        {
                            bool suc = UserDatabase.Instance.AddUser(request.username, request.password);
                            RegisterResultRequest response = new RegisterResultRequest(suc);

                            SendMessage(response);
                        }
                    }

                    break;
                case LoginRequest request:
                    if (clientVersionValid)
                    {
                        ServerLog.Instance.PrintClientStates("Client " + ClientId + " state: " + ClientState);

                        if (ClientState != ClientStates.GetId)
                        {
                            Server.SV.SGMM.RemoveGame(this);
                            ClientState = ClientStates.GetId;
                        }

                        if (ClientState == ClientStates.GetId)
                        {
                            LoginResultRequest response;

                            string password = UserDatabase.Instance.GetUserPasswordByUsername(request.username);
                            if (password != null)
                            {
                                if (password == request.password)
                                {
                                    bool suc = UserDatabase.Instance.AddLoginUser(ClientId, request.username);
                                    if (suc)
                                    {
                                        response = new LoginResultRequest(request.username, LoginResultRequest.StateCodes.Success);
                                        SendMessage(response);
                                        ClientState = ClientStates.Login;
                                        username = request.username;

                                        if (BuildStoryDatabase.Instance.PlayerStoryStates.ContainsKey(username))
                                        {
                                            Story story = BuildStoryDatabase.Instance.PlayerStoryStates[username];
                                            ClientBuildInfosRequest request1 = new ClientBuildInfosRequest(BuildStoryDatabase.Instance.GetPlayerBuilds(username), GamePlaySettings.OnlineGamePlaySettings, true, story);
                                            SendMessage(request1);
                                        }
                                        else
                                        {
                                            ClientBuildInfosRequest request1 = new ClientBuildInfosRequest(BuildStoryDatabase.Instance.GetPlayerBuilds(username), GamePlaySettings.OnlineGamePlaySettings, false);
                                            SendMessage(request1);
                                        }
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
                    }

                    break;
                case LogoutRequest request:
                {
                    ServerLog.Instance.PrintClientStates("Client " + ClientId + " state: " + ClientState);

                    LogoutResultRequest response;
                    if (ClientState != ClientStates.GetId)
                    {
                        Server.SV.SGMM.RemoveGame(this);
                        bool suc = UserDatabase.Instance.RemoveLoginUser(ClientId);
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

                case StartNewStoryRequest _:
                {
                    Story newStory = BuildStoryDatabase.Instance.StoryStartDict["Story1"].Variant();
                    List<int> removeBuildIDs = BuildStoryDatabase.Instance.RemovePlayerStory(UserName);
                    foreach (int removeBuildID in removeBuildIDs)
                    {
                        DeleteBuildRequestResponse res = new DeleteBuildRequestResponse(removeBuildID);
                        SendMessage(res);
                    }

                    BuildStoryDatabase.Instance.PlayerStoryStates.Add(UserName, newStory);

                    //TODO

                    foreach (KeyValuePair<int, BuildInfo> kv in newStory.PlayerBuildInfos)
                    {
                        BuildStoryDatabase.Instance.BuildInfoDict.Add(kv.Key, kv.Value);
                    }

                    StartNewStoryRequestResponse response = new StartNewStoryRequestResponse(newStory);
                    SendMessage(response);
                    break;
                }

                case BonusGroupRequest request:
                {
                    Story story = BuildStoryDatabase.Instance.PlayerStoryStates[username];
                    if (story != null)
                    {
                        foreach (Bonus bonus in request.BonusGroup.Bonuses)
                        {
                            switch (bonus.M_BonusType)
                            {
                                case Bonus.BonusType.AdjustDeck:
                                {
                                    //Todo
                                    break;
                                }
                                case Bonus.BonusType.LifeUpperLimit:
                                {
                                    story.StoryGamePlaySettings.DefaultLifeMax += bonus.BonusFinalValue;
                                    story.StoryGamePlaySettings.DefaultLife += bonus.BonusFinalValue;
                                    foreach (KeyValuePair<int, BuildInfo> kv in story.PlayerBuildInfos)
                                    {
                                        kv.Value.Life += bonus.BonusFinalValue;
                                    }

                                    break;
                                }
                                case Bonus.BonusType.EnergyUpperLimit:
                                {
                                    story.StoryGamePlaySettings.DefaultEnergyMax += bonus.BonusFinalValue;
                                    story.StoryGamePlaySettings.DefaultEnergy += bonus.BonusFinalValue;
                                    foreach (KeyValuePair<int, BuildInfo> kv in story.PlayerBuildInfos)
                                    {
                                        kv.Value.Energy += bonus.BonusFinalValue;
                                    }

                                    break;
                                }
                                case Bonus.BonusType.Budget:
                                {
                                    story.StoryGamePlaySettings.DefaultCoin += bonus.BonusFinalValue;
                                    break;
                                }
                                case Bonus.BonusType.UnlockCardByID:
                                {
                                    story.EditAllCardLimitDict(bonus.BonusFinalValue, 1);
                                    break;
                                }
                            }
                        }
                    }

                    break;
                }

                case EndBattleRequest _:
                {
                    Story story = BuildStoryDatabase.Instance.PlayerStoryStates[username];
                    StartNewStoryRequestResponse response = new StartNewStoryRequestResponse(story);
                    SendMessage(response);

                    EndBattleRequestResponse request = new EndBattleRequestResponse();
                    SendMessage(request);
                    break;
                }

                case BuildRequest request:
                {
                    if (request.BuildInfo.BuildID == -1)
                    {
                        request.BuildInfo.BuildID = BuildInfo.GenerateBuildID();
                        CreateBuildRequestResponse response = new CreateBuildRequestResponse(request.BuildInfo);
                        SendMessage(response);
                    }
                    else
                    {
                        BuildUpdateRequest response = new BuildUpdateRequest(request.BuildInfo);
                        SendMessage(response);
                    }

                    string username = UserDatabase.Instance.GetUsernameByClientId(ClientId);
                    if (username != null)
                    {
                        BuildStoryDatabase.Instance.AddOrModifyBuild(username, request.BuildInfo, request.isSingle);
                    }
                    else
                    {
                        ServerLog.Instance.PrintError("No such user in server：[ClientID]=" + ClientId);
                    }

                    break;
                }

                case DeleteBuildRequest request:
                {
                    BuildStoryDatabase.Instance.DeleteBuild(username, request.buildID, request.isSingle);
                    DeleteBuildRequestResponse response = new DeleteBuildRequestResponse(request.buildID);
                    SendMessage(response);
                    break;
                }

                case MatchRequest request:

                    ServerLog.Instance.PrintClientStates("Client " + ClientId + " state: " + ClientState);

                    if (ClientState == ClientStates.Playing)
                    {
                        Server.SV.SGMM.RemoveGame(this);
                        ClientState = ClientStates.Login;
                    }

                    if (ClientState == ClientStates.Login)
                    {
                        CurrentBuildInfo = BuildStoryDatabase.Instance.GetBuildInfoByID(request.buildID);
                        ClientState = ClientStates.Matching;
                        Server.SV.SGMM.OnClientMatchGames(this);
                    }

                    break;

                case CancelMatchRequest _:

                    ServerLog.Instance.PrintClientStates("Client " + ClientId + " state: " + ClientState);

                    if (ClientState == ClientStates.Playing)
                    {
                        Server.SV.SGMM.RemoveGame(this);
                        ClientState = ClientStates.Login;
                    }

                    if (ClientState == ClientStates.Matching)
                    {
                        ClientState = ClientStates.Login;
                        Server.SV.SGMM.OnClientCancelMatch(this);
                    }

                    break;
            }
        }
    }
}