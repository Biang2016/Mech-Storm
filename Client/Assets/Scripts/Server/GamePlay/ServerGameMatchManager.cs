using System.Collections.Generic;

internal class ServerGameMatchManager
{
    List<Battle> Battles = new List<Battle>();
    Dictionary<int, Battle> clientBattleMapping = new Dictionary<int, Battle>();
    List<ServerProxy> matchingClients = new List<ServerProxy>();

    public void OnClientMatchGames(ServerProxy clientProxy)
    {
        matchingClients.Add(clientProxy);

        ServerLog.Instance.PrintServerStates("Player " + clientProxy.ClientId + " begin matching. Currently " + matchingClients.Count + " people are matching");

        if (matchingClients.Count == 2)
        {
            ServerProxy clientA = matchingClients[0];
            ServerProxy clientB = matchingClients[1];
            matchingClients.Remove(clientA);
            matchingClients.Remove(clientB);
            Battle battle = new Battle(clientA.GameProxy.BattleProxy, clientB.GameProxy.BattleProxy);
            Battles.Add(battle);
            clientBattleMapping.Add(clientA.ClientId, battle);
            clientBattleMapping.Add(clientB.ClientId, battle);

            ServerLog.Instance.PrintServerStates("Player " + clientA.ClientId + " and " + clientB.ClientId + " begin game, currently " + clientBattleMapping.Count + " people are in games," + matchingClients.Count + " people are matching");
        }
    }

    public void OnClientCancelMatch(ServerProxy clientProxy)
    {
        if (matchingClients.Contains(clientProxy))
        {
            matchingClients.Remove(clientProxy);

            ServerLog.Instance.PrintServerStates("Player " + clientProxy.ClientId + " cancels matching. Currently " + clientBattleMapping.Count + " people are in games," + matchingClients.Count + " people are matching");
        }
    }

    public void RemoveGame(ServerProxy client)
    {
        if (clientBattleMapping.ContainsKey(client.ClientId))
        {
            Battle battle = clientBattleMapping[client.ClientId];
            Battles.Remove(battle);
            int a = battle.ClientA.ClientId;
            int b = battle.ClientB.ClientId;
            clientBattleMapping.Remove(a);
            clientBattleMapping.Remove(b);

            ServerLog.Instance.PrintServerStates("Player " + a + " and " + b + " stop game, currently " + clientBattleMapping.Count + " people are in games," + matchingClients.Count + " people are matching");
        }
    }
}