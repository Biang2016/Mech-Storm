using System.Collections.Generic;
using System.Linq;

internal class Database
{
    public static Database Instance = new Database();

    #region Users

    private Dictionary<string, string> UsernamePasswordTable = new Dictionary<string, string>();
    private Dictionary<int, string> LoginClientIdUsernameTable = new Dictionary<int, string>();
    private List<string> LoginUserNames = new List<string>();

    public bool AddUser(string username, string password, bool isSuperAccount = false)
    {
        if (!isExistUsername(username))
        {
            UsernamePasswordTable.Add(username, password);
            if (!isSuperAccount) //每个玩家都有几个默认卡组
            {
                foreach (int playerDefaultBuildID in SpecialBuildsDict["PlayerAdmin"].Builds.Values)
                {
                    BuildInfo newBI = BuildInfoDict[playerDefaultBuildID].Clone();
                    BuildInfoDict.Add(newBI.BuildID, newBI);
                    AddOrModifyBuild(username, newBI);
                }
            }

            return true;
        }

        return false;
    }

    public bool AddLoginUser(int clientId, string username)
    {
        if (!isExistLoginClientId(clientId))
        {
            if (!isExistLoginUser(username))
            {
                LoginClientIdUsernameTable.Add(clientId, username);
                LoginUserNames.Add(username);
                return true;
            }
        }

        return false;
    }

    public bool RemoveLoginUser(int clientId)
    {
        if (isExistLoginClientId(clientId))
        {
            string username = LoginClientIdUsernameTable[clientId];
            LoginClientIdUsernameTable.Remove(clientId);
            if (isExistLoginUser(username))
            {
                LoginUserNames.Remove(username);
                return true;
            }
        }

        return false;
    }

    private bool isExistUsername(string username)
    {
        return UsernamePasswordTable.ContainsKey(username);
    }

    public string GetUserPasswordByUsername(string username)
    {
        if (!isExistUsername(username)) return null;
        return UsernamePasswordTable[username];
    }

    private bool isExistLoginUser(string username)
    {
        return LoginUserNames.Contains(username);
    }

    private bool isExistLoginClientId(int clientId)
    {
        return LoginClientIdUsernameTable.ContainsKey(clientId);
    }

    public string GetUsernameByClientId(int clientId)
    {
        if (!isExistLoginClientId(clientId)) return null;
        return LoginClientIdUsernameTable[clientId];
    }

    #endregion

    #region Builds

    public Dictionary<int, BuildInfo> BuildInfoDict = new Dictionary<int, BuildInfo>();

    public void AddOrModifyBuild(string username, BuildInfo buildInfo, bool isSingle = false)
    {
        if (!BuildInfoDict.ContainsKey(buildInfo.BuildID))
        {
            BuildInfoDict.Add(buildInfo.BuildID, buildInfo);
        }
        else
        {
            BuildInfoDict[buildInfo.BuildID] = buildInfo;
        }

        if (!isSingle)
        {
            if (!PlayerBuilds.ContainsKey(username))
            {
                PlayerBuilds.Add(username, new List<int>());
            }

            if (!PlayerBuilds[username].Contains(buildInfo.BuildID))
            {
                PlayerBuilds[username].Add(buildInfo.BuildID);
            }
        }
        else
        {
            if (PlayerStoryStates.ContainsKey(username))
            {
                Story story = PlayerStoryStates[username];
                if (!story.PlayerBuildInfos.ContainsKey(buildInfo.BuildID))
                {
                    story.PlayerBuildInfos.Add(buildInfo.BuildID, buildInfo);
                }
                else
                {
                    story.PlayerBuildInfos[buildInfo.BuildID] = buildInfo;
                }
            }
        }

        if (SpecialBuildsDict.ContainsKey(username))
        {
            SpecialBuilds sb = SpecialBuildsDict[username];
            if (!sb.Builds.ContainsKey(buildInfo.BuildName))
            {
                sb.Builds.Add(buildInfo.BuildName, buildInfo.BuildID);
            }

            AllServerBuilds.ExportBuilds(sb);
        }
    }

