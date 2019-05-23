using System.Collections.Generic;
using System.Linq;

public class BuildGroup
{
    public string ManagerName;
    public Dictionary<string, int> Builds = new Dictionary<string, int>();

    public BuildGroup(string managerName)
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
        if (Builds.ContainsKey(buildName))
        {
            if (BuildStoryDatabase.Instance.BuildInfoDict.ContainsKey(Builds[buildName]))
            {
                return BuildStoryDatabase.Instance.BuildInfoDict[Builds[buildName]];
            }
        }

        return null;
    }

    public List<BuildInfo> AllBuildInfo()
    {
        List<int> ids = Builds.Values.ToList();
        List<BuildInfo> BuildInfos = new List<BuildInfo>();
        foreach (int id in ids)
        {
            BuildInfos.Add(BuildStoryDatabase.Instance.BuildInfoDict[id]);
        }

        return BuildInfos;
    }
}