using System.Collections.Generic;

public class BuildGroup
{
    public string ManagerName;
    public Dictionary<string, BuildInfo> Builds = new Dictionary<string, BuildInfo>();

    public BuildGroup(string managerName)
    {
        ManagerName = managerName;
    }

    public void AddBuild(string buildName, BuildInfo buildInfo)
    {
        if (!Builds.ContainsKey(buildName))
        {
            Builds.Add(buildName, buildInfo);
        }
    }

    public BuildInfo GetBuildInfo(string buildName)
    {
        if (Builds.ContainsKey(buildName))
        {
            return Builds[buildName];
        }

        return null;
    }
}