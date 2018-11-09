using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

internal class ClientProxy : ProxyBase
{
    protected Queue<ServerRequestBase> SendRequestsQueue = new Queue<ServerRequestBase>();
    protected Queue<ClientRequestBase> ReceiveRequestsQueue = new Queue<ClientRequestBase>();

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
if (ServerConsole.Platform==ServerConsole.DEVELOP.DEVELOP||ServerConsole.Platform==ServerConsole.DEVELOP.TEST)
            ServerLog.PrintClientStates("Client " + ClientId + " state change: " + before + " -> " + ClientState);

        }
    }

    public ClientProxy(Socket socket, int clientId, bool isStopReceive) : base(socket, clientId, isStopReceive)
    {
        ClientIdRequest request = new ClientIdRequest(clientId);
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
                //Thread thread = new Thread(Server.SV.DoSendToClient);
                //thread.IsBackground = true;
                //thread.Start(msg);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }


    //private void OnSendMessage()
    //{
    //    Thread sendMessageThread = new Thread(SendMessage);
    //    sendMessageThread.IsBackground = true;
    //    sendMessageThread.Start();
    //}

    //private void SendMessage()
    //{
    //    lock (SendRequestsQueue)
    //    {
    //        if (SendRequestsQueue.Count > 0)
    //        {
    //            try
    //            {
    //                Thread thread = new Thread(Server.SV.DoSendToClient);
    //                SendMsg msg = new SendMsg(Socket, SendRequestsQueue.Dequeue(), ClientId);
    //                thread.IsBackground = true;
    //                thread.Start(msg);
    //            }
    //            catch (Exception e)
    //            {
    //                Console.WriteLine(e);
    //            }
    //        }
    //    }
    //}

    public virtual void ReceiveMessage(ClientRequestBase request)
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
        if (MyServerGameManager != null && MyServerGameManager.IsStopped)
        {
            MyServerGameManager.StopGame();
        }

        while (ReceiveRequestsQueue.Count > 0)
        {
            ClientRequestBase r = ReceiveRequestsQueue.Dequeue();
            switch (r)
            {
                //以下是进入游戏前的请求
                case RegisterRequest _:
if (ServerConsole.Platform==ServerConsole.DEVELOP.DEVELOP||ServerConsole.Platform==ServerConsole.DEVELOP.TEST)
                    ServerLog.PrintClientStates("Client " + ClientId + " state: " + ClientState);

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
if (ServerConsole.Platform==ServerConsole.DEVELOP.DEVELOP||ServerConsole.Platform==ServerConsole.DEVELOP.TEST)
                    ServerLog.PrintClientStates("Client " + ClientId + " state: " + ClientState);

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

                    break;
                case LogoutRequest _:
                {
if (ServerConsole.Platform==ServerConsole.DEVELOP.DEVELOP||ServerConsole.Platform==ServerConsole.DEVELOP.TEST)
                    ServerLog.PrintClientStates("Client " + ClientId + " state: " + ClientState);

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

                case StartNewStoryRequest _:
                {
                    Story newStory = Database.Instance.StoryStartDict["Story1"].Variant();
                    Database.Instance.RemovePlayerStory(UserName, this);
                    Database.Instance.PlayerStoryStates.Add(UserName, newStory);

                    newStory.PlayerBuildInfos.Add(newStory.PlayerCurrentBuildInfo.BuildID, newStory.PlayerCurrentBuildInfo);
                    newStory.UnlockLevelBosses(0);
                    Database.Instance.BuildInfoDict.Add(newStory.PlayerCurrentBuildInfo.BuildID, newStory.PlayerCurrentBuildInfo);
                    StartNewStoryRequestResponse response = new StartNewStoryRequestResponse(newStory);
                    SendMessage(response);
                    break;
                }

                case BonusGroupRequest _:
                {
                    BonusGroupRequest request = (BonusGroupRequest) r;
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
                                    story.StoryGamePlaySettings.DefaultLifeMax += bonus.Value;
                                    story.StoryGamePlaySettings.DefaultLife += bonus.Value;
                                    foreach (KeyValuePair<int, BuildInfo> kv in story.PlayerBuildInfos)
                                    {
                                        kv.Value.Life += bonus.Value;
                                    }

                                    break;
                                }
                                case Bonus.BonusType.EnergyUpperLimit:
                                {
                                    story.StoryGamePlaySettings.DefaultEnergyMax += bonus.Value;
                                    story.StoryGamePlaySettings.DefaultEnergy += bonus.Value;
                                    foreach (KeyValuePair<int, BuildInfo> kv in story.PlayerBuildInfos)
                                    {
                                        kv.Value.Energy += bonus.Value;
                                    }

                                    break;
                                }
                                case Bonus.BonusType.Budget:
                                {
                                    story.StoryGamePlaySettings.DefaultCoin += bonus.Value;
                                    break;
                                }
                                case Bonus.BonusType.UnlockCardByID:
                                {
                                    if (!story.PlayerCurrentUnlockedBuildInfo.CardIDs.Contains(bonus.Value)) story.PlayerCurrentUnlockedBuildInfo.CardIDs.Add(bonus.Value);
                                    break;
                                }
                            }
                        }
                    }

                    StartNewStoryRequestResponse response = new StartNewStoryRequestResponse(story);
                    SendMessage(response);
                    break;
                }

                case BuildRequest _:
                {
                    BuildRequest request = (BuildRequest) r;
                    if (request.BuildInfo.BuildID == -1)
                    {
                        request.BuildInfo.BuildID = BuildInfo.GenerateBuildID();
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
                        Database.Instance.AddOrModifyBuild(username, request.BuildInfo, request.isSingle);
                    }
                    else
                    {
                        ServerLog.PrintError("No such user in server：[ClientID]=" + ClientId);
                    }

                    break;
                }

                case DeleteBuildRequest _:
                {
                    DeleteBuildRequest request = (DeleteBuildRequest) r;
                    Database.Instance.DeleteBuild(username, request.buildID, request.isSingle);
                    DeleteBuildRequestResponse response = new DeleteBuildRequestResponse(request.buildID);
                    SendMessage(response);
                    break;
                }

                case MatchRequest _:
