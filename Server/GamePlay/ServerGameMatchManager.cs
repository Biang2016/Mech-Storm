using System.Collections.Generic;

internal class ServerGameMatchManager
{
    List<ServerGameManager> SMGS = new List<ServerGameManager>();
    Dictionary<int, ServerGameManager> clientGameMapping = new Dictionary<int, ServerGameManager>(); //玩家和游戏的映射关系
    List<ClientProxy> matchingClients = new List<ClientProxy>(); //匹配中的玩家

    public void OnClientMatchGames(ClientProxy clientProxy)
    {
        matchingClients.Add(clientProxy);
#if DEBUG
        ServerLog.PrintServerStates("Player " + clientProxy.ClientId + " begin matching. Currently " + matchingClients.Count + " people are matching");
#endif
        if (matchingClients.Count == 2)
        {
            ClientProxy clientA = matchingClients[0];
            ClientProxy clientB = matchingClients[1];
            matchingClients.Remove(clientA);
            matchingClients.Remove(clientB);
            ServerGameManager sgm = new ServerGameManager(clientA, clientB);
            SMGS.Add(sgm);
            clientGameMapping.Add(clientA.ClientId, sgm);
            clientGameMapping.Add(clientB.ClientId, sgm);
#if DEBUG
            ServerLog.PrintServerStates("Player " + clientA.ClientId + " and " + clientB.ClientId + " begin game, currently " + clientGameMapping.Count + " people are in games," + matchingClients.Count + " people are matching");
#endif
        }
    }


    public void OnClientCancelMatch(ClientProxy clientProxy)
    {
        if (matchingClients.Contains(clientProxy))
        {
            matchingClients.Remove(clientProxy);
#if DEBUG
            ServerLog.PrintServerStates("Player " + clientProxy.ClientId + " cancels matching. Currently " + clientGameMapping.Count + " people are in games," + matchingClients.Count + " people are matching");
#endif
        }
    }

    public void RemoveGame(ClientProxy client)
    {
        if (clientGameMapping.ContainsKey(client.ClientId))
        {
            ServerGameManager sgm = clientGameMapping[client.ClientId];
            SMGS.Remove(sgm);
            int a = sgm.ClientA.ClientId;
            int b = sgm.ClientB.ClientId;
            clientGameMapping.Remove(a);
            clientGameMapping.Remove(b);
#if DEBUG
            ServerLog.PrintServerStates("Player " + a + " and " + b + " stop game, currently " + clientGameMapping.Count + " people are in games," + matchingClients.Count + " people are matching");
#endif
        }

        if (clientGameMapping_Standalone.ContainsKey(client.ClientId))
        {
            ServerGameManager sgm = clientGameMapping_Standalone[client.ClientId];
            SMGS_Standalone.Remove(sgm);
            int a = sgm.ClientA.ClientId;
            int b = sgm.ClientB.ClientId;
            clientGameMapping_Standalone.Remove(a);
            clientGameMapping_Standalone.Remove(b);
#if DEBUG
            ServerLog.PrintServerStates("Player " + a + " and AI:" + b + " stop game, currently " + clientGameMapping_Standalone.Count / 2 + " people are in StandAlone games.");
#endif
        }
    }

    #region StandAlone

    List<ServerGameManager> SMGS_Standalone = new List<ServerGameManager>();
    Dictionary<int, ServerGameManager> clientGameMapping_Standalone = new Dictionary<int, ServerGameManager>();

    public void OnClientMatchStandAloneGames(ClientProxy clientProxy, int LevelID, int BossID)
    {
#if DEBUG
        ServerLog.PrintServerStates("Player " + clientProxy.ClientId + " begin standalone game.");
#endif
        ClientProxy clientA = clientProxy;
        int AI_ClientId = Server.SV.GenerateClientId();
        ClientProxy clientB = new ClientProxyAI(AI_ClientId, false);

        string bossBuildName = Database.Instance.PlayerStoryStates[clientProxy.UserName].Levels[LevelID].Bosses[BossID].BuildName;
        BuildInfo buildInfo = Database.Instance.SpecialBuildsDict["ServerAdmin"].GetBuildInfo(bossBuildName).Clone();
        clientB.CurrentBuildInfo = buildInfo;

        ServerGameManager sgm = new ServerGameManager(clientA, clientB);
        SMGS_Standalone.Add(sgm);
        clientGameMapping_Standalone.Add(clientA.ClientId, sgm);
        clientGameMapping_Standalone.Add(clientB.ClientId, sgm);
#if DEBUG
        ServerLog.PrintServerStates("Player " + clientA.ClientId + " and AI:" + clientB.ClientId + " begin game, currently " + clientGameMapping_Standalone.Count + " people are in AI games.");
#endif
    }

    #endregion
}