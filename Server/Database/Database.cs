using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

internal class Database
{
    public static Database Instance = new Database();

    #region Users

    public Dictionary<string, string> UserTable = new Dictionary<string, string>();
    public Dictionary<string, string> LoginUserTable = new Dictionary<string, string>();

    #endregion

    #region Builds

    Dictionary<int, BuildInfo> BuildInfoDict = new Dictionary<int, BuildInfo>();

    public void AddOrModifyBuild(BuildInfo buildInfo)
    {
        if (!BuildInfoDict.ContainsKey(buildInfo.BuildID))
        {
            BuildInfoDict.Add(buildInfo.BuildID, buildInfo);
        }
        else
        {
            BuildInfoDict[buildInfo.BuildID] = buildInfo;
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