using System.Collections.Generic;

internal class ServerGameMatchManager
{
    List<Battle> Battles = new List<Battle>();
    Dictionary<int, Battle> clientBattleMapping = new Dictionary<int, Battle>();
    List<ClientProxy> matchingClients = new List<ClientProxy>();

    public void OnClientMatchGames(ClientProxy clientProxy)
    {
        matchingClients.Add(clientProxy);

        ServerLog.Instance.PrintServerStates("Player " + clientProxy.ClientId + " begin matching. Currently " + matchingClients.Count + " people are matching");

        if (matchingClients.Count == 2)
        {
            ClientProxy clientA = matchingClients[0];
            ClientProxy clientB = matchingClients[1];
            matchingClients.Remove(clientA);
            matchingClients.Remove(clientB);
            Battle battle = new Battle(clientA.BattleClientProxy, clientB.BattleClientProxy, Server.SV.DoSendToClient);
            Battles.Add(battle);
            clientBattleMapping.Add(clientA.ClientId, battle);
            clientBattleMapping.Add(clientB.ClientId, battle);

            ServerLog.Instance.PrintServerStates("Player " + clientA.ClientId + " and " + clientB.ClientId + " begin game, currently " + clientBattleMapping.Count + " people are in games," + matchingClients.Count + " people are matching");
        }
    }

    public void OnClientCancelMatch(ClientProxy clientProxy)
    {
        if (matchingClients.Contains(clientProxy))
        {
            matchingClients.Remove(clientProxy);

            ServerLog.Instance.PrintServerStates("Player " + clientProxy.ClientId + " cancels matching. Currently " + clientBattleMapping.Count + " people are in games," + matchingClients.Count + " people are matching");
        }
    }

    public void RemoveGame(ClientProxy client)
    {
        if (clientBattleMapping.ContainsKey(client.ClientId))
        {
            Battle batle = clientBattleMapping[client.ClientId];
            Battles.Remove(batle);
            int a = batle.ClientA.ClientId;
            int b = batle.ClientB.ClientId;
            clientBattleMapping.Remove(a);
            clientBattleMapping.Remove(b);

            ServerLog.Instance.PrintServerStates("Player " + a + " and " + b + " stop game, currently " + clientBattleMapping.Count + " people are in games," + matchingClients.Count + " people are matching");
        }
    }
}