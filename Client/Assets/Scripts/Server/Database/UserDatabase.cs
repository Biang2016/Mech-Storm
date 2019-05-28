using System.Collections.Generic;
using System.Linq;

public class UserDatabase
{
    public static UserDatabase Instance = new UserDatabase();

    #region Users

    private Dictionary<string, string> UsernamePasswordTable = new Dictionary<string, string>();
    private Dictionary<int, string> LoginClientIdUsernameTable = new Dictionary<int, string>();
    private List<string> LoginUserNames = new List<string>();

    public bool AddUser(string username, string password)
    {
        if (!isExistUsername(username))
        {
            UsernamePasswordTable.Add(username, password);
            //每个玩家都有几个默认卡组
            foreach (BuildInfo bi in BuildStoryDatabase.Instance.BuildGroupDict[BuildGroups.OnlineBuilds].AllBuildInfo())
            {
                BuildInfo newBI = bi.Clone();
                BuildStoryDatabase.Instance.BuildInfoDict.Add(newBI.BuildID, newBI);
                BuildStoryDatabase.Instance.AddOrModifyBuild(username, newBI);
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
}