using System;
using System.Collections.Generic;
using System.Net.Sockets;

internal class ServerProxy : ProxyBase
{
    private Queue<ServerRequestBase> SendRequestsQueue = new Queue<ServerRequestBase>();
    private Queue<ClientRequestBase> ReceiveRequestsQueue = new Queue<ClientRequestBase>();

    public GameProxy GameProxy;

    public ServerProxy(Socket socket, int clientId, bool isStopReceive) : base(socket, clientId, isStopReceive)
    {
        GameProxy = new GameProxy(clientId, "", Server.ServerVersion, SendMessage, ServerLog.Instance);
        GameProxy.SendClientIDRequest();
    }

    private bool isClosed = false;

    private string UserName
    {
        get { return GameProxy.UserName; }
        set { GameProxy.UserName = value; }
    }

    private ClientStates ClientState
    {
        get { return GameProxy.ClientState; }
        set { GameProxy.ClientState = value; }
    }

    public void OnClose()
    {
        if (isClosed) return;

        Socket?.Close();

        SendRequestsQueue.Clear();
        ReceiveRequestsQueue.Clear();

        isClosed = true;
    }

    protected virtual void SendMessage(ServerRequestBase request)
    {
        if (isClosed) return;
        SendRequestsQueue.Enqueue(request);
        if (SendRequestsQueue.Count > 0)
        {
            try
            {
                SendMsg msg = new SendMsg(Socket, SendRequestsQueue.Dequeue(), ClientID);
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
            ClientRequestBase request = ReceiveRequestsQueue.Dequeue();
            // Server 端特例覆写GameProxy
            switch (request)
            {
                case RegisterRequest r:
                {
                    ServerLog.Instance.PrintClientStates("Client " + ClientID + " state: " + ClientState);
                    if (ClientState != ClientStates.GetId)
                    {
                        Server.SV.SGMM.RemoveGame(this);
                        ClientState = ClientStates.GetId;
                    }

                    bool suc = UserDatabase.Instance.AddUser(r.username, r.password);
                    RegisterResultRequest response = new RegisterResultRequest(suc);
                    SendMessage(response);
                    break;
                }

                case LoginRequest r:
                {
                    ServerLog.Instance.PrintClientStates("Client " + ClientID + " state: " + ClientState);

                    if (ClientState != ClientStates.GetId)
                    {
                        Server.SV.SGMM.RemoveGame(this);
                        ClientState = ClientStates.GetId;
                    }

                    if (ClientState == ClientStates.GetId)
                    {
                        LoginResultRequest response;
                        string password = UserDatabase.Instance.GetUserPasswordByUsername(r.username);
                        if (password != null)
                        {
                            if (password == r.password)
                            {
                                bool suc = UserDatabase.Instance.AddLoginUser(ClientID, r.username);
                                if (suc)
                                {
                                    response = new LoginResultRequest(r.username, LoginResultRequest.StateCodes.Success);
                                    SendMessage(response);
                                    ClientState = ClientStates.Login;
                                    UserName = r.username;

                                    if (BuildStoryDatabase.Instance.PlayerStoryStates.ContainsKey(UserName))
                                    {
                                        Story story = BuildStoryDatabase.Instance.PlayerStoryStates[UserName];
                                        ClientBuildInfosRequest request1 = new ClientBuildInfosRequest(BuildStoryDatabase.Instance.GetPlayerBuilds(UserName), GamePlaySettings.OnlineGamePlaySettings, true, story);
                                        SendMessage(request1);
                                    }
                                    else
                                    {
                                        ClientBuildInfosRequest request1 = new ClientBuildInfosRequest(BuildStoryDatabase.Instance.GetPlayerBuilds(UserName), GamePlaySettings.OnlineGamePlaySettings, false);
                                        SendMessage(request1);
                                    }
                                }
                                else
                                {
                                    response = new LoginResultRequest(r.username, LoginResultRequest.StateCodes.AlreadyOnline);
                                    SendMessage(response);
                                }
                            }
                            else
                            {
                                response = new LoginResultRequest(r.username, LoginResultRequest.StateCodes.WrongPassword);
                                SendMessage(response);
                            }
                        }
                        else
                        {
                            response = new LoginResultRequest(r.username, LoginResultRequest.StateCodes.UnexistedUser);
                            SendMessage(response);
                        }
                    }

                    break;
                }

                case LogoutRequest r:
                {
                    ServerLog.Instance.PrintClientStates("Client " + ClientID + " state: " + ClientState);
                    LogoutResultRequest response;
                    if (ClientState != ClientStates.GetId)
                    {
                        Server.SV.SGMM.RemoveGame(this);
                        bool suc = UserDatabase.Instance.RemoveLoginUser(ClientID);
                        ClientState = ClientStates.GetId;
                        response = new LogoutResultRequest(r.username, suc);
                    }
                    else
                    {
                        response = new LogoutResultRequest(r.username, false);
                    }

                    SendMessage(response);
                    break;
                }

                case MatchRequest r:

                    ServerLog.Instance.PrintClientStates("Client " + ClientID + " state: " + ClientState);

                    if (ClientState == ClientStates.Playing)
                    {
                        Server.SV.SGMM.RemoveGame(this);
                        ClientState = ClientStates.Login;
                    }

                    if (ClientState == ClientStates.Login)
                    {
                        CurrentBuildInfo = BuildStoryDatabase.Instance.GetBuildInfoByID(r.buildID);
                        ClientState = ClientStates.Matching;
                        Server.SV.SGMM.OnClientMatchGames(this);
                    }

                    break;

                case CancelMatchRequest _:

                    ServerLog.Instance.PrintClientStates("Client " + ClientID + " state: " + ClientState);

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
                default:
                    GameProxy.ReceiveRequest(request);
                    break;
            }
        }
    }
}