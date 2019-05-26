using System.Collections.Generic;

/// <summary>
/// Proxy of all game events like battles, stories, builds, card edits and so on.
/// Including BattleProxy, BuildProxy, StoryProxy ...
/// Its duty is to allocate request/response to each sub-proxy.
///
/// If client plays standalone, then the GameProxy inside Client-end would start.
/// If client plays through Internet, then the GameProxy inside Server-end would start and the one inside Client-end would be disabled.
/// </summary>
public class GameProxy
{
    public ILog DebugLog;
    public SendMessageDelegate SendMessage;

    public BattleProxy BattleProxy;

    public int ClientID;
    public string UserName;
    public string ServerVersion;

    private ProxyBase.ClientStates clientState;

    public ProxyBase.ClientStates ClientState
    {
        get => clientState;
        set
        {
            ProxyBase.ClientStates before = ClientState;
            clientState = value;
            BattleProxy.ClientState = value;
            DebugLog.PrintClientStates("Client " + ClientID + " state change: " + before + " -> " + ClientState);
        }
    }

    public GameProxy(int clientID, string userName, string serverVersion, SendMessageDelegate sendMessageDelegate, ILog debugLog)
    {
        UserName = userName;
        ClientID = clientID;
        ServerVersion = serverVersion;
        SendMessage = sendMessageDelegate;
        DebugLog = debugLog;
        BattleProxy = new BattleProxy(clientID, userName, null, sendMessageDelegate, debugLog);
        ClientState = ProxyBase.ClientStates.Offline;
    }

    public void SendClientIDRequest()
    {
        ClientIdRequest request = new ClientIdRequest(ClientID, ServerVersion);
        SendMessage(request);
    }

    public void ReceiveRequest(ClientRequestBase request)
    {
        switch (request)
        {
            case ClientVersionValidRequest _:
            {
                DebugLog.PrintClientStates("Client " + ClientID + " version valid.");
                break;
            }
            case RegisterRequest r:
            {
                DebugLog.PrintClientStates("Client " + ClientID + " state: " + ClientState);
                if (ClientState != ProxyBase.ClientStates.GetId)
                {
                    ClientState = ProxyBase.ClientStates.GetId;
                }

                UserName = r.username;
                RegisterResultRequest response = new RegisterResultRequest(true);
                SendMessage(response);
                break;
            }
            case LoginRequest r:
            {
                DebugLog.PrintClientStates("Client " + ClientID + " state: " + ClientState);

                if (ClientState != ProxyBase.ClientStates.GetId)
                {
                    ClientState = ProxyBase.ClientStates.GetId;
                }

                LoginResultRequest response = new LoginResultRequest(r.username, LoginResultRequest.StateCodes.Success);
                SendMessage(response);
                ClientState = ProxyBase.ClientStates.Login;
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

                break;
            }
            case LogoutRequest r:
            {
                DebugLog.PrintClientStates("Client " + ClientID + " state: " + ClientState);
                LogoutResultRequest response;
                if (ClientState != ProxyBase.ClientStates.GetId)
                {
                    ClientState = ProxyBase.ClientStates.GetId;
                    response = new LogoutResultRequest(r.username, true);
                }
                else
                {
                    response = new LogoutResultRequest(r.username, false);
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
            case MatchStandAloneRequest r:

                DebugLog.PrintClientStates("Client " + ClientID + " state: " + ClientState);

                if (ClientState != ProxyBase.ClientStates.Login)
                {
                    ClientState = ProxyBase.ClientStates.Login;
                }

                if (ClientState == ProxyBase.ClientStates.Login)
                {
                    CurrentBuildInfo = Database.Instance.GetBuildInfoByID(r.BuildID);
                    ClientState = ProxyBase.ClientStates.Matching;
                    if (r.StoryPaceID == -1)
                    {
                        Server.SV.SGMM.OnClientMatchStandAloneCustomGames(this);
                    }
                    else
                    {
                        Server.SV.SGMM.OnClientMatchStandAloneGames(this, r.StoryPaceID);
                    }
                }

                break;
            case BonusGroupRequest r:
            {
                Story story = BuildStoryDatabase.Instance.PlayerStoryStates[UserName];
                if (story != null)
                {
                    foreach (Bonus bonus in r.BonusGroup.Bonuses)
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
                Story story = BuildStoryDatabase.Instance.PlayerStoryStates[UserName];
                StartNewStoryRequestResponse response = new StartNewStoryRequestResponse(story);
                SendMessage(response);

                EndBattleRequestResponse r = new EndBattleRequestResponse();
                SendMessage(r);
                break;
            }

            case BuildRequest r:
            {
                if (r.BuildInfo.BuildID == -1)
                {
                    r.BuildInfo.BuildID = BuildInfo.GenerateBuildID();
                    CreateBuildRequestResponse response = new CreateBuildRequestResponse(r.BuildInfo);
                    SendMessage(response);
                }
                else
                {
                    BuildUpdateRequest response = new BuildUpdateRequest(r.BuildInfo);
                    SendMessage(response);
                }

                BuildStoryDatabase.Instance.AddOrModifyBuild(UserName, r.BuildInfo, r.isSingle);

                break;
            }

            case DeleteBuildRequest r:
            {
                BuildStoryDatabase.Instance.DeleteBuild(UserName, r.buildID, r.isSingle);
                DeleteBuildRequestResponse response = new DeleteBuildRequestResponse(r.buildID);
                SendMessage(response);
                break;
            }
        }
    }
}