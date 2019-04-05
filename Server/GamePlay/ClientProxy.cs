using System;
using System.Collections.Generic;
using System.Net.Sockets;

internal class ClientProxy : ProxyBase
{
    protected Queue<ServerRequestBase> SendRequestsQueue = new Queue<ServerRequestBase>();
    protected Queue<ClientRequestBase> ReceiveRequestsQueue = new Queue<ClientRequestBase>();

    public ServerGameManager MyServerGameManager;

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

            ServerLog.PrintClientStates("Client " + ClientId + " state change: " + before + " -> " + ClientState);
        }
    }

    public ClientProxy(Socket socket, int clientId, bool isStopReceive) : base(socket, clientId, isStopReceive)
    {
        ClientIdRequest request = new ClientIdRequest(clientId, Server.ServerVersion);
        ClientState = ClientStates.GetId;
        SendMessage(request);
    }

    protected ServerPlayer MyPlayer;
    protected ServerPlayer EnemyPlayer;
    protected ServerHandManager MyHandManager;
    protected ServerHandManager EnemyHandManager;
    protected ServerBattleGroundManager MyBattleGroundManager;
    protected ServerBattleGroundManager EnemyBattleGroundManager;

    public void InitGameInfo()
    {
        MyPlayer = MyServerGameManager.GetPlayerByClientId(ClientId);
        EnemyPlayer = MyPlayer.MyEnemyPlayer;
        MyHandManager = MyPlayer.MyHandManager;
        EnemyHandManager = EnemyPlayer.MyHandManager;
        MyBattleGroundManager = MyPlayer.MyBattleGroundManager;
        EnemyBattleGroundManager = EnemyPlayer.MyBattleGroundManager;
    }

    private bool isClosed = false;

    public void OnClose()
    {
        if (isClosed) return;

        MyServerGameManager?.OnLeaveGame(ClientId); //先结束对应的游戏

        Socket?.Close();

        SendRequestsQueue.Clear();
        ReceiveRequestsQueue.Clear();

        isClosed = true;
    }

    public virtual void SendMessage(ServerRequestBase request)
    {
        if (isClosed) return;
        SendRequestsQueue.Enqueue(request);
        //OnSendMessage();
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
        if (MyServerGameManager != null && MyServerGameManager.IsStopped)
        {
            MyServerGameManager.StopGame();
        }

        while (ReceiveRequestsQueue.Count > 0)
        {
            ClientRequestBase r = ReceiveRequestsQueue.Dequeue();
            switch (r)
            {
                case ClientVersionValidRequest _:

                    ServerLog.PrintClientStates("Client " + ClientId + " version valid.");
                    clientVersionValid = true;
                    break;
                case RegisterRequest request:
                    if (clientVersionValid)
                    {
                        ServerLog.PrintClientStates("Client " + ClientId + " state: " + ClientState);

                        if (ClientState != ClientStates.GetId)
                        {
                            Server.SV.SGMM.RemoveGame(this);
                            ClientState = ClientStates.GetId;
                        }

                        if (ClientState == ClientStates.GetId)
                        {
                            bool suc = Database.Instance.AddUser(request.username, request.password);
                            RegisterResultRequest response = new RegisterResultRequest(suc);

                            SendMessage(response);
                        }
                    }

                    break;
                case LoginRequest request:
                    if (clientVersionValid)
                    {
                        ServerLog.PrintClientStates("Client " + ClientId + " state: " + ClientState);

                        if (ClientState != ClientStates.GetId)
                        {
                            Server.SV.SGMM.RemoveGame(this);
                            ClientState = ClientStates.GetId;
                        }

                        if (ClientState == ClientStates.GetId)
                        {
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

                                        bool superAccout = username == "StoryAdmin" || username == "ServerAdmin";

                                        if (Database.Instance.PlayerStoryStates.ContainsKey(username))
                                        {
                                            Story story = Database.Instance.PlayerStoryStates[username];
                                            ClientBuildInfosRequest request1 = new ClientBuildInfosRequest(Database.Instance.GetPlayerBuilds(username), superAccout ? GamePlaySettings.ServerGamePlaySettings : GamePlaySettings.OnlineGamePlaySettings, true, story);
                                            SendMessage(request1);
                                        }
                                        else
                                        {
                                            ClientBuildInfosRequest request1 = new ClientBuildInfosRequest(Database.Instance.GetPlayerBuilds(username), superAccout ? GamePlaySettings.ServerGamePlaySettings : GamePlaySettings.OnlineGamePlaySettings, false);
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
                    ServerLog.PrintClientStates("Client " + ClientId + " state: " + ClientState);

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

                case StartNewStoryRequest _:
                {
                    Story newStory = Database.Instance.StoryStartDict["Story1"].Variant();
                    Database.Instance.RemovePlayerStory(UserName, this);
                    Database.Instance.PlayerStoryStates.Add(UserName, newStory);

                    //TODO

                    foreach (KeyValuePair<int, BuildInfo> kv in newStory.PlayerBuildInfos)
                    {
                        Database.Instance.BuildInfoDict.Add(kv.Key, kv.Value);
                    }

                    StartNewStoryRequestResponse response = new StartNewStoryRequestResponse(newStory);
                    SendMessage(response);
                    break;
                }

                case BonusGroupRequest request:
                {
                    Story story = Database.Instance.PlayerStoryStates[username];
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
                    Story story = Database.Instance.PlayerStoryStates[username];
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

                    string username = Database.Instance.GetUsernameByClientId(ClientId);
                    if (username != null)
                    {
                        Database.Instance.AddOrModifyBuild(username, request.BuildInfo, request.isSingle);
                    }
                    else
                    {
                        ServerLog.PrintError("No such user in server：[ClientID]=" + ClientId);
                    }

                    break;
                }

                case DeleteBuildRequest request:
                {
                    Database.Instance.DeleteBuild(username, request.buildID, request.isSingle);
                    DeleteBuildRequestResponse response = new DeleteBuildRequestResponse(request.buildID);
                    SendMessage(response);
                    break;
                }

                case MatchRequest request:

                    ServerLog.PrintClientStates("Client " + ClientId + " state: " + ClientState);

                    if (ClientState == ClientStates.Playing)
                    {
                        Server.SV.SGMM.RemoveGame(this);
                        ClientState = ClientStates.Login;
                    }

                    if (ClientState == ClientStates.Login)
                    {
                        CurrentBuildInfo = Database.Instance.GetBuildInfoByID(request.buildID);
                        ClientState = ClientStates.Matching;
                        Server.SV.SGMM.OnClientMatchGames(this);
                    }

                    break;

                case MatchStandAloneRequest request:

                    ServerLog.PrintClientStates("Client " + ClientId + " state: " + ClientState);

                    if (ClientState == ClientStates.Playing)
                    {
                        Server.SV.SGMM.RemoveGame(this);
                        ClientState = ClientStates.Login;
                    }

                    if (ClientState == ClientStates.Login)
                    {
                        CurrentBuildInfo = Database.Instance.GetBuildInfoByID(request.BuildID);
                        ClientState = ClientStates.Matching;
                        if (request.StoryPaceID == -1)
                        {
                            Server.SV.SGMM.OnClientMatchStandAloneCustomGames(this);
                        }
                        else
                        {
                            Server.SV.SGMM.OnClientMatchStandAloneGames(this, request.StoryPaceID);
                        }
                    }

                    break;
                case CancelMatchRequest _:

                    ServerLog.PrintClientStates("Client " + ClientId + " state: " + ClientState);

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

            if (ClientState == ClientStates.Playing)
            {
                if (MyServerGameManager == null) return;

                try
                {
                    if (MyServerGameManager.CurrentPlayer == MyServerGameManager.GetPlayerByClientId(ClientId))
                    {
                        switch (r)
                        {
                            case WinDirectlyRequest req:
                                MyServerGameManager?.OnWinDirectlyRequest(req, MyPlayer);
                                break;
                            case EndRoundRequest req:
                                MyServerGameManager?.OnEndRoundRequest(req);
                                break;
                            case SummonRetinueRequest req:
                                MyServerGameManager?.OnClientSummonRetinueRequest(req);
                                break;
                            case EquipWeaponRequest req:
                                MyServerGameManager?.OnClientEquipWeaponRequest(req);
                                break;
                            case EquipShieldRequest req:
                                MyServerGameManager?.OnClientEquipShieldRequest(req);
                                break;
                            case EquipPackRequest req:
                                MyServerGameManager?.OnClientEquipPackRequest(req);
                                break;
                            case EquipMARequest req:
                                MyServerGameManager?.OnClientEquipMARequest(req);
                                break;
                            case UseSpellCardRequest req:
                                MyServerGameManager?.OnClientUseSpellCardRequest(req);
                                break;
                            case UseSpellCardToRetinueRequest req:
                                MyServerGameManager?.OnClientUseSpellCardToRetinueRequest(req);
                                break;
                            case UseSpellCardToShipRequest req:
                                MyServerGameManager?.OnClientUseSpellCardToShipRequest(req);
                                break;
                            case UseSpellCardToEquipRequest req:
                                MyServerGameManager?.OnClientUseSpellCardToEquipRequest(req);
                                break;
                            case LeaveGameRequest req: //quit game normally
                                MyServerGameManager?.OnLeaveGameRequest(req);
                                break;
                            case RetinueAttackRetinueRequest req:
                                MyServerGameManager?.OnClientRetinueAttackRetinueRequest(req);
                                break;
                            case RetinueAttackShipRequest req:
                                MyServerGameManager?.OnClientRetinueAttackShipRequest(req);
                                break;
                        }
                    }
                    else
                    {
                        switch (r)
                        {
                            case LeaveGameRequest req: //quit game normally
                                MyServerGameManager?.OnLeaveGameRequest(req);
                                break;
                        }
                    }
                }
                catch (Exception e)
                {
                    ServerLog.PrintError(e.ToString());

                    if (MyServerGameManager != null && !MyServerGameManager.IsStopped)
                    {
                        MyServerGameManager.OnEndGameByServerError();
                        MyServerGameManager.StopGame();
                    }

                    MyServerGameManager = null;
                }
            }
        }
    }
}