    public void DeleteBuild(string username, int buildID, bool isSingle = false)
    {
        if (SpecialBuildsDict.ContainsKey(username))
        {
            SpecialBuildsDict[username].Builds.Remove(BuildInfoDict[buildID].BuildName);
        }

        if (BuildInfoDict.ContainsKey(buildID))
        {
            BuildInfoDict.Remove(buildID);
        }

        if (!isSingle)
        {
            if (PlayerBuilds.ContainsKey(username))
            {
                if (PlayerBuilds[username].Contains(buildID))
                {
                    PlayerBuilds[username].Remove(buildID);
                }
            }
        }
        else
        {
            if (PlayerStoryStates.ContainsKey(username))
            {
                PlayerStoryStates[username].PlayerBuildInfos.Remove(buildID);
            }
        }

        if (SpecialBuildsDict.ContainsKey(username))
        {
            AllServerBuilds.ExportBuilds(SpecialBuildsDict[username]);
        }
    }

    public BuildInfo GetBuildInfoByID(int buildId)
    {
        BuildInfoDict.TryGetValue(buildId, out BuildInfo buildInfo);
        return buildInfo;
    }

    #endregion

    #region PlayerBuilds

    #region Online

    Dictionary<string, List<int>> PlayerBuilds = new Dictionary<string, List<int>>();

    public List<BuildInfo> GetPlayerBuilds(string username)
    {
        List<int> buildInfoIds = new List<int>();
        List<BuildInfo> buildInfos = new List<BuildInfo>();
        if (PlayerBuilds.ContainsKey(username))
        {
            buildInfoIds = PlayerBuilds[username];
        }

        foreach (int buildInfoId in buildInfoIds)
        {
            if (BuildInfoDict.ContainsKey(buildInfoId))
            {
                buildInfos.Add(BuildInfoDict[buildInfoId]);
            }
        }

        return buildInfos;
    }

    #endregion

    #region SingleMode

    public Dictionary<string, Story> StoryStartDict = new Dictionary<string, Story>();

    public Dictionary<string, Story> PlayerStoryStates = new Dictionary<string, Story>();

    public void RemovePlayerStory(string username, ClientProxy proxy)
    {
        if (PlayerStoryStates.ContainsKey(username))
        {
            Story story = PlayerStoryStates[username];
            foreach (int removeBuildID in story.PlayerBuildInfos.Keys)
            {
                DeleteBuildRequestResponse response = new DeleteBuildRequestResponse(removeBuildID);
                proxy.SendMessage(response);
            }

            PlayerStoryStates.Remove(username);
        }
    }

    #endregion

    #endregion

    #region SpecialBuilds

    public Dictionary<string, SpecialBuilds> SpecialBuildsDict = new Dictionary<string, SpecialBuilds>();

    public class SpecialBuilds
    {
        public string ManagerName;
        public Dictionary<string, int> Builds = new Dictionary<string, int>();

        public SpecialBuilds(string managerName)
        {
            ManagerName = managerName;
        }

        public void AddBuild(string buildname, int buildID)
        {
            if (!Builds.ContainsKey(buildname))
            {
                Builds.Add(buildname, buildID);
            }
        }

        public BuildInfo GetBuildInfo(string buildName)
        {
            Builds.TryGetValue(buildName, out int id);
            Instance.BuildInfoDict.TryGetValue(id, out BuildInfo buildInfo);
            return buildInfo;
        }

        public List<BuildInfo> AllBuildInfo()
        {
            List<int> ids = Builds.Values.ToList();
            List<BuildInfo> BuildInfos = new List<BuildInfo>();
            foreach (int id in ids)
            {
                BuildInfos.Add(Instance.BuildInfoDict[id]);
            }

            return BuildInfos;
        }
    }

    #endregion
}