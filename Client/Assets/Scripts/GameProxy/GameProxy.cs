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
    public Battle CurrentBattle;

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
            if (BattleProxy != null) BattleProxy.ClientState = value;
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
        BattleProxy = new BattleProxy(clientID, userName, null, sendMessageDelegate);
        ClientState = ProxyBase.ClientStates.Offline;
    }

    public void SendClientIDRequest()
    {
        ClientIdRequest request = new ClientIdRequest(ClientID, ServerVersion);
        SendMessage(request);
    }

    public void ReceiveRequest(RequestBase requestBase)
    {
        ClientRequestBase request = (ClientRequestBase) requestBase;
        switch (request)
        {
            case ClientVersionValidRequest _:
            {
                DebugLog.PrintClientStates("Client " + ClientID + " version valid.");
                break;
            }
            case RegisterRequest r: //for single mode
            {
                DebugLog.PrintClientStates("Client " + ClientID + " state: " + ClientState);
                if (ClientState != ProxyBase.ClientStates.GetId)
                {
                    ClientState = ProxyBase.ClientStates.GetId;
                }

                UserName = r.username;
                BuildStoryDatabase.Instance.CreateSinglePlayer(UserName);
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
                    ClientBuildInfosRequest request1 = new ClientBuildInfosRequest(BuildStoryDatabase.Instance.GetPlayerBuilds(UserName), GamePlaySettings.OnlineGamePlaySettings);
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
                Story newStory = AllStories.StoryDict["DefaultStory"].Variant();
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
            case MatchStandaloneRequest r:
            {
                DebugLog.PrintClientStates("Client " + ClientID + " state: " + ClientState);

                if (ClientState != ProxyBase.ClientStates.Login)
                {
                    ClientState = ProxyBase.ClientStates.Login;
                }

                if (ClientState == ProxyBase.ClientStates.Login)
                {
                    BattleProxy.BuildInfo = BuildStoryDatabase.Instance.GetBuildInfoByID(r.BuildID);
                    ClientState = ProxyBase.ClientStates.Matching;
                    if (r.LevelID == -1)
                    {
                        DebugLog.PrintServerStates("Player " + ClientID + " begin standalone custom game.");

                        BattleProxy clientA = BattleProxy;

                        int AI_ClientId = 998;
                        BattleProxy clientB = new BattleProxyAI(AI_ClientId, "CustomAI");
                        clientB.BuildInfo = AllBuilds.GetBuildInfo(BuildGroups.EnemyBuilds, "CustomBattle");
                        CurrentBattle = new Battle(clientA, clientB, DebugLog, null);
                        DebugLog.PrintServerStates("Player " + clientA.ClientID + " and AI:" + clientB.ClientID + " begin game");
                    }
                    else
                    {
                        DebugLog.PrintServerStates("Player " + ClientID + " begin standalone game.");

                        BattleProxy clientA = BattleProxy;

                        int AI_ClientId = 998;
                        BattleProxy clientB = new BattleProxyAI(AI_ClientId, "CustomAI");
                        clientB.BuildInfo = ((Enemy) BuildStoryDatabase.Instance.PlayerStoryStates[UserName].Chapters[r.ChapterID].Levels[r.LevelID]).BuildInfo.Clone();

                        CurrentBattle = new Battle(clientA, clientB, DebugLog, delegate(int winnerClientID)
                        {
                            if (clientB is BattleProxyAI AI)
                            {
                                if (winnerClientID == clientA.ClientID)
                                {
                                    BeatEnemyRequest request2 = new BeatEnemyRequest(r.LevelID);
                                    clientA.SendMessage(request2);

//                                    Story story = Database.Instance.PlayerStoryStates[ClientA.UserName];
//                                    story.BeatEnemy(AI.LevelID, AI.EnemyPicID);
//                                    if (AI.LevelID < story.Chapters.Count - 1)
//                                    {
//                                        story.UnlockChapterEnemies(AI.LevelID + 1);
//                                        List<int> nextLevelBossPicIDs = new List<int>();
//                                        nextLevelBossPicIDs = story.LevelUnlockBossInfo[AI.LevelID + 1];
//                                        NextChapterEnemiesRequest request3 = new NextChapterEnemiesRequest(AI.LevelID + 1, nextLevelBossPicIDs);
//                                        ClientA.SendMessage(request3);
//                                    }
                                }
                            }
                        });

                        DebugLog.PrintServerStates("Player " + clientA.ClientID + " and AI:" + clientB.ClientID + " begin game");
                    }
                }

                break;
            }
            case BonusGroupRequest r:
            {
                Story story = BuildStoryDatabase.Instance.PlayerStoryStates[UserName];
                if (story != null)
                {
                    foreach (Bonus bonus in r.BonusGroup.Bonuses)
                    {
                        switch (bonus)
                        {
                            case Bonus_UnlockCardByID b_UnlockCardByID:
                            {
                                story.EditAllCardLimitDict(b_UnlockCardByID.CardID, 1);
                                break;
                            }
                            case Bonus_UnlockCardByLevelNum b_UnlockCardByLevelNum:
                            {
//                                story.EditAllCardLimitDict(bonus.BonusFinalValue, 1);
                                break;
                            }
                            case Bonus_LifeUpperLimit b_LifeUpperLimit:
                            {
                                story.StoryGamePlaySettings.DefaultLifeMax += b_LifeUpperLimit.LifeUpperLimit;
                                story.StoryGamePlaySettings.DefaultLife += b_LifeUpperLimit.LifeUpperLimit;
                                foreach (KeyValuePair<int, BuildInfo> kv in story.PlayerBuildInfos)
                                {
                                    kv.Value.Life += b_LifeUpperLimit.LifeUpperLimit;
                                }

                                break;
                            }
                            case Bonus_EnergyUpperLimit b_EnergyUpperLimit:
                            {
                                story.StoryGamePlaySettings.DefaultEnergyMax += b_EnergyUpperLimit.EnergyUpperLimit;
                                story.StoryGamePlaySettings.DefaultEnergy += b_EnergyUpperLimit.EnergyUpperLimit;
                                foreach (KeyValuePair<int, BuildInfo> kv in story.PlayerBuildInfos)
                                {
                                    kv.Value.Energy += b_EnergyUpperLimit.EnergyUpperLimit;
                                }

                                break;
                            }
                            case Bonus_Budget b_Budget:
                            {
                                story.StoryGamePlaySettings.DefaultCoin += b_Budget.Budget;
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

            default:
            {
                if (BattleProxy != null)
                {
                    BattleProxy.Response(request);
                }
                else
                {
                    DebugLog.PrintWarning("BattleProxy is Empty!");
                }

                break;
            }
        }
    }
}