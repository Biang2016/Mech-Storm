using System.Collections.Generic;

public class BuildStoryDatabase
{
    public static BuildStoryDatabase Instance = new BuildStoryDatabase();

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

        if (BuildGroupDict.ContainsKey(username))
        {
            BuildGroup sb = BuildGroupDict[username];
            if (!sb.Builds.ContainsKey(buildInfo.BuildName))
            {
                sb.Builds.Add(buildInfo.BuildName, buildInfo.BuildID);
            }

            AllBuilds.ExportBuilds(sb);
        }
    }

    public void DeleteBuild(string username, int buildID, bool isSingle = false)
    {
        if (BuildGroupDict.ContainsKey(username))
        {
            BuildGroupDict[username].Builds.Remove(BuildInfoDict[buildID].BuildName);
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

        if (BuildGroupDict.ContainsKey(username))
        {
            AllBuilds.ExportBuilds(BuildGroupDict[username]);
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

    public List<int> RemovePlayerStory(string username)
    {
        List<int> removeBuildIDs = new List<int>();
        if (PlayerStoryStates.ContainsKey(username))
        {
            Story story = PlayerStoryStates[username];
            foreach (int removeBuildID in story.PlayerBuildInfos.Keys)
            {
                removeBuildIDs.Add(removeBuildID);
                BuildInfoDict.Remove(removeBuildID);
            }

            PlayerStoryStates.Remove(username);
        }

        return removeBuildIDs;
    }

    #endregion

    #region SingleMode

    public Dictionary<string, Story> StoryStartDict = new Dictionary<string, Story>();

    public Dictionary<string, Story> PlayerStoryStates = new Dictionary<string, Story>();

    #endregion

    #endregion

    #region SpecialBuilds

    public Dictionary<string, BuildGroup> BuildGroupDict = new Dictionary<string, BuildGroup>();

    #endregion
}