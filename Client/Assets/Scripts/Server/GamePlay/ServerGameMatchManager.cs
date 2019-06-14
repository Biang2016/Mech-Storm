using System.Collections.Generic;

public class ServerGameMatchManager
{
    List<Battle> Battles = new List<Battle>();
    Dictionary<int, Battle> clientBattleMapping = new Dictionary<int, Battle>();
    List<ServerProxy> matchingClients = new List<ServerProxy>();

    public void OnClientMatchGames(ServerProxy clientProxy)
    {
        matchingClients.Add(clientProxy);

        ServerLog.Instance.PrintServerStates("Player " + clientProxy.ClientID + " begin matching. Currently " + matchingClients.Count + " people are matching");

        if (matchingClients.Count == 2)
        {
            ServerProxy clientA = matchingClients[0];
            ServerProxy clientB = matchingClients[1];
            matchingClients.Remove(clientA);
            matchingClients.Remove(clientB);
            Battle battle = new Battle(clientA.GameProxy.BattleProxy, clientB.GameProxy.BattleProxy, ServerLog.Instance);

            Battles.Add(battle);
            clientBattleMapping.Add(clientA.ClientID, battle);
            clientBattleMapping.Add(clientB.ClientID, battle);

            ServerLog.Instance.PrintServerStates("Player " + clientA.ClientID + " and " + clientB.ClientID + " begin game, currently " + clientBattleMapping.Count + " people are in games," + matchingClients.Count + " people are matching");
        }
    }

    public void OnClientCancelMatch(ServerProxy clientProxy)
    {
        if (matchingClients.Contains(clientProxy))
        {
            matchingClients.Remove(clientProxy);

            ServerLog.Instance.PrintServerStates("Player " + clientProxy.ClientID + " cancels matching. Currently " + clientBattleMapping.Count + " people are in games," + matchingClients.Count + " people are matching");
        }
    }

    public void RemoveGame(ServerProxy client)
    {
        if (clientBattleMapping.ContainsKey(client.ClientID))
        {
            Battle battle = clientBattleMapping[client.ClientID];
            Battles.Remove(battle);
            int a = battle.ClientA.ClientID;
            int b = battle.ClientB.ClientID;
            clientBattleMapping.Remove(a);
            clientBattleMapping.Remove(b);

            ServerLog.Instance.PrintServerStates("Player " + a + " and " + b + " stop game, currently " + clientBattleMapping.Count + " people are in games," + matchingClients.Count + " people are matching");
        }
    }
}