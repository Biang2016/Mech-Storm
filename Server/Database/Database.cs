using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

internal class Database
{
    public static Database Instance = new Database();

    #region Users

    private Dictionary<string, string> UsernamePasswordTable = new Dictionary<string, string>();
    private Dictionary<int, string> LoginClientIdUsernameTable = new Dictionary<int, string>();
    private List<string> LoginUserNames = new List<string>();

    public bool AddUser(string username, string password)
    {
        if (!isExistUsername(username))
        {
            UsernamePasswordTable.Add(username, password);
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

    Dictionary<int, BuildInfo> BuildInfoDict = new Dictionary<int, BuildInfo>();

    public void AddOrModifyBuild(string username, BuildInfo buildInfo)
    {
        if (!BuildInfoDict.ContainsKey(buildInfo.BuildID))
        {
            BuildInfoDict.Add(buildInfo.BuildID, buildInfo);
        }
        else
        {
            BuildInfoDict[buildInfo.BuildID] = buildInfo;
        }

        if (!PlayerBuilds.ContainsKey(username))
        {
            PlayerBuilds.Add(username, new List<int>());
        }

        if (!PlayerBuilds[username].Contains(buildInfo.BuildID))
        {
            PlayerBuilds[username].Add(buildInfo.BuildID);
        }
    }

    public void DeleteBuild(string username, int buildID)
    {
        if (BuildInfoDict.ContainsKey(buildID))
        {
            BuildInfoDict.Remove(buildID);
        }

        if (PlayerBuilds.ContainsKey(username))
        {
            if (PlayerBuilds[username].Contains(buildID))
            {
                PlayerBuilds[username].Remove(buildID);
            }
        }
    }

    public BuildInfo GetBuildInfoByID(int buildId)
    {
        BuildInfoDict.TryGetValue(buildId, out BuildInfo buildInfo);
        return buildInfo;
    }

    private int BuildIdIndex = 1;

    public int GenerateBuildID()
    {
        return BuildIdIndex++;
    }

    #endregion

    #region PlayerBuilds

    Dictionary<string, List<int>> PlayerBuilds = new Dictionary<string, List<int>>();

    public void AddPlayerBuild(string username, int buildID)
    {
        if (!PlayerBuilds.ContainsKey(username))
        {
            PlayerBuilds.Add(username, new List<int>());
        }

        PlayerBuilds[username].Add(buildID);
    }

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
}