using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

internal class ServerGameMatchManager
{
    List<ServerGameManager> SMGS = new List<ServerGameManager>();
    Dictionary<int, ServerGameManager> clientGameMapping = new Dictionary<int, ServerGameManager>(); //玩家和游戏的映射关系
    List<ClientProxy> matchingClients = new List<ClientProxy>(); //匹配中的玩家

    public void OnClientMatchGames(ClientProxy clientProxy)
    {
        matchingClients.Add(clientProxy);
        ServerLog.Print("Add Player Success: [ClientId] " + clientProxy.ClientId + ". Matching queue: " + matchingClients.Count);
        if (matchingClients.Count == 2)
        {
            ClientProxy clientA = matchingClients[0];
            ClientProxy clientB = matchingClients[1];
            ServerGameManager sgm = new ServerGameManager(clientA, clientB);
            SMGS.Add(sgm);
            clientGameMapping.Add(clientA.ClientId, sgm);
            clientGameMapping.Add(clientB.ClientId, sgm);
        }
    }

    public void OnClientCancelMatch(ClientProxy clientProxy)
    {
        matchingClients.Remove(clientProxy);
        ServerLog.Print("Player cancel match: [ClientId] " + clientProxy.ClientId + ". Matching queue: " + matchingClients.Count);
    }

    public void StopGame(ServerGameManager serverGameManager)
    {
        SMGS.Remove(serverGameManager);
    }

    public void KickOutClient(ClientProxy clientProxy)
    {
        if (clientGameMapping.ContainsKey(clientProxy.ClientId))
        {
            clientGameMapping.Remove(clientProxy.ClientId);
            matchingClients.Remove(clientProxy);
            ServerLog.Print("Player kickout: [ClientId] " + clientProxy.ClientId + ". Matching queue: " + matchingClients.Count);
        }
    }
}

public class ClientAndCardDeckInfo
{
    public int ClientId;
    public CardDeckInfo CardDeckInfo;

    public ClientAndCardDeckInfo(int clientId, CardDeckInfo cardDeckInfo)
    {
        ClientId = clientId;
        CardDeckInfo = cardDeckInfo;
    }
}