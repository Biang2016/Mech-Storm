using System.Collections.Generic;
using System.Linq;

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
            case StandaloneStartLevelRequest r:
            {
                DebugLog.PrintClientStates("Client " + ClientID + " state: " + ClientState);

                if (ClientState != ProxyBase.ClientStates.Login)
                {
                    ClientState = ProxyBase.ClientStates.Login;
                }

                if (ClientState == ProxyBase.ClientStates.Login)
                {
                    if (r.BuildID != -1)
                    {
                        BattleProxy.BuildInfo = BuildStoryDatabase.Instance.GetBuildInfoByID(r.BuildID);
                        ClientState = ProxyBase.ClientStates.Matching;
                        if (r.LevelID == -1) // Custom Game
                        {
                            DebugLog.PrintServerStates("Player " + ClientID + " begin standalone custom game.");

                            BattleProxy clientA = BattleProxy;
                            clientA.BuildInfo.BeginMetal = 10;

                            int AI_ClientId = 998;
                            BattleProxy clientB = new BattleProxyAI(AI_ClientId, "CustomAI", (Enemy) AllLevels.LevelDict[LevelTypes.Enemy]["CustomEnemy"].Clone(), true);
                            clientB.BuildInfo = ((Enemy) AllLevels.LevelDict[LevelTypes.Enemy]["CustomEnemy"]).BuildInfo.Clone();
                            CurrentBattle = new Battle(clientA, clientB, DebugLog, null);
                            DebugLog.PrintServerStates("Player " + clientA.ClientID + " and AI:" + clientB.ClientID + " begin game");
                        }
                        else
                        {
                            DebugLog.PrintServerStates("Player " + ClientID + " begin standalone game.");
                            Level level = BuildStoryDatabase.Instance.PlayerStoryStates[UserName].Chapters[r.ChapterID].Levels[r.LevelID];
                            if (level is Enemy enemy)
                            {
                                BattleProxy clientA = BattleProxy;

                                int AI_ClientId = 998;
                                BattleProxy clientB = new BattleProxyAI(AI_ClientId, "StoryAI", enemy, false);
                                clientB.BuildInfo = enemy.BuildInfo.Clone();

                                CurrentBattle = new Battle(clientA, clientB, DebugLog, delegate(int winnerClientID, BattleStatistics battleStatistics_ClientA, BattleStatistics battleStatistics_ClientB)
                                {
                                    if (clientB is BattleProxyAI AI)
                                    {
                                        if (winnerClientID == clientA.ClientID)
                                        {
                                            BuildStoryDatabase.Instance.PlayerStoryStates[UserName].Crystal += battleStatistics_ClientA.totalCrystal;
                                        }
                                    }
                                });

                                DebugLog.PrintServerStates("Player " + clientA.ClientID + " and AI:" + clientB.ClientID + " begin game");
                            }
                        }
                    }
                    else
                    {
                        Level level = BuildStoryDatabase.Instance.PlayerStoryStates[UserName].Chapters[r.ChapterID].Levels[r.LevelID];
                        if (level is Shop shop)
                        {
                            VisitShopRequestResponse request1 = new VisitShopRequestResponse(r.LevelID, (Shop) shop.Clone());
                            SendMessage(request1);
                        }
                    }
                }

                break;
            }
            case LeaveShopRequest r:
            {
                BeatLevelRequest _r = new BeatLevelRequest(r.LevelID);
                SendMessage(_r);
                Story story = BuildStoryDatabase.Instance.PlayerStoryStates[UserName];
                story.CurrentFightingChapter.LevelBeatedDictionary[r.LevelID] = true;
                RefreshStoryRequest req = new RefreshStoryRequest(story);
                SendMessage(req);
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
                            case Bonus_LifeUpperLimit b_LifeUpperLimit:
                            {
                                story.StoryGamePlaySettings.DefaultLifeMax += b_LifeUpperLimit.LifeUpperLimit;
                                break;
                            }
                            case Bonus_EnergyUpperLimit b_EnergyUpperLimit:
                            {
                                story.StoryGamePlaySettings.DefaultEnergyMax += b_EnergyUpperLimit.EnergyUpperLimit;
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
            case BuyShopItemRequest r:
            {
                Story story = BuildStoryDatabase.Instance.PlayerStoryStates[UserName];
                if (story != null)
                {
                    switch (r.ShopItem)
                    {
                        case ShopItem_Card si_card:
                        {
                            story.EditAllCardLimitDict(si_card.GenerateCardID, 1);
                            break;
                        }
                        case ShopItem_Budget si_budget:
                        {
                            story.StoryGamePlaySettings.DefaultCoin += si_budget.Budget;
                            break;
                        }
                        case ShopItem_LifeUpperLimit si_life:
                        {
                            story.StoryGamePlaySettings.DefaultLifeMax += si_life.LifeUpperLimit;
                            break;
                        }
                        case ShopItem_EnergyUpperLimit si_energy:
                        {
                            story.StoryGamePlaySettings.DefaultEnergyMax += si_energy.EnergyUpperLimit;
                            break;
                        }
                    }

                    story.Crystal -= r.ShopItem.Price;
                    BuyShopItemRequestResponse response = new BuyShopItemRequestResponse(r.ShopItem);
                    SendMessage(response);
                }

                break;
            }

            case EndBattleRequest r: // 战斗领取奖励，确认结束战斗
            {
                Story story = BuildStoryDatabase.Instance.PlayerStoryStates[UserName];
                story.Chapters[r.ChapterID].LevelBeatedDictionary[r.LevelID] = true;

                BeatLevelRequest request2 = new BeatLevelRequest(r.LevelID);
                SendMessage(request2);

                RefreshStoryRequest response = new RefreshStoryRequest(story);
                SendMessage(response);
                break;
            }

            case BuildRequest r:
            {
                if (r.BuildInfo.BuildID == -1)
                {
                    if (r.isSingle && r.isStory)
                    {
                        Story story = BuildStoryDatabase.Instance.PlayerStoryStates[UserName];
                        r.BuildInfo = story.PlayerBuildInfos[story.PlayerBuildInfos.Keys.ToList()[0]].Clone();
                    }

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