if (ServerConsole.Platform==ServerConsole.DEVELOP.DEVELOP||ServerConsole.Platform==ServerConsole.DEVELOP.TEST)
                    ServerLog.PrintClientStates("Client " + ClientId + " state: " + ClientState);

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

                case MatchStandAloneRequest _:
if (ServerConsole.Platform==ServerConsole.DEVELOP.DEVELOP||ServerConsole.Platform==ServerConsole.DEVELOP.TEST)
                    ServerLog.PrintClientStates("Client " + ClientId + " state: " + ClientState);

                    if (ClientState == ClientStates.Playing)
                    {
                        Server.SV.SGMM.RemoveGame(this);
                        ClientState = ClientStates.Login;
                    }

                    if (ClientState == ClientStates.Login)
                    {
                        MatchStandAloneRequest request = (MatchStandAloneRequest) r;
                        CurrentBuildInfo = Database.Instance.GetBuildInfoByID(request.BuildID);
                        ClientState = ClientStates.Matching;
                        Server.SV.SGMM.OnClientMatchStandAloneGames(this, request.LevelID, request.BossPicID);
                    }

                    break;
                case CancelMatchRequest _:
if (ServerConsole.Platform==ServerConsole.DEVELOP.DEVELOP||ServerConsole.Platform==ServerConsole.DEVELOP.TEST)
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
                            case WinDirectlyRequest _:
                                MyServerGameManager?.OnWinDirectlyRequest((WinDirectlyRequest) r, MyPlayer);
                                break;
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
                            case UseSpellCardToRetinueRequest _:
                                MyServerGameManager?.OnClientUseSpellCardToRetinueRequest((UseSpellCardToRetinueRequest) r);
                                break;
                            case UseSpellCardToShipRequest _:
                                MyServerGameManager?.OnClientUseSpellCardToShipRequest((UseSpellCardToShipRequest) r);
                                break;
                            case UseSpellCardToEquipRequest _:
                                MyServerGameManager?.OnClientUseSpellCardToEquipRequest((UseSpellCardToEquipRequest) r);
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
                    else
                    {
                        switch (r)
                        {
                            case LeaveGameRequest _: //正常退出游戏请求
                                MyServerGameManager?.OnLeaveGameRequest((LeaveGameRequest) r);
                                break;
                        }
                    }
                }
                catch (Exception e)
                {
if (ServerConsole.Platform==ServerConsole.DEVELOP.DEVELOP||ServerConsole.Platform==ServerConsole.DEVELOP.TEST)
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