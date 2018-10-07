using System.Collections.Generic;

internal class ServerGameMatchManager
{
    List<ServerGameManager> SMGS = new List<ServerGameManager>();
    Dictionary<int, ServerGameManager> clientGameMapping = new Dictionary<int, ServerGameManager>(); //玩家和游戏的映射关系
    List<ClientProxy> matchingClients = new List<ClientProxy>(); //匹配中的玩家

    public void OnClientMatchGames(ClientProxy clientProxy)
    {
        matchingClients.Add(clientProxy);
        ServerLog.PrintServerStates("Player " + clientProxy.ClientId + " begin matching. Currently " + matchingClients.Count + " people are matching");
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
            ServerLog.PrintServerStates("Player " + clientA.ClientId + " and " + clientB.ClientId + " begin game, currently " + clientGameMapping.Count + " people are in games," + matchingClients.Count + " people are matching");
        }
    }

    public void OnClientCancelMatch(ClientProxy clientProxy)
    {
        if (matchingClients.Contains(clientProxy))
        {
            matchingClients.Remove(clientProxy);
            ServerLog.PrintServerStates("Player " + clientProxy.ClientId + " cancels matching. Currently " + clientGameMapping.Count + " people are in games," + matchingClients.Count + " people are matching");
        }
    }

    public void RemoveGame(ServerGameManager serverGameManager, ClientProxy clientA, ClientProxy clientB)
    {
        SMGS.Remove(serverGameManager);
        clientGameMapping.Remove(clientA.ClientId);
        clientGameMapping.Remove(clientB.ClientId);
        ServerLog.PrintServerStates("Player " + clientA.ClientId + " and " + clientB.ClientId + " stop game, currently " + clientGameMapping.Count + " people are in games," + matchingClients.Count + " people are matching");
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
            ServerLog.PrintServerStates("Player " + a + " and " + b + " stop game, currently " + clientGameMapping.Count + " people are in games," + matchingClients.Count + " people are matching");
        }
    }
